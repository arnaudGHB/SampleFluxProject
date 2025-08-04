

using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;


namespace CBS.Customer.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new GroupCustomer.
    /// </summary>
    public class AddGroupCustomerCommand : IRequest<ServiceResponse<List<CreateGroupCustomer>>>
    {

        public List<string> CustomerIds { get; set; }
        public string GroupId { get; set; }
        public bool commit { get; set; } = true;


    }

}
