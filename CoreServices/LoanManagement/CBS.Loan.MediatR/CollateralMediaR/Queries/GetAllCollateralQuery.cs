using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CollateralMediaR.Queries
{
    public class GetAllCollateralQuery : IRequest<ServiceResponse<List<CollateralDto>>>
    {
    }
}
