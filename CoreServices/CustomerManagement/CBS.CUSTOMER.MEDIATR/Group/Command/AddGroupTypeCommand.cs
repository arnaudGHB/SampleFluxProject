
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto.Groups;

namespace CBS.Customer.MEDIATR
{
    /// <summary>
    /// Represents a command to add a new GroupType.
    /// </summary>
    public class AddGroupTypeCommand : IRequest<ServiceResponse<CreateGroupTypeDto>>
    {
        public string GroupTypeName { get; set; }
        public string? Description { get; set; }

    }

}
