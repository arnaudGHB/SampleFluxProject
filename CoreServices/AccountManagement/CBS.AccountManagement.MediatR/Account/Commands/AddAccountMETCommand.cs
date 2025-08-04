using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Account.Commands
{
    
    /// <summary>
    /// Represents a command to use when creating an account during manual entry.
    /// </summary>
    public class AddAccountMETCommand : IRequest<ServiceResponse<AccountResponseDto>>
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountTypeId { get; set; } = "";
        public string AccountOwnerId { get; set; }
        public string ChartOfAccountManagementPositionId { get; set; }
        public string AccountNumberManagementPosition { get; set; }
        public string OwnerBranchCode { get; set; }
        public string LiaisonBranchCode { get; set; }
        public string AccountNumberNetwok { get; set; }
        public string AccountCategoryId { get; set; }
        public string AccountNumberCU { get; set; }
        public bool IsNormalCreation { get; set; }
    }

    public class AccountResponseDto  
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
       

    }


}
