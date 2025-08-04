// Ignore Spelling: MEDIATR Mediat organisation

using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Global;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;

namespace CBS.GroupCustomer.MEDIATR.GroupCustomerMediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific GroupCustomer based on its unique identifier.
    /// </summary>
    public class GetSystemCustomerByIDQueryHandler : IRequestHandler<GetCustomerByIDAndProfileCommand, ServiceResponse<GetByCustomerByIdWithProfileTypeDto>>
    {
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomer data.
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Group data.
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IOrganizationRepository _OrganisationRepository; // Repository for accessing Organization data.
        private readonly IOrganizationCustomerRepository _OrganisationCustomerRepository; // Repository for accessing Organization Customer data.

        private readonly ILogger<GetSystemCustomerByIDQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCustomerByIDAndProfileQueryHandler.
        /// </summary>
        /// <param name="GroupCustomerRepository">Repository for GroupCustomer data access.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetSystemCustomerByIDQueryHandler(
            IGroupCustomerRepository GroupCustomerRepository,
            ILogger<GetSystemCustomerByIDQueryHandler> logger,
            ICustomerRepository customerRepository,
            IOrganizationRepository organizationRepository,
            IOrganizationCustomerRepository organisationCustomerRepository,
            IGroupRepository groupRepository)
        {
            _GroupCustomerRepository = GroupCustomerRepository;
            _logger = logger;
            _CustomerRepository = customerRepository;
            _OrganisationRepository = organizationRepository;
            _OrganisationCustomerRepository = organisationCustomerRepository;
            _GroupRepository = groupRepository;
        }

        /// <summary>
        /// Handles the GetCustomerByIDAndProfileQuery to retrieve a specific GroupCustomer.
        /// </summary>
        /// <param name="request">The GetCustomerByIDAndProfileQuery containing GroupCustomer ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<GetByCustomerByIdWithProfileTypeDto>> Handle(GetCustomerByIDAndProfileCommand request, CancellationToken cancellationToken)
        {
            string? errorMessage = null;
            try
            {
                // Retrieve the GroupCustomer entity with the specified ID from the repository
                var customerGroup = await _GroupCustomerRepository.FindAsync(id: request.Id);
                if (customerGroup != null)
                {

                    var result = new GetByCustomerByIdWithProfileTypeDto()
                    {
                        CreatedBy = customerGroup.CreatedBy,
                        CreationDate = customerGroup.CreatedDate,
                        CustomerId = customerGroup.CustomerId,
                        Email = "",
                        IDNumber ="",
                        Name = "",
                        Phone = "",
                        ProfileType = "Group Customer"

                    };



                    return ServiceResponse<GetByCustomerByIdWithProfileTypeDto>.ReturnResultWith200(result);
                }
                else
                {
                    var customer = await _CustomerRepository.FindAsync(request.Id);
                    if (customer != null)
                    {
                        var result = new GetByCustomerByIdWithProfileTypeDto()
                        {
                            CreatedBy = customer.CreatedBy,
                            CreationDate = customer.CreatedDate,
                            CustomerId = customer.CustomerId,
                            Email = customer.Email,
                            IDNumber = customer.IDNumber,
                            Name = customer.LastName,
                            Phone = customer.Phone,
                            ProfileType = "Customer"

                        };



                        return ServiceResponse<GetByCustomerByIdWithProfileTypeDto>.ReturnResultWith200(result);

                    }
                    else
                    {
                        var organisation = await _OrganisationRepository.FindAsync(request.Id);

                        if (organisation != null)
                        {
                            var result = new GetByCustomerByIdWithProfileTypeDto()
                            {
                                CreatedBy = organisation.CreatedBy,
                                CreationDate = organisation.CreatedDate,
                                CustomerId = organisation.OrganizationId,
                                Email = organisation.Email,
                                IDNumber = organisation.OrganizationIdentificationNumber,
                                Name = organisation.OrganizationName,
                                Phone = "",
                                ProfileType = "Organization"

                            };
                            return ServiceResponse<GetByCustomerByIdWithProfileTypeDto>.ReturnResultWith200(result);

                        }
                        else
                        {
                         
                                var group = await _GroupRepository.FindAsync(request.Id);

                                if (group != null)
                                {
                                    var result = new GetByCustomerByIdWithProfileTypeDto()
                                    {
                                        CreatedBy = group.CreatedBy,
                                        CreationDate = group.CreatedDate,
                                        //Email = group.Email,
                                        IDNumber = "",
                                        Name = group.GroupName,
                                        //Phone = group.Phone,
                                        ProfileType = "Group"

                                    };
                                    return ServiceResponse<GetByCustomerByIdWithProfileTypeDto>.ReturnResultWith200(result);

                                }
                                else
                                {

                                // If the Customer entity was not found, log the error and return a 404 Not Found response
                                _logger.LogError("Customer not found.");
                                return ServiceResponse<GetByCustomerByIdWithProfileTypeDto>.Return404();
                                }

                            

                        }

                       

                    }


                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting GroupCustomer: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<GetByCustomerByIdWithProfileTypeDto>.Return500(e);
            }
        }



    }

}
