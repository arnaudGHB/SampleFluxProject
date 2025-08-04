using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
    public class AccountingPostingConfirmation:BaseEntity
    {
        public string Id { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime ValueDate { get; set; }
        public string EntryType { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string ReferenceId { get; set; }
        public string Status { get; set; }
        public string ReviewedBy { get; set; }
        public string Source { get; set; }
        public string EventCode { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string OperationType { get; set; }
        public string AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string ConfirmationID { get; set; }
    }

}
