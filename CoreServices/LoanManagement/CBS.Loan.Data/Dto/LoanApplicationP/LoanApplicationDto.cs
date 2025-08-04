using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanPurposeP;
using CBS.NLoan.Data.Entity.Notifications;
using CBS.NLoan.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.NLoan.Data.Dto.LoanApplicationP
{
    public class LoanApplicationDto
    {
        public string Id { get; set; }
        public string LoanProductId { get; set; }
        public string? LoanId { get; set; }
        public bool IsChargesInclussive { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal ProcessingFee { get; set; }
        public int NumberOfRepayment { get; set; }
        public string RepaymentCircle { get; set; }
        public string LoanApplicationType { get; set; }
        public string LoanType { get; set; }
        public int LoanDuration { get; set; }
        public decimal VatRate { get; set; }
        public DateTime FirstInstallmentDate { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime ApprovalDate { get; set; }
        public DateTime DisbursementDate { get; set; }
        public string CustomerId { get; set; }
        public string EconomicActivityId { get; set; }
        public string DisburstmentType { get; set; }
        public string InterestMethod { get; set; }
        public string InterestType { get; set; }
        public int GracePeriod { get; set; }
        public string Status { get; set; }
        public decimal GracePeriodRate { get; set; }
        public decimal InsuranceCoverageRate { get; set; }
        public decimal CollateralCoverageRate { get; set; }
        public decimal ShareAccountCoverageRate { get; set; }
        public decimal SavingAccountCoverageRate { get; set; }
        public bool IsGuaranteeProvided { get; set; }
        public bool IsThereGuarantor { get; set; }
        public string? LoanTarget { get; set; }//Employee, Government, Groups etc
        public string? LoanCategory { get; set; }//Main OR Special Saving Facilities
        public bool IsCollateralProvided { get; set; }
        public string LoanPurposeId { get; set; }
        public decimal TotalLoanRiskCoverage { get; set; }
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public DateTime DateOfPayment { get; set; }
        public bool IsIninitalProcessingFeePaid { get; set; }
        public bool IsPaidAllFeeUpFront { get; set; }
        public bool IsPaidFeeBeforeProcessing { get; set; }
        public bool IsPaidFeeAfterProcessing { get; set; }
        public string BankId { get; set; }
        public string ApprovalStatus { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDisbursed { get; set; }
        public string ApprovalComment { get; set; }
        public decimal OldLoanAmount { get; set; }
        public decimal OldLoanCapital { get; set; }
        public decimal OldLoanInterest { get; set; }
        public decimal OldLoanVat { get; set; }
        public decimal OldLoanPenalty { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal RestructuredBalance { get; set; }
        public bool IsOverRightOldLoanInterestAndBalance { get; set; }
        public string CustomerName { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }

        public bool RequiredDownPaymentCoverageRate { get; set; }
        public decimal ShareAccountCoverageAmount { get; set; }

        public decimal PreferenceShareAccountCoverageAmount { get; set; }
        public decimal DepositAccountCoverageAmount { get; set; }
        public decimal SalaryAccountCoverageAmount { get; set; }
        public decimal TermDeposiAccountCoverageAmount { get; set; }
        public bool IsPreferenceShareAccountCoverageAmount { get; set; }
        public bool IsDepositAccountCoverageAmount { get; set; }
        public bool IsTermDeposiAccountCoverageAmount { get; set; }
        public decimal SalaryAccountCoverageRate { get; set; }
        public decimal GuaratorSavingAccountCoverageRate { get; set; }

        public decimal DownPaymentCoverageAmountProvided { get; set; }
        public string LoanManager { get; set; }
        public bool ApplyInterestToThisLoan { get; set; }
        public bool ApplyFeeToThisLoan { get; set; }
        public bool IsInterestWaiverApplied { get; set; }
        public decimal InterestWaiverPercentage { get; set; }
        public bool IsChargesApplied { get; set; }
        public decimal ChargesPercentage { get; set; }
        public int NumberOfDaysToApplyCharges { get; set; }

        public bool? IsUpload { get; set; }

        public bool IsInterestPaidUpFront { get; set; } = true;

        public bool IsInterestRunning { get; set; } = true;
        public bool StopInterestCalculation { get; set; }
        public DateTime? DateInterestCalaculationWasStoped { get; set; }
        public string? StopedBy { get; set; }

        public bool IsDisbursted { get; set; }
        public string AmortizationType { get; set; }
        public int GracePeriodBeforeFirstPayment { get; set; }
        public int GracePeriodAfterMaturityDate { get; set; }





      
        public virtual ICollection<LoanApplicationFee> LoanApplicationFees { get; set; }
        public virtual LoanProduct LoanProduct { get; set; }
        public virtual ICollection<LoanApplicationCollateral> Collateras { get; set; }
        public virtual ICollection<LoanGuarantor> Guarantors { get; set; }
        public virtual ICollection<LoanCommiteeValidationHistory> LoanCommiteeValidations { get; set; }
        public virtual ICollection<Loan> Loans { get; set; }

        public virtual ICollection<DocumentAttachedToLoan> DocumentAttachedToLoans { get; set; }
        public virtual LoanPurpose LoanPurpose { get; set; }
        public virtual ICollection<OTPNotification> OTPNotifications { get; set; }


    }
}
//jessicatiako@yahoo.com