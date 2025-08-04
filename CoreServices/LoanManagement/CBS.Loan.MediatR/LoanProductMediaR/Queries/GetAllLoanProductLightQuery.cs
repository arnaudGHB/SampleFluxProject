using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Queries
{
    public class GetAllLoanProductLightQuery : IRequest<ServiceResponse<List<LoanProductLightDto>>>
    {
    }
}
