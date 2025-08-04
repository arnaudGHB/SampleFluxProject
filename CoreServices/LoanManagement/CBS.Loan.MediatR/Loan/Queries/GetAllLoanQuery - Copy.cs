using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanMediaR.Queries
{
    public class GetOpenLoansByBranchQuery : IRequest<ServiceResponse<List<LightLoanDto>>>
    {
        public string BranchId { get; set; }
    }

}
