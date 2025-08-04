using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Branch.
    /// </summary>
    public class UpdateBranchCommand : IRequest<ServiceResponse<BranchDto>>
    {
        public string Id { get; set; }
        public string BranchCode { get; set; }
        public string Name { get; set; }
        public string? Location { get; set; }
        public string? Telephone { get; set; }

        public bool IsHavingBank { get; set; }

        public string? Email { get; set; }
        public string? Address { get; set; }
        public string BankId { get; set; }
        public string? Capital { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? LogoUrl { get; set; }
        public string? ImmatriculationNumber { get; set; }
        public string? TaxPayerNUmber { get; set; }
        public string? PBox { get; set; }
        public string? WebSite { get; set; }
        public string? DateOfCreation { get; set; }
        public string? BankInitial { get; set; }

        public string? Motto { get; set; }
        public bool ActiveStatus { get; set; }
        public bool IsHeadOffice { get; set; }
        public string? HeadOfficeTelehoneNumber { get; set; }
        public string? HeadOfficeAddress { get; set; }// Foreign key
    }

    //public class Loan : BaseEntity
    //{
    //    public string Id { get; set; }
    //    //loan information
    //    public string LoanCode { get; set; }
    //    public string LoanProduct { get; set; }
    //    public int LoanAmount { get; set; }
    //    public double loanAmount { get; set; }
    //    public double interestRate { get; set; }
    //    public int durationMonths { get; set; }
    //    public string loanType { get; set; }
    //    public int loanDueAmount { get; set; }
    //    public string loanPeriode { get; set; }
    //    public int loanPay { get; set; }
    //    public string loanAgentValidator { get; set; }
    //    public string loanAdministratorValidator { get; set; }
    //    public string monthlyInstallment { get; set; }
    //    public DateTime loanFirstPreferenceDisburse { get; set; }
    //    public DateTime applicationDate { get; set; }
    //    public DateTime approvalDate { get; set; }
    //    public DateTime disbursementDate { get; set; }
    //    public DateTime nextInstallmentDate { get; set; }
    //    public string remainingInstallments { get; set; }
    //    public string loanStatus { get; set; }
    //    public string maturityDate { get; set; }
    //    public string repaymentSchedule { get; set; }
    //    public string status { get; set; }
    //    // ****customer information
    //    public string CustomerName { get; set; }
    //    public string cni { get; set; }
    //    public string Cni { get; set; }
    //    public int scoring { get; set; }
    //    public int salary { get; set; }
    //    public string economicSector { get; set; }
    //    public string borrowerDescription { get; set; }
    //    public string economicActivity { get; set; }
    //    // *** Risk inforamation
    //    public int loanScoreRisk { get; set; }
    //    public bool guaranty { get; set; }
    //    public int guarantyValue { get; set; }
    //    public string guarantyDescription { get; set; }
    //    public bool garantyDocVerified { get; set; }
    //    public bool CollateralAppraisalDone { get; set; }
    //    public string loanPurpose { get; set; }
    //    public bool KYCVerified { get; set; }
    //    public bool CreditCheckPassed { get; set; }


    //}
    //public class LoanProductPackage : BaseEntity
    //{
    //    [Key]
    //    public string Id { get; set; }
    //    public string Code { get; set; }
    //    public string FeeKey { get; set; }
    //    public string Name { get; set; }
    //    public string FundingLineID { get; set; }
    //    public string CurrencyID { get; set; }
    //    public double LoanMinimumAmount { get; set; }
    //    public double LoanMaximumAmount { get; set; }
    //    public string ClientTypeGroupeID { get; set; }
    //    //Interest
    //    public double MinimumInterestRate { get; set; }
    //    public double MaximumInterestRate { get; set; }
    //    public double InterestRate { get; set; }
    //    //Penalties
    //    public double AnticipatedTotalRepaymentPenaltyAmount { get; set; }
    //    public double MinimumAnticipatedTotalRepaymentPenaltyAmount { get; set; }
    //    public double MaximumAnticipatedTotalRepaymentPenaltyAmount { get; set; }
    //    //Installments
    //    public string InstallemtTypeID { get; set; }
    //    public int NumberOfInstallment { get; set; }
    //    public int MinimumNumberOfInstallment { get; set; }
    //    public int MaximumNumberOfInstallment { get; set; }
    //    //GracePeriod
    //    public bool IsChargedAppliedWithinGracePeriod { get; set; }
    //    public double GracePeriodeMinAmount { get; set; }
    //    public double GracePeriodeMaxAmount { get; set; }
    //    public double GracePeriodAmount { get; set; }
    //    public double GracePeriodLateFee { get; set; }
    //    //Gaurantor
    //    public bool UseGuarantor { get; set; }
    //    public bool SetGurantorCollateral { get; set; }
    //    public int MinPercentageGuaranties { get; set; }
    //    public int MinPercentageCollaterals { get; set; }
    //    //guarantees
     
    //}
    //public class Fee : BaseEntity
    //{
    //    [Key]
    //    public string Id { get; set; }
    //    public double EntryFeeStarting { get; set; }
    //    public double EntryFeeAdmin { get; set; }
    //    public int FeeLoanAmountMin { get; set; }
    //    public int FeeLoanAmountMax { get; set; }
    //    public int FeeOLBMin { get; set; }
    //    public int FeeOLBMax { get; set; }
    //    public int FeeOverDuePrincipalMin { get; set; }
    //    public int FeeOverDuePrincipalMax { get; set; }
    //    public int FeeOverDueInterestMax { get; set; }
    //    public int FeeOverDueInterestMin { get; set; }
    //}
    //public class LineOfCredit
    //{
    //    public string Id { get; set; }
    //    public double AmountMin { get; set; }
    //    public double AmountMax { get; set; }
    //    public int NumberInstallmentMin { get; set; }
    //    public int NumberInstallmentMax { get; set; }
    //}

}
