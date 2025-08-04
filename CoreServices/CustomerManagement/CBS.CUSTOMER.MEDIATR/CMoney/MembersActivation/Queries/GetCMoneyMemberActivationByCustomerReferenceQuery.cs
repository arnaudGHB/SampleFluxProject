using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Queries
{
    /// <summary>
    /// Query for retrieving a C-MONEY member activation by CustomerReference.
    /// </summary>
    public class GetCMoneyMemberActivationByCustomerReferenceQuery : IRequest<ServiceResponse<CMoneyMembersActivationAccountDto>>
    {
        /// <summary>
        /// The CustomerReference of the activation record to be retrieved.
        /// </summary>
        public string CustomerReference { get; set; }
    }
}
