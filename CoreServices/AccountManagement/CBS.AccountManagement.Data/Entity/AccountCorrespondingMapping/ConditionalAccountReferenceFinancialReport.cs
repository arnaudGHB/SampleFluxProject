namespace CBS.AccountManagement.Data
{
    public class ConditionalAccountReferenceFinancialReport
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string DocumentReferenceCodeId { get; set; }


        public virtual DocumentReferenceCode DocumentReferenceCode { get; set; }
    }
}