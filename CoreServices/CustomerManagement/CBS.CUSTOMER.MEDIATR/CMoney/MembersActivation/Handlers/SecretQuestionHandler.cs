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
    /// Handles the retrieval and validation of secret questions and answers for C-MONEY members.
    /// </summary>
    public class SecretQuestionHandler : IRequestHandler<ManageSecretQuestionCommand, ServiceResponse<bool>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _CMoneyMembersActivationAccountRepository; // Repository for accessing C-MONEY activation data.
        private readonly ILogger<SecretQuestionHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _unitOfWork; // Unit of Work for transaction management.

        /// <summary>
        /// Constructor for initializing the SecretQuestionHandler.
        /// </summary>
        /// <param name="CMoneyMembersActivationAccountRepository">Repository for C-MONEY activation account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="unitOfWork">Unit of Work for transaction management.</param>
        public SecretQuestionHandler(
            ICMoneyMembersActivationAccountRepository CMoneyMembersActivationAccountRepository,
            ILogger<SecretQuestionHandler> logger,
            IUnitOfWork<POSContext> unitOfWork)
        {
            _CMoneyMembersActivationAccountRepository = CMoneyMembersActivationAccountRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handles the management of secret questions and answers for a C-MONEY member.
        /// </summary>
        /// <param name="request">The command containing secret question and answer details.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response indicating success or failure.</returns>
        public async Task<ServiceResponse<bool>> Handle(ManageSecretQuestionCommand request, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            try
            {
                _logger.LogInformation($"Starting management of secret question for LoginId {request.LoginId}.");

                // Retrieve the member's activation account by LoginId
                var memberActivation = await _CMoneyMembersActivationAccountRepository.FindAsync(request.LoginId);
                if (memberActivation == null)
                {
                    message = $"Member with LoginId {request.LoginId} not found.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.CMoneySecurityQuestionSET, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(message);
                }

                _logger.LogInformation($"Member found for LoginId {request.LoginId}. Updating secret question and answer.");

                // Update secret question and answer
                memberActivation.SecretQuestion = request.SecretQuestion;
                memberActivation.SecretAnswer = request.SecretAnswer;

                // Save changes
                _CMoneyMembersActivationAccountRepository.Update(memberActivation);
                await _unitOfWork.SaveAsync();

                message = $"Secret question and answer updated successfully for LoginId {request.LoginId}.";
                _logger.LogInformation(message);
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.CMoneySecurityQuestionSET, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, message);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while managing secret question and answer for LoginId {request.LoginId}: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CMoneySecurityQuestionSET, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }


}
