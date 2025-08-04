
using CBS.BankMGT.Data;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankZoneBranchMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a BankZoneBranch.
    /// </summary>
    public class UpdateBankZoneBranchCommand : IRequest<ServiceResponse<BankZoneBranchDto>>
    {
        public string BankingZoneId { get; set; } // Identifier for the associated Banking Zone
        public string Type { get; set; } // Identifier for the corresponding bank branch
        public string BranchId { get; set; } // Identifier for the specific branch

    }

}
