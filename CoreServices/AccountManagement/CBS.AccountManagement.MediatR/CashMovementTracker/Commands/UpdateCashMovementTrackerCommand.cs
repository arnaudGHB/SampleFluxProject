using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a AccountPolicyCommand.
    /// </summary>
 

    public class UpdateCashMovementTrackerCommand : IRequest<ServiceResponse<CashMovementTrackerDto>>
    {
        public string Id { get; set; }
        public string OperationType { get; set; }
        public string ReferenceId { get; set; }
        public string Constraint { get; set; }
        public string StartTime { get; set; }
        public string ExpectedEndTime { get; set; }
        public string EndTime { get; set; }
        public string CashMovementTrackingConfigurationId { get; set; }

    }
}