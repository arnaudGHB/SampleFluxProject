using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanMediaR.Queries
{
    public class GetLoanByCustomerOpenLoanLightQuery : IRequest<ServiceResponse<List<LoanDto>>>
    {
        public string CustomerId { get; set; }
    }
}
