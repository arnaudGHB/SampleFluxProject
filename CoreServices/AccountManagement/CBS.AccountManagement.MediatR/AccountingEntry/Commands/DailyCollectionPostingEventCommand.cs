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
    public class DailyCollectionPostingEventCommand : IRequest<ServiceResponse<bool>>
    {
        public string TellerSource { get; set; }//DailyCollector
        public string? TransactionReferenceId { get; set; } // Loan contract reference 
        public string? ProductId { get;  set; } // Daily saving productId
        public DateTime TransactionDate { get; set; }
        public string? MemberReference { get; set; } // MemberReference
        public List<DailyAmountCollection>? DailyAmountCollection { get; set; }
        
    }
   
    public class DailyAmountCollection
    {
        public string? EventAttributeName { get; set; }
        public decimal Amount { get; set; }
        public bool IsPrincipal { get; set; }
        public string Naration { get;   set; }

        public string GetDailySavingAccountEventCode(string ProductId)
        {
            return ProductId + "@Principal_Saving_Account";
        }



      
    }
}
