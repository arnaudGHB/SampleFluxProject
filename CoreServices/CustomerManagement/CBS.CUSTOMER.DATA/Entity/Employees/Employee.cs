using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class Employee : BaseEntity
    {
        [Key]
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
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


        public virtual Department? Department { get; set; }
        public virtual JobTitle? JobTitle { get; set; }

        public virtual List<EmployeeLeave>? EmployeeLeaves { get; set; }
        public virtual List<EmployeeTraining>? EmployeeTrainings { get; set; }

    }
}
