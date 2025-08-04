using System.ComponentModel.DataAnnotations;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class OrganizationCustomer : BaseEntity
    {
        public string? OrganizationCustomerId { get; set; }
        public string? CustomerId { get; set; }
        public string? OrganizationId { get; set; }
        public string? Position { get; set; }
    }
}
