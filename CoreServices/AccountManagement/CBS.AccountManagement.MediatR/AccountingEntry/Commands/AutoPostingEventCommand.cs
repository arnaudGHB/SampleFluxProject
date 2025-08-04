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
    public class AutoPostingEventCommand : IRequest<ServiceResponse<bool>>
    {
        public string Source { get; set; }//Members_Account, Teller_Account,Vitual_Teller
        public string? TransactionReferenceId { get; set; } // Loan contract reference 
        public string? ProductId { get;  set; } // productId is set whent source is equals Members_Account
        public DateTime TransactionDate { get; set; }
        public string? MemberReference { get; set; }
        public List<AmountEventCollection>? AmountEventCollections { get; set; }
  
        public bool IsmemberAccount()
        {
           return this.Source == "Members_Account";
        }
        public string GetAccountingEventCode()
        {
            return this.ProductId + "@Principal_Saving_Account";
        }
    }
    public class AmountEventCollection  
    {

        public string? EventCode { get; set; }   // Generated from rule
        public decimal Amount { get; set; }
        public string? Naration { get; set; }
    }
  
}
