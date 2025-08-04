using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.MediatR.AccountingEntry.Commands;
using MediatR;
using CBS.AccountManagement.Helper;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class MobileMoneyOperationCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReference { get; set; }
        public string? MemberReference { get; set; }
        public decimal Amount { get; set; }
        public string OperationType { get; set; }
        public string TellerSources { get; set; }
 
        public DateTime TransactionDate { get; set; }
        public string? Naration { get;  set; }
    }

   
}
