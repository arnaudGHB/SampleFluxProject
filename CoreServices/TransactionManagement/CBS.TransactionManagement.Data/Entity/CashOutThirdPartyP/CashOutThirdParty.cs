using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.CashOutThirdPartyP
{
    public class CashOutThirdParty:BaseEntity
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountId { get; set; }
        public string? Status { get; set; } = "PENDING";
        public string? TransactionReference { get; set; }
        public string? ExternalTransactionReference { get; set; }
        public string? SourceType { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string? CustomerId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CallBackURL { get; set; }
        public DateTime DateOfInitiation { get; set; }
        public DateTime DateOfConfirmation { get; set; }
        public string? TellerId { get; set; }
        public virtual Teller Teller { get; set; }
        public virtual Account Account { get; set; }
    }
}
