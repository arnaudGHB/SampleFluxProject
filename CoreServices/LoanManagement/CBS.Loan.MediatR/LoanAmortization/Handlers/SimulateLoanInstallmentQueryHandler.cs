using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Commands;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Validations;
using CBS.NLoan.MediatR.LoanCalculatorHelper;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanProductP;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class SimulateLoanInstallmentQueryHandler : IRequestHandler<SimulateLoanInstallementQuery, ServiceResponse<List<LoanAmortizationDto>>>
    {
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<SimulateLoanInstallmentQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly ITaxRepository _taxRepository;
        /// <summary>
        /// Constructor for initializing the GetLoanQueryHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public SimulateLoanInstallmentQueryHandler(
            IMapper mapper,
            ILogger<SimulateLoanInstallmentQueryHandler> logger,
            ITaxRepository taxRepository = null)
        {
            _mapper = mapper;
            _logger = logger;
            _taxRepository = taxRepository;
        }

        /// <summary>
        /// Handles the GetLoanQuery to retrieve a specific Loan.
        /// </summary>
        /// <param name="request">The GetLoanQuery containing Loan ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanAmortizationDto>>> Handle(SimulateLoanInstallementQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                var tax = await _taxRepository.All.ToListAsync();
               var vat=tax.Where(x=>x.IsVat).FirstOrDefault();
                if (request.Amount>=vat.SavingControlAmount)
                {
                    request.VatRate=vat.TaxRate;
                }
                var loanParameter = _mapper.Map<LoanParameters>(request);
                var result = LoanCalculator.GenerateAmortizationSchedule(loanParameter);
                var loanInstallmentDto = _mapper.Map<List<LoanAmortizationDto>>(result);
                return ServiceResponse<List<LoanAmortizationDto>>.ReturnResultWith200(loanInstallmentDto);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Loan: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<LoanAmortizationDto>>.Return500(e);
            }
        }
    }

}
