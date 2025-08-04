using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanMediaR.Queries
{
    public class GetAllLoanByBranchIdQuery : IRequest<ServiceResponse<List<LoanDto>>>
    {
        public string BranchId { get; set; }
    }
}
