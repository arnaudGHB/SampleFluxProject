using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.RefundMediaR.Queries
{
    public class GetAllRefundQuery : IRequest<ServiceResponse<List<RefundDto>>>
    {
    }
}
