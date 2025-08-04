using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.FeeP
{
    public class Fee : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FeeBase { get; set; }//Percentage Or Range
        public string? AccountingEventCode { get; set; }

        public bool IsBoforeProcesing { get; set; }
        public virtual ICollection<FeeRange> FeeRanges { get; set; }

    }


}
