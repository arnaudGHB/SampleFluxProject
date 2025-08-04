using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanApplicationFeeCommand : IRequest<ServiceResponse<decimal>>
    {
        public List<string> FeeId { get; set; }
        public string LoanApplicationId { get; set; }
        public bool IsWithin { get; set; }
        public decimal Amount { get; set; }
        public bool IsCashDeskPayment { get; set; }
        public string CustomerId { get; set; }
    }

   
}
