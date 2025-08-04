using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class EmployeeLeave : BaseEntity
    {
        [Key]
        public string? EmployeeLeaveId { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public DateTime LeaveEndDate { get; set; }
     
        public string? Approver { get; set; }

        public string? EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }

        public LeaveType? LeaveType { get; set; }

        public Employee? Employee { get; set; }

    }
}
