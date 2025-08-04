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
using CBS.CUSTOMER.REPOSITORY.GroupRepo;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the retrieval of all Customers based on the SearchByAnyCriterialGroupQuery.
    /// </summary>
    public class SearchByAnyCriterialGroupQueryHandler : IRequestHandler<SearchByAnyCriterialGroupQuery, ServiceResponse<GroupsList>>
    {
        private readonly IGroupRepository _customerRepository; // Repository for accessing Customers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<SearchByAnyCriterialGroupQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the SearchByAnyCriterialGroupQueryHandler.
        /// </summary>
        /// <param name="customerRepository">Repository for Customers data access.</param>
        /// <param name="documentBaseUrlRepository">Repository for document base URL access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public SearchByAnyCriterialGroupQueryHandler(
            IGroupRepository customerRepository,
            IMapper mapper,
            ILogger<SearchByAnyCriterialGroupQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the SearchByAnyCriterialGroupQuery to retrieve paginated Customers.
        /// </summary>
        /// <param name="request">The SearchByAnyCriterialGroupQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<GroupsList>> Handle(SearchByAnyCriterialGroupQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Fetch paginated customers from the repository based on search criteria.
                GroupsList customersList;

                if (request.ResourceParameter.IsByBranch)
                {
                    customersList = await _customerRepository.GetGroupsAsyncByBranch(request.ResourceParameter);
                }
                else
                {
                    customersList = await _customerRepository.GetGroupsAsync(request.ResourceParameter);
                }

                // Return a successful response with the paginated customers list.
                return ServiceResponse<GroupsList>.ReturnResultWith200(customersList);
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response.
                _logger.LogError($"Failed to get paginated groups: {e.Message}");
                return ServiceResponse<GroupsList>.Return500(e, "Failed to get paginated groups");
            }
        }
    }

    // Define the paginated response class

}
