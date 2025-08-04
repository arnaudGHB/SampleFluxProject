using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateLoanCommeteeMemberCommand : IRequest<ServiceResponse<LoanCommiteeMemberDto>>
    {
        public string Id { get; set; }
        public string LoanCommiteeGroupId { get; set; }
        public string UserId { get; set; }

    }

}
