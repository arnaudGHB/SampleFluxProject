using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Queries
{
    public class GetAllFeeRangeQuery : IRequest<ServiceResponse<List<FeeRangeDto>>>
    {
    }
}
