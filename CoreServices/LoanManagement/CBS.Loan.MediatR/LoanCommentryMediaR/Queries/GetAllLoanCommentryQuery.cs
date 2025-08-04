using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanCommentryMediaR.Queries
{
    public class GetAllLoanCommentryQuery : IRequest<ServiceResponse<List<LoanCommentryDto>>>
    {
    }
}
