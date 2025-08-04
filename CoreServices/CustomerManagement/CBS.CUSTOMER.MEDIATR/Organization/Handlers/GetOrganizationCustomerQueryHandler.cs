using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using Microsoft.EntityFrameworkCore;

namespace CBS.OrganizationCustomer.MEDIATR.OrganizationCustomerMediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific OrganizationCustomer based on its unique identifier.
    /// </summary>
    public class GetOrganizationCustomerQueryHandler : IRequestHandler<GetOrganizationCustomerQuery, ServiceResponse<CUSTOMER.DATA.Entity.OrganizationCustomer>>
    {
        private readonly IOrganizationCustomerRepository _OrganizationCustomerRepository; // Repository for accessing OrganizationCustomer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetOrganizationCustomerQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetOrganizationCustomerQueryHandler.
        /// </summary>
        /// <param name="OrganizationCustomerRepository">Repository for OrganizationCustomer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetOrganizationCustomerQueryHandler(
            IOrganizationCustomerRepository OrganizationCustomerRepository,
            IMapper mapper,
            ILogger<GetOrganizationCustomerQueryHandler> logger)
        {
            _OrganizationCustomerRepository = OrganizationCustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetOrganizationCustomerQuery to retrieve a specific OrganizationCustomer.
        /// </summary>
        /// <param name="request">The GetOrganizationCustomerQuery containing OrganizationCustomer ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CUSTOMER.DATA.Entity.OrganizationCustomer>> Handle(GetOrganizationCustomerQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the OrganizationCustomer entity with the specified ID from the repository
                var entity = await _OrganizationCustomerRepository.FindAsync(request.OrganizationCustomerId);
                if (entity != null)
                {
                   
                 
                    var OrganizationCustomer = _mapper.Map<CUSTOMER.DATA.Entity.OrganizationCustomer>(entity);

                    return ServiceResponse<CUSTOMER.DATA.Entity.OrganizationCustomer>.ReturnResultWith200(OrganizationCustomer);
                }
                else
                {
                    // If the OrganizationCustomer entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("OrganizationCustomer not found.");
                    return ServiceResponse<CUSTOMER.DATA.Entity.OrganizationCustomer>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting OrganizationCustomer: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CUSTOMER.DATA.Entity.OrganizationCustomer>.Return500(e);
            }
        }
    }

}
