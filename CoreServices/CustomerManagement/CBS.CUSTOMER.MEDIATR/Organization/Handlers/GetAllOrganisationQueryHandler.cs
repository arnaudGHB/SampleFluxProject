using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;

namespace CBS.Organization.MEDIATR.OrganizationMediatR
{
    /// <summary>
    /// Handles the retrieval of all Organizations based on the GetAllOrganizationQuery.
    /// </summary>
    public class GetAllOrganizationQueryHandler : IRequestHandler<GetAllOrganizationQuery, ServiceResponse<List<CUSTOMER.DATA.Entity.Organization>>>
    {
        private readonly IOrganizationRepository _OrganizationRepository; // Repository for accessing Organizations data.
        private readonly IOrganizationCustomerRepository _OrganizationCustomerRepository; // Repository for accessing OrganizationCustomers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllOrganizationQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOrganizationQueryHandler.
        /// </summary>
        /// <param name="OrganizationRepository">Repository for Organizations data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllOrganizationQueryHandler(
            IOrganizationRepository OrganizationRepository,
            IOrganizationCustomerRepository OrganizationCustomerRepository,
        IMapper mapper, ILogger<GetAllOrganizationQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _OrganizationRepository = OrganizationRepository;
            _OrganizationCustomerRepository = OrganizationCustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllOrganizationQuery to retrieve all Organizations.
        /// </summary>
        /// <param name="request">The GetAllOrganizationQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CUSTOMER.DATA.Entity.Organization>>> Handle(GetAllOrganizationQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Retrieve all Organizations entities from the repository
                var entities = _mapper.Map<List<CUSTOMER.DATA.Entity.Organization>>(await _OrganizationRepository.All.ToListAsync());

               /* foreach(var entity in entities)
                {
                   

                    List<OrganizationRequirement> OrganizationRequirements= new List<OrganizationRequirement>();

                    OrganizationRequirements=await _OrganizationRequirementRepository.All.Where(e => e.OrganizationId == entity.OrganizationId).ToListAsync();
                    if (OrganizationRequirements.Any())
                    {
                        entity.ProfileRequirements = OrganizationRequirements;
                    }

                    List<CUSTOMER.DATA.Entity.OrganizationCustomer> OrganizationCustomers = new List<CUSTOMER.DATA.Entity.OrganizationCustomer>();

                    OrganizationCustomers = await _OrganizationCustomerRepository.All.Where(e => e.OrganizationId == entity.OrganizationId).ToListAsync();
                    if (OrganizationCustomers.Any())
                    {
                        entity.OrganizationCustomers = OrganizationCustomers;
                    }
                }
*/
                





                return ServiceResponse<List<CUSTOMER.DATA.Entity.Organization>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Organizations: {e.Message}");
                return ServiceResponse<List<CUSTOMER.DATA.Entity.Organization>>.Return500(e, "Failed to get all Organizations");
            }
        }
    }
}
