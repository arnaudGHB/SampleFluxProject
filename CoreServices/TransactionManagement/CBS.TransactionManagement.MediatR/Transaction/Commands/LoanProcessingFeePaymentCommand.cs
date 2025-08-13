using CBS.TransactionManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.Commands
{

    public class LoanProcessingFeePaymentCommand : IRequest<ServiceResponse<bool>>
    {
        public string LoanApplicationId { get; set; }
        public List<PaymentRequest> PaymentRequests { get; set; }

    }

    public class PaymentRequest
    {
        public string LoanApplicationFeeId { get; set; }
        public decimal Amount { get; set; }
    }
}
