using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddBudgetCommand.
    /// </summary>
    public class AddTrailBalanceUploudCommand : IRequest<ServiceResponse<TrailBalanceUploudDto>>
    {
 
            public string BranchName { get; set; }

            public string UserName { get; set; }

            public string AccountAbsent { get; set; }
            public string FilePath { get; set; }


      

    }
}