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
    /// Handles the command to approve a phone number change request for a C-MONEY member.
    /// </summary>
    public class ApproveChangePhoneNumberCommandHandler : IRequestHandler<ApprovePhoneNumberRequestCommand, ServiceResponse<bool>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _cMoneyMembersActivationAccountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPhoneNumberChangeHistoryRepository _phoneNumberChangeHistoryRepository;
        private readonly SmsHelper _smsHelper;
        private readonly IMediator _otpService;
        private readonly ILogger<ApproveChangePhoneNumberCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the ApproveChangePhoneNumberCommandHandler.
        /// </summary>
        public ApproveChangePhoneNumberCommandHandler(
            ICMoneyMembersActivationAccountRepository cMoneyMembersActivationAccountRepository,
            ICustomerRepository customerRepository,
            IPhoneNumberChangeHistoryRepository phoneNumberChangeHistoryRepository,
            SmsHelper smsHelper,
            IMediator otpService,
            ILogger<ApproveChangePhoneNumberCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken)
        {
            _cMoneyMembersActivationAccountRepository = cMoneyMembersActivationAccountRepository;
            _customerRepository = customerRepository;
            _phoneNumberChangeHistoryRepository = phoneNumberChangeHistoryRepository;
            _smsHelper = smsHelper;
            _otpService = otpService;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the ApprovePhoneNumberRequestCommand to update the phone number for a member.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(ApprovePhoneNumberRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string message = string.Empty;
                // Retrieve phone number change history record
                var phoneNumberChangeHistory = _phoneNumberChangeHistoryRepository.Find(request.PhoneNumberChangeHistoryId);
                if (phoneNumberChangeHistory == null)
                {
                    message = "Phone number change history record not found.";
                    _logger.LogWarning(message);
                    return ServiceResponse<bool>.Return404(message);
                }

                string oldPhoneNumber = null;

                if (request.Approved)
                {
                    // Retrieve the member's activation account
                    var activationAccount = await _cMoneyMembersActivationAccountRepository.FindBy(x => x.CustomerId == phoneNumberChangeHistory.CustomerId).FirstOrDefaultAsync();
                    oldPhoneNumber = activationAccount?.PhoneNumber;

                    if (activationAccount != null)
                    {
                        message = $"Updating phone number for CMoney activation account: {phoneNumberChangeHistory.CustomerId}. Name: {activationAccount?.Name}";
                        activationAccount.PhoneNumber = phoneNumberChangeHistory.NewPhoneNumber;
                        _cMoneyMembersActivationAccountRepository.Update(activationAccount);
                    }
                    else
                    {
                        message = $"No activation account found for member reference {phoneNumberChangeHistory.CustomerId}. Skipping activation account update.";
                        await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.InternalServerError, LogAction.ChangePhoneNumber, LogLevelInfo.Error);
                        //return ServiceResponse<bool>.Return404(message);
                    }


                    // Update phone number in customer repository
                    var customer = await _customerRepository.FindBy(x => x.CustomerId == phoneNumberChangeHistory.CustomerId).FirstOrDefaultAsync();
                    if (customer != null)
                    {
                        customer.Phone = phoneNumberChangeHistory.NewPhoneNumber;
                        _customerRepository.Update(customer);
                        message = $"Customer repository updated for member reference {phoneNumberChangeHistory.CustomerId}. Name: {customer.FirstName}";
                        await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.ChangePhoneNumber, LogLevelInfo.Information);
                    }
                    else
                    {
                        message = $"No customer record found for member reference {phoneNumberChangeHistory.CustomerId}.";
                        await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.InternalServerError, LogAction.ChangePhoneNumber, LogLevelInfo.Error);
                        return ServiceResponse<bool>.Return404(message);
                    }
                }

                // Register the phone number change in the history repository
                phoneNumberChangeHistory.ApprovalComment = request.ApprovalComment;
                phoneNumberChangeHistory.Status = request.Approved ? ChangePhoneNumberStatus.Approved.ToString() : ChangePhoneNumberStatus.Rejected.ToString();
                phoneNumberChangeHistory.ApprovedBy = _userInfoToken.FullName;
                phoneNumberChangeHistory.ApproveDate = BaseUtilities.UtcNowToDoualaTime();
                phoneNumberChangeHistory.ApprovalUserId = _userInfoToken.Id;
                _phoneNumberChangeHistoryRepository.Update(phoneNumberChangeHistory);

                // Save changes using Unit of Work
                await _uow.SaveAsync();
                _logger.LogInformation($"Phone number change registered in history for member reference {phoneNumberChangeHistory.CustomerId}.");

                message = $"Phone number successfully updated for member reference {phoneNumberChangeHistory.CustomerId} by {_userInfoToken.FullName}";
               
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.ChangePhoneNumber, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, message);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error occurred while updating phone number for PhoneNumber Change History {request.PhoneNumberChangeHistoryId}: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.ChangePhoneNumber, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }


}
