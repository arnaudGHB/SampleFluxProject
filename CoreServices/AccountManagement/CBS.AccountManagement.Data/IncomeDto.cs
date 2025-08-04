namespace CBS.AccountManagement.Data
{
    public class IncomeDto
    {
        public string DocumentId { get; set; }
        public string DocumentTypeId { get; set; }
        public string? Reference { get; set; }
        public string? HeadingFR { get; set; }
        public string? HeadingEN { get; set; }
        public string? AccountList { get; set; }
        public string? AccountExceptionList { get; set; }
    }
}