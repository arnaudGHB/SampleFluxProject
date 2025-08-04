using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Queries
{


    public class CMoneyMembersPagginationQuery : IRequest<ServiceResponse<CMoneyMembersActivationAccountsList>>
    {
        public CustomerResource CustomerResource { get; set; }

    }

}
