using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;

namespace CBS.NLoan.Data.Dto.CommiteeP
{
    public class LoanCommiteeValidationHistoryDto
    {
        public string Id { get; set; }
        public string LoanApplicationId { get; set; }
        public string LoanCommiteeMemberId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public LoanApplication LoanApplication { get; set; }
        public LoanCommiteeMember LoanCommiteeMember { get; set; }


    }
}
