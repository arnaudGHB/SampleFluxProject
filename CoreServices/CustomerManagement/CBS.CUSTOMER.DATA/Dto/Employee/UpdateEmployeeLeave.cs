using CBS.CUSTOMER.DATA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class CreateEmployeeLeave
    {

        public string? EmployeeLeaveId { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public DateTime LeaveEndDate { get; set; }

        public string? Approver { get; set; }

        public string? LeavetypeId { get; set; }

        public string? EmployeeId { get; set; }





    }
}
