using AutoMapper;
using CBS.NLoan.Data.Dto.CustomerLoanAccountP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Queries;
using CBS.NLoan.Repository.CustomerLoanAccountP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetCustomerLoanAccountHandler : IRequestHandler<GetCustomerLoanAccountQuery, ServiceResponse<CustomerLoanAccountDto>>
    {
        private readonly ICustomerLoanAccountRepository _CustomerLoanAccountRepository; // Repository for accessing CustomerLoanAccount data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCustomerLoanAccountHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCustomerLoanAccountQueryHandler.
        /// </summary>
        /// <param name="CustomerLoanAccountRepository">Repository for CustomerLoanAccount data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCustomerLoanAccountHandler(
            ICustomerLoanAccountRepository CustomerLoanAccountRepository,
            IMapper mapper,
            ILogger<GetCustomerLoanAccountHandler> logger)
        {
            _CustomerLoanAccountRepository = CustomerLoanAccountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetCustomerLoanAccountQuery to retrieve a specific CustomerLoanAccount.
        /// </summary>
        /// <param name="request">The GetCustomerLoanAccountQuery containing CustomerLoanAccount ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerLoanAccountDto>> Handle(GetCustomerLoanAccountQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the CustomerLoanAccount entity with the specified ID from the repository
                var entity = await _CustomerLoanAccountRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the CustomerLoanAccount entity to CustomerLoanAccountDto and return it with a success response
                    var CustomerLoanAccountDto = _mapper.Map<CustomerLoanAccountDto>(entity);
                    return ServiceResponse<CustomerLoanAccountDto>.ReturnResultWith200(CustomerLoanAccountDto);
                }
                else
                {
                    // If the CustomerLoanAccount entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("CustomerLoanAccount not found.");
                    return ServiceResponse<CustomerLoanAccountDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting CustomerLoanAccount: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CustomerLoanAccountDto>.Return500(e);
            }
        }
    }

}
