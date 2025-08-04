
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;

using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;

using BusinessServiceLayer.Objects.SmsObject;


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

using CBS.CUSTOMER.REPOSITORY.GroupRepo;

using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using System.Text.RegularExpressions;
using CBS.Customer.MEDIATR.CustomerMediatR;



namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand, ServiceResponse<CreateCustomer>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly ICustomerCategoryRepository _CustomerCategoryRepository; // Repository for accessing Category data.
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomer data.
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Group data.
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationRepository; // Repository for accessing CustomerSmsConfiguration data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;
        private readonly SmsHelper _SmsHelper;
        private readonly IMediator _mediator;

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
            PathHelper pathHelper,
            UserInfoToken userInfoToken,
            IGroupRepository groupRepository,
            IGroupCustomerRepository groupCustomerRepository,
            IMediator mediator)
        {
            _CustomerRepository = CustomerRepository;
            _CustomerSmsConfigurationRepository = CustomerSmsConfigurationRepository;
            _mapper = mapper;
            _logger = logger;

            _CustomerCategoryRepository = CustomerCategoryRepository;
            _uow = uow;
            _PathHelper = pathHelper;
            _UserInfoToken = userInfoToken;
            _SmsHelper = new SmsHelper(_PathHelper);
            _GroupRepository = groupRepository;
            _GroupCustomerRepository = groupCustomerRepository;
            _mediator = mediator;
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
                var customerCode = "";
                if (!request.IsFileData)
                {
                    var getLastCustomer = await _CustomerRepository.All.OrderByDescending(x => x.CreatedDate).Where(x => x.BranchId == request.BranchId).FirstOrDefaultAsync();
                    if (getLastCustomer != null)
                    {
                        var getCustomerIdSubString = getLastCustomer.CustomerId.Substring(getLastCustomer.CustomerId.Length - 7);
                        customerId = int.Parse(getCustomerIdSubString);
                    }
                    customerId += 1;
                    customerCode = customerId.ToString("D7");
                }
                else
                {
                    customerCode = request.CustomerCode;
                }



                if (!request.IsNotDirectRequest)
                {
                    request.LegalForm = LegalForm.Physical_Person.ToString();
                }
                // Check if a Customer with the same name already exists (case-insensitive)
                var existingCustomer = await _CustomerRepository.FindBy(c => c.Phone == request.Phone && c.IsDeleted == false).FirstOrDefaultAsync();

                // If a Customer with the same Phone already exists, return a conflict response
                if (existingCustomer != null)
                {
                    var errorMessage = $"Customer With Phone {(request.Phone)} already exists.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _UserInfoToken.Token);
                    return ServiceResponse<CreateCustomer>.Return409(errorMessage);
                };
                var existingCategory = _CustomerCategoryRepository.Find(request.CustomerCategoryId);
                if (existingCategory == null)
                {
                    var errorMessage = $"Category With Id {request.CustomerCategoryId} does not  exists.";
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

                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _UserInfoToken.Token);
                    return ServiceResponse<CreateCustomer>.Return409(errorMessage);
                };

                // Map the AddCustomerCommand to a Customer entity
                var CustomerEntity = _mapper.Map<DATA.Entity.Customer>(request);
                //LoginDto login = await APICallHelper.AuthenthicationFromIdentityServer(_pathHelper);


                CustomerEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);


                //Auto generateKey For Unique Identification
                CustomerEntity.CustomerId = $"{request.BranchCode}{customerCode}";

                var temporalPin = BaseUtilities.GenerateUniqueNumber(4);
                CustomerEntity.NumberOfAttemptsOfMobileOrOnLineBankingLogin = 0;
                CustomerEntity.CustomerCode = customerCode;
                CustomerEntity.MobileOrOnLineBankingLoginFailedAttempts = 0;
                CustomerEntity.Active = false;
                CustomerEntity.MembershipApprovalStatus = MembershipApprovalStatus.Awaits_Validation.ToString();
                CustomerEntity.Email = request.Email == null ? CustomerEntity.Phone + "@gmail.com" : request.Email;
                if (request.IsFileData)
                {

                    CustomerEntity.MembershipApprovalStatus = MembershipApprovalStatus.Approved.ToString();
                    CustomerEntity.Active = true;
                    CustomerEntity.CustomerId = request.CustomerId == null ? CustomerEntity.CustomerId : request.CustomerId;
                    temporalPin = "1234";

                    // Check if a Customer with the same name already exists (case-insensitive)
                    var existingEntity = await _CustomerRepository.FindBy(c => c.CustomerId == CustomerEntity.CustomerId && c.IsDeleted == false).FirstOrDefaultAsync();

                    // If a Customer with the same Phone already exists, return a conflict response
                    if (existingEntity != null)
                    {
                        var errorMessage = $"Customer With customer {(existingEntity.CustomerId)} already exists.";
                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _UserInfoToken.Token);
                        return ServiceResponse<CreateCustomer>.Return409(errorMessage);
                    };
                }
                CustomerEntity.Pin = BCrypt.Net.BCrypt.HashPassword(temporalPin);




                // Add the new Customer entity to the repository
                _CustomerRepository.Add(CustomerEntity);


                if (!request.IsNotDirectRequest)
                {
                    await _uow.SaveAsync();

                    if (!request.IsFileData)
                    {
                        var memberOrdinaryAccount = new CreateMemberOrdinaryAccount { CustomerId = CustomerEntity.CustomerId, CustomerName = $"{CustomerEntity.FirstName} {CustomerEntity.LastName}" };
                        var GetTokenResponse = await APICallHelper.CreateAccount<ServiceResponse<bool>>(_PathHelper.BaseMemberURL, _PathHelper.CreateMemberUrl, memberOrdinaryAccount, _UserInfoToken.Token);


                        await SendCustomerCreationSms(CustomerEntity);
                    }


                }
                // Map the Customer entity to CreateCustomer and return it with a success response
                var CreateCustomer = _mapper.Map<CreateCustomer>(CustomerEntity);
                //CreateCustomer.Pin = temporalPin;
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), CreateCustomer, $"Creating New Customer {request.FirstName} {request.LastName} Successful  ", LogLevelInfo.Information.ToString(), 200, _UserInfoToken.Token);

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

        private async Task SendCustomerCreationSms(DATA.Entity.Customer CustomerEntity)
        {
            DATA.Entity.CustomerSmsConfigurations getSmsTemplate = new();
            if (CustomerEntity != null && CustomerEntity.Language != null && CustomerEntity.Language.ToLower() == "english")
            {
                getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("CUSTOMER_CREATION_NOTIFICATION_ENGLISH");
            }
            else
            {
                getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("CUSTOMER_CREATION_NOTIFICATION_FRENCH");
            }

            if (getSmsTemplate != null && getSmsTemplate.SmsTemplate != null)
            {
                var smsRequest = new SubSmsRequestDto()
                {
                    Message = getSmsTemplate.SmsTemplate.Replace("{FirstName}", CustomerEntity.FirstName).Replace("{LastName}", $"{CustomerEntity.LastName}").Replace("{BankName}", $"{CustomerEntity.BankName}").Replace("{CustomerId}", $"{CustomerEntity.CustomerId}"),
                    Msisdn = CustomerEntity.Phone,
                    Token = _UserInfoToken.Token
                };
                await _SmsHelper.SendSms(smsRequest);
            }
            else
            {
                if (CustomerEntity != null && CustomerEntity.Language != null && CustomerEntity.Language.ToLower() == "english")
                {
                    // English message
                    string msg = $"Dear {CustomerEntity.FirstName} {CustomerEntity.LastName}, Welcome to {CustomerEntity.BankName}! We are delighted to have you onboard. Your membership has been successfully created. Membership code: {CustomerEntity.CustomerId}.\nYour For any assistance or inquiries, feel free to visit our nearest branch or contact our customer support at 8080.\nThank you for choosing {CustomerEntity.BankName}.";

                    var smsRequest = new SubSmsRequestDto()
                    {
                        Message = msg,
                        Msisdn = CustomerEntity.Phone,
                        Token = _UserInfoToken.Token
                    };
                    await _SmsHelper.SendSms(smsRequest);
                }
                else
                {
                    // French message
                    string msg = $"Cher {CustomerEntity.FirstName} {CustomerEntity.LastName}, Bienvenue a {CustomerEntity.BankName}! Nous sommes ravis de vous accueillir à bord. Votre adhésion a ete cree avec succes. Code d adhesion : {CustomerEntity.CustomerId}.\nPour toute assistance ou question, n hesitez pas à vous rendre dans notre succursale la plus proche ou a contacter notre service clientele au 8080.\nMerci d avoir choisi {CustomerEntity.BankName}.";

                    var smsRequest = new SubSmsRequestDto()
                    {
                        Message = msg,
                        Msisdn = CustomerEntity.Phone,
                        Token = _UserInfoToken.Token
                    };
                    await _SmsHelper.SendSms(smsRequest);
                }
            }
        }



    }

}
