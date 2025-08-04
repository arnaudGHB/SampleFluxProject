using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.FundingLineP;
using CBS.NLoan.Data.Entity.PenaltyP;
using CBS.NLoan.Data.Entity.TaxP;

namespace CBS.NLoan.Data.Dto.LoanApplicationP
{
    public class LoanProductConfigurationAgregatesDto
    {
        public List<Tax> Taxes { get; set; }
        public List<FeeDto> Fees { get; set; }
        public List<Penalty> Penalties { get; set; }
        public List<DocumentPackDto> DocumentPacks { get; set; }
        public List<FundingLine> FundingLines { get; set; }
    }
}
