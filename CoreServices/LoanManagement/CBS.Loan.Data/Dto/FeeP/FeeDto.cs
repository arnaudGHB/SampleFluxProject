using CBS.NLoan.Data.Entity.FeeP;

namespace CBS.NLoan.Data.Dto.FeeP
{
    public class FeeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FeeBase { get; set; }//Percentage Or Range
        public string AccountingEventCode { get; set; }
        public bool IsBoforeProcesing { get; set; }


        public List<FeeRangeDto> FeeRanges { get; set; }
    }
}
