using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CycleNameMediaR.Queries
{
    public class GetAllLoanProductCategoryQuery : IRequest<ServiceResponse<List<LoanProductCategoryDto>>>
    {
    }
}
