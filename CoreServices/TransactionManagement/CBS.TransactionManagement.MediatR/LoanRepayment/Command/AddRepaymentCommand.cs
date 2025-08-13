using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.LoanRepayment.Command
{
    public class AddRepaymentCommand : IRequest<ServiceResponse<RefundDto>>
    {
        public string LoanId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentChannel { get; set; }
        public string TransactionCode { get; set; }

    }
}
