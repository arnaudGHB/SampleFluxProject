
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

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class ResetCustomerPinCommandHandler : IRequestHandler<ResetCustomerPinCommand, ServiceResponse<CreateCustomer>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationRepository; // Repository for accessing CustomerSmsConfiguration data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<ResetCustomerPinCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly PathHelper _PathHelper;
  
        /// <summary>
        /// Constructor for initializing the ResetCustomerPinCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public ResetCustomerPinCommandHandler(
            ICustomerRepository CustomerRepository,
            ICustomerSmsConfigurationRepository CustomerSmsConfigurationRepository,
            IMapper mapper,
            ILogger<ResetCustomerPinCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper
            )
        {
            _CustomerRepository = CustomerRepository;
            _CustomerSmsConfigurationRepository = CustomerSmsConfigurationRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _PathHelper =pathHelper;
       
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
                
              

             

                var temporalPin = BaseUtilities.GenerateUniqueNumber(5);

                existingCustomer.Pin = BCrypt.Net.BCrypt.HashPassword(temporalPin);

                existingCustomer.NumberOfAttemptsOfMobileOrOnLineBankingLogin =0;
                existingCustomer.MobileOrOnLineBankingLoginFailedAttempts = 0;
       
            
               
            
            
                // Add the new Customer entity to the repository
                _CustomerRepository.Update(existingCustomer);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CreateCustomer>.Return500();
                }

                var getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("RESET_PIN_NOTIFICATION");
                if (getSmsTemplate != null)
                {
                    var smsRequest = new SubSmsRequestDto()
                    {
                        Message =getSmsTemplate.SmsTemplate==null?"Your Pin is "+temporalPin :  getSmsTemplate.SmsTemplate.Replace("{pin}",temporalPin),
                        Msisdn = existingCustomer.Phone
                    };
                    //await SendSms(smsRequest);

                
                  

                }




                // Map the Customer entity to GetCustomer and return it with a success response
                var GetCustomer = _mapper.Map<CreateCustomer>(existingCustomer);
                GetCustomer.Pin = temporalPin;
                return ServiceResponse<CreateCustomer>.ReturnResultWith200(GetCustomer);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Customer: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateCustomer>.Return500(e);
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
