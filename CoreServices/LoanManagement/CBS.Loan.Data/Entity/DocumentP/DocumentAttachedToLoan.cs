using CBS.NLoan.Data.Entity.LoanApplicationP;

namespace CBS.NLoan.Data.Entity.DocumentP
{
    public class DocumentAttachedToLoan : BaseEntity
    {
        public string Id { get; set; }
        public string LoanApplicationId { get; set; }
        public string DocumentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public DateTime Date { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }
        public virtual Document Document { get; set; }
    }
}
