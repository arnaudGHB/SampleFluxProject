using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCollateralMediaR.Commands;
using CBS.NLoan.MediatR.LoanGuarantorMediaR.Commands;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>


    public class AddLoanApplicationCommand : IRequest<ServiceResponse<LoanApplicationDto>>
    {
        public string? LoanId { get; set; }

        public bool RequiredDownPaymentCoverageRate { get; set; }

        [Required(ErrorMessage = "Loan target is required.")]
        [StringLength(50, ErrorMessage = "Loan target must not exceed 50 characters.")]
        public string LoanTarget { get; set; } // Employee, Government, Group, Company, Individual etc

        [Required(ErrorMessage = "Loan category is required.")]
        [StringLength(50, ErrorMessage = "Loan category must not exceed 50 characters.")]
        public string LoanCategory { get; set; } // Main_Loan OR Special_Saving_Facilities

        [Required(ErrorMessage = "Fee IDs are required.")]
        [MinLength(1, ErrorMessage = "At least one Fee ID must be provided.")]
        public List<string> FeeIds { get; set; }

        [Required(ErrorMessage = "Loan product ID is required.")]
        public string LoanProductId { get; set; }

        [Required(ErrorMessage = "Loan amount is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Loan amount must be greater than zero.")]
        public decimal Amount { get; set; }
        public decimal InterestAmountUpfront { get; set; }

        public bool IsPaidAllFeeUpFront { get; set; }
        public bool IsPaidFeeBeforeProcessing { get; set; }
        public bool IsPaidFeeAfterProcessing { get; set; }
        public bool IsThereGuarantor { get; set; }
        public bool IsThereCollateral { get; set; }

        [Required(ErrorMessage = "Interest rate is required.")]
        [Range(0, 100, ErrorMessage = "Interest rate must be between 0 and 100%.")]
        public decimal InterestRate { get; set; }

        public bool IsOverRightOldLoanInterestAndBalance { get; set; }
        public bool StopInterestCalculation { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "New interest must be a positive value.")]
        public decimal NewInterest { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "New balance must be a positive value.")]
        public decimal NewBalance { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "New VAT must be a positive value.")]
        public decimal NewVAT { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "New penalty must be a positive value.")]
        public decimal NewPenalty { get; set; }

        public bool IsInterestPaidUpFront { get; set; } = true;

        [Required(ErrorMessage = "Repayment circle is required.")]
        public string RepaymentCircle { get; set; }

        [Required(ErrorMessage = "Loan type is required.")]
        public string LoanType { get; set; }

        [Required(ErrorMessage = "Loan duration is required.")]
        [Range(1, 360, ErrorMessage = "Loan duration must be between 1 and 360 months.")]
        public int LoanDuration { get; set; }

        [Required(ErrorMessage = "First installment date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime FirstInstallmentDate { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        public string CustomerId { get; set; }

        [Required(ErrorMessage = "Economic activity ID is required.")]
        public string EconomicActivityId { get; set; }

        [Required(ErrorMessage = "Amortization type is required.")]
        public string AmortizationType { get; set; }

        [Range(0, 12, ErrorMessage = "Grace period before first payment must be between 0 and 12 months.")]
        public int GracePeriodBeforeFirstPayment { get; set; }

        public int GracePeriodAfterMaturityDate { get; set; }

        [Required(ErrorMessage = "Loan application type is required.")]
        public string LoanApplicationType { get; set; }

        [Range(0, 100, ErrorMessage = "Collateral coverage rate must be between 0% and 100%.")]
        public decimal CollateralCoverageRate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Share account coverage amount must be a positive value.")]
        public decimal ShareAccountCoverageAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Preference share account coverage amount must be a positive value.")]
        public decimal PreferenceShareAccountCoverageAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Deposit account coverage amount must be a positive value.")]
        public decimal DepositAccountCoverageAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary account coverage amount must be a positive value.")]
        public decimal SalaryAccountCoverageAmount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Term deposit account coverage amount must be a positive value.")]
        public decimal TermDeposiAccountCoverageAmount { get; set; }

        public bool IsPreferenceShareAccountCoverageAmount { get; set; }
        public bool IsDepositAccountCoverageAmount { get; set; }
        public bool IsTermDeposiAccountCoverageAmount { get; set; }

        [Range(0, 100, ErrorMessage = "Saving account coverage rate must be between 0% and 100%.")]
        public decimal SavingAccountCoverageRate { get; set; }

        [Range(0, 100, ErrorMessage = "Salary account coverage rate must be between 0% and 100%.")]
        public decimal SalaryAccountCoverageRate { get; set; }

        [Range(0, 100, ErrorMessage = "Guarantor saving account coverage rate must be between 0% and 100%.")]
        public decimal GuaratorSavingAccountCoverageRate { get; set; }

        [Required(ErrorMessage = "Loan purpose is required.")]
        [RegularExpression("^(?!0$).*", ErrorMessage = "Loan purpose cannot be empty")]
        public string LoanPurposeId { get; set; }


        [Range(0, double.MaxValue, ErrorMessage = "Down payment coverage amount must be a positive value.")]
        public decimal DownPaymentCoverageAmountProvided { get; set; }

        [Required(ErrorMessage = "Branch is required.")]
        public string BranchId { get; set; }

        public bool IsInterestWaiverApplied { get; set; }

        [Range(0, 100, ErrorMessage = "Interest waiver percentage must be between 0% and 100%.")]
        public decimal InterestWaiverPercentage { get; set; }

        public bool ApplyInterestToThisLoan { get; set; }
        public bool ApplyFeeToThisLoan { get; set; }

        [Range(0, 100, ErrorMessage = "Charges percentage must be between 0% and 100%.")]
        public decimal ChargesPercentage { get; set; }

        [Range(0, 365, ErrorMessage = "Number of days to apply charges must be between 0 and 365.")]
        public int NumberOfDaysToApplyCharges { get; set; }
        public OldLoanPayment? OldLoanPayment { get; set; }
    }



}
