using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanDisbumentCommand : IRequest<ServiceResponse<bool>>
    {
        public string LoanId { get; set; }
        public string AccountNumber { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public bool IsAlreadyManage { get; set; }
    }

}
