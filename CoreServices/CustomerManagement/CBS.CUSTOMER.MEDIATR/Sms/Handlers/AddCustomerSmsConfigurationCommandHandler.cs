
using MediatR;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.Customer.MEDIATR;
using CBS.CustomerSmsConfigurations.MEDIAT;
using CBS.CUSTOMER.REPOSITORY;

namespace CBS.CustomerSmsConfiguration.MEDIATR.CustomerSmsConfigurationMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new CustomerSmsConfiguration.
    /// </summary>
    public class AddCustomerSmsConfigurationCommandHandler : IRequestHandler<AddCustomerSmsConfigurationsCommand, ServiceResponse<CreateCustomerSmsConfigurations>>
    {
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationRepository; // Repository for accessing CustomerSmsConfiguration data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCustomerSmsConfigurationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddCustomerSmsConfigurationCommandHandler.
        /// </summary>
        /// <param name="CustomerSmsConfigurationRepository">Repository for CustomerSmsConfiguration data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCustomerSmsConfigurationCommandHandler(
            ICustomerSmsConfigurationRepository CustomerSmsConfigurationRepository,
            IMapper mapper,
            ILogger<AddCustomerSmsConfigurationCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _CustomerSmsConfigurationRepository = CustomerSmsConfigurationRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddCustomerSmsConfigurationCommand to add a new CustomerSmsConfiguration.
        /// </summary>
        /// <param name="request">The AddCustomerSmsConfigurationCommand containing CustomerSmsConfiguration data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateCustomerSmsConfigurations>> Handle(AddCustomerSmsConfigurationsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Map the AddCustomerSmsConfigurationCommand to a CustomerSmsConfiguration entity
                var CustomerSmsConfigurationEntity = _mapper.Map<CUSTOMER.DATA.Entity.CustomerSmsConfigurations>(request);
                var smsConfiguration = await _CustomerSmsConfigurationRepository.FindAsync(request.Id);
                if (smsConfiguration == null)
                {
                    // Add the new CustomerSmsConfiguration entity to the repository
                    _CustomerSmsConfigurationRepository.Add(CustomerSmsConfigurationEntity);
                }
                else
                {
                    smsConfiguration.SmsTemplate= request.SmsTemplate;
                    _CustomerSmsConfigurationRepository.Update(smsConfiguration);
                }
               
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CreateCustomerSmsConfigurations>.Return500();
                }
                // Map the CustomerSmsConfiguration entity to CreateCustomerSmsConfiguration and return it with a success response
                var CreateCustomerSmsConfiguration = _mapper.Map<CreateCustomerSmsConfigurations>(CustomerSmsConfigurationEntity);
                return ServiceResponse<CreateCustomerSmsConfigurations>.ReturnResultWith200(CreateCustomerSmsConfiguration);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving CustomerSmsConfiguration: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateCustomerSmsConfigurations>.Return500(e);
            }
        }
    }

}
