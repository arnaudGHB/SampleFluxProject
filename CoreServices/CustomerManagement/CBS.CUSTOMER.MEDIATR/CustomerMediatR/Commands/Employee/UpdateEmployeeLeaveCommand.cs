
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.Customer.MEDIATR
{
    /// <summary>
    /// Represents a command to add a new EmployeeLeave.
    /// </summary>
    public class AddEmployeeLeaveCommand : IRequest<ServiceResponse<CreateEmployeeLeave>>
    {

        public DateTime LeaveStartDate { get; set; }
        public DateTime LeaveEndDate { get; set; }

        public string? Approver { get; set; }

        public string? LeavetypeId { get; set; }

        public string? EmployeeId { get; set; }


    }

}
