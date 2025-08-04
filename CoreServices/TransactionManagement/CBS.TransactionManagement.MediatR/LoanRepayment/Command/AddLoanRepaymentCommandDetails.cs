using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.LoanRepayment.Command
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class AddLoanRepaymentCommandDetails : IRequest<ServiceResponse<RefundDto>>
    {
        public decimal Amount { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Tax { get; set; }
        public decimal Penalty { get; set; }
        public string LoanId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentChannel { get; set; }
        public string Comment { get; set; }
        public string TransactionCode { get; set; }
    }
  
}




