using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Handler;
using CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Queries;
using CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Command;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Handlers
{
    /// <summary>
    /// Handles the login process for C-MONEY members.
    /// </summary>
    public class LoginCMoneyMemberCommandHandler : IRequestHandler<LoginCMoneyMemberCommand, ServiceResponse<LoginResponseDto>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _CMoneyMembersActivationAccountRepository; //Repository for accessing C-MONEY activation data.
        private readonly ILogger<LoginCMoneyMemberCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _unitOfWork; // Unit of Work for transaction management.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository;
        private readonly PathHelper _PathHelper;
        private readonly IMediator _mediator;

        private const int MaxFailedAttempts = 3; // Maximum allowed login attempts

        /// <summary>
        /// Constructor for initializing the LoginCMoneyMemberCommandHandler.
        /// </summary>
        /// <param name="CMoneyMembersActivationAccountRepository">Repository for C-MONEY activation account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="unitOfWork">Unit of Work for transaction management.</param>
        public LoginCMoneyMemberCommandHandler(
            ICMoneyMembersActivationAccountRepository CMoneyMembersActivationAccountRepository,
            ILogger<LoginCMoneyMemberCommandHandler> logger,
            IUnitOfWork<POSContext> unitOfWork,
            IDocumentBaseUrlRepository documentBaseUrlRepository = null,
            PathHelper pathHelper = null,
            IMediator mediator = null)
        {
            _CMoneyMembersActivationAccountRepository = CMoneyMembersActivationAccountRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _DocumentBaseUrlRepository = documentBaseUrlRepository;
            _PathHelper = pathHelper;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the login command for a C-MONEY member.
        /// </summary>
        /// <param name="request">The login command containing member login details.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response containing login status or an error message.</returns>
        public async Task<ServiceResponse<LoginResponseDto>> Handle(LoginCMoneyMemberCommand request, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            try
            {
                //if (string.IsNullOrEmpty(request.AppSecret)  || string.IsNullOrEmpty(request.AppSecret) || string.IsNullOrEmpty(request.AndroidVersion))
                //{
                //  message = "A new version of C-MONEY is available! Your current version is no longer supported. Please update to continue enjoying our services. Need help? Visit your nearest branch office or contact Member Service for support.";
                //    _logger.LogWarning(message);
                //    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.MobileUserLogin, LogLevelInfo.Warning);
                //    return ServiceResponse<LoginResponseDto>.Return401(null, message);
                //}

                //var getAndriodVersionConfiguration = await _mediator.Send(new GetAndriodVersionByAppCodeAndAppSecretConfigurationQuery
                //{
                //    AppCode = request.AppCode,
                //    AppSecret = request.AppSecret
                //}, cancellationToken);

                //if (!getAndriodVersionConfiguration.Success ||  getAndriodVersionConfiguration.StatusCode!= 200 || getAndriodVersionConfiguration.Data==null)
                //{
                //    message = getAndriodVersionConfiguration.Message;
                //    _logger.LogWarning(message);
                //    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.MobileUserLogin, LogLevelInfo.Warning);
                //    return ServiceResponse<LoginResponseDto>.Return401(null,message);
                //}

                //if(getAndriodVersionConfiguration.Data.Version!= request.AndroidVersion) 
                //{

                //    message = $"New Version is available for CMONEY. Current version {getAndriodVersionConfiguration.Data.Version}";
                //    _logger.LogWarning(message);
                //    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.MobileUserLogin, LogLevelInfo.Warning);

                //    return ServiceResponse<LoginResponseDto>.Return401(new LoginResponseDto()
                //    {
                //        ApkUrl=getAndriodVersionConfiguration.Data.ApkUrl
                //    }, message);
                //}



                // Retrieve the member's activation account by LoginId
                var memberActivation = await _CMoneyMembersActivationAccountRepository.FindBy(x => x.LoginId == request.LoginId).Include(x => x.Customer).FirstOrDefaultAsync();
                if (memberActivation == null)
                {
                    message = $"Invalid login credentials for LoginId {request.LoginId}.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.MobileUserLogin, LogLevelInfo.Warning);
                    return ServiceResponse<LoginResponseDto>.Return401(null, message);
                }

                // Check if the account is active
                if (!memberActivation.IsActive)
                {
                    message = $"Account is inactive for LoginId {request.LoginId}.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.MobileUserLogin, LogLevelInfo.Warning);
                    return ServiceResponse<LoginResponseDto>.Return403(message);
                }

                // Check if the account is blocked due to failed attempts
                if (memberActivation.FailedAttempts >= MaxFailedAttempts)
                {
                    message = $"Account is blocked due to multiple failed login attempts for LoginId {request.LoginId}.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.MobileUserLogin, LogLevelInfo.Warning);
                    return ServiceResponse<LoginResponseDto>.Return403(message);
                }

                // Verify the PIN
                if (!PinSecurity.VerifyPin(request.PIN, memberActivation.PIN))
                {
                    memberActivation.FailedAttempts++;

                    // Block the account if maximum attempts exceeded
                    if (memberActivation.FailedAttempts >= MaxFailedAttempts)
                    {
                        message = $"Account blocked due to failed attempts for LoginId {request.LoginId}. Name: {memberActivation.Customer.FirstName} {memberActivation.Customer.LastName}, Phone: {memberActivation.PhoneNumber}.";

                        memberActivation.IsActive = false;
                        memberActivation.DeactivationReason = $"Account blocked due to failed attempts.";
                        _logger.LogWarning(message);
                        await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.MobileUserLogin, LogLevelInfo.Warning);

                    }

                    _CMoneyMembersActivationAccountRepository.Update(memberActivation);
                    await _unitOfWork.SaveAsync();

                    message = $"Invalid PIN for LoginId {request.LoginId}. Name: {memberActivation.Customer.FirstName} {memberActivation.Customer.LastName}, Phone: {memberActivation.PhoneNumber}.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.MobileUserLogin, LogLevelInfo.Warning);

                    return ServiceResponse<LoginResponseDto>.Return403(message);
                }

                var documentBaseUrl = "";
                var baseDocumentUrl = _DocumentBaseUrlRepository.Find("0");

                if (baseDocumentUrl != null)
                {
                    documentBaseUrl = baseDocumentUrl.baseURL;
                }



                // Check if the member is using the default PIN
                if (!memberActivation.HasChangeDefaultPin)
                {
                    message = $"Login successful for LoginId {request.LoginId}, but the user is still using the default PIN. Please change the PIN immediately.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.MobileUserLogin, LogLevelInfo.Warning);

                }

                // Reset failed attempts on successful login
                memberActivation.FailedAttempts = 0;
                memberActivation.NotificationToken = request.NotificationToken;
                _CMoneyMembersActivationAccountRepository.Update(memberActivation);
                await _unitOfWork.SaveAsync();

                // Prepare login response
                var customer = memberActivation.Customer;
                var loginResponse = new LoginResponseDto
                {
                    CustomerId = customer.CustomerId,
                    Name = customer.FirstName + " " + customer.LastName,
                    DateOfBirth = customer.DateOfBirth.ToString("dd-MM-yyyy"),
                    PlaceOfBirth = customer.PlaceOfBirth,
                    Occupation = customer.Occupation,
                    Address = customer.Address,
                    IdNumber = customer.IDNumber,
                    IdNumberIssueDate = customer.IDNumberIssueDate,
                    IdNumberIssueAt = customer.IDNumberIssueAt,
                    MembershipApprovalStatus = customer.MembershipApprovalStatus,
                    Gender = customer.Gender,
                    Email = customer.Email,
                    CustomerCode = customer.CustomerCode,
                    BankName = customer.BankName,
                    //PhotoUrl = $"{documentBaseUrl}/{memberActivation.Customer.PhotoUrl}",
                    PhotoUrl = string.IsNullOrEmpty(memberActivation.Customer.PhotoUrl)? _PathHelper.DefaultPhotoURL: (memberActivation.Customer.PhotoUrl.StartsWith("Docume")? $"{documentBaseUrl}/{memberActivation.Customer.PhotoUrl}"
        : memberActivation.Customer.PhotoUrl),
                    IsUseOnLineMobileBanking = customer.IsUseOnLineMobileBanking,
                    Language = customer.Language,
                    NotificationToken= request.NotificationToken,
                    Active = memberActivation.IsActive,
                    Telephone = memberActivation.PhoneNumber,
                    SecretAnswer=memberActivation.SecretAnswer,
                    SecretQuestion=memberActivation.SecretQuestion,
                    IsBlocked = !memberActivation.IsActive,
                    LoginId=request.LoginId,
                    BranchName=memberActivation.BranchCode,
                    BranchCode=memberActivation.BranchCode,
                    ChangePinIsRequired = false,
                    Message = "Login successful."
                };
                message = $"Member {memberActivation.CustomerId} (Name: {customer.FirstName} {customer.LastName}, Phone: {memberActivation.PhoneNumber}) logged in successfully.";
                _logger.LogInformation(message);
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.MobileUserLogin, LogLevelInfo.Information);
                return ServiceResponse<LoginResponseDto>.ReturnResultWith200(loginResponse, message);
            }
            catch (Exception e)
            {
                // Log error and return an error response
                var errorMessage = $"Error occurred during login for LoginId {request.LoginId}: {e.Message}. Name: {request.LoginId}.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.MobileUserLogin, LogLevelInfo.Error);
                return ServiceResponse<LoginResponseDto>.Return500(errorMessage);
            }
        }
    }





}
