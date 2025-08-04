using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.FundingLineP
{
    public class FundingLine : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public string CurrencyId { get; set; }
        public string AccountingRuleId { get; set; }
        public string OrganizationId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
    }
}
