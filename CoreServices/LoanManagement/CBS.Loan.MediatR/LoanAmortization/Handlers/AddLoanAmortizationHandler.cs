using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Commands;
using CBS.NLoan.MediatR.LoanCalculatorHelper;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanAmortizationP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.Handlers
{
    public class AddLoanAmortizationHandler : IRequestHandler<AddLoanAmortizationCommand, ServiceResponse<List<LoanAmortizationDto>>>
    {
        private readonly ILoanAmortizationRepository _loanAmortizationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AddLoanAmortizationHandler> _logger;
        private readonly IUnitOfWork<LoanContext> _unitOfWork;
        private readonly ILoanApplicationRepository _loanApplicationRepository;

        /// <summary>
        /// Constructor for initializing the AddLoanAmortizationHandler.
        /// </summary>
        /// <param name="loanAmortizationRepository">Repository for LoanAmortization data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="unitOfWork">Unit of Work for database transactions.</param>
        /// <param name="loanApplicationRepository">Optional repository for LoanApplication data access.</param>
        public AddLoanAmortizationHandler(
            ILoanAmortizationRepository loanAmortizationRepository,
            IMapper mapper,
            ILogger<AddLoanAmortizationHandler> logger,
            IUnitOfWork<LoanContext> unitOfWork,
            ILoanApplicationRepository loanApplicationRepository = null)
        {
            _loanAmortizationRepository = loanAmortizationRepository ?? throw new ArgumentNullException(nameof(loanAmortizationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _loanApplicationRepository = loanApplicationRepository;
        }

        /// <summary>
        /// Handles the AddLoanAmortizationCommand to add a new LoanAmortization.
        /// </summary>
        /// <param name="request">The AddLoanAmortizationCommand containing LoanAmortization data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanAmortizationDto>>> Handle(AddLoanAmortizationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a LoanAmortization with the same name already exists (case-insensitive)
                var existingLoanAmortization = await _loanAmortizationRepository.FindBy(c => c.LoanId == request.LoanId).ToListAsync();

                // If a LoanAmortization with the same name already exists, return a conflict response
                if (existingLoanAmortization.Any())
                {
                    var errorMessage = $"LoanAmortization already exists for loan with id {request.LoanId}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<List<LoanAmortizationDto>>.Return409(errorMessage);
                }
                var loanApplication = new LoanApplication();
                if (request.LoanApplication==null)
                {
                    // Check if the associated loan application exists
                    loanApplication = await _loanApplicationRepository.AllIncluding(x => x.LoanProduct).Include(x => x.LoanProduct.LoanProductRepaymentCycles).FirstOrDefaultAsync(x => x.Id == request.LoanApplicationId);
                }
                else
                {
                    loanApplication=request.LoanApplication;
                }
                // If the associated loan application does not exist, return a not found response
                if (loanApplication == null)
                {
                    var errorMessage = $"Loan application with id {request.LoanApplicationId} does not exist.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<List<LoanAmortizationDto>>.Return404(errorMessage);
                }

                if (loanApplication.LoanApplicationType==LoanApplicationTypes.Restructure.ToString() || loanApplication.LoanApplicationType == LoanApplicationTypes.Reschedule.ToString())
                {
                    var amortizations = await _loanAmortizationRepository.FindBy(x => x.LoanId == request.LoanId).ToListAsync();
                    var lewLoanAmortimzations = new List<LoanAmortization>();
                    foreach (var loanAmortization in amortizations)
                    {
                        loanAmortization.IsStructured = true;
                        loanAmortization.LoanStructuringStatus = loanApplication.LoanApplicationType;
                        lewLoanAmortimzations.Add(loanAmortization);
                    }
                    _loanAmortizationRepository.UpdateRange(lewLoanAmortimzations);

                }
                // Generate loan parameters for amortization
                var loanParameters = LoanValidation.AddLoanAmortizationCommandRequest(loanApplication, loanApplication.VatRate, 0, request.LoanId);

                // Generate amortization schedule
                var loanAmortizations = LoanCalculator.GenerateAmortizationSchedule(loanParameters);

                // Add generated amortizations to the repository and save changes
                _loanAmortizationRepository.AddRange(loanAmortizations);
                //await _unitOfWork.SaveAsync();

                // Map the LoanAmortization entities to LoanAmortizationDto objects
                var loanAmortizationDto = _mapper.Map<List<LoanAmortizationDto>>(loanAmortizations);

                // Return success response with the mapped LoanAmortizationDto objects
                return ServiceResponse<List<LoanAmortizationDto>>.ReturnResultWith200(loanAmortizationDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving LoanAmortization: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<List<LoanAmortizationDto>>.Return500(e);
            }
        }
    }


}
