using CBS.CUSTOMER.DATA.Dto;

using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Group.
    /// </summary>
    public class UpdateGroupTypeCommand : IRequest<ServiceResponse<UpdateGroupTypeDto>>
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }



    }

}
