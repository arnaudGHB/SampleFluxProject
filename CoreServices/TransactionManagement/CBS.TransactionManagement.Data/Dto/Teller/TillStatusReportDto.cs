using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Data.Dto
{
    public class TillStatusReportDto
    {
        public string TillId { get; set; }
        public string TillName { get; set; }
        public string TillUserName { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string Satus { get; set; }
        public decimal CashAtHand { get; set; }
        public decimal LastOperationAmount { get; set; }
        public string LastOperationType { get; set; }
        public CurrencyNotesRequest OpeningDenominations { get; set; }
    }

    public class VaultStatusReportDto
    {
        public string VaultName { get; set; }
        public string BranchCode { get; set; }
        public string Leader { get; set; }
        public string Satus { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal PreviouseBalance { get; set; }
        public decimal LastOperationAmount { get; set; }
        public string LastOperationType { get; set; }
        public CurrencyNotesRequest OpeningDenominations { get; set; }
    }
}
