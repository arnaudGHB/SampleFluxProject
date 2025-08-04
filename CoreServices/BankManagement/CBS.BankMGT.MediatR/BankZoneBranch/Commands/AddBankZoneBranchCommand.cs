 
using CBS.BankMGT.Data;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new BankZoneBranch.
    /// </summary>
    public class AddBankZoneBranchCommand : IRequest<ServiceResponse<BankZoneBranchDto>>
    {
        public string BankingZoneId { get; set; } // Identifier for the associated Banking Zone
        public string Type { get; set; } // Identifier for the corresponding bank branch
        public List<string> BranchId { get; set; } // Identifier for the specific branch

    }

}
