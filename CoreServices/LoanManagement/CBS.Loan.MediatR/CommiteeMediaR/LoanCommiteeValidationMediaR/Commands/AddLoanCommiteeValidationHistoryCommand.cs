using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanCommiteeValidationHistoryCommand : IRequest<ServiceResponse<LoanCommiteeValidationHistoryDto>>
    {
        public string LoanApplicationId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
    }

}
