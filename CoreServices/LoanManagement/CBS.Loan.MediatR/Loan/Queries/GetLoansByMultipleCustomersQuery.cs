using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanMediaR.Queries
{
    public class GetLoansByMultipleCustomersQuery : IRequest<ServiceResponse<List<LoanDto>>>
    {
        public List<string> CustomerIds { get; set; }
        public string QueryParameter { get; set; }

    }
}
