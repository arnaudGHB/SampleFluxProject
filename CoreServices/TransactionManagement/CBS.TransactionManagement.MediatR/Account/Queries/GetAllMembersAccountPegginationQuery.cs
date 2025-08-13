using CBS.TransactionManagement.Data.Dto.Resource;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using MediatR;

namespace CBS.TransactionManagement.MediatR.Queries
{

    public class GetAllMembersAccountPegginationQuery : IRequest<ServiceResponse<MemberAccountSituationListing>>
    {
        public AccountResource AccountResource { get; set; }

    }
}
