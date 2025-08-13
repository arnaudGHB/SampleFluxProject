using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Queries
{
    public class GetAllMemberAccountActivationPolicyQuery : IRequest<ServiceResponse<List<MemberAccountActivationPolicyDto>>>
    {
    }
}
