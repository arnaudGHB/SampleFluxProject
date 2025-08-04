using CBS.NLoan.Data.Entity.LoanApplicationP;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Dto.DocumentP
{
    public class DocumentDto
    {
        public string Id { get; set; }
        public string DocumentType { get; set; }
        public string Name { get; set; }
        public string LinkDoc { get; set; }
        public List<LoanApplication> LoanApplication { get; set; }
    }

}
