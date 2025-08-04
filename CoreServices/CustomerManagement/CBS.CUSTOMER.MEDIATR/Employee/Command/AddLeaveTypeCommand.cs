
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.Customer.MEDIATR
{
    /// <summary>
    /// Represents a command to add a new LeaveType.
    /// </summary>
    public class AddLeaveTypeCommand : IRequest<ServiceResponse<CreateLeaveType>>
    {

        public string? LeaveTypeName { get; set; }



    }

}
