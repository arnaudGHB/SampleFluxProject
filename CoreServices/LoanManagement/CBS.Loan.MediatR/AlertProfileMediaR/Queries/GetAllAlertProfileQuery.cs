using CBS.NLoan.Data.Dto.AlertProfileP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.AlertProfileMediaR.Queries
{
    public class GetAllAlertProfileQuery : IRequest<ServiceResponse<List<AlertProfileDto>>>
    {
    }
}
