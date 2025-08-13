using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.Loans.Queries
{
    public class GetLoanRepaymentOrderQuery : IRequest<ServiceResponse<LoanRepaymentOrderDto>>
    {
        public string  LoanRepaymentOrderType { get; set; }
    }
}
