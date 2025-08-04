using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.PeriodP
{
    public class Period : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
