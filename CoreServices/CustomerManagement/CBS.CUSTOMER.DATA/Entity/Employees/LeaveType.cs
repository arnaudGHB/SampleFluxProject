using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class LeaveType : BaseEntity
    {
        [Key]
        public int LeaveTypeId { get; set; }
        public string? LeaveTypeName { get; set; }

    }
}
