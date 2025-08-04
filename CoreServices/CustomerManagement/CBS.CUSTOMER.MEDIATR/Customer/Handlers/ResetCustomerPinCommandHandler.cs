
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;
using BusinessServiceLayer.Objects.SmsObject;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class ResetCustomerPinCommandHandler : IRequestHandler<ResetCustomerPinCommand, ServiceResponse<CreateCustomer>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
    
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<ResetCustomerPinCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly PathHelper _PathHelper;
        private readonly UserInfoToken _UserInfoToken;
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationRepository; // Repository for accessing CustomerSmsConfiguration data.
        private readonly SmsHelper _SmsHelper;


        /// <summary>
        /// Constructor for initializing the ResetCustomerPinCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public ResetCustomerPinCommandHandler(
            ICustomerRepository CustomerRepository,
            IMapper mapper,
            ILogger<ResetCustomerPinCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
         ICustomerSmsConfigurationRepository CustomerSmsConfigurationRepository,
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
            _SmsHelper = new SmsHelper(_PathHelper);
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the ResetCustomerPinCommand to add a new Customer.
        /// </summary>
        /// <param name="request">The ResetCustomerPinCommand containing Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateCustomer>> Handle(ResetCustomerPinCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.Phone= BaseUtilities.Add237Prefix(request.Phone);

                // Check if a Customer with the same name already exists (case-insensitive)
                var existingCustomer = await _CustomerRepository.FindBy(c => c.Phone == request.Phone && c.IsDeleted==false).FirstOrDefaultAsync();

                // If a Customer with the same Phone already exists, return a conflict response
                if (existingCustomer == null)
                {
              
                    return ServiceResponse<CreateCustomer>.Return404();
                };
                
              

             

                var temporalPin = $"{BaseUtilities.GenerateUniqueNumber(5)}";

                existingCustomer.Pin = BCrypt.Net.BCrypt.HashPassword(temporalPin);
                existingCustomer.CustomerPackageId = temporalPin;

                existingCustomer.NumberOfAttemptsOfMobileOrOnLineBankingLogin =0;
                existingCustomer.MobileOrOnLineBankingLoginFailedAttempts = 0;
       
            
               
            
            
                // Add the new Customer entity to the repository
                _CustomerRepository.Update(existingCustomer);
                await _uow.SaveAsync();


                 await  SendSms(existingCustomer,temporalPin);



                // Map the Customer entity to CustomerDto and return it with a success response
                var GetCustomer = _mapper.Map<CreateCustomer>(existingCustomer);
                GetCustomer.Pin = temporalPin;
                return ServiceResponse<CreateCustomer>.ReturnResultWith200(GetCustomer, temporalPin);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Customer: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateCustomer>.Return500(e);
            }
        }

        private async Task SendSms(DATA.Entity.Customer CustomerEntity,string temporalPin)
        {
            DATA.Entity.CustomerSmsConfigurations getSmsTemplate = new();
            if (CustomerEntity != null && CustomerEntity.Language != null && CustomerEntity.Language.ToLower() == "english")
            {
                getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("RESET_PIN_NOTIFICATION_ENGLISH");




            }
            else
            {
                getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("RESET_PIN_NOTIFICATION_FRENCH");



            }

            if (getSmsTemplate != null && getSmsTemplate.SmsTemplate != null)
            {
                var smsRequest = new SubSmsRequestDto()
                {
                    Message = getSmsTemplate.SmsTemplate.Replace("{pin}",temporalPin).Replace("{FirstName}", CustomerEntity.FirstName).Replace("{LastName}", $"{CustomerEntity.LastName}").Replace("{BankName}", CustomerEntity.BankName),
                    Msisdn = CustomerEntity.Phone,
                    Token = _UserInfoToken.Token
                };


                await _SmsHelper.SendSms(smsRequest);
            }
            else
            {
                if (CustomerEntity != null && CustomerEntity.Language != null)
                {
                    string branchName = CustomerEntity.BankName; // Replace [Branch Name] with the actual branch name
                    string customerName = CustomerEntity.FirstName + " " + CustomerEntity.LastName; // Replace [Customer's Name] with the customer's name
                    string maskedPassword = temporalPin; // Use the password as is
                    //string customerSupportNumber = "8018"; // Replace [Customer Support Number] with the customer support number

                    string msg = $"TSC:REST: {temporalPin}";
                    var smsRequest = new SubSmsRequestDto()
                    {
                        Message = msg,
                        Msisdn = CustomerEntity.Phone,
                        Token = _UserInfoToken.Token
                    };

                    if (CustomerEntity.Language.ToLower() == "english")
                    {
                        await _SmsHelper.SendSms(smsRequest);
                    }
                    else if (CustomerEntity.Language.ToLower() == "french")
                    {
                        // Translate the message to French
                        msg = $"TSC:REST: {temporalPin}";
                        // Update the SMS request with the translated message
                        smsRequest.Message = msg;
                        // Send the SMS
                        await _SmsHelper.SendSms(smsRequest);
                    }
                    else
                    {
                        // Handle other languages if needed
                    }
                }

            }

        }




    }

}


