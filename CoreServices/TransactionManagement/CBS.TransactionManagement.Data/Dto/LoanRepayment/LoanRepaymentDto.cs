using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.LoanRepayment
{


    public class RefundDto
    {
        public string Id { get; set; }
        public string LoanId { get; set; }
        public string CustomerId { get; set; }
        public string Comment { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentChannel { get; set; }
        public decimal Amount { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Penalty { get; set; }
        public decimal Tax { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public decimal Balance { get; set; }
        public DateTime DateOfPayment { get; set; }
        public LoanProduct LoanProduct { get; set; }
        public Loan Loan { get; set; }
        public bool IsComplete { get; set; }
    }
    public class Loan
    {
        public string Id { get; set; }
        public string LoanApplicationId { get; set; }
        public decimal Principal { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestForcasted { get; set; }
        public decimal InterestRate { get; set; }
        public decimal LastPayment { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
        public decimal DueAmount { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal RestructuredBalance { get; set; }
        public decimal AccrualInterest { get; set; }
        public decimal LastCalculatedInterest { get; set; }
        public decimal AccrualInterestPaid { get; set; }
        public decimal TotalPrincipalPaid { get; set; }
        public decimal Tax { get; set; }
        public decimal TaxPaid { get; set; }
        public decimal FeePaid { get; set; }
        public decimal Fee { get; set; }
        public decimal Penalty { get; set; }
        public decimal PenaltyPaid { get; set; }
        public DateTime DisbursementDate { get; set; }
        public DateTime FirstInstallmentDate { get; set; }
        public DateTime NextInstallmentDate { get; set; }
        public DateTime LoanDate { get; set; }
        public bool IsLoanDisbursted { get; set; }
        public string? DisbursmentStatus { get; set; }
        public DateTime LastInterestCalculatedDate { get; set; }
        public DateTime LastRefundDate { get; set; }
        public DateTime LastEventData { get; set; }
        public string CustomerId { get; set; }
        public string? LoanManager { get; set; }
        public string LoanStatus { get; set; }
        public bool IsRestructured { get; set; }
        public string NewLoanId { get; set; }
        public bool IsWriteOffLoan { get; set; }
        public bool IsDeliquentLoan { get; set; }
        public bool IsCurrentLoan { get; set; }
        public DateTime MaturityDate { get; set; }
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public string? LoanType { get; set; }
        public string? BranchCode { get; set; }
        public string? LoanId { get; set; } = "N/A";
        public string? CustomerName { get; set; }
        public int LoanDuration { get; set; }
        public LoanApplication LoanApplication { get; set; }
        public string? LoanJourneyStatus { get; set; }
        public decimal VatRate { get; set; }
        public string? LoanTarget { get; set; }//Employee, Government, Group, Company, Individual etc
        public string? LoanCategory { get; set; }//Main_Loan OR Special_Saving_Facilities
        public string? AccountNumber { get; set; }
        public bool IsUpload { get; set; }

        public int NumberOfInstallments { get; set; }
        public string RepaymentCycle { get; set; }
        public decimal DeliquentInterest { get; set; }
        public int AdvancedPaymentDays { get; set; }
        public int DeliquentDays { get; set; }
        public decimal AdvancedPaymentAmount { get; set; }
        public decimal DeliquentAmount { get; set; }
        public string? LoanStructuringStatus { get; set; }
        public DateTime LoanStructuringDate { get; set; }
        public decimal OldCapital { get; set; }
        public decimal OldInterest { get; set; }
        public decimal OldVAT { get; set; }
        public decimal OldPenalty { get; set; }
        public decimal OldBalance { get; set; }
        public decimal OldDueAmount { get; set; }
        public string? DeliquentStatus { get; set; }
        public bool StopInterestCalculation { get; set; } = false;
        public string? StoppedBy { get; set; } = "Normal";
        public DateTime? DateInterestWastStoped { get; set; } = DateTime.MinValue;
        public DateTime? LastDeliquecyProcessedDate { get; set; } // Nullable to support unprocessed loans

    }

    //public class Loan
    //{
    //    public string Id { get; set; }
    //    public string LoanApplicationId { get; set; }
    //    public decimal Principal { get; set; }
    //    public decimal LoanAmount { get; set; }
    //    public decimal InterestForcasted { get; set; }
    //    public decimal InterestRate { get; set; }
    //    public decimal LastPayment { get; set; }
    //    public decimal Paid { get; set; }
    //    public decimal Balance { get; set; }
    //    public decimal DueAmount { get; set; }
    //    public decimal AccrualInterest { get; set; }
    //    public decimal LastCalculatedInterest { get; set; }
    //    public decimal AccrualInterestPaid { get; set; }
    //    public decimal TotalPrincipalPaid { get; set; }
    //    public decimal Tax { get; set; }
    //    public decimal TaxPaid { get; set; }
    //    public decimal FeePaid { get; set; }
    //    public decimal Fee { get; set; }
    //    public decimal Penalty { get; set; }
    //    public decimal PenaltyPaid { get; set; }
    //    public DateTime DisbursementDate { get; set; }
    //    public DateTime FirstInstallmentDate { get; set; }
    //    public DateTime NextInstallmentDate { get; set; }
    //    public DateTime LoanDate { get; set; }
    //    public bool IsLoanDisbursted { get; set; }
    //    public string? DisbursmentStatus { get; set; }
    //    public DateTime LastInterestCalculatedDate { get; set; }
    //    public DateTime LastRefundDate { get; set; }
    //    public DateTime LastEventData { get; set; }
    //    public string CustomerId { get; set; }
    //    public string LoanManager { get; set; }
    //    public string LoanStatus { get; set; }
    //    public bool IsRestructured { get; set; }
    //    public string NewLoanId { get; set; }
    //    public bool IsWriteOffLoan { get; set; }
    //    public bool IsDeliquentLoan { get; set; }
    //    public bool IsUpload { get; set; }
    //    public bool IsCurrentLoan { get; set; }
    //    public string? LoanTarget { get; set; }//Employee, Government, Group, Company, Individual etc
    //    public string? LoanCategory { get; set; }//Main_Loan OR Special_Saving_Facilities
    //    public string? AccountNumber { get; set; }

    //    public DateTime MaturityDate { get; set; }
    //    public string OrganizationId { get; set; }
    //    public string BranchId { get; set; }
    //    public string BankId { get; set; }
    //    public virtual LoanApplication LoanApplication { get; set; }

    //}

    public class LoanProduct
    {
        public string Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string? LoanInterestPeriod { get; set; }//Per Day, Per Week, Per Month, Per Year
        public decimal MinimumInterestRate { get; set; }
        public decimal MaximumInterestRate { get; set; }
        public string? LoanDurationPeriod { get; set; }//Days, Weeks, Months, Years
        public int MinimumDurationPeriod { get; set; }
        public int MaximumDurationPeriod { get; set; }
        public bool RequiresGuarantor { get; set; }
        public bool IsInterestWaiverApplied { get; set; }
        public decimal MinimumInterestWaiver { get; set; }
        public decimal MaximumInterestWaiver { get; set; }
        public bool IsChargesApplied { get; set; }
        public decimal MinimumChargesToAppliedInPercentage { get; set; }
        public decimal MaximumChargesToAppliedPercentage { get; set; }
        public decimal DefaultChargeToAppliedPercentage { get; set; }
        public decimal MinimumChargesStartDayAfterLoanDueDate { get; set; }
        public decimal MaximumChargesStartDayAfterLoanDueDate { get; set; }
        public decimal MinimumDownPaymentPercentage { get; set; }
        public decimal DefaulChargesStartDayAfterLoanDueDate { get; set; } = 60;
        public string ChargesAreAppliedToInterestOrBalance { get; set; } = "Interest";
        public int ChargesStopAfterHowManyDaysFromStart { get; set; } = 30;
        public bool StartGeneratingInterestAfterDisbustment { get; set; }
        public int MinimumNumberOfRepayment { get; set; }
        public string Description { get; set; }
        public decimal LoanMinimumAmount { get; set; }
        public decimal LoanMaximumAmount { get; set; }
        public string? TargetType { get; set; }
        public decimal MinimumCollateralPercentage { get; set; }
        public bool IsRequiredShareAccount { get; set; }
        public bool IsRequiredSalaryccount { get; set; }
        public bool IsRequiredSavingAccount { get; set; }
        public bool IsRequresRegisteredPublicAuthority { get; set; }
        public bool IsRequredIrrivocableSalaryTransfer { get; set; }
        public bool IsRequiredCollateral { get; set; }
        public bool BlockedSavingAccount { get; set; }
        public bool BlockedGuarantorAccount { get; set; }
        public bool BlockedSalaryAccount { get; set; }
        public decimal MinimumSavingAccountBalanceRateForTheRequestAmount { get; set; }
        public decimal MinimumSalaryAccountBalanceRateForTheRequestAmount { get; set; }
        public decimal MinimumShareAccountBalanceForTheRequestAmount { get; set; }
        public bool ActiveStatus { get; set; }
        public bool HasTopUp { get; set; }
        public string? AccountNumber { get; set; }
        public string? ChartOfAccountManagementPositionId { get; set; }
        public string? ChartOfAccountIdForPrincipalAmount { get; set; }
        public string? ChartOfAccountIdForAccrualInterest { get; set; }
        public string? ChartOfAccountIdForPenalty { get; set; }
        public string? ChartOfAccountIdForFee { get; set; }
        public string? ChartOfAccountIdForTax { get; set; }
        public string? ChartOfAccountIdForLoanTransition { get; set; }
        public string? ChartOfAccountIdForWriteOffPrincipal { get; set; }
        public string? ChartOfAccountIdForProvisionOnPrincipal { get; set; }
    }
    public class LoanApplication
    {
        public string Id { get; set; }
        public string LoanProductId { get; set; }
        public decimal Amount { get; set; }
        public bool IsThereGuarantor { get; set; }
        public bool IsThereCollateral { get; set; }
        public decimal InterestRate { get; set; }
        public decimal OtherFee { get; set; }
        public decimal ProcessingFee { get; set; }
        public bool IsFeePaidUpFront { get; set; } = true;
        public bool IsInterestRunning { get; set; } = true;
        public decimal VatRate { get; set; }
        public int NumberOfRepayment { get; set; }
        public string RepaymentCircle { get; set; }
        public string LoanType { get; set; }
        public int LoanDuration { get; set; }
        public DateTime FirstInstallmentDate { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime ApprovalDate { get; set; }
        public DateTime DisbursementDate { get; set; }
        public string CustomerId { get; set; }
        public string EconomicActivityId { get; set; }
        public string DisburstmentType { get; set; }
        public bool IsDisbursted { get; set; }
        public string AmortizationType { get; set; }
        public int GracePeriodBeforeFirstPayment { get; set; }
        public int GracePeriodAfterMaturityDate { get; set; }
        public string Status { get; set; }
        public decimal CollateralCoverageRate { get; set; }
        public decimal ShareAccountCoverageRate { get; set; }
        public decimal SavingAccountCoverageRate { get; set; }
        public decimal SalaryAccountCoverageRate { get; set; }
        public decimal GuaratorSavingAccountCoverageRate { get; set; }
        public string LoanPurposeId { get; set; }
        public decimal TotalLoanRiskCoverage { get; set; }
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public string ApprovalStatus { get; set; }
        public string LoanManager { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDisbursed { get; set; }
        public string ApprovalComment { get; set; }
        public virtual LoanProduct LoanProduct { get; set; }
        public virtual ICollection<LoanGuarantor> Guarantors { get; set; }

    }
    
    public class LoanGuarantor
    {


        public string Id { get; set; }
        public string LoanApplicationId { get; set; }
        public string GuarantorName { get; set; }
        public string? CustomerId { get; set; }
        public string IdCardNumber { get; set; }
        public string ExpireDate { get; set; }
        public string IssueDate { get; set; }
        public string? Relationship { get; set; }
        public string? Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsCoMember { get; set; }
        public string AccountNumber { get; set; }
        public string? Email { get; set; }
        public decimal GuaranteeAmount { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }

    }

    public class LightLoanDto
    {
        public string Id { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal DueAmount { get; set; }
        public decimal Balance { get; set; }
        public string CustomerId { get; set; }
        public string LoanStatus { get; set; }
        public string? LoanType { get; set; }
        public bool IsUpload { get; set; }
        public string LoanCategory { get; set; }
        public decimal AccrualInterest { get; set; }
        public decimal InterestRate { get; set; }
        public decimal Tax { get; set; }
        public decimal Principal { get; set; }
        public decimal TotalPrincipalPaid { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public LightLoanApplicationDto LoanApplication { get; set; }

    }
    public class LightLoanApplicationDto
    {
        public string Id { get; set; }
        public LightLoanProductDto LoanProduct { get; set; }
        public string LoanProductId { get; set; }
    }

    public class LightLoanProductDto
    {
        public string Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string? TargetType { get; set; }
    }
}