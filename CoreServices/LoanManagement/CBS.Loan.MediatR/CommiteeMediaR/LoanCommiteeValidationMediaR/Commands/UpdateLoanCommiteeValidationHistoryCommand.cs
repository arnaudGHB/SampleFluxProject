using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateLoanCommiteeValidationHistoryCommand : IRequest<ServiceResponse<LoanCommiteeValidationHistoryDto>>
    {
        public string Id { get; set; }
        public string LoanApplicationId { get; set; }
        public string LoanCommiteeMemberId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
    }

}
