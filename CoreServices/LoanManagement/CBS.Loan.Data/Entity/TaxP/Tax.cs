using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.NLoan.Data.Entity.TaxP
{
    public class Tax : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal TaxRate { get; set; }
        public bool AppliedWhenLoanRequestIsGreaterThanSaving { get; set; }
        public bool AppliedOnInterest { get; set; }
        public bool IsVat { get; set; }
        public decimal SavingControlAmount { get; set; }
        public string Description { get; set; }
        public virtual ICollection<LoanApplication> LoanApplication { get; set; }
    }
}
