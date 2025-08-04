
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new customer.
    /// </summary>
    public class ChangeMembershipStatusCommand : IRequest<ServiceResponse<CreateCustomer>>
    {
        public string CustomerId { get; set; }

        public string MembershipApprovalStatus { get; set; }
        public string BranchCode { get; set; }
        public bool IsAutomatic { get; set; }
    }

}
