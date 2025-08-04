
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.Customer.MEDIATR
{
    /// <summary>
    /// Represents a command to add a new Department.
    /// </summary>
    public class AddDepartmentCommand : IRequest<ServiceResponse<CreateDepartment>>
    {

        public string? DepartmentName { get; set; }



    }

}
