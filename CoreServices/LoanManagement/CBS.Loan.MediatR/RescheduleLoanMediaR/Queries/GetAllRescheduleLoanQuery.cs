using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.RescheduleLoanMediaR.Queries
{
    public class GetAllRescheduleLoanQuery : IRequest<ServiceResponse<List<RescheduleLoanDto>>>
    {
    }
}
