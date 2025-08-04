using AutoMapper;
using CBS.NLoan.Data.Dto.CustomerLoanAccountP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Queries;
using CBS.NLoan.Repository.CustomerLoanAccountP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllCustomerLoanAccountHandler : IRequestHandler<GetAllCustomerLoanAccountQuery, ServiceResponse<List<CustomerLoanAccountDto>>>
    {
        private readonly ICustomerLoanAccountRepository _CustomerLoanAccountRepository; // Repository for accessing CustomerLoanAccounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCustomerLoanAccountHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllCustomerLoanAccountQueryHandler.
        /// </summary>
        /// <param name="CustomerLoanAccountRepository">Repository for CustomerLoanAccounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCustomerLoanAccountHandler(
            ICustomerLoanAccountRepository CustomerLoanAccountRepository,
            IMapper mapper, ILogger<GetAllCustomerLoanAccountHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CustomerLoanAccountRepository = CustomerLoanAccountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllCustomerLoanAccountQuery to retrieve all CustomerLoanAccounts.
        /// </summary>
        /// <param name="request">The GetAllCustomerLoanAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CustomerLoanAccountDto>>> Handle(GetAllCustomerLoanAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all CustomerLoanAccounts entities from the repository
                var entities = await _CustomerLoanAccountRepository.All.ToListAsync();
                return ServiceResponse<List<CustomerLoanAccountDto>>.ReturnResultWith200(_mapper.Map<List<CustomerLoanAccountDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all CustomerLoanAccounts: {e.Message}");
                return ServiceResponse<List<CustomerLoanAccountDto>>.Return500(e, "Failed to get all CustomerLoanAccounts");
            }
        }
    }
}
