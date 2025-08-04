using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.RefundMediaR.Commands
{
    public class AddLoanSORepaymentCommand : IRequest<ServiceResponse<SavingAmtWithRefundDto>>
    {
        public string LoanId { get; set; }
        public string TransactionReference { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal Interest { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalRepaymentAmount { get; set; }
        public string PaymentMethod { get; internal set; }
        public string PaymentChannel { get; internal set; }
    }
}
