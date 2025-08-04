using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCollateralMediaR.Commands;
using CBS.NLoan.MediatR.LoanGuarantorMediaR.Commands;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class FeePaymentConfirmationCommand : IRequest<ServiceResponse<List<FeePaymentConfirmationDto>>>
    {
        public string LoanApplicationId { get; set; }
        public string TransactionReference { get; set; }
        public string Period { get; set; }
        public List<PaymentRequest> PaymentRequests { get; set; }

    }

    public class PaymentRequest
    {
        public string LoanApplicationFeeId { get; set; }
        public decimal Amount { get; set; }
    }
    public class FeePaymentConfirmationDto
    {
        public string FeeName { get; set; }
        public decimal AmountPaid { get; set; }
        public string EventCode { get; set; }
    }
}
