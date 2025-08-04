using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.RefundMediaR.Commands
{
    public class AddRefundCommand : IRequest<ServiceResponse<RefundDto>>
    {
        public string LoanId { get; set; }
        public decimal Amount { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Tax { get; set; }
        public decimal Penalty { get; set; }
        public string TransactionCode { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentChannel { get; set; }
        public string Comment { get; set; }

    }
}
