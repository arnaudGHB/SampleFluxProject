using CBS.NLoan.Data.Dto.PenaltyP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.PenaltyMediaR.Queries
{
    public class GetAllPenaltyQuery : IRequest<ServiceResponse<List<PenaltyDto>>>
    {
    }
}
