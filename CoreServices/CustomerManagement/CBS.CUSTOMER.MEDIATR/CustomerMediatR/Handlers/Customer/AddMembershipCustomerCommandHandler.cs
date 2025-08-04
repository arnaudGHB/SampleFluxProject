
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;

using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;
using BusinessServiceLayer.Objects.SmsObject;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class AddMembershipCustomerCommandHandler : IRequestHandler<AddMembershipCustomerCommand, ServiceResponse<CreateMembershipCustomer>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationRepository; // Repository for accessing CustomerSmsConfiguration data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;

        /// <summary>
        /// Constructor for initializing the AddCustomerCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddMembershipCustomerCommandHandler(
            ICustomerRepository CustomerRepository,
            ICustomerSmsConfigurationRepository CustomerSmsConfigurationRepository,
            IMapper mapper,
            ILogger<AddCustomerCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper
,
            UserInfoToken userInfoToken)
        {
            _CustomerRepository = CustomerRepository;
            _CustomerSmsConfigurationRepository = CustomerSmsConfigurationRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _PathHelper = pathHelper;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddCustomerCommand to add a new Customer.
        /// </summary>
        /// <param name="request">The AddCustomerCommand containing Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateMembershipCustomer>> Handle(AddMembershipCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Map the AddCustomerCommand to a Customer entity
                var CustomerEntity = _mapper.Map<DATA.Entity.Customer>(request);
                //LoginDto login = await APICallHelper.AuthenthicationFromIdentityServer(_pathHelper);


                CustomerEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                CustomerEntity.CustomerId = BaseUtilities.GenerateUniqueNumber();


                var temporalPin = BaseUtilities.GenerateUniqueNumber(5);

                CustomerEntity.Pin = BCrypt.Net.BCrypt.HashPassword(temporalPin);

                CustomerEntity.NumberOfAttemptsOfMobileOrOnLineBankingLogin = 0;
                CustomerEntity.MobileOrOnLineBankingLoginFailedAttempts = 0;
                CustomerEntity.Active = false;



                // Add the new Customer entity to the repository
                _CustomerRepository.Add(CustomerEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, $"Internal Server Error occurred while saving new Customer, CustomerName : {request.FirstName + " " + request.LastName}", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);
                    return ServiceResponse<CreateMembershipCustomer>.Return500();
                }

                var getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("PIN_NOTIFICATION");


                if (getSmsTemplate != null)
                {
                    var smsRequest = new SubSmsRequestDto()
                    {
                        Message = getSmsTemplate.SmsTemplate == null ? "Your Pin is " + temporalPin : getSmsTemplate.SmsTemplate.Replace("{pin}", temporalPin),
                        Msisdn = CustomerEntity.Phone
                    };


                   // await SendSms(smsRequest);


                    /* if (request.AccountType.Count > 0)
                     {
                         foreach (var account in request.AccountType)
                         {
                             var CreateAccountTempRequest = new CreateAccountTempRequest()
                             {
                                 ProductId=account,
                                 CustomerId = CustomerEntity.CustomerId
                             };

                                await   CreateTempAccount(CreateAccountTempRequest);


                         }
                     }*/


                }




                // Map the Customer entity to CreateCustomer and return it with a success response
                var CreateCustomer = _mapper.Map<CreateMembershipCustomer>(CustomerEntity);

                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), CreateCustomer, $"Creating New Customer   {request.FirstName} {request.LastName}  Successful  ", LogLevelInfo.Information.ToString(), 200, _UserInfoToken.Token);

                return ServiceResponse<CreateMembershipCustomer>.ReturnResultWith200(CreateCustomer);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Customer: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new Customer, CustomerName : {request.FirstName} {request.LastName} ", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);

                return ServiceResponse<CreateMembershipCustomer>.Return500(e);
            }
        }

        public async Task<SmsResponseDto> SendSms(SubSmsRequestDto subSmsRequestDto)
        {

            SmsRequestDto smsRequestDto = new()
            {
                Recipient = subSmsRequestDto.Msisdn,
                MessageBody = subSmsRequestDto.Message,
                SenderService = _PathHelper.CbsSmsServiceNameUrl
            };
            var GetTokenResponse = await APICallHelper.WithOutAuthenthicationFromOtherServer<SmsResponseDto>(_PathHelper.CbsSMSSmsUrl, smsRequestDto);
            return GetTokenResponse;
        }
    }
}
/*

        public async Task<CreateAccountTempReponse> CreateTempAccount(CreateAccountTempRequest createAccount)
        {
            var createAccountTempReponse = await APICallHelper.AuthenthicationFromIdentityServer<CreateAccountTempReponse>(_PathHelper,_PathHelper.CreateAccountUrl, createAccount,_UserInfoToken.Token);
          
            return createAccountTempReponse;
        }


    }

}*/
