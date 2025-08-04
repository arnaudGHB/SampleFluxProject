using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the retrieval of all Customers based on the CMoneyMembersPagginationQuery.
    /// </summary>
    public class SearchByAnyCriterialQueryHandler : IRequestHandler<SearchByAnyCriterialQuery, ServiceResponse<CustomersList>>
    {
        private readonly ICustomerRepository _customerRepository; // Repository for accessing Customers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<SearchByAnyCriterialQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the SearchByAnyCriterialQueryHandler.
        /// </summary>
        /// <param name="customerRepository">Repository for Customers data access.</param>
        /// <param name="documentBaseUrlRepository">Repository for document base URL access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public SearchByAnyCriterialQueryHandler(
            ICustomerRepository customerRepository,
            IMapper mapper,
            ILogger<SearchByAnyCriterialQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the CMoneyMembersPagginationQuery to retrieve paginated Customers.
        /// </summary>
        /// <param name="request">The CMoneyMembersPagginationQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomersList>> Handle(SearchByAnyCriterialQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch paginated customers from the repository based on search criteria.
                CustomersList customersList;

                if (request.CustomerResource.IsByBranch)
                {
                    customersList = await _customerRepository.GetCustomersAsyncByBranch(request.CustomerResource);
                }
                else
                {
                    customersList = await _customerRepository.GetCustomersAsync(request.CustomerResource);
                }

                // Return a successful response with the paginated customers list.
                return ServiceResponse<CustomersList>.ReturnResultWith200(customersList);
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response.
                _logger.LogError($"Failed to get paginated Customers: {e.Message}");
                return ServiceResponse<CustomersList>.Return500(e, "Failed to get paginated Customers");
            }
        }
    }

    // Define the paginated response class

}
