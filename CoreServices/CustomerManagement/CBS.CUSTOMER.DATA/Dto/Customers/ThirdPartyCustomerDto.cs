using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.Customers
{
    public class ThirdPartyCustomerDto
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string VillageOfOrigin { get; set; }
        public string DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public string Occupation { get; set; }
        public string Address { get; set; }
        public string IDNumber { get; set; }
        public string IDNumberIssueDate { get; set; }
        public string IDNumberIssueAt { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
    }
}
