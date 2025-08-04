using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Queries
{
    /// <summary>
    /// Query for retrieving a C-MONEY member activation by ID.
    /// </summary>
    public class GetCMoneyMemberActivationByIdQuery : IRequest<ServiceResponse<CMoneyMembersActivationAccountDto>>
    {
        /// <summary>
        /// The ID of the activation record to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
