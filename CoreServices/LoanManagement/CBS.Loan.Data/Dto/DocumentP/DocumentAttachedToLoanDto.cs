using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Data.Entity.LoanApplicationP;

namespace CBS.NLoan.Data.Dto.DocumentP
{
    public class DocumentAttachedToLoanDto
    {
        public string Id { get; set; }
        public string LoanApplicationId { get; set; }
        public string DocumentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public DateTime Date { get; set; }
        public LoanApplication LoanApplication { get; set; }
        public Document Document { get; set; }

    }
}
