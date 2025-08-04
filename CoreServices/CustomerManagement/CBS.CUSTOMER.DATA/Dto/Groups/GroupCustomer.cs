
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.HELPER.Helper;


namespace CBS.CUSTOMER.DATA.Dto
{
    public class CreateGroupCustomer
    {

        public string? CustomerGroupId { get; set; }

        public bool IsGroupLeader { get; set; }
        public bool IsCurrentlyIn { get; set; }
        public DateTime LeftDate { get; set; }

        public string? GroupId { get; set; }
        public string? CustomerId { get; set; }

    }
}
