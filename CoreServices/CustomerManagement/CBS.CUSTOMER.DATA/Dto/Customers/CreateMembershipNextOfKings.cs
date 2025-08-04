
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBS.CUSTOMER.HELPER.Helper;


namespace CBS.CUSTOMER.DATA.Dto
{
    public class CreateMembershipNextOfKings
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? CustomerId { get; set; }
        public string? Relation { get; set; }
        public string? Ratio { get; set; }
        public string? SignatureUrl { get; set; }
        public string? BranchId { get; set; }


    }
}
