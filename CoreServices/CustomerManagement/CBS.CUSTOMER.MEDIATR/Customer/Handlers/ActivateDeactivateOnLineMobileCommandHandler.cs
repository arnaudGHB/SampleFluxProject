using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.Customer.Command;
using BusinessServiceLayer.Objects.SmsObject;

namespace CBS.CUSTOMER.MEDIATR
{

    /// <summary>
    /// Handles the command to update a Customer based on ActivateDeactivateOnLineMobileCommand.
    /// </summary>
    public class ActivateDeactivateOnLineMobileCommandHandler : IRequestHandler<ActivateDeactivateOnLineMobilePackagesCommand, ServiceResponse<UpdateCustomer>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationRepository; // Repository for accessing Customer Sms Configuration data.
        private readonly ILogger<ActivateDeactivateOnLineMobileCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly PathHelper _PathHelper;
        private readonly SmsHelper _SmsHelper;
        private readonly UserInfoToken _UserInfoToken;

        /// <summary>
        /// Constructor for initializing the ActivateDeactivateOnLineMobileCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public ActivateDeactivateOnLineMobileCommandHandler(
            ICustomerRepository CustomerRepository,
            ILogger<ActivateDeactivateOnLineMobileCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null
,
            ICustomerSmsConfigurationRepository customerSmsConfigurationRepository = null
,
            PathHelper pathHelper = null,
            UserInfoToken userInfoToken = null)
        {
            _CustomerRepository = CustomerRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _CustomerSmsConfigurationRepository = customerSmsConfigurationRepository;
            _PathHelper = pathHelper;
            _SmsHelper = new SmsHelper(_PathHelper);
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the ActivateDeactivateOnLineMobileCommand to update a Customer.
        /// </summary>
        /// <param name="request">The ActivateDeactivateOnLineMobileCommand containing updated Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UpdateCustomer>> Handle(ActivateDeactivateOnLineMobilePackagesCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Retrieve the Customer entity to be updated from the repository
                var existingCustomer = await _CustomerRepository.FindAsync(request.CustomerId);

                // Check if the Customer entity exists
                if (existingCustomer != null)
                {

                    string temporalPin = "";
                    existingCustomer.IsUseOnLineMobileBanking = request.Active;
                    temporalPin = BaseUtilities.GenerateUniqueNumber(5);
                    existingCustomer.Pin = BCrypt.Net.BCrypt.HashPassword(temporalPin);

                    _CustomerRepository.Update(existingCustomer);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<UpdateCustomer>.Return500();
                    }

                    if (existingCustomer.IsUseOnLineMobileBanking)
                    {
                        await SendSmsForMobilePackageActivation(existingCustomer, temporalPin);

                    }


                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UpdateCustomer>.ReturnResultWith200(_mapper.Map<UpdateCustomer>(existingCustomer));
                    _logger.LogInformation($"Customer {request.CustomerId} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Customer entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.CustomerId} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UpdateCustomer>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Customer: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UpdateCustomer>.Return500(e);
            }
        }



        private async Task SendSmsForMobilePackageActivation(DATA.Entity.Customer CustomerEntity, string temporalPin)
        {
            DATA.Entity.CustomerSmsConfigurations getSmsTemplate = new();
            if (CustomerEntity != null && CustomerEntity.Language != null && CustomerEntity.Language.ToLower() == "english")
            {
                getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("PIN_NOTIFICATION_ENGLISH");




            }
            else
            {
                getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("PIN_NOTIFICATION_FRENCH");



            }

            if (getSmsTemplate != null && getSmsTemplate.SmsTemplate != null)
            {
                var smsRequest = new SubSmsRequestDto()
                {
                    Message = getSmsTemplate.SmsTemplate.Replace("{pin}", temporalPin).Replace("{FirstName}", CustomerEntity.FirstName).Replace("{LastName}", $"{CustomerEntity.LastName}").Replace("{BankName}", $"{CustomerEntity.BankName}"),
                    Msisdn = CustomerEntity.Phone,
                    Token=_UserInfoToken.Token
                };


                await _SmsHelper.SendSms(smsRequest);
            }

        }
    }

}
