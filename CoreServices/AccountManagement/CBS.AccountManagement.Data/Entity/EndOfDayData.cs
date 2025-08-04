using CBS.AccountManagement.Data;

namespace CBS.AccountManagement.Data
{
    public class EndOfDayData : BaseEntity
    {
        public decimal BeginningBalance { get; set; }
        public decimal DebitBalance { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal EndingBalance { get; set; }
        public List<AccountingEntryDto> Entries { get; set; }
    }

    public class CloseOfDayData
    {
        public bool WasProcessingSuccessful { get; set; }
        public DateTime ClosingTime { get; set; }
        public List<EndOfDayData> Entries { get; set; }
    }
}