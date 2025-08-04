using CBS.TransactionManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.NewFolder.LoanProcessingFeeServices;

namespace CBS.TransactionManagement.MediatR.LoanRepayment.Command
{
    public class FeePaymentConfirmationCommand : IRequest<ServiceResponse<List<FeePaymentConfirmationDto>>>
    {
        public string LoanApplicationId { get; set; }
        public string TransactionReference { get; set; }
        public string Period { get; set; }

        public List<LoanPaymentRequest> PaymentRequests { get; set; }

    }

    public class LoanPaymentRequest
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
