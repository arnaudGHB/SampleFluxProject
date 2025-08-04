using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace CBS.OrganisationCustomer.MEDIATR.OrganisationCustomerMediatR
{
    /// <summary>
    /// Handles the retrieval of all OrganisationCustomers based on the GetAllOrganisationCustomerQuery.
    /// </summary>
    public class GetAllOrganizationCustomerQueryHandler : IRequestHandler<GetAllOrganizationCustomerQuery, ServiceResponse<List<CUSTOMER.DATA.Entity.OrganizationCustomer>>>
    {
        private readonly IOrganizationCustomerRepository _OrganisationCustomerRepository; // Repository for accessing OrganisationCustomers data.
         private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllOrganizationCustomerQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOrganisationCustomerQueryHandler.
        /// </summary>
        /// <param name="OrganisationCustomerRepository">Repository for OrganisationCustomers data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllOrganizationCustomerQueryHandler(
            IOrganizationCustomerRepository OrganisationCustomerRepository,
            IMapper mapper, ILogger<GetAllOrganizationCustomerQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _OrganisationCustomerRepository = OrganisationCustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllOrganisationCustomerQuery to retrieve all OrganisationCustomers.
        /// </summary>
        /// <param name="request">The GetAllOrganisationCustomerQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CUSTOMER.DATA.Entity.OrganizationCustomer>>> Handle(GetAllOrganizationCustomerQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Retrieve all OrganisationCustomers entities from the repository
                var entities = _mapper.Map<List<CUSTOMER.DATA.Entity.OrganizationCustomer>>(await _OrganisationCustomerRepository.All.ToListAsync());

/*
                if (entities.Any())
                {
                    foreach (var entity in entities)
                    {

                        List<CUSTOMER.DATA.Entity.OrganizationCustomerRequirement> customerRequirements = new List<CUSTOMER.DATA.Entity.OrganizationCustomerRequirement>();

                        customerRequirements = await _OrganisationCustomerRequirementRepository.All.Where(e => e.CustomerId == entity.CustomerId).ToListAsync();
                        if (customerRequirements.Any())
                        {
                            entity.OrganistionCustomerRequirements = customerRequirements;
                        }
                    }
                }
*/





                return ServiceResponse<List<CUSTOMER.DATA.Entity.OrganizationCustomer>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all OrganisationCustomers: {e.Message}");
                return ServiceResponse<List<CUSTOMER.DATA.Entity.OrganizationCustomer>>.Return500(e, "Failed to get all OrganisationCustomers");
            }
        }
    }
}
