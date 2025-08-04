using CBS.CUSTOMER.DATA.Entity.Groups;
using CBS.CUSTOMER.DATA.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.Groups
{

    public class GroupDto
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupTypeId { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? TaxPayerNumber { get; set; }
        public string? DateOfEstablishment { get; set; }
        public string? PhotoSource { get; set; }
        public string GroupLeaderId { get; set; }
        public bool Active { get; set; }
        public PaginationMetadata PaginationMetadata { get; set; }
        public GroupType? GroupType { get; set; }
        public List<GroupCustomer> GroupCustomers { get; set; }
        public List<GroupDocument> GroupDocuments { get; set; }
    }

}
