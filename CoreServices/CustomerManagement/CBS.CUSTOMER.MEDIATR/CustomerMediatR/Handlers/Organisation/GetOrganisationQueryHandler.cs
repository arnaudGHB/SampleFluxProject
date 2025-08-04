using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.REPOSITORY;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;

namespace CBS.Organization.MEDIATR.OrganizationMediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Organization based on its unique identifier.
    /// </summary>
    public class GetOrganizationQueryHandler : IRequestHandler<GetOrganizationQuery, ServiceResponse<CUSTOMER.DATA.Entity.Organization>>
    {
        private readonly IOrganizationRepository _OrganizationRepository; // Repository for accessing Organization data.
        private readonly IOrganizationCustomerRepository _OrganizationCustomerRepository; // Repository for accessing OrganizationCustomers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetOrganizationQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetOrganizationQueryHandler.
        /// </summary>
        /// <param name="OrganizationRepository">Repository for Organization data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetOrganizationQueryHandler(
            IOrganizationRepository OrganizationRepository,
             IOrganizationCustomerRepository OrganizationCustomerRepository,
            IMapper mapper,
            ILogger<GetOrganizationQueryHandler> logger)
        {
            _OrganizationRepository = OrganizationRepository;
           _OrganizationCustomerRepository = OrganizationCustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetOrganizationQuery to retrieve a specific Organization.
        /// </summary>
        /// <param name="request">The GetOrganizationQuery containing Organization ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CUSTOMER.DATA.Entity.Organization>> Handle(GetOrganizationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Organization entity with the specified ID from the repository
                var entity = await _OrganizationRepository.FindAsync(request.OrganizationId);
                if (entity != null)
                {
                    // Map the Organization entity to Organization and return it with a success response
                    var Organization = _mapper.Map<CUSTOMER.DATA.Entity.Organization>(entity);

                 

                    List<CUSTOMER.DATA.Entity.OrganizationCustomer> OrganizationCustomers = new List<CUSTOMER.DATA.Entity.OrganizationCustomer>();

                    OrganizationCustomers = await _OrganizationCustomerRepository.All.Where(e => e.OrganizationId == entity.OrganizationId).ToListAsync();
                    if (OrganizationCustomers.Any())
                    {
                        entity.OrganizationCustomers = OrganizationCustomers;
                    }

                    return ServiceResponse<CUSTOMER.DATA.Entity.Organization>.ReturnResultWith200(Organization);
                }
                else
                {
                    // If the Organization entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Organization not found.");
                    return ServiceResponse<CUSTOMER.DATA.Entity.Organization>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Organization: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<CUSTOMER.DATA.Entity.Organization>.Return500(e);
            }
        }
    }

}
