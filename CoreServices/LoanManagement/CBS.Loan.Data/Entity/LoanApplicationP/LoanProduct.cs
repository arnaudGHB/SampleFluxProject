using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.LoanTermP;
using CBS.NLoan.Data.Entity.PenaltyP;
using CBS.NLoan.Data.Entity.TaxP;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class LoanProduct : BaseEntity
    {
        public string Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string? LoanInterestPeriod { get; set; }//Per Day, Per Week, Per Month, Per Year
        public decimal MinimumInterestRate { get; set; }
        public decimal MaximumInterestRate { get; set; }
        public string? LoanDurationPeriod { get; set; }//Days, Weeks, Months, Years
        //public int MinimumDurationPeriod { get; set; }
        //public int MaximumDurationPeriod { get; set; }
        public bool IsPaidFeeBeforeProcessing { get; set; }
        public bool StopInterestCalculationAtLoanMaturityDate { get; set; }
        public int NumberOfDaysToStopInterestCalculation { get; set; }
        public decimal MinimumPercentageRefundBeforeRefinancing { get; set; }
        public bool ShorteeMustHaveFundToGuranteeLoan { get; set; }
        public decimal MinimumPercentageCoverageOfShortee { get; set; }
        public bool Co_obligorMustHaveFundToGuranteeLoan { get; set; }
        public bool RequiresGuarantor { get; set; }
        public bool IsInterestWaiverApplied { get; set; }
        public decimal MinimumInterestWaiver { get; set; }
        public decimal MaximumInterestWaiver { get; set; }
        public bool IsProductWithSavingFacilities { get; set; }
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
        public decimal LoanMinimumAmount{ get; set; }
        public decimal LoanMaximumAmount { get; set; }
        public string? TargetType { get; set; }
        public string? LoanTermId { get; set; }
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
        public bool InterestMustBePaidUpFront { get; set; }

        public string? ChartOfAccountManagementPositionId { get; set; }
        public string? ChartOfAccountIdForPrincipalAmount { get; set; }
        public string? ChartOfAccountIdForInterestReceived { get; set; }
        public string? ChartOfAccountIdForProvisionMoreThanOneYear { get; set; }
        public string? ChartOfAccountIdForProvisionMoreThanTwoYear { get; set; }
        public string? ChartOfAccountIdForProvisionMoreThanThreeYear { get; set; }
        public string? ChartOfAccountIdForProvisionMoreThanFourYear { get; set; }
        public string? ChartOfAccountIdForPenalty { get; set; }
        public string? ChartOfAccountIdForTax { get; set; }
        public string? ChartOfAccountIdForLoanTransition { get; set; }
        public string? ChartOfAccountIdForWriteOffPrincipal { get; set; }
        public string? LoanProductCategoryId { get; set; }
        public virtual LoanProductCategory LoanProductCategory { get; set; }
        public virtual ICollection<Penalty> Penalties { get; set; }
        public virtual ICollection<LoanProductRepaymentCycle> LoanProductRepaymentCycles { get; set; }//Daily, Weekly, Biweekly, Monthly, Bimonthly, Quarterly, Every 4 Months, Semi-Annual, Every 9 Months, Yearly, Lump-Sum
       // public virtual ICollection<LoanProductRepaymentOrder> LoanProductRepaymentOrders { get; set; }
        public virtual ICollection<LoanApplication> LoanApplications { get; set; }
        public virtual ICollection<LoanProductCollateral> LoanProductCollaterals { get; set; }
        public virtual ICollection<LoanProductMaturityPeriodExtension> LoanProductMaturityPeriodExtensions { get; set; }//Daily, Weekly, Biweekly, Monthly, Bimonthly, Quarterly, Every 4 Months, Semi-Annual, Every 9 Months, Yearly, Lump-Sum> LoanProductCollaterals { get; set; }
        public virtual LoanTerm LoanTerm { get; set; }
    }
  
    

}
