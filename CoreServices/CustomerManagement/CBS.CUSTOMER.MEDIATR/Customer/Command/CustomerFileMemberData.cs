using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.Customer.Command
{
    public  class CustomerFileMemberData
    {
        public string? MemberNumber { get; set; }
        public string? MemberName { get; set; }
        public string? MemberSurName { get; set; }
        public string? Cni { get; set; }
        public string? CniDeliveranceDate { get; set; }
        public string? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? CniLocation { get; set; }
        public string? Telephone { get; set; }
        public string? CreationDate { get; set; }
        public string? Type { get; set; }
        public string? Town { get; set; }
        public string? Quater { get; set; }
        public string? Genre { get; set; }
        public string? BranchCode { get; set; }
    }
}
