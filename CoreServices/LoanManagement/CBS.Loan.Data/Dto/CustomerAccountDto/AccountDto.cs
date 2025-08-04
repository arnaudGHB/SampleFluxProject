using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Dto.CustomerAccountDto
{
    public class AccountDto
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; } = 0;
        public decimal BlockedAmount { get; set; } = 0;
        public decimal PreviousBalance { get; set; } = 0;
        public string Status { get; set; }
        public string ProductId { get; set; }
        public string? CustomerId { get; set; }
        public string? TellerId { get; set; }
        public string? EncryptedBalance { get; set; }
        public decimal InterestGenerated { get; set; }
        public decimal LastInterestPosted { get; set; }
        public DateTime? DateOfLastOperation { get; set; } = DateTime.MinValue;
        public DateTime? LastInterestCalculatedDate { get; set; } = DateTime.MinValue;
        public string? AccountName { get; set; }
        public string? LastOperation { get; set; }
        public string AccountType { get; set; }
        public bool IsTellerAccount { get; set; }
        public decimal OpeningBalance { get; set; }
        public DateTime? DateOfOpeningBalance { get; set; } = DateTime.MinValue;
        public decimal? LastOperationAmount { get; set; } = 0;
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public virtual SavingProductDto Product { get; set; }
    }
}
