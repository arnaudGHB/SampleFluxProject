using CBS.AccountManagement.Data;
 
using CBS.AccountManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddOperationEventNameCommand.
    /// </summary>
    public class AddCashMovementTrackingConfigurationCommand : IRequest<ServiceResponse<CashMovementTrackingConfigurationDto>>
    {

 
        public string Name { get; set; }
        public string MovementType { get; set; }
        public string Duration { get; set; }

        public string AlertTimeBefore { get; set; }
        public string MessageBeforeAlertTime { get; set; }
        public string AlertTimeAfter { get; set; }
        public string MessageAfterAlertTime { get; set; }
        public string From { get; set; }
        public string To { get; set; }

    }
}