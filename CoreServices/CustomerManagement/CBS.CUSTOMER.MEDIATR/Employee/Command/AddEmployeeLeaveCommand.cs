
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.Customer.MEDIATR
{
    /// <summary>
    /// Represents a command to Update EmployeeLeave.
    /// </summary>
    public class UpdateEmployeeLeaveCommand : IRequest<ServiceResponse<UpdateEmployeeLeave>>
    {
        public string? EmployeeLeaveId { get; set; }
        public DateTime LeaveStartDate { get; set; }
        public DateTime LeaveEndDate { get; set; }

        public string? Approver { get; set; }

        public string? LeavetypeId { get; set; }

        public string? EmployeeId { get; set; }


    }

}
