using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class LineOfCredit
    {
        [Key]
        public string Id { get; set; }
        public double AmountMin { get; set; }
        public double AmountMax { get; set; }
        public int NumberInstallmentMin { get; set; }
        public int NumberInstallmentMax { get; set; }
    }
}
