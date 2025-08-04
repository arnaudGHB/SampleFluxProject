using BusinessServiceLayer.Objects.SmsObject;
using CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Command;
using CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation;
using CBS.CUSTOMER.REPOSITORY;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.MEDIATR.OTP.Commands;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.REPOSITORY.ChangePhoneNumber;
using CBS.CUSTOMER.DATA.Entity.ChangePhoneNumber;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CMoney.ChangePhoneNumber.Command;
using CBS.CUSTOMER.HELPER.Enums;

namespace CBS.CUSTOMER.MEDIATR.CMoney.ChangePhoneNumber.Handlers
{
    /// <summary>
    /// Handles the command to request a phone number change for a C-MONEY member.
    /// </summary>
    public class ChangePhoneNumberCommandHandler : IRequestHandler<ChangePhoneNumberRequestCommand, ServiceResponse<bool>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _cMoneyMembersActivationAccountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPhoneNumberChangeHistoryRepository _phoneNumberChangeHistoryRepository;
        private readonly SmsHelper _smsHelper;
        private readonly IMediator _otpService;
        private readonly ILogger<ChangePhoneNumberCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public ChangePhoneNumberCommandHandler(
            ICMoneyMembersActivationAccountRepository cMoneyMembersActivationAccountRepository,
            ICustomerRepository customerRepository,
            IPhoneNumberChangeHistoryRepository phoneNumberChangeHistoryRepository,
            SmsHelper smsHelper,
            IMediator otpService,
            ILogger<ChangePhoneNumberCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken)
        {
            _cMoneyMembersActivationAccountRepository = cMoneyMembersActivationAccountRepository ?? throw new ArgumentNullException(nameof(cMoneyMembersActivationAccountRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _phoneNumberChangeHistoryRepository = phoneNumberChangeHistoryRepository ?? throw new ArgumentNullException(nameof(phoneNumberChangeHistoryRepository));
            _smsHelper = smsHelper ?? throw new ArgumentNullException(nameof(smsHelper));
            _otpService = otpService ?? throw new ArgumentNullException(nameof(otpService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
        }

        /// <summary>
        /// Handles the ChangePhoneNumberCommand to request a phone number update for a member.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(ChangePhoneNumberRequestCommand request, CancellationToken cancellationToken)
        {
            string correlationId = Guid.NewGuid().ToString(); // Unique tracking ID
            string mismatchMessage = string.Empty;
            try
            {
                // 🔍 Step 1: Validate Input
                if (string.IsNullOrWhiteSpace(request.CustomerId) || string.IsNullOrWhiteSpace(request.NewPhoneNumber))
                {
                    string validationError = $"[ERROR] Missing required fields. CustomerId or NewPhoneNumber is null. CorrelationId: '{correlationId}'";
                    _logger.LogError(validationError);
                    await BaseUtilities.LogAndAuditAsync(validationError, request, HttpStatusCodeEnum.BadRequest, LogAction.ChangePhoneNumber, LogLevelInfo.Error, correlationId);
                    return ServiceResponse<bool>.Return400(validationError);
                }

                request.NewPhoneNumber = BaseUtilities.Add237Prefix(request.NewPhoneNumber);
                _logger.LogInformation($"[INFO] Initiating phone number change for Member '{request.CustomerId}'. CorrelationId: '{correlationId}'");

                // 🔍 Step 2: Retrieve customer details
                var customer = await _customerRepository.FindBy(x => x.CustomerId == request.CustomerId).FirstOrDefaultAsync();
                if (customer == null)
                {
                    string notFoundMessage = $"[ERROR] Customer with ID '{request.CustomerId}' not found. CorrelationId: '{correlationId}'";
                    _logger.LogWarning(notFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.ChangePhoneNumber, LogLevelInfo.Warning, correlationId);
                    return ServiceResponse<bool>.Return404(notFoundMessage);
                }
                // Verify OTP
                var otpValidationResult = await _otpService.Send(new VerifyTemporalOTPCommand { OtpCode = request.OtpCode.ToString(), UserId = request.CustomerId });
                if (otpValidationResult.StatusCode != 200)
                {
                    mismatchMessage = $"Invalid OTP for phone number {request.NewPhoneNumber}. Change number aborted.";
                    _logger.LogWarning(mismatchMessage);
                    await BaseUtilities.LogAndAuditAsync(mismatchMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.CMoneyMemberActivation, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return403(mismatchMessage);
                }
                // 🔍 Step 3: Validate old phone number (if required)
                if (!string.IsNullOrWhiteSpace(request.OldPhoneNumber))
                {
                    if (BaseUtilities.Add237Prefix(customer.Phone) != request.OldPhoneNumber)
                    {
                        mismatchMessage = $"[ERROR] Provided old phone number does not match existing record for Customer '{request.CustomerId}'. CorrelationId: '{correlationId}'";
                        _logger.LogWarning(mismatchMessage);
                        await BaseUtilities.LogAndAuditAsync(mismatchMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.ChangePhoneNumber, LogLevelInfo.Warning, correlationId);
                        return ServiceResponse<bool>.Return403(false, mismatchMessage);
                    }
                }

                // 🔄 Step 4: Register the phone number change request in history
                var changeHistory = new PhoneNumberChangeHistory
                {
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    CustomerId = request.CustomerId,
                    OldPhoneNumber = request.OldPhoneNumber,
                    NewPhoneNumber = request.NewPhoneNumber,
                    RequestBy = _userInfoToken.FullName,
                    RequestComment = request.RequestComment,
                    MemberName = $"{customer.FirstName} {customer.LastName}",
                    DateOfRequest = BaseUtilities.UtcNowToDoualaTime(),
                    Status = ChangePhoneNumberStatus.Pending.ToString(),
                    BranchId = _userInfoToken.BranchID,
                    BranchCode = _userInfoToken.BranchCode,
                    ApprovalComment="n/a",
                    ApprovalUserId="n/a",
                    ApprovedBy="n/a",
                    ApproveDate=DateTime.MinValue,
                    BranchName = _userInfoToken.BranchName,
                    RequestUserId = _userInfoToken.Id
                };

                _phoneNumberChangeHistoryRepository.Add(changeHistory);
                await _uow.SaveAsync();

                // ✅ Step 5: Log and Audit Success
                string successMessage = $"[SUCCESS] Phone number change request logged successfully for Member '{request.CustomerId}'. New Number: '{request.NewPhoneNumber}'. CorrelationId: '{correlationId}'";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.ChangePhoneNumber, LogLevelInfo.Information, correlationId);

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // ❌ Step 6: Handle Unexpected Errors
                string errorMessage = $"[ERROR] Unexpected error while processing phone number change for Member '{request.CustomerId}'. Error: {ex.Message}. CorrelationId: '{correlationId}'";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.ChangePhoneNumber, LogLevelInfo.Error, correlationId);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }

}
