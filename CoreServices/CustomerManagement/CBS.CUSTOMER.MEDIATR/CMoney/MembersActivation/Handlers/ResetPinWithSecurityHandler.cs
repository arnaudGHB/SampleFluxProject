using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Command;
using CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation;
using MediatR;
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
    /// Handles the reset of a C-MONEY member's PIN using the provided secret question and answer.
    /// </summary>
    public class ResetPinWithSecurityHandler : IRequestHandler<ResetPinWithSecurityCommand, ServiceResponse<bool>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _CMoneyMembersActivationAccountRepository; // Repository for accessing C-MONEY activation data.
        private readonly ILogger<ResetPinWithSecurityHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _unitOfWork; // Unit of Work for transaction management.

        /// <summary>
        /// Constructor for initializing the ResetPinWithSecurityHandler.
        /// </summary>
        /// <param name="CMoneyMembersActivationAccountRepository">Repository for C-MONEY activation account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="unitOfWork">Unit of Work for transaction management.</param>
        public ResetPinWithSecurityHandler(
            ICMoneyMembersActivationAccountRepository CMoneyMembersActivationAccountRepository,
            ILogger<ResetPinWithSecurityHandler> logger,
            IUnitOfWork<POSContext> unitOfWork)
        {
            _CMoneyMembersActivationAccountRepository = CMoneyMembersActivationAccountRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the reset PIN process using security question and answer.
        /// </summary>
        /// <param name="request">The command containing login ID, secret question, and answer.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response indicating success or failure.</returns>
        public async Task<ServiceResponse<bool>> Handle(ResetPinWithSecurityCommand request, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            try
            {
                _logger.LogInformation($"Starting PIN reset process for LoginId {request.LoginId}.");

                // Retrieve the member's activation account by LoginId
                var memberActivation = await _CMoneyMembersActivationAccountRepository.FindBy(x => x.LoginId == request.LoginId).FirstOrDefaultAsync();
                if (memberActivation == null)
                {
                    message = $"Member with LoginId {request.LoginId} not found.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.NotFound, LogAction.CMoneyResetPin, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(message);
                }

                _logger.LogInformation($"Member found for LoginId {request.LoginId}. Validating secret question and answer.");

                // Validate secret question and answer
                if (!string.Equals(memberActivation.SecretQuestion, request.SecretQuestion, StringComparison.OrdinalIgnoreCase) ||
                    !string.Equals(memberActivation.SecretAnswer, request.SecretAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    message = $"Invalid secret question or answer for LoginId {request.LoginId}.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.CMoneyResetPin, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return403(message);
                }

                _logger.LogInformation($"Secret question and answer validated for LoginId {request.LoginId}. Proceeding to reset PIN.");
                string NewPin = BaseUtilities.GenerateUniqueNumber(4);

                // Reset PIN to default
                memberActivation.PIN = PinSecurity.HashPin(NewPin);
                memberActivation.FailedAttempts = 0;

                // Save changes
                _CMoneyMembersActivationAccountRepository.Update(memberActivation);
                await _unitOfWork.SaveAsync();

                message = $"PIN reset successfully for LoginId {request.LoginId}. New PIN: {NewPin}";
                _logger.LogInformation(message);
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.CMoneyResetPin, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, message);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while resetting PIN for LoginId {request.LoginId}: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CMoneyResetPin, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }

}
