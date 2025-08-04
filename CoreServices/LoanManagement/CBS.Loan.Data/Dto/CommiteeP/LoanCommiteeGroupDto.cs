using CBS.NLoan.Data.Entity.CommiteeP;

namespace CBS.NLoan.Data.Dto.CommiteeP
{
    public class LoanCommiteeGroupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal MinimumLoanAmount { get; set; }
        public decimal MaximumLoanAmount { get; set; }
        public int NumberOfMembers { get; set; }
        public int NumberToApprovalsToValidationALoan { get; set; }
        public string CommiteeLeaderUserId { get; set; }
        public bool Status { get; set; }

        public List<LoanCommiteeMember> LoanCommiteeMembers { get; set; }
    }
}
