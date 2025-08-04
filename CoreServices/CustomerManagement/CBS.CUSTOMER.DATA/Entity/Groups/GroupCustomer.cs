
using CBS.CUSTOMER.HELPER.Helper;
using System.ComponentModel.DataAnnotations;


namespace CBS.CUSTOMER.DATA.Entity
{
    public class GroupCustomer : BaseEntity
    {
        [Key]
        public string GroupCustomerId { get; set; }
        public bool IsGroupLeader { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string GroupId { get; set; }
        public string CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Group? Group { get; set; }
    }
}
