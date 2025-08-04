using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class DailyCollectionMonthlyCommisionEventCommand : IRequest<ServiceResponse<bool>>
    {
        public string ProductId { get; set; }//DailyCollector
        public string? TransactionReferenceId { get; set; }  
      //  public string? RevenueAccount { get;  set; } // Commission_Account
        public DateTime TransactionDate { get; set; }
        public string? Amount { get; set; }
        public string? MemberReference { get; set; }//DailyCollector
        public string Naration { get;   set; }
    }
   
 
}
