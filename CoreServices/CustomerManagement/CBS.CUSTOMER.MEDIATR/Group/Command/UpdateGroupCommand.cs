using CBS.CUSTOMER.DATA.Dto;

using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Group.
    /// </summary>
    public class UpdateGroupCommand : IRequest<ServiceResponse<UpdateGroup>>
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupTypeId { get; set; }
        public string RegistrationNumber { get; set; }
        public string TaxPayerNumber { get; set; }
        public bool Active { get; set; }
    }

}
