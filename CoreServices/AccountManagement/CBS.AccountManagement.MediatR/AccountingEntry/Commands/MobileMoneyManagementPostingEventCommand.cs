using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class MobileMoneyManagementPostingCommand : IRequest<ServiceResponse<bool>>
    {
        public string ChartOfAccountIdFrom { get; set; }
        public string? TransactionReferenceId { get; set; } 
        public string? ChartOfAccountIdTo { get;  set; }
         public string? Direction { get; set; }// HeadOffice-To-Branch,Branch-To-HeadOffice,Auxilary-To-Branch,Branch-To-Branch
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? MemberReference { get; set; }
        public string? Naration { get; internal set; }
    }

}
