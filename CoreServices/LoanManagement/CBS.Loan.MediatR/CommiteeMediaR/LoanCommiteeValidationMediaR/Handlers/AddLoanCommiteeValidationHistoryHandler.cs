using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Data.Helper;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Commands;
using CBS.NLoan.Repository.CollateralP;
using CBS.NLoan.Repository.CommiteeP.LoanCommeteeMemberP;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeGroupP;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeValidationP;
using CBS.NLoan.Repository.DocumentP;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanGuarantorP;
using CBS.NLoan.Repository.LoanProductP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.Entity;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanCommiteeValidationHistoryHandler : IRequestHandler<AddLoanCommiteeValidationHistoryCommand, ServiceResponse<LoanCommiteeValidationHistoryDto>>
    {
        private readonly ILoanCommiteeValidationHistoryRepository _LoanCommiteeValidationRepository; // Repository for accessing LoanCommiteeValidation data.
        private readonly ILoanCommeteeMemberRepository _LoanCommeteeMemberRepository; // Repository for accessing LoanCommiteeValidation data.
        private readonly ILoanCommiteeGroupRepository _loanCommiteeGroupRepository; // Repository for accessing LoanCommiteeValidation data.
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository; // Repository for accessing LoanCommiteeValidation data.
        private readonly ILoanGuarantorRepository _loanGuarantorRepository; // Repository for accessing LoanGuarantor data.
        private readonly ILoanCollateralRepository _loanCollateralRepository; // Repository for accessing LoanGuarantor data.
        private readonly IDocumentAttachedToLoanRepository _documentAttachedToLoanRepository; // Repository for accessing LoanGuarantor data.

        private readonly ILoanApplicationRepository _LoanApplicationRepository; // Repository for accessing LoanCommiteeValidation data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanCommiteeValidationHistoryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        private readonly UserInfoToken _userToken;
        /// <summary>
        /// Constructor for initializing the AddLoanCommiteeValidationCommandHandler.
        /// </summary>
        /// <param name="LoanCommiteeValidationRepository">Repository for LoanCommiteeValidation data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanCommiteeValidationHistoryHandler(
            ILoanCommiteeValidationHistoryRepository LoanCommiteeValidationRepository,
            ILoanApplicationRepository LoanProductRepository,
            IMapper mapper,
            ILogger<AddLoanCommiteeValidationHistoryHandler> logger,
            IUnitOfWork<LoanContext> uow,
            ILoanCommeteeMemberRepository loanCommeteeMemberRepository,
            UserInfoToken userToken = null,
            ILoanCommiteeGroupRepository loanCommiteeGroupRepository = null,
            ILoanApplicationFeeRepository loanApplicationFeeRepository = null,
            ILoanGuarantorRepository loanGuarantorRepository = null,
            ILoanCollateralRepository loanCollateralRepository = null,
            IDocumentAttachedToLoanRepository documentAttachedToLoanRepository = null)
        {
            _LoanCommiteeValidationRepository = LoanCommiteeValidationRepository;
            _LoanApplicationRepository = LoanProductRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _LoanCommeteeMemberRepository = loanCommeteeMemberRepository;
            _userToken = userToken;
            _loanCommiteeGroupRepository = loanCommiteeGroupRepository;
            _loanApplicationFeeRepository = loanApplicationFeeRepository;
            _loanGuarantorRepository = loanGuarantorRepository;
            _loanCollateralRepository = loanCollateralRepository;
            _documentAttachedToLoanRepository = documentAttachedToLoanRepository;
        }

        /// <summary>
        /// Handles the AddLoanCommiteeValidationCommand to add a new LoanCommiteeValidation.
        /// </summary>
        /// <param name="request">The AddLoanCommiteeValidationCommand containing LoanCommiteeValidation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommiteeValidationHistoryDto>> Handle(AddLoanCommiteeValidationHistoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var loanApplication = await _LoanApplicationRepository.FindAsync(request.LoanApplicationId);

                var loanCommiteeMembers = _LoanCommeteeMemberRepository.FindBy(x => x.UserId == request.UserId).ToList();
                var loanCommiteeMember = new LoanCommiteeMember();
                foreach (var item in loanCommiteeMembers)
                {
                    var loanCG = _loanCommiteeGroupRepository.Find(item.LoanCommiteeGroupId);

                    if (loanApplication.Amount >= loanCG.MinimumLoanAmount && loanApplication.Amount <= loanCG.MaximumLoanAmount)
                    {
                        loanCommiteeMember = item;
                        loanCommiteeMember.LoanCommiteeGroup = loanCG;
                        break;
                    }
                }
                var loanApplicationFees = _loanApplicationFeeRepository.FindBy(x => x.LoanApplicationId == request.LoanApplicationId).ToList();
                if (loanCommiteeMembers.Any())
                {
                    if (loanCommiteeMember==null)
                    {
                        return ServiceResponse<LoanCommiteeValidationHistoryDto>.Return403(
                            $"You can only validate loans within the range of {loanCommiteeMember.LoanCommiteeGroup.MinimumLoanAmount} to {loanCommiteeMember.LoanCommiteeGroup.MaximumLoanAmount}."
                        );

                    }

                    if (loanApplication.Amount >= loanCommiteeMember.LoanCommiteeGroup.MinimumLoanAmount && loanApplication.Amount <= loanCommiteeMember.LoanCommiteeGroup.MaximumLoanAmount)
                    {
                        var loanCommiteeValidations = _LoanCommiteeValidationRepository.FindBy(x => x.LoanApplicationId == request.LoanApplicationId && x.Status == LoanCommiteeValidationStatuses.Approved.ToString()).GroupBy(x => x.UserId).ToList();
                        var rejections = loanApplication.LoanCommiteeValidations.Where(x => x.Status == LoanCommiteeValidationStatuses.Rejected.ToString()).GroupBy(x => x.UserId).ToList();
                        if (rejections.Count() == loanCommiteeMember.LoanCommiteeGroup.NumberToApprovalsToValidationALoan)
                        {
                            loanApplication.ApprovalStatus = LoanApplicationStatusX.Rejected.ToString();
                        }
                        int i = 0;
                        if (request.Status == LoanCommiteeValidationStatuses.Approved.ToString())
                        {
                            i++;
                            //Check if all fee is paid
                            if (!loanApplication.IsChargesInclussive)
                            {
                                //Check if all fee is paid
                                if (loanApplicationFees.Any())
                                {

                                    if (LoanHelper.HasUnpaidFees(loanApplicationFees, "Before"))
                                    {
                                        string errorMessage = "This loan application cannot be validated as there are outstanding fees that need to be settled prior to validation. Please ensure all fees are paid before proceeding.";
                                        _logger.LogError(errorMessage);
                                        return ServiceResponse<LoanCommiteeValidationHistoryDto>.Return404(errorMessage);
                                    }
                                }
                            }
                            var loanGuarantors = _loanGuarantorRepository.FindBy(x => x.LoanApplicationId == request.LoanApplicationId).ToList();
                            var documentAttachedToLoans = _documentAttachedToLoanRepository.FindBy(x => x.LoanApplicationId == request.LoanApplicationId).ToList();
                            var loanApplicationCollaterals =  _loanCollateralRepository.FindBy(x => x.LoanApplicationId == request.LoanApplicationId).ToList();
                            //
                            _LoanApplicationRepository.LoanApplicationValidator(loanApplication, loanApplication.LoanProduct, loanGuarantors, loanApplicationCollaterals, documentAttachedToLoans);
                        }
                        if (loanCommiteeValidations.Count() + i >= loanCommiteeMember.LoanCommiteeGroup.NumberToApprovalsToValidationALoan)
                        {
                            loanApplication.ApprovalStatus = LoanApplicationStatusX.Validated.ToString();
                        }
                        else
                        {

                            loanApplication.ApprovalStatus = LoanApplicationStatusX.Under_review_By_Loan_Comminitee.ToString();

                        }
                        // Map the AddLoanCommiteeValidationCommand to a LoanCommiteeValidation entity
                        var LoanCommiteeValidationEntity = _mapper.Map<LoanCommiteeValidationHistory>(request);
                        // Convert UTC to local time and set it in the entity
                        LoanCommiteeValidationEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                        LoanCommiteeValidationEntity.Id = BaseUtilities.GenerateUniqueNumber();
                        LoanCommiteeValidationEntity.LoanCommiteeMemberId = loanCommiteeMember.Id;
                        LoanCommiteeValidationEntity.UserId = _userToken.FullName;
                        // Add the new LoanCommiteeValidation entity to the repository
                        _LoanApplicationRepository.Update(loanApplication);
                        _LoanCommiteeValidationRepository.Add(LoanCommiteeValidationEntity);
                        await _uow.SaveAsync();
                        // Map the LoanCommiteeValidation entity to LoanCommiteeValidationDto and return it with a success response
                        var LoanCommiteeValidationDto = _mapper.Map<LoanCommiteeValidationHistoryDto>(LoanCommiteeValidationEntity);
                        return ServiceResponse<LoanCommiteeValidationHistoryDto>.ReturnResultWith200(LoanCommiteeValidationDto);

                    }
                    else
                    {
                        return ServiceResponse<LoanCommiteeValidationHistoryDto>.Return403(
                            $"You can only validate loans within the range of {loanCommiteeMember.LoanCommiteeGroup.MinimumLoanAmount} to {loanCommiteeMember.LoanCommiteeGroup.MaximumLoanAmount}."
                        );

                    }
                }
                else
                {

                    return ServiceResponse<LoanCommiteeValidationHistoryDto>.Return403(
                        "You are not authorized to validate loans as you are not a member of the loan validation committee."
                    );

                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LoanCommiteeValidation: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommiteeValidationHistoryDto>.Return500(e);
            }
        }
    }

}
