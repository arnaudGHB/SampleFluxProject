using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.AlertProfileP
{
    public class AlertProfile : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Msisdn { get; set; }
        public bool SendSMS { get; set; }
        public bool SendEmail { get; set; }
        public bool IsSupperAdmin { get; set; }
        public string Language { get; set; }
        public bool ActiveStatus { get; set; }
        public string ServiceId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string OrganizationId { get; set; }
    }
}
