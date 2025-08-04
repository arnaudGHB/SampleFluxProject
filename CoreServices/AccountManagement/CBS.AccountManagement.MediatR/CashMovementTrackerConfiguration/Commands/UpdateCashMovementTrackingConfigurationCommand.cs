using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a AccountPolicyCommand.
    /// </summary>
 

    public class UpdateCashMovementTrackingConfigurationCommand : IRequest<ServiceResponse<CashMovementTrackingConfigurationDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MovementType { get; set; }
        public string Duration { get; set; }

        public string AlertTime { get; set; }
        public string MessageBeforeAlertTime { get; set; }
        public string MessageAfterAlertTime { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}