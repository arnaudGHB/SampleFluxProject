using CBS.NLoan.Data.Entity.LoanApplicationP;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.PenaltyP
{

    public class Penalty : BaseEntity
    {
        public string Id { get; set; }
        public string LoanProductId { get; set; }
        public string PenaltyType { get; set; }//Late_Repayment_Penalty Or Penalty_After_Maturity_Date
        public string? Description { get; set; }
        public string PenaltyName { get; set; }
        public decimal PenaltyValue { get; set; }
        public bool IsRate { get; set; }
        public bool IsEnabled { get; set; }
        public string CalculatePenaltyOn { get; set; }//Overdue_Principal_Amount,Overdue_Interest_Amount,Overdue_Principal_Plus_Interest_Plus_Amount,
        public bool WaivePenaltyOnBranchHolidays { get; set; }
        public int GracePeriodInDaysBeforeApplyingPenalty { get; set; }
        public string? RecuringInterval { get; set; }// Every 1 to 365 Days
        public string? RecurringPeriod { get; set; }//Days, Months, Years, Weeks
        public int DaysToApplyPenalty { get; set; }
        public virtual LoanProduct LoanProduct { get; set; }
    }
}
