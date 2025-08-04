using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeP.Queries
{
    public class GetAllFeeQuery : IRequest<ServiceResponse<List<FeeDto>>>
    {
    }
}
