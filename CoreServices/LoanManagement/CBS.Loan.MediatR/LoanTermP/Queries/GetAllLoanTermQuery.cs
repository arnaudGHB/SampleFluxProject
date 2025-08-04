using CBS.NLoan.Data.Dto.LoanTermP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanTermP.Queries
{
    public class GetAllLoanTermQuery : IRequest<ServiceResponse<List<LoanTermDto>>>
    {
    }
}
