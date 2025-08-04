using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a AccountPolicyCommand.
    /// </summary>
    public class UpdateAccountPolicyCommand : IRequest<ServiceResponse<CashMovementTrackerDto>>
    
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public decimal MaximumAlert { get; set; }
        public decimal MinimumAlert { get; set; }

        public string Name { get; set; }
        public string MinMessage { get; set; }

        public string MaxMessage { get; set; }
    }
}