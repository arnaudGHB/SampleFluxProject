using CBS.CUSTOMER.DATA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class GetAllEmployees
    {
        public string? EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateJoined { get; set; }
        public string? Gender { get; set; }

        public string? NationalIdNumber { get; set; }

        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }

        public string? BranchOffice { get; set; }

        public bool IsManagement { get; set; }
        public string? Photo { get; set; }

        public string? DepartmentId { get; set; }
        public string? JobTitleId { get; set; }


    }
}
