using CBS.NLoan.Data.Dto.PeriodP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.PeriodMediaR.Queries
{
    public class GetAllPeriodQuery : IRequest<ServiceResponse<List<PeriodDto>>>
    {
    }
}
