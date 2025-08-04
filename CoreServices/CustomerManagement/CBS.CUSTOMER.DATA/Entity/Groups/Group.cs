

using CBS.CUSTOMER.DATA.Entity.Groups;
using System.ComponentModel.DataAnnotations;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class Group : BaseEntity
    {
        [Key]
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupTypeId { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? TaxPayerNumber { get; set; }
        public string? DateOfEstablishment { get; set; }
        public string? PhotoSource { get; set; }
        public string GroupLeaderId { get; set; }
        public string BranchId { get; set; }
        public bool Active { get; set; }
        public virtual GroupType? GroupType { get; set; }
        public virtual ICollection<GroupCustomer> GroupCustomers { get; set;}
        public virtual ICollection<GroupDocument> GroupDocuments { get; set;}
    }
}
