using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanMediaR.Queries
{
  
    public class GetLoanDashboardQuery : IRequest<ServiceResponse<LoanMainDashboardDto>>
    {
        public string BranchId { get; set; }
        public string QueryParameter { get; set; }
    }
}
