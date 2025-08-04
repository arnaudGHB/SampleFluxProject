using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a UpdateBudgetCommand.
    /// </summary>
    public class UpdateTrailBalanceUploudCommand : IRequest<ServiceResponse<TrailBalanceUploudDto>>
    {

        public string Id { get; set; }
        public string BranchName { get; set; }

        public string UserName { get; set; }

        public string AccountAbsent { get; set; }
        public string FilePath { get; set; }


    }
}