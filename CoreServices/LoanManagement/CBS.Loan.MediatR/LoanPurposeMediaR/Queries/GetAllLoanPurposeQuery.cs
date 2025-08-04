using CBS.NLoan.Data.Dto.LoanPurposeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanPurposeMediaR.Queries
{
    public class GetAllLoanPurposeQuery : IRequest<ServiceResponse<List<LoanPurposeDto>>>
    {
    }
}
