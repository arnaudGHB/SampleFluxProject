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
    /// Handles the command to reactivate a C-MONEY member.
    /// </summary>
    public class ReactivateCMoneyMemberCommandHandler : IRequestHandler<ReactivateCMoneyMemberCommand, ServiceResponse<bool>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _CMoneyMembersActivationAccountRepository;
        private readonly ILogger<ReactivateCMoneyMemberCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the ReactivateCMoneyMemberCommandHandler.
        /// </summary>
        /// <param name="CMoneyMembersActivationAccountRepository">Repository for C-MONEY activation account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for transaction management.</param>
        public ReactivateCMoneyMemberCommandHandler(
            ICMoneyMembersActivationAccountRepository CMoneyMembersActivationAccountRepository,
            ILogger<ReactivateCMoneyMemberCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _CMoneyMembersActivationAccountRepository = CMoneyMembersActivationAccountRepository;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the command to reactivate a C-MONEY member.
        /// </summary>
        /// <param name="request">The command containing the member ID to reactivate.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response indicating success or failure.</returns>
        public async Task<ServiceResponse<bool>> Handle(ReactivateCMoneyMemberCommand request, CancellationToken cancellationToken)
        {
            string message;
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

                    await BaseUtilities.LogAndAuditAsync(
                        message,
                        request,
                        HttpStatusCodeEnum.NotFound,
                        LogAction.CMoneyMemberReactivation,
                        LogLevelInfo.Warning
                    );

                    return ServiceResponse<bool>.Return404(message);
                }

                // Reactivate the member
                memberActivation.IsActive = true;
                memberActivation.DeactivationReason = null; // Clear any previous deactivation reason
                memberActivation.FailedAttempts = 0;
                // Update the repository
                _CMoneyMembersActivationAccountRepository.Update(memberActivation);

                // Save changes using Unit of Work
                await _uow.SaveAsync();

                message = $"Member {request.CustomerId} successfully reactivated.";
                _logger.LogInformation(message);

                await BaseUtilities.LogAndAuditAsync(
                    message,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.CMoneyMemberReactivation,
                    LogLevelInfo.Information
                );

                return ServiceResponse<bool>.ReturnResultWith200(true, message);
            }
            catch (Exception e)
            {
                message = $"Error occurred while reactivating member {request.CustomerId}: {e.Message}";
                _logger.LogError(message);

                await BaseUtilities.LogAndAuditAsync(
                    message,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.CMoneyMemberReactivation,
                    LogLevelInfo.Error
                );

                return ServiceResponse<bool>.Return500(message);
            }
        }
    }



}
