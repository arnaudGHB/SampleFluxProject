using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MemberAccountConfiguration.Queries
{
    public class GetAllMemberAccountActivationQuery : IRequest<ServiceResponse<List<TransferParameterDto>>>
    {
    }
}
