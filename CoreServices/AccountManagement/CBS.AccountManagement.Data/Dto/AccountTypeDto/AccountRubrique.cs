namespace CBS.AccountManagement.Data
{
    public class ProductAccountRubric
    {
        public string? OperationEventAttributeName{ get; set; }
        public bool? IsDebit { get; set; } //true if debit, false if credit
        public string? ChartOfAccountId { get; set; } //ChartOfAccountId 
    }
}