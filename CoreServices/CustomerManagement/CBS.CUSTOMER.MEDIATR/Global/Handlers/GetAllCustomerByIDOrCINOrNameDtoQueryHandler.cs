// Ignore Spelling: MEDIATR Mediat Dto

using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Global;

namespace CBS.GroupCustomer.MEDIATR.GroupCustomerMediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific GroupCustomer , Customer, Organization, OrganizationCustomer, Group  based on its unique identifier.
    /// </summary>
    public class GetAllCustomerByIDOrCINOrNameDtoQueryHandler : IRequestHandler<GetCustomerByIDOrCINOrNameCommand, ServiceResponse<List<GetCustomerByIDOrCINOrNameDto>>>
    {
        // Repositories for data access
        private readonly IGroupCustomerRepository _GroupCustomerRepository;
        private readonly IGroupRepository _GroupRepository;
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IOrganizationRepository _OrganisationRepository;
        private readonly IOrganizationCustomerRepository _OrganisationCustomerRepository;

        // Logger for logging handler actions and errors
        private readonly ILogger<GetAllCustomerByIDOrCINOrNameDtoQueryHandler> _logger;

        /// <summary>
        /// Constructor for initializing the GetAllCustomerByIDOrCINOrNameDtoQueryHandler.
        /// </summary>
        /// <param name="GroupCustomerRepository">Repository for GroupCustomer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="customerRepository">Repository for Customer data access.</param>
        /// <param name="organizationRepository">Repository for Organization data access.</param>
        /// <param name="organisationCustomerRepository">Repository for Organization Customer data access.</param>
        /// <param name="groupRepository">Repository for Group data access.</param>
        public GetAllCustomerByIDOrCINOrNameDtoQueryHandler(
            IGroupCustomerRepository GroupCustomerRepository,
            ILogger<GetAllCustomerByIDOrCINOrNameDtoQueryHandler> logger,
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
        public async Task<ServiceResponse<List<GetCustomerByIDOrCINOrNameDto>>> Handle(GetCustomerByIDOrCINOrNameCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;

            try
            {
                // List to store results of GetAllSystemCustomer for different customer types
                List<GetCustomerByIDOrCINOrNameDto> getAllSystemCustomers = new List<GetCustomerByIDOrCINOrNameDto>();

                // Retrieve GroupCustomer entities from the repository
                var customerGroups = await _GroupCustomerRepository.All.Where(x => x.IsDeleted == false && x.CustomerId == request.SearchItem) .ToListAsync();

                // Process GroupCustomer entities
                if (customerGroups.Count > 0)
                {
                    customerGroups.ForEach(e =>
                    {
                        var result = new GetCustomerByIDOrCINOrNameDto()
                        {
                            CreatedBy = e.CreatedBy,
                            CreationDate = e.CreatedDate,
                            CustomerId = e.CustomerId,
                            Email ="",
                            IDNumber = "",
                            Name = "",
                            Phone = "",
                            ProfileType = "Group Customer",
                            active = true,
                        };

                        getAllSystemCustomers.Add(result);
                    });
                }

                // Retrieve Customer entities from the repository
                var customers = await _CustomerRepository.All.Where(x => x.IsDeleted == false
                && (

              (
                                (x.CustomerId == request.SearchItem) ||
             (x.FirstName != null && x.FirstName.ToUpper() == request.SearchItem) ||

                (x.LastName != null && x.LastName.ToUpper() == request.SearchItem) ||

                  (x.IDNumber != null && x.IDNumber == request.SearchItem)
                  )

               )).ToListAsync();

                // Process Customer entities
                if (customers.Count > 0)
                {
                    customers.ForEach(e =>
                    {
                        var result = new GetCustomerByIDOrCINOrNameDto()
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
                var organisations = await _OrganisationRepository.All.Where(x => x.IsDeleted == false

                  && (

              (x.OrganizationId == request.SearchItem) ||

                (x.OrganizationName != null && x.OrganizationName.ToUpper() == request.SearchItem) ||

                  (x.OrganizationIdentificationNumber != null && x.OrganizationIdentificationNumber == request.SearchItem)
                  )

                ).ToListAsync();
                // Process Organization entities
                if (organisations.Count > 0)
                {

                    organisations.ForEach(e =>
                    {
                        var result = new GetCustomerByIDOrCINOrNameDto()
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

                // Retrieve Organization Customer entities from the repository
             

                // Retrieve Group entities from the repository
                var groups = await _GroupRepository.All.Where(x => x.IsDeleted == false
                  && (

             
              (x.GroupId == request.SearchItem) ||

                (x.GroupName != null && x.GroupName.ToUpper() == request.SearchItem) ||

                  (x.GroupType != null && x.GroupTypeId == request.SearchItem)
                  )


                ).ToListAsync();

                if (groups != null)
                {
                    // Process Group entities
                    groups.ForEach(e =>
                    {
                        var result = new GetCustomerByIDOrCINOrNameDto()
                        {
                            CreatedBy = e.CreatedBy,
                            CreationDate = e.CreatedDate,
                            //Email = e.Email,
                            IDNumber = "",
                            Name = e.GroupName,
                            //Phone = e.Phone,
                            ProfileType = "Group",
                            active = e.Active

                        };

                        getAllSystemCustomers.Add(result);
                    });


                }

                // Similar processing for Organization, Organization Customer, and Group entities

                // Return the result with a 200 status code
                return ServiceResponse<List<GetCustomerByIDOrCINOrNameDto>>.ReturnResultWith200(getAllSystemCustomers);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting customer information: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<GetCustomerByIDOrCINOrNameDto>>.Return500(e);
            }
        }
    }
}

