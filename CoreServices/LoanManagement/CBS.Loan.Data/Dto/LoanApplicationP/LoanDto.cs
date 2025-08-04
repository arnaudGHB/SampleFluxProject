using CBS.NLoan.Data.Entity.InterestCalculationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.RefundP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Dto.LoanApplicationP
{
    public class LoanDto
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
        public bool IsUpload { get; set; }
        public string? BranchCode { get; set; }
        public string? LoanId { get; set; } = "N/A";
        public string? CustomerName { get; set; }
        public int LoanDuration { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }
        public string? LoanJourneyStatus { get; set; }
        public decimal VatRate { get; set; }
        public string? LoanTarget { get; set; }//Employee, Government, Group, Company, Individual etc
        public string? LoanCategory { get; set; }//Main_Loan OR Special_Saving_Facilities
        public string? AccountNumber { get; set; }
        
        public OldLoanPayment? OldLoanPayment { get; set; }

        public int NumberOfInstallments { get; set; }
        public string RepaymentCycle { get; set; }
        public PaginationMetadata PaginationMetadata { get; set; }
        public virtual ICollection<Refund> Refunds { get; set; }
        public virtual ICollection<LoanAmortization> LoanAmortizations { get; set; }
        public virtual ICollection<DisburstedLoan> DisburstedLoans { get; set; }
        public virtual ICollection<DailyInterestCalculation> DailyInterestCalculations { get; set; }
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


        public bool IsCompleted { get; set; }
       
        public DateTime? MigrationDate { get; set; } = DateTime.MinValue;
      
        public bool InterestMustBePaidUpFront { get; set; }
        public decimal InterestAmountUpfront { get; set; }
       
        public string? LoanDeliquencyConfigurationId { get; set; }
      
        public decimal Savings { get; set; }
        public decimal OShares { get; set; }
        public decimal PShares { get; set; }
        public decimal Deposit { get; set; }
        public decimal Salary { get; set; }
        public decimal Shortee { get; set; }
        public decimal Co_Obligor { get; set; }
        public decimal Co_OperationGurantor { get; set; }
        public decimal OtherGuaranteeFund { get; set; }
        public decimal TotalFundGuranteed { get; set; }
        public decimal PercentageOfLiquidityCoverage { get; set; }
        public decimal PercentageOfCollateralCoverage { get; set; }
        public decimal PercentageOfOverAllCoverage { get; set; }
        public LoanDeliquencyConfiguration LoanDeliquencyConfiguration { get; set; }
        public string? LoanDeliquencyConfigurationName { get; set; }
    }
    public class OldLoanPayment
    {
        public decimal Amount { get; set; }
        public decimal Capital { get; set; }
        public decimal VAT { get; set; }
        public decimal Interest { get; set; }
        public decimal Penalty { get; set; }
        public string LoanId { get; set; }
    }
    public class PaginationMetadata
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int Skip { get; set; }
        public int TotalPages { get; set; }
    }
    public class LoanMainDashboardDto
    {
        public int TotalNumberOfLoans { get; set; }
        public decimal TotalVolumeOfLoanGranted { get; set; }
        public decimal TotalRemainingBalance { get; set; }
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
    }

    public class LightLoanProductDto
    {
        public string Id { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string? TargetType { get; set; }
    }

}
