
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.Customer.MEDIATR
{
    /// <summary>
    /// Represents a command to add a new Employee.
    /// </summary>
    public class UpdateEmployeeCommand : IRequest<ServiceResponse<UpdateEmployee>>
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
