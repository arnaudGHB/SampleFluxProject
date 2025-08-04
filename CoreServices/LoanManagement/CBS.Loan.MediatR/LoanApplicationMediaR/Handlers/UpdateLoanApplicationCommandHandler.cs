using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Commands;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Commands;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Validations;
using CBS.NLoan.Repository.FeeP.FeeP;
using CBS.NLoan.Repository.FeeRangeP;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanProductP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanApplicationCommandHandler : IRequestHandler<UpdateLoanApplicationCommand, ServiceResponse<LoanApplicationDto>>
    {
        private readonly ILoanApplicationRepository _LoanRepository; // Repository for accessing Loan data.
        private readonly ILoanProductRepository _LoanProductRepository; // Repository for accessing Loan Product data.
        private readonly ILogger<UpdateLoanApplicationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository; // Repository for accessing Loan data.
        private readonly IFeeRepository _feeRepository;
        private readonly IFeeRangeRepository _feeRangeRepository;

        /// <summary>
        /// Constructor for initializing the UpdateLoanCommandHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loan data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanApplicationCommandHandler(
            ILoanApplicationRepository LoanRepository,
            ILoanProductRepository LoanProductRepository,
            ILogger<UpdateLoanApplicationCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null,
            ILoanApplicationFeeRepository loanApplicationFeeRepository = null,
            IFeeRepository feeRepository = null,
            IFeeRangeRepository feeRangeRepository = null)
        {
            _LoanRepository = LoanRepository;
            _LoanProductRepository = LoanProductRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _loanApplicationFeeRepository=loanApplicationFeeRepository;
            _feeRepository=feeRepository;
            _feeRangeRepository=feeRangeRepository;
        }

        /// <summary>
        /// Handles the UpdateLoanCommand to update a Loan.
        /// </summary>
        /// <param name="request">The UpdateLoanCommand containing updated Loan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanApplicationDto>> Handle(UpdateLoanApplicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Loan entity to be updated from the repository
                var loanApplication = await _LoanRepository.FindAsync(request.Id);

                // Check if the Loan entity exists
                if (loanApplication != null)
                {
                    // Update Loan entity properties with values from the request
                    // Check if a LoanProduct with the same name already exists (case-insensitive)
                    var existingProductLoan = await _LoanProductRepository.FindBy(c => c.Id == loanApplication.LoanProductId).Include(x=>x.LoanTerm).FirstOrDefaultAsync();

                    // If a LoanProduct with the same name already exists, return a conflict response
                    if (existingProductLoan == null)
                    {
                        var errorMessage = $"LoanProduct {loanApplication.LoanProductId} does not exists.";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<LoanApplicationDto>.Return404(errorMessage);
                    }

                    var validator = LoanValidation.UpdateloanapplicationValidation(request, existingProductLoan, loanApplication);

                    if (!validator.Equals(""))
                    {
                        return ServiceResponse<LoanApplicationDto>.Return403(validator);
                    }
                 
                    // Step 1: Convert FeeIds to a List
                    var feeGuids = request.FeeIds.ToList();

                    // Step 2: Retrieve all fees from the database
                    var allFees = await _feeRepository.All.ToListAsync();

                    // Step 3: Manually filter using nested foreach loops (No Contains)
                    var feesBefore = new List<Fee>();

                    foreach (var fee in allFees)
                    {
                        foreach (var feeId in feeGuids)
                        {
                            if (fee.Id == feeId && fee.IsBoforeProcesing)
                            {
                                feesBefore.Add(fee);
                                break; // Exit inner loop early to optimize performance
                            }
                        }
                    }

                  
                    decimal Totalcharge = 0;

                    foreach (var feeId in request.FeeIds)
                    {
                        var fee = await _feeRepository.FindAsync(feeId);
                        if (fee == null)
                        {
                            var errorMessage = $"The fee with ID {feeId} does not exist. Please check and try again.";
                            _logger.LogError(errorMessage);
                            throw new InvalidOperationException(errorMessage);
                        }

                        var feeRange = fee.FeeBase == "Range" ?
                            await _feeRangeRepository.FindBy(x => x.AmountFrom <= request.Amount && x.AmountTo >= request.Amount && x.FeeId == feeId && x.IsDeleted == false).FirstOrDefaultAsync() :
                            await _feeRangeRepository.FindBy(policy => policy.FeeId == feeId && policy.IsDeleted == false).FirstOrDefaultAsync();

                        if (feeRange == null)
                        {
                            var errorMessage = $"No applicable charge found in the range for fee {fee.Name}. Ensure the requested amount falls within a defined range.";
                            _logger.LogError(errorMessage);
                            throw new InvalidOperationException(errorMessage);
                        }

                        var loanApplicationFeeEntity = _mapper.Map<LoanApplicationFee>(request);
                        var loanApplicationFee = await _loanApplicationFeeRepository.FindBy(x => x.FeeRangeId == feeId && x.IsDeleted == false && x.LoanApplicationId==request.Id).FirstOrDefaultAsync();

                        if (loanApplicationFee == null)
                        {
                            loanApplicationFeeEntity.Id = BaseUtilities.GenerateUniqueNumber();
                            loanApplicationFeeEntity.FeeRangeId = feeRange.Id;
                            decimal charge = BaseUtilities.GetChargeRoundedUp(request.Amount, feeRange.PercentageValue, fee.FeeBase, feeRange.Charge);
                            Totalcharge += charge;
                            loanApplicationFeeEntity.Period = fee.IsBoforeProcesing ? "Before" : "After";
                            loanApplicationFeeEntity.AmountPaid = 0;
                            loanApplicationFeeEntity.IsCashDeskPayment = fee.IsBoforeProcesing;
                            loanApplicationFeeEntity.CustomerId = request.CustomerId;
                            loanApplicationFeeEntity.Status = "Pending";
                            loanApplicationFeeEntity.FeeAmount = charge;
                            loanApplicationFeeEntity.EventCode = fee.AccountingEventCode ?? "N/A";
                            loanApplicationFeeEntity.FeeLable = fee.Name;
                            _loanApplicationFeeRepository.Add(loanApplicationFeeEntity);
                        }

                    }




                    var loanApplicationToUpdate = _mapper.Map<LoanApplication>(request);
                    // Use the repository to update the existing Loan entity
                    _LoanRepository.Update(loanApplicationToUpdate);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<LoanApplicationDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanApplicationDto>.ReturnResultWith200(_mapper.Map<LoanApplicationDto>(loanApplication));
                    _logger.LogInformation($"Loan {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Loan entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanApplicationDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Loan: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanApplicationDto>.Return500(e);
            }
        }
    }

}
