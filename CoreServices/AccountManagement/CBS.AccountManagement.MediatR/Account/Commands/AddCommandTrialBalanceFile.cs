using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using MediatR;
using Microsoft.Extensions.ObjectPool;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class AddCommandTrialBalanceFile : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public string FilePath { get; set; }
        public string Size { get; set; }
 
    }
}