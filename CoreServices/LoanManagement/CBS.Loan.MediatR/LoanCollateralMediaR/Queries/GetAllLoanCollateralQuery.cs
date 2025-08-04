using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanCollateralMediaR.Queries
{
    public class GetAllLoanCollateralQuery : IRequest<ServiceResponse<List<LoanApplicationCollateralDto>>>
    {
    }
}
