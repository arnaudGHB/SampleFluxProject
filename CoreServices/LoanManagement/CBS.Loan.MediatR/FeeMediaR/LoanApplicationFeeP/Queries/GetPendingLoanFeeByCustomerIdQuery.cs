using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Queries
{
    public class GetPendingLoanFeeByCustomerIdQuery : IRequest<ServiceResponse<List<LoanApplicationFeeDto>>>
    {
        public string CustomerId { get; set; }

    }
}
