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
    /// Handles the command to deactivate a C-MONEY member.
    /// </summary>
    public class DeactivateCMoneyMemberCommandHandler : IRequestHandler<DeactivateCMoneyMemberCommand, ServiceResponse<bool>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _CMoneyMembersActivationAccountRepository;
        private readonly ILogger<DeactivateCMoneyMemberCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeactivateCMoneyMemberCommandHandler.
        /// </summary>
        public DeactivateCMoneyMemberCommandHandler(
            ICMoneyMembersActivationAccountRepository CMoneyMembersActivationAccountRepository,
            ILogger<DeactivateCMoneyMemberCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _CMoneyMembersActivationAccountRepository = CMoneyMembersActivationAccountRepository;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the command to deactivate a C-MONEY member.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(DeactivateCMoneyMemberCommand request, CancellationToken cancellationToken)
        {
            string message;
            try
            {
                // Retrieve the member's activation account
                var memberActivation = await _CMoneyMembersActivationAccountRepository.FindBy(x => x.CustomerId == request.CustomerId).FirstOrDefaultAsync();
                if (memberActivation == null)
                {
                    message = $"C-MONEY member activation not found for member reference {request.CustomerId}.";
                    _logger.LogInformation(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.NotFound, LogAction.CMoneyMemberDeactivate, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(message);
                }

                // Deactivate the member and set the reason
                memberActivation.IsActive = false;
                memberActivation.DeactivationReason = request.Reason;

                // Update the repository
                _CMoneyMembersActivationAccountRepository.Update(memberActivation);

                // Save changes using Unit of Work
                await _uow.SaveAsync();

                message = $"Member {request.CustomerId} successfully deactivated with reason: {request.Reason}.";
                _logger.LogInformation(message);
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.CMoneyMemberDeactivate, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, message);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while deactivating member {request.CustomerId}: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CMoneyMemberDeactivate, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }



}
