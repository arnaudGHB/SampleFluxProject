using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanCommeteeMemberCommand : IRequest<ServiceResponse<LoanCommiteeMemberDto>>
    {
        public string LoanCommiteeGroupId { get; set; }
        public string UserId { get; set; }
   
    }

}
