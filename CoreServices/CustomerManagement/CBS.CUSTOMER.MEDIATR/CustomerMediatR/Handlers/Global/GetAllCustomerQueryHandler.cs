// Ignore Spelling: MEDIATR Mediat organisation

using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;

namespace CBS.GroupCustomer.MEDIATR.GroupCustomerMediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific GroupCustomer based on its unique identifier.
    /// </summary>
    public class GetAllCustomerQueryHandler : IRequestHandler<GetAllSystemCustomerQuery, ServiceResponse<List<GetAllSystemCustomer>>>
    {
        // Repositories for data access
        private readonly IGroupCustomerRepository _GroupCustomerRepository;
        private readonly IGroupRepository _GroupRepository;
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IOrganizationRepository _OrganisationRepository;
        private readonly IOrganizationCustomerRepository _OrganisationCustomerRepository;

        // Logger for logging handler actions and errors
        private readonly ILogger<GetAllCustomerQueryHandler> _logger;

        /// <summary>
        /// Constructor for initializing the GetAllCustomerQueryHandler.
        /// </summary>
        /// <param name="GroupCustomerRepository">Repository for GroupCustomer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="customerRepository">Repository for Customer data access.</param>
        /// <param name="organizationRepository">Repository for Organization data access.</param>
        /// <param name="organisationCustomerRepository">Repository for Organization Customer data access.</param>
        /// <param name="groupRepository">Repository for Group data access.</param>
        public GetAllCustomerQueryHandler(
            IGroupCustomerRepository GroupCustomerRepository,
            ILogger<GetAllCustomerQueryHandler> logger,
            ICustomerRepository customerRepository,
            IOrganizationRepository organizationRepository,
            IOrganizationCustomerRepository organisationCustomerRepository,
            IGroupRepository groupRepository)
        {
            // Inject repositories and logger into the handler
            _GroupCustomerRepository = GroupCustomerRepository;
            _logger = logger;
            _CustomerRepository = customerRepository;
            _OrganisationRepository = organizationRepository;
            _OrganisationCustomerRepository = organisationCustomerRepository;
            _GroupRepository = groupRepository;
        }

        /// <summary>
        /// Handles the GetAllSystemCustomerQuery to retrieve information about various types of customers.
        /// </summary>
        /// <param name="request">The GetAllSystemCustomerQuery containing parameters for customer retrieval.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetAllSystemCustomer>>> Handle(GetAllSystemCustomerQuery request, CancellationToken cancellationToken)
        {
            string? errorMessage = null;

            try
            {
                // List to store results of GetAllSystemCustomer for different customer types
                List<GetAllSystemCustomer> getAllSystemCustomers = new List<GetAllSystemCustomer>();

                // Retrieve GroupCustomer entities from the repository
                var customerGroups = await _GroupCustomerRepository.All.Where(x => x.IsDeleted == false).ToListAsync();

                // Process GroupCustomer entities
                if (customerGroups.Count > 0)
                {
                    customerGroups.ForEach(e =>
                    {
                        var result = new GetAllSystemCustomer()
                        {
                            CreatedBy = e.CreatedBy,
                            CreationDate = e.CreatedDate,
                            CustomerId = e.CustomerId,
                            Email = "",
                            IDNumber = "",
                            Name = "",
                            Phone = "",
                            ProfileType = "Group Customer",
                            active=null
                        };

                        getAllSystemCustomers.Add(result);
                    });
                }

                // Retrieve Customer entities from the repository
                var customers = await _CustomerRepository.All.Where(x => x.IsDeleted == false).ToListAsync();

                // Process Customer entities
                if (customers.Count > 0)
                {
                    customers.ForEach(e =>
                    {
                        var result = new GetAllSystemCustomer()
                        {
                            CreatedBy = e.CreatedBy,
                            CreationDate = e.CreatedDate,
                            CustomerId = e.CustomerId,
                            Email = e.Email,
                            IDNumber = e.IDNumber,
                            Name = e.FirstName + " " + e.LastName,
                            Phone = e.Phone,
                            ProfileType = "Customer",
                            active = e.Active
                        };

                        getAllSystemCustomers.Add(result);
                    });
                }
                // Retrieve Organization entities from the repository
                var organisations = await _OrganisationRepository.All.Where(x => x.IsDeleted == false).ToListAsync();
                // Process Organization entities
                if (organisations.Count > 0)
                {

                    organisations.ForEach(e =>
                    {
                        var result = new GetAllSystemCustomer()
                        {
                            CreatedBy = e.CreatedBy,
                            CreationDate = e.CreatedDate,
                            CustomerId = e.OrganizationId,
                            Email = e.Email,
                            IDNumber = e.OrganizationIdentificationNumber,
                            Name = e.OrganizationName,
                            Phone = "",
                            ProfileType = "Organization",
                            active = e.Active

                        };

                        getAllSystemCustomers.Add(result);
                    });

                }

              

                // Retrieve Group entities from the repository
                var groups = await _GroupRepository.All.Where(x => x.IsDeleted == false).ToListAsync();

                if (groups != null)
                {
                    // Process Group entities
                    groups.ForEach(e =>
                    {
                        var result = new GetAllSystemCustomer()
                        {
                            CreatedBy = e.CreatedBy,
                            CreationDate = e.CreatedDate,
                            CustomerId = e.OrganizationId,
                            Email = e.Email,
                            IDNumber = "",
                            Name = e.GroupName,
                            Phone = e.PersonalPhone,
                            ProfileType = "Group",
                            active = e.Active

                        };

                        getAllSystemCustomers.Add(result);
                    });


                }

                // Similar processing for Organization, Organization Customer, and Group entities

                // Return the result with a 200 status code
                return ServiceResponse<List<GetAllSystemCustomer>>.ReturnResultWith200(getAllSystemCustomers);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting customer information: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<GetAllSystemCustomer>>.Return500(e);
            }
        }
    }
}

