using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanInstallmentQueryHandler : IRequestHandler<GetLoanInstallmentQuery, ServiceResponse<List<LoanAmortizationDto>>>
    {
        private readonly ILoanApplicationRepository _LoanRepository; // Repository for accessing Loan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanInstallmentQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanQueryHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanInstallmentQueryHandler(
            ILoanApplicationRepository LoanRepository,
            IMapper mapper,
            ILogger<GetLoanInstallmentQueryHandler> logger)
        {
            _LoanRepository = LoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanQuery to retrieve a specific Loan.
        /// </summary>
        /// <param name="request">The GetLoanQuery containing Loan ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanAmortizationDto>>> Handle(GetLoanInstallmentQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Loan entity with the specified ID from the repository
                var entity = await _LoanRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the Loan entity to LoanDto and return it with a success response
                    var LoanDto = _mapper.Map<LoanApplicationDto>(entity);

                    List<LoanAmortizationDto> Installments = new List<LoanAmortizationDto>();

                    //for (int i = 0; i < LoanDto.NumberOfInstallment; i++)
                    //{
                    //    LoanInstallmentDto installment = new LoanInstallmentDto();
                    //    var capital = LoanDto.Amount / LoanDto.NumberOfInstallment;
                    //    var interest = capital * LoanDto.InterestRate / 100;
                    //    installment.Id = i + 1 + "";
                    //    installment.LoanId = installment.LoanId;
                    //    installment.Status = installment.Status;
                    //    installment.CapitalAmount = capital;
                    //    installment.BalanceAmount = 0;
                    //    installment.InterestAmount = interest;
                    //    installment.TotalAmount = capital + interest;
                    //    installment.Date = DateTime.Now;
                    //    Installments.Add(installment);
                    //}

                    return ServiceResponse<List<LoanAmortizationDto>>.ReturnResultWith200(Installments);
                }
                else
                {
                    // If the Loan entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Loan not found.");
                    return ServiceResponse<List<LoanAmortizationDto>>.Return404();
                }
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
