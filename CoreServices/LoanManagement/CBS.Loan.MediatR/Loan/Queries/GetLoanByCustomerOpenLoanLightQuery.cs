using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanMediaR.Queries
{
    public class GetAllLoanByCustomerIdQuery : IRequest<ServiceResponse<List<LoanDto>>>
    {
        public string CustomerId { get; set; }
        public string QueryParameter { get; set; }
    }
}
