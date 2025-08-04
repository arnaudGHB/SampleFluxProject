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
    /// Handles the command to change a C-MONEY Member PIN.
    /// </summary>
    public class ChangeCMoneyMemberPinCommandHandler : IRequestHandler<ChangeCMoneyMemberPinCommand, ServiceResponse<bool>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _CMoneyMembersActivationAccountRepository;
        private readonly ILogger<ChangeCMoneyMemberPinCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly SmsHelper _smsHelper;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the ChangeCMoneyMemberPinCommandHandler.
        /// </summary>
        public ChangeCMoneyMemberPinCommandHandler(
            ICMoneyMembersActivationAccountRepository CMoneyMembersActivationAccountRepository,
            ILogger<ChangeCMoneyMemberPinCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            SmsHelper smsHelper = null,
            UserInfoToken userInfoToken = null)
        {
            _CMoneyMembersActivationAccountRepository = CMoneyMembersActivationAccountRepository;
            _logger = logger;
            _uow = uow;
            _smsHelper=smsHelper;
            _userInfoToken=userInfoToken;
        }

        /// <summary>
        /// Handles the ChangeCMoneyMemberPinCommand to update the PIN for a C-MONEY member.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(ChangeCMoneyMemberPinCommand request, CancellationToken cancellationToken)
        {
            string message;
            try
            {
                // Retrieve the member's activation account
                var memberActivation = await _CMoneyMembersActivationAccountRepository.FindAsync(request.LoginId);
                if (memberActivation == null)
                {
                    message = $"C-MONEY member Login not found for Login CustomerId: {request.LoginId}.";
                    _logger.LogInformation(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.NotFound, LogAction.CMoneyMemberUpdatePin, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(message);
                }

                // Verify the old PIN
                if (!PinSecurity.VerifyPin(request.OldPin, memberActivation.PIN))
                {
                    message = $"Incorrect old PIN provided for member login {memberActivation.Id}.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Conflict, LogAction.CMoneyMemberUpdatePin, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(message);
                }

                // Hash the new PIN
                memberActivation.PIN = PinSecurity.HashPin(request.NewPin);
                memberActivation.HasChangeDefaultPin=true;

                // Update the repository
                _CMoneyMembersActivationAccountRepository.Update(memberActivation);

                // Save changes using Unit of Work
                await _uow.SaveAsync();

                message = $"PIN successfully changed for login {memberActivation.Id}. New Pin: ("+request.NewPin+")";
                _logger.LogInformation(message);
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.CMoneyMemberUpdatePin, LogLevelInfo.Information);
                string smsMessage = "TSC:CHANGE-NEW PIN: ("+request.NewPin+"). LOGIN: ("+request.LoginId+")";
                var smsRequest = new SubSmsRequestDto
                {
                    Message = smsMessage,
                    Msisdn = memberActivation.PhoneNumber,
                    Token = _userInfoToken.Token
                };

                // Send SMS
                await _smsHelper.SendSms(smsRequest);
                return ServiceResponse<bool>.ReturnResultWith200(true, message);
            }
            catch (Exception e)
            {
                // Log error and return an error response
                var errorMessage = $"Error occurred while updating PIN for member Login CustomerId: {request.LoginId}: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CMoneyMemberUpdatePin, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }



}
