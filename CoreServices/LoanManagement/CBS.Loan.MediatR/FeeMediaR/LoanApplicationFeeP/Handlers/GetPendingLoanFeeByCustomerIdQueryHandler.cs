using AutoMapper;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Queries;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using CBS.NLoan.Repository.LoanApplicationP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetPendingLoanFeeByCustomerIdQueryHandler : IRequestHandler<GetPendingLoanFeeByCustomerIdQuery, ServiceResponse<List<LoanApplicationFeeDto>>>
    {
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository; // Repository for accessing LoanApplicationFees data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetPendingLoanFeeByCustomerIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly ILoanApplicationRepository _loanApplicationRepository; // Repository for accessing LoanApplicationFees data.

        /// <summary>
        /// Constructor for initializing the GetPendingLoanFeeByCustomerIdQueryHandler.
        /// </summary>
        /// <param name="loanApplicationFeeRepository">Repository for LoanApplicationFees data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetPendingLoanFeeByCustomerIdQueryHandler(
            ILoanApplicationFeeRepository loanApplicationFeeRepository,
            IMapper mapper,
            ILogger<GetPendingLoanFeeByCustomerIdQueryHandler> logger,
            ILoanApplicationRepository loanApplicationRepository = null)
        {
            // Assign provided dependencies to local variables.
            _loanApplicationFeeRepository = loanApplicationFeeRepository;
            _mapper = mapper;
            _logger = logger;
            _loanApplicationRepository = loanApplicationRepository;
        }

        public async Task<ServiceResponse<List<LoanApplicationFeeDto>>> Handle(GetPendingLoanFeeByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Projecting only the necessary fields directly to DTO
                var entities = await _loanApplicationFeeRepository.All
                    .Where(fee => fee.CustomerId == request.CustomerId
                                && !fee.IsDeleted
                                && fee.Status == LoanApplicationStatus.Pending.ToString()
                                && fee.IsCashDeskPayment
                                && _loanApplicationRepository.All.Any(loan => loan.CustomerId == fee.CustomerId && loan.Status == LoanApplicationStatus.Pending.ToString()))
                    .ToListAsync(cancellationToken);


                // Map the result to DTO
                var result = _mapper.Map<List<LoanApplicationFeeDto>>(entities);

                return ServiceResponse<List<LoanApplicationFeeDto>>.ReturnResultWith200(result);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response
                _logger.LogError($"Failed to get all LoanApplicationFees: {e.Message}");
                return ServiceResponse<List<LoanApplicationFeeDto>>.Return500(e, "Failed to get all LoanApplicationFees");
            }
        }


    }
}
