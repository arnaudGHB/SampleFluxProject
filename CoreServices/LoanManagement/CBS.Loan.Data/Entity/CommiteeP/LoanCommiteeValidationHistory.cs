using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.NLoan.Data.Entity.CommiteeP
{
    public class LoanCommiteeValidationHistory : BaseEntity
    {

        public string Id { get; set; }
        public string LoanApplicationId { get; set; }
        public string LoanCommiteeMemberId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }
        public virtual LoanCommiteeMember LoanCommiteeMember { get; set; }

    }
}
