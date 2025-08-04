using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class CreateOrganizationCustomer 
    {

        public string? OrganizationCustomerId { get; set; }
        public string? CustomerId { get; set; }
        public string? OrganizationId { get; set; }
        public string? Position { get; set; }



        public Organization? Organization { get; set; }
        public Customer? Customer { get; set; }

    }
}
