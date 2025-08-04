using CBS.NLoan.Data.Entity.CommiteeP;

namespace CBS.NLoan.Data.Dto.CommiteeP
{
    public class LoanCommiteeMemberDto
    {
        public string Id { get; set; }
        public string LoanCommiteeGroupId { get; set; }
        public string UserId { get; set; }
        public LoanCommiteeGroup LoanCommiteeGroup { get; set; }
        public List<LoanCommiteeValidationHistory> LoanCommiteeValidationHistories { get; set; }

    }
}
