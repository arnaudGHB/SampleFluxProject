using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.CommiteeP
{
    public class LoanCommiteeMember : BaseEntity
    {

        public string Id { get; set; }
        public string LoanCommiteeGroupId { get; set; }
        public string UserId { get; set; }
        public virtual LoanCommiteeGroup LoanCommiteeGroup { get; set; }
        public virtual ICollection<LoanCommiteeValidationHistory> LoanCommiteeValidationHistories { get; set; }
    }
}
