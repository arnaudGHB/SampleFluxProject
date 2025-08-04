using BusinessServiceLayer.Objects.SmsObject;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Command;
using CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Handlers
{
    /// <summary>
    /// Handles the command to reset a C-MONEY Member PIN.
    /// </summary>
    public class ResetCMoneyMemberPinCommandHandler : IRequestHandler<ResetCMoneyMemberPinCommand, ServiceResponse<bool>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _CMoneyMembersActivationAccountRepository;
        private readonly ILogger<ResetCMoneyMemberPinCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private const string DefaultPin = "0000"; // Default PIN to be set during reset.
        private readonly UserInfoToken _userLoginInfo;
        private readonly SmsHelper _smsHelper;

        /// <summary>
        /// Constructor for initializing the ResetCMoneyMemberPinCommandHandler.
        /// </summary>
        public ResetCMoneyMemberPinCommandHandler(
            ICMoneyMembersActivationAccountRepository CMoneyMembersActivationAccountRepository,
            ILogger<ResetCMoneyMemberPinCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userLoginInfo = null,
            SmsHelper smsHelper = null)
        {
            _CMoneyMembersActivationAccountRepository = CMoneyMembersActivationAccountRepository;
            _logger = logger;
            _uow = uow;
            _userLoginInfo=userLoginInfo;
            _smsHelper=smsHelper;
        }

        /// <summary>
        /// Handles the ResetCMoneyMemberPinCommand to reset a member's PIN to the default PIN.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(ResetCMoneyMemberPinCommand request, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            try
            {
                // Retrieve the member's activation account
                var memberActivation = await _CMoneyMembersActivationAccountRepository
                    .FindBy(x => x.CustomerId == request.CustomerId)
                    .FirstOrDefaultAsync();

                if (memberActivation == null)
                {
                    message = $"C-MONEY member activation not found for member reference {request.CustomerId}.";
                    _logger.LogWarning(message);

                    // Log and audit the failed attempt
                    await BaseUtilities.LogAndAuditAsync(
                        message,
                        request,
                        HttpStatusCodeEnum.NotFound,
                        LogAction.CMoneyMemberPinReset,
                        LogLevelInfo.Warning
                    );

                    return ServiceResponse<bool>.Return404(message);
                }
                string NewPin = BaseUtilities.GenerateUniqueNumber(4);

                // Reset the PIN to the default PIN
                memberActivation.PIN = PinSecurity.HashPin(NewPin);
                memberActivation.FailedAttempts=0;
                // Update the repository
                _CMoneyMembersActivationAccountRepository.Update(memberActivation);
                memberActivation.HasChangeDefaultPin=false;
                // Save changes using Unit of Work
                await _uow.SaveAsync();

                message = $"PIN successfully reset to default for member {request.CustomerId}. New Pin: {NewPin}";
                _logger.LogInformation(message);

                // Log and audit the success
                await BaseUtilities.LogAndAuditAsync(
                    message+" New Pin "+NewPin,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.CMoneyMemberPinReset,
                    LogLevelInfo.Information
                );
                // Prepare SMS message
                string smsMessage = "TSC:RESET-NEW PIN: ("+NewPin+").";
                var smsRequest = new SubSmsRequestDto
                {
                    Message = smsMessage,
                    Msisdn = memberActivation.PhoneNumber,
                    Token = _userLoginInfo.Token
                };

                // Send SMS
                await _smsHelper.SendSms(smsRequest);
                return ServiceResponse<bool>.ReturnResultWith200(true, message);
            }
            catch (Exception e)
            {
                // Log error and return an error response
                var errorMessage = $"Error occurred while resetting PIN for member {request.CustomerId}: {e.Message}";
                _logger.LogError(errorMessage);

                // Log and audit the exception
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.CMoneyMemberPinReset,
                    LogLevelInfo.Error
                );

                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }


}
