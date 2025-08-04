using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class GroupType:BaseEntity
    {
        [Key]
        public string GroupTypeId { get; set; }
        public string GroupTypeName { get; set; }
        public string? Description { get; set; }
    }
}
