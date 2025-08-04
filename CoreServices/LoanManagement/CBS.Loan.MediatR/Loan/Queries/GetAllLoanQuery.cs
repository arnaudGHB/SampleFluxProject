using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanMediaR.Queries
{
    public class GetAllLoanQuery : IRequest<ServiceResponse<List<LoanDto>>>
    {
        public string? QueryParam { get; set; }
        public bool IsByBranch { get; set; }
        public string? BranchId { get; set; }
    }
}
