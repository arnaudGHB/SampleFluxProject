using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Queries
{
    public class GetAllLoanApplicationFeeQuery : IRequest<ServiceResponse<List<LoanApplicationFeeDto>>>
    {
    }
}
