using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationCriteriaMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanCommiteeGroupCommand : IRequest<ServiceResponse<LoanCommiteeGroupDto>>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal MinimumLoanAmount { get; set; }
        public decimal MaximumLoanAmount { get; set; }
        public int NumberOfMembers { get; set; }
        public int NumberToApprovalsToValidationALoan { get; set; }
        public string CommiteeLeaderUserId { get; set; }
        public bool Status { get; set; }
    }

}
