using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Queries
{
    public class GetAllLoanApplicationByCustomerIdQuery : IRequest<ServiceResponse<List<LoanApplicationDto>>>
    {
        public string CustomerId { get; set; }
    }
}
