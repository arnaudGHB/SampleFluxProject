using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductCollateralMediaR.Queries
{
    public class GetAllLoanProductCollateralQuery : IRequest<ServiceResponse<List<LoanProductCollateralDto>>>
    {
    }
}
