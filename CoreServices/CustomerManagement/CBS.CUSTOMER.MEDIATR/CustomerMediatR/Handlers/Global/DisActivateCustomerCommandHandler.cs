using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Handlers.Global
{
    /// <summary>
    /// Handles the command to DisActivate a Customer based on DisActivateCustomerCommand.
    /// </summary>
    public class DisActivateCustomerCommandHandler : IRequestHandler<DisActivateCustomerCommand, ServiceResponse<bool>>
    {


        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomer data.
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Group data.
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IOrganizationRepository _OrganisationRepository; // Repository for accessing Organization data.
        private readonly IOrganizationCustomerRepository _OrganisationCustomerRepository; // Repository for accessing Organization 
        private readonly ILogger<DisActivateCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DisActivateCustomerCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DisActivateCustomerCommandHandler(
            ICustomerRepository CustomerRepository,
            ILogger<DisActivateCustomerCommandHandler> logger
, IUnitOfWork<POSContext> uow,
IGroupCustomerRepository groupCustomerRepository,
IGroupRepository groupRepository,
IOrganizationRepository organisationRepository,
IOrganizationCustomerRepository organisationCustomerRepository)
        {
            _CustomerRepository = CustomerRepository;
            _logger = logger;
            _uow = uow;
            _GroupCustomerRepository = groupCustomerRepository;
            _GroupRepository = groupRepository;
            _OrganisationRepository = organisationRepository;
            _OrganisationCustomerRepository = organisationCustomerRepository;
        }

        /// <summary>
        /// Handles the DisActivateCustomerCommand to DisActivate a Customer.
        /// </summary>
        /// <param name="request">The DisActivateCustomerCommand containing Customer ID to be DisActivated.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DisActivateCustomerCommand request, CancellationToken cancellationToken)
        {
            string? errorMessage = null;
            try
            {
                // Check if the Customer entity with the specified ID exists
                var existingCustomer = await _CustomerRepository.FindAsync(request.CustomerId);
                if (existingCustomer != null)
                {

                    existingCustomer.Active = request.activate;

                    _CustomerRepository.Update(existingCustomer);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<bool>.Return500();
                    }
                    return ServiceResponse<bool>.ReturnResultWith200(true);

                }
                else
                {
                     
                            // Check if the Group entity with the specified ID exists
                            var existingGroup = await _GroupRepository.FindAsync(request.CustomerId);

                            if (existingGroup != null)
                            {
                                existingGroup.Active = request.activate;

                                _GroupRepository.Update(existingGroup);
                                if (await _uow.SaveAsync() <= 0)
                                {
                                    return ServiceResponse<bool>.Return500();
                                }
                                return ServiceResponse<bool>.ReturnResultWith200(true);
                            }
                            else
                            {
                                // Check if the Organization  entity with the specified ID exists
                                var existingOrganisation = await _OrganisationRepository.FindAsync(request.CustomerId);

                                if (existingOrganisation != null)
                                {
                                    existingOrganisation.Active = request.activate;

                                    _OrganisationRepository.Update(existingOrganisation);
                                    if (await _uow.SaveAsync() <= 0)
                                    {
                                        return ServiceResponse<bool>.Return500();
                                    }
                                    return ServiceResponse<bool>.ReturnResultWith200(true);
                                }
                                else
                                {
                                    errorMessage = $"Customer with ID {request.CustomerId} not found.";
                                    _logger.LogError(errorMessage);
                                    return ServiceResponse<bool>.Return404(errorMessage);
                                }
                            }
                    
                    


                }
             

            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while Activating Or Dis-activating Customer: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
