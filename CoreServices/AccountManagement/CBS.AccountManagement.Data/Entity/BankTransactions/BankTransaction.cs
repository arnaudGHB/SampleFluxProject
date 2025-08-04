using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class BankTransaction : BaseEntity
    {
      
        public string Id { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public string Balance { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceId { get; set; } //CashReplenishmenId Or DepositAnnoucementId
        public string TransactionType { get; set; }
        public string BankTransactionReference { get; set; }
        public string Description { get; set; }
 
        public string FileUpload { get; set; }
        public string ValueDate { get; set; }
    }
}
