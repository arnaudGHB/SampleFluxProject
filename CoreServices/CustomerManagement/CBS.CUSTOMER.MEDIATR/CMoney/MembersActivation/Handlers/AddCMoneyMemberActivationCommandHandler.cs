
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
using CBS.CustomerSmsConfigurations.MEDIAT.CMoney.MembersActivation;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation;
using CBS.CUSTOMER.DATA.Entity.CMoneyP;
using CBS.CUSTOMER.REPOSITORY.CMoney.SubcriptionDetail;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CUSTOMER.MEDIATR.OTP.Commands;
using BusinessServiceLayer.Objects.SmsObject;
using CBS.CUSTOMER.REPOSITORY.ChangePhoneNumber;
using AutoMapper.Execution;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Handlers
{
    /// <summary>
    /// Handles the command to add a new C-MONEY Member Activation.
    /// </summary>
    public class AddCMoneyMemberActivationCommandHandler : IRequestHandler<AddCMoneyMemberActivationCommand, ServiceResponse<CMoneyMembersActivationAccountDto>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _CMoneyMembersActivationAccountRepository; // Repository for accessing C-MONEY activation data.
        private readonly ICMoneySubscriptionDetailRepository _cMoneySubscriptionDetailRepository; // Repository for subscription details.
        private readonly ICustomerRepository _customerRepository; // Repository for customer data.
        private readonly SmsHelper _smsHelper; // Service for sending SMS.
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCMoneyMemberActivationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IMediator _otpService;

        /// <summary>
        /// Constructor for initializing the AddCMoneyMemberActivationCommandHandler.
        /// </summary>
        /// <param name="CMoneyMembersActivationAccountRepository">Repository for C-MONEY activation account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for transaction management.</param>
        /// <param name="cMoneySubscriptionDetailRepository">Repository for subscription details.</param>
        /// <param name="customerRepository">Repository for customer data.</param>
        /// <param name="smsService">Service for sending SMS notifications.</param>
        public AddCMoneyMemberActivationCommandHandler(
            ICMoneyMembersActivationAccountRepository CMoneyMembersActivationAccountRepository,
            IMapper mapper,
            ILogger<AddCMoneyMemberActivationCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            ICMoneySubscriptionDetailRepository cMoneySubscriptionDetailRepository,
            ICustomerRepository customerRepository,
            UserInfoToken userInfoToken,
            SmsHelper smsService = null,
            IMediator otpService = null)
        {
            _CMoneyMembersActivationAccountRepository = CMoneyMembersActivationAccountRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _cMoneySubscriptionDetailRepository = cMoneySubscriptionDetailRepository;
            _customerRepository = customerRepository;
            _userInfoToken = userInfoToken;
            _smsHelper = smsService;
            _otpService = otpService;
        }

        /// <summary>
        /// Handles the AddCMoneyMemberActivationCommand to activate C-MONEY services for a member.
        /// </summary>
        /// <param name="request">The command containing activation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response containing activation details or an error message.</returns>
        public async Task<ServiceResponse<CMoneyMembersActivationAccountDto>> Handle(AddCMoneyMemberActivationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string message;
                request.PhoneNumber = BaseUtilities.Add237Prefix(request.PhoneNumber);

                // Check if the activation already exists for the customer
                var existingActivation = await _CMoneyMembersActivationAccountRepository
                    .FindBy(x => x.CustomerId == request.CustomerId)
                    .FirstOrDefaultAsync();
                if (existingActivation != null)
                {
                    message = $"C-MONEY activation already exists for member reference {request.CustomerId}. No action was taken.";
                    _logger.LogInformation(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Conflict, LogAction.CMoneyMemberActivation, LogLevelInfo.Warning);
                    return ServiceResponse<CMoneyMembersActivationAccountDto>.Return409(message);
                }

                // Check if the phone number is already in use by another customer
                var phoneNumberExists = await _CMoneyMembersActivationAccountRepository
                    .FindBy(x => x.PhoneNumber == request.PhoneNumber)
                    .FirstOrDefaultAsync();
                if (phoneNumberExists != null)
                {
                    message = $"The phone number {request.PhoneNumber} is already associated with another customer. Please provide a unique phone number.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Conflict, LogAction.CMoneyMemberActivation, LogLevelInfo.Warning);
                    return ServiceResponse<CMoneyMembersActivationAccountDto>.Return409(message);
                }
                // Verify OTP
                var otpValidationResult = await _otpService.Send(new VerifyTemporalOTPCommand { OtpCode = request.OTP, UserId = request.CustomerId });
                if (otpValidationResult.StatusCode != 200)
                {
                    message = $"Invalid OTP for phone number {request.PhoneNumber}. Activation aborted.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.CMoneyMemberActivation, LogLevelInfo.Warning);
                    return ServiceResponse<CMoneyMembersActivationAccountDto>.Return403(message);
                }

                // Fetch existing Login IDs for the branch to ensure uniqueness
                var existingLoginIds = new HashSet<string>(await _CMoneyMembersActivationAccountRepository
                    .FindBy(x => x.BranchCode == request.BranchCode)
                    .Select(x => x.LoginId)
                    .ToListAsync());
                string NewPin = BaseUtilities.GenerateUniqueNumber(4);
                // Map the command to the entity and initialize properties
                var cMoneyMembersActivation = _mapper.Map<CMoneyMembersActivationAccount>(request);
                cMoneyMembersActivation.ActivationDate = BaseUtilities.UtcNowToDoualaTime();
                cMoneyMembersActivation.IsActive = true;
                cMoneyMembersActivation.IsSubcribed = true;
                cMoneyMembersActivation.DefaultPin = NewPin;
                cMoneyMembersActivation.ActivatingBranchCode = _userInfoToken.BranchCode;
                cMoneyMembersActivation.ActivatingBranchId = _userInfoToken.BranchID;
                cMoneyMembersActivation.ActivatingBranchName = _userInfoToken.BranchName;
                cMoneyMembersActivation.ActivatedBy = _userInfoToken.FullName;
                cMoneyMembersActivation.DefaultPin = NewPin;
                cMoneyMembersActivation.LastPaymentAmount = 0;
                cMoneyMembersActivation.LastPaymentDate = BaseUtilities.UtcNowToDoualaTime();
                cMoneyMembersActivation.PIN = PinSecurity.HashPin(NewPin);
                cMoneyMembersActivation.LoginId = BaseUtilities.GenerateMemberLoginId(request.BranchCode, request.CustomerId, existingLoginIds);
                cMoneyMembersActivation.Id = cMoneyMembersActivation.LoginId;

                // Create a new subscription detail entry
                var subscriptionDetail = new CMoneySubscriptionDetail
                {
                    ActionType = CMoneySubcriptionTypes.NewSubcription.ToString(),
                    Amount = 0,
                    BranchId = request.BranchId,
                    MemberId = request.CustomerId,
                    CMoneyMembersActivationAccountId = cMoneyMembersActivation.Id,
                    DateTime = cMoneyMembersActivation.ActivationDate,
                    Id = cMoneyMembersActivation.Id
                };



                // Get all members with the same phone number
                var membersWithSamePhoneNumber = await _customerRepository.FindBy(x => x.Phone == request.PhoneNumber).ToListAsync();

                // Update phone numbers of other members with the word "Over-Written"
                foreach (var member in membersWithSamePhoneNumber)
                {
                    member.Phone = "Over-Written";
                    _customerRepository.Update(member);
                }
                var customer= await _customerRepository.FindAsync(request.CustomerId);
                customer.Phone = request.PhoneNumber;
                customer.Language = request.Language;
                customer.MobileOrOnLineBankingLoginState=cMoneyMembersActivation.Id;
                customer.IsUseOnLineMobileBanking=true;
                cMoneyMembersActivation.NotificationToken="N/A";
                _customerRepository.Update(customer);
                cMoneyMembersActivation.Name=$"{customer.FirstName} {customer.LastName}";   
                // Add entities to repositories
                _cMoneySubscriptionDetailRepository.Add(subscriptionDetail);
                _CMoneyMembersActivationAccountRepository.Add(cMoneyMembersActivation);
                // Save changes using Unit of Work
                await _uow.SaveAsync();

                // Send SMS to the member
                var language = request.Language ?? "English"; // Default to English if no language is provided
                var smsMessage = language switch
                {
                    "en" => "Congratulations! Your C-MONEY account has been activated. Login ID: ("+cMoneyMembersActivation.LoginId+"). Default PIN: ("+NewPin+"). Please change your default PIN for security.",
                    "fr" => "Félicitations! Votre compte C-MONEY a été activé. ID de connexion: ("+cMoneyMembersActivation.LoginId+"). Code PIN par défaut: ("+NewPin+"). Veuillez changer votre code PIN par défaut pour des raisons de sécurité.",
                    _ => "Congratulations! Your C-MONEY account has been activated. Login ID: ("+cMoneyMembersActivation.LoginId+"). Default PIN: ("+NewPin+"). Please change your default PIN for security."
                };

                var smsRequest = new SubSmsRequestDto
                {
                    Message = smsMessage,
                    Msisdn = request.PhoneNumber,
                    Token = _userInfoToken.Token
                };

                // Send SMS
                await _smsHelper.SendSms(smsRequest);

                // Log success and prepare response
                message = $"C-MONEY member activation completed successfully for CustomerId {request.CustomerId}. Name: {customer.FirstName} {customer.LastName} BY [{_userInfoToken.FullName}]. Login: {cMoneyMembersActivation.LoginId}, Pin Code: {NewPin}";
                _logger.LogInformation(message);
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.CMoneyMemberActivation, LogLevelInfo.Information);

                // Map the entity to a DTO and return a success response
                var cMoneyMembersActivationAccountDto = _mapper.Map<CMoneyMembersActivationAccountDto>(cMoneyMembersActivation);
                return ServiceResponse<CMoneyMembersActivationAccountDto>.ReturnResultWith200(cMoneyMembersActivationAccountDto, message);
            }
            catch (Exception e)
            {
                // Log error and return an error response
                var errorMessage = $"Error occurred while activating C-MONEY services for member {request.CustomerId}: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CMoneyMemberActivation, LogLevelInfo.Error);
                return ServiceResponse<CMoneyMembersActivationAccountDto>.Return500(errorMessage);
            }
        }
    }

}


