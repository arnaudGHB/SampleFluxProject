
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;

using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using BusinessServiceLayer.Objects.SmsObject;
using Newtonsoft.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.APICaller.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using Microsoft.IdentityModel.Tokens;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand, ServiceResponse<CreateCustomer>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly ICustomerCategoryRepository _CustomerCategoryRepository; // Repository for accessing Category data.
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
        public AddCustomerCommandHandler(
            ICustomerRepository CustomerRepository,
            ICustomerSmsConfigurationRepository CustomerSmsConfigurationRepository,
            ICustomerCategoryRepository CustomerCategoryRepository,
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
            _CustomerCategoryRepository = CustomerCategoryRepository;
            _uow = uow;
            _PathHelper = pathHelper;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddCustomerCommand to add a new Customer.
        /// </summary>
        /// <param name="request">The AddCustomerCommand containing Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateCustomer>> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {


                request.Phone = BaseUtilities.Add237Prefix(request.Phone);

                if (request.BranchCode.IsNullOrEmpty())
                {
                    var errorMessage = $"Branch Code cannot be Null or Empty";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _UserInfoToken.Token);
                    return ServiceResponse<CreateCustomer>.Return409(errorMessage);
                };  
                
                
                if (request.BankCode.IsNullOrEmpty())
                {
                    var errorMessage = $"Bank Code cannot be Null or Empty";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _UserInfoToken.Token);
                    return ServiceResponse<CreateCustomer>.Return409(errorMessage);
                };

                int customerId = 0;

                var getLastCustomer = await _CustomerRepository.All.OrderByDescending(x => x.CreatedDate).FirstOrDefaultAsync();

                if(getLastCustomer != null)
                {

                    var getCustomerIdSubString = getLastCustomer.CustomerId.Substring(getLastCustomer.CustomerId.Length-4);

                     customerId=int.Parse(getCustomerIdSubString);

                }

                // Check if a Customer with the same name already exists (case-insensitive)
                var existingCustomer = await _CustomerRepository.FindBy(c => c.Phone == request.Phone && c.IsDeleted==false).FirstOrDefaultAsync();

                // If a Customer with the same Phone already exists, return a conflict response
                if (existingCustomer != null)
                {
                    var errorMessage = $"Customer With Phone {(request.Phone)} already exists.";
                    _logger.LogError(errorMessage);
                  await   APICallHelper.AuditLogger(_UserInfoToken.Email,LogAction.Create.ToString(),request,errorMessage,LogLevelInfo.Information.ToString(),409,_UserInfoToken.Token);
                    return ServiceResponse<CreateCustomer>.Return409(errorMessage);
                };


                var existingCategory =   _CustomerCategoryRepository.Find(request.CustomerCategoryId);

                if (existingCategory == null)
                {
                    var errorMessage = $"Category With Id {(request.CustomerCategoryId)} does not  exists.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _UserInfoToken.Token);
                    return ServiceResponse<CreateCustomer>.Return404(errorMessage);
                };

                // Check if a Customer with the same name already exists (case-insensitive)
                var existingEmail = await _CustomerRepository.FindBy(c => c.Email == request.Email && c.IsDeleted == false).FirstOrDefaultAsync();

                // If a Customer with the same Email already exists, return a conflict response
                if (existingCustomer != null)
                {
                    var errorMessage = $"Customer With Email {(request.Email)} already exists. Try Another Email";
                    _logger.LogError(errorMessage);

                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409,_UserInfoToken.Token);
                    return ServiceResponse<CreateCustomer>.Return409(errorMessage);
                };

                // Map the AddCustomerCommand to a Customer entity
                var CustomerEntity = _mapper.Map<DATA.Entity.Customer>(request);
                //LoginDto login = await APICallHelper.AuthenthicationFromIdentityServer(_pathHelper);

                
                CustomerEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);




                customerId  += 1;
                //Auto generateKey For Unique Identification
                CustomerEntity.CustomerId = $"{request.BankCode}{request.BranchCode}{customerId.ToString("D4")}";

                var temporalPin =  BaseUtilities.GenerateUniqueNumber(5);

                CustomerEntity.Pin = BCrypt.Net.BCrypt.HashPassword(temporalPin);
                
                CustomerEntity.NumberOfAttemptsOfMobileOrOnLineBankingLogin =0;
                CustomerEntity.MobileOrOnLineBankingLoginFailedAttempts = 0;
                CustomerEntity.Active = false;
                CustomerEntity.Email = request.Email==null ? CustomerEntity.Phone + "@gmail.com" : request.Email;

           
                // Add the new Customer entity to the repository
                _CustomerRepository.Add(CustomerEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, $"Internal Server Error occurred while saving new Customer, CustomerName : {request.FirstName+" "+ request.LastName}", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);
                    return ServiceResponse<CreateCustomer>.Return500();
                }

                var getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("PIN_NOTIFICATION");

               
                if (getSmsTemplate != null)
                {
                    var smsRequest = new SubSmsRequestDto()
                    {
                        Message =getSmsTemplate.SmsTemplate==null?"Your Pin is "+temporalPin :  getSmsTemplate.SmsTemplate.Replace("{pin}",temporalPin),
                        Msisdn = CustomerEntity.Phone
                    };

                 
                //    await SendSms(smsRequest);


                  


                }




                // Map the Customer entity to CreateCustomer and return it with a success response
                var CreateCustomer = _mapper.Map<CreateCustomer>(CustomerEntity);
                CreateCustomer.Pin = temporalPin;
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), CreateCustomer, $"Creating New Customer   {request.FirstName} {request.LastName}  Successful  ", LogLevelInfo.Information.ToString(), 200, _UserInfoToken.Token);

                return ServiceResponse<CreateCustomer>.ReturnResultWith200(CreateCustomer);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Customer: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new Customer, CustomerName : {request.FirstName} {request.LastName} ", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);

                return ServiceResponse<CreateCustomer>.Return500(e);
            }
        }

        public async Task<SmsResponseDto> SendSms(SubSmsRequestDto subSmsRequestDto)
        {

            SmsRequestDto smsRequestDto = new()
            {
                Recipient =  subSmsRequestDto.Msisdn ,
                MessageBody = subSmsRequestDto.Message,
                SenderService =_PathHelper.CbsSmsServiceNameUrl
            };
            var GetTokenResponse = await APICallHelper.WithOutAuthenthicationFromOtherServer<SmsResponseDto>(_PathHelper.CbsSMSSmsUrl, smsRequestDto);
            return GetTokenResponse;
        }
/*
        public async Task<CreateAccountTempReponse> CreateTempAccount(CreateAccountTempRequest createAccount)
        {
            var createAccountTempReponse = await APICallHelper.AuthenthicationFromIdentityServer<CreateAccountTempReponse>(_PathHelper,_PathHelper.CreateAccountUrl, createAccount,_UserInfoToken.Token);
          
            return createAccountTempReponse;
        }*/


    }

}
