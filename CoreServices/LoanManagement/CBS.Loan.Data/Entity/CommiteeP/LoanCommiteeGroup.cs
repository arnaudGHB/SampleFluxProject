using CBS.NLoan.Data.Entity.LoanApplicationP;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.NLoan.Data.Entity.CommiteeP
{
    public class LoanCommiteeGroup : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int NumberOfMembers { get; set; }
        public int NumberToApprovalsToValidationALoan { get; set; }
        public decimal MinimumLoanAmount { get; set; }
        public decimal MaximumLoanAmount { get; set; }
        public string CommiteeLeaderUserId { get; set; }
        public bool Status { get; set; }
        public virtual ICollection<LoanCommiteeMember> LoanCommiteeMembers { get; set; }

    }
}
