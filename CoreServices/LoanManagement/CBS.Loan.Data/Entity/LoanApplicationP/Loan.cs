using CBS.NLoan.Data.Entity.InterestCalculationP;
using CBS.NLoan.Data.Entity.RefundP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Helper.Helper;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class Loan : BaseEntity
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
        public decimal DeliquentInterest { get; set; }// This is where you need to do the change
        public decimal DueAmount { get; set; }
        public decimal AccrualInterest { get; set; }
        public decimal LastCalculatedInterest { get; set; }
        public decimal AccrualInterestPaid { get; set; }
        public decimal TotalPrincipalPaid { get; set; }
        public int AdvancedPaymentDays { get; set; }
        public int DeliquentDays { get; set; }
        public decimal AdvancedPaymentAmount { get; set; }
        public decimal DeliquentAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal TaxPaid { get; set; }
        public decimal FeePaid { get; set; }
        public decimal Fee { get; set; }
        public decimal Penalty { get; set; }
        public string? LoanStructuringStatus { get; set; }
        public DateTime LoanStructuringDate { get; set; }
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
        public string NewLoanId { get; set; }
        public bool IsWriteOffLoan { get; set; }
        public bool IsDeliquentLoan { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCurrentLoan { get; set; }
        public DateTime MaturityDate { get; set; }
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public decimal OldCapital { get; set; }
        public decimal OldInterest { get; set; }
        public decimal OldVAT { get; set; }
        public decimal OldPenalty { get; set; }
        public decimal OldBalance { get; set; }
        public decimal OldDueAmount { get; set; }
        public string? LoanType { get; set; }
        public DateTime? MigrationDate { get; set; } = DateTime.MinValue;
        public string? BranchCode { get; set; }
        public string? LoanId { get; set; } = "N/A";
        public bool InterestMustBePaidUpFront { get; set; }
        public decimal InterestAmountUpfront { get; set; }
        public string? CustomerName { get; set; }
        public int LoanDuration { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }
        public string? LoanJourneyStatus { get; set; }
        public string? DeliquentStatus { get; set; }
        public string? LoanDeliquencyConfigurationId{ get; set; }
        public decimal VatRate { get; set; }
        public string? LoanTarget { get; set; }//Employee, Government, Group, Company, Individual etc
        public string? LoanCategory { get; set; }//Main_Loan OR Special_Saving_Facilities
        public string? AccountNumber { get; set; }
        public bool IsUpload { get; set; }
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
        public bool StopInterestCalculation { get; set; } = false;
        public string? StoppedBy { get; set; } = "Normal";
        public DateTime? DateInterestWastStoped { get; set; } = DateTime.MinValue;
        public DateTime? LastDeliquecyProcessedDate { get; set; } // Nullable to support unprocessed loans
        public virtual ICollection<Refund> Refunds { get; set; }
        public virtual ICollection<LoanAmortization> LoanAmortizations { get; set; }
        public virtual ICollection<DisburstedLoan> DisburstedLoans { get; set; }
        public virtual ICollection<DailyInterestCalculation> DailyInterestCalculations { get; set; }


        public Loan() { }

        public Loan(string loanApplicationId, decimal principal, decimal loanAmount, decimal interestForcasted, decimal interestRate,
            decimal paid, decimal balance, decimal dueAmount,
            DateTime disbursementDate, DateTime firstInstallmentDate, DateTime nextInstallmentDate, DateTime loanDate,
            string customerId, DateTime maturityDate, string organizationId, string branchId, string bankId, string disbursmentStatus,
            string loanStatus, bool isCurrentLoan, DateTime lastRefundDate, string memberName, string branchcode, int loanDuration,
            string loanType, decimal accrualInterest, decimal accrualInterestPaid, decimal totalPrincipalPaid, string accountNumber,
            decimal deliquentInterest, int advancedPaymentDays, int deliquentDays, decimal advancedPaymentAmount, decimal deliquentAmount,string delinquentStatus )
        {
            LoanApplicationId = loanApplicationId;
            Principal = principal;
            LoanAmount = loanAmount;
            InterestForcasted = interestForcasted;
            InterestRate = interestRate;
            LastPayment = 0; // Initialise à 0
            Paid = paid;
            Balance = balance;
            DueAmount = dueAmount;
            AccrualInterest = accrualInterest; // Initialise à 0
            LastCalculatedInterest = 0; // Initialise à 0
            AccrualInterestPaid = accrualInterestPaid; // Initialise à 0
            TotalPrincipalPaid = totalPrincipalPaid; // Initialise à 0
            Tax = 0; // Initialise à 0
            TaxPaid = 0; // Initialise à 0
            FeePaid = 0; // Initialise à 0
            Fee = 0; // Initialise à 0
            Penalty = 0; // Initialise à 0
            PenaltyPaid = 0; // Initialise à 0
            DisbursementDate = disbursementDate;
            FirstInstallmentDate = firstInstallmentDate;
            NextInstallmentDate = nextInstallmentDate;
            LoanDate = loanDate;
            IsLoanDisbursted = true; // Initialise à false
            DisbursmentStatus = disbursmentStatus; // Initialise à null
            LastInterestCalculatedDate = DateTime.Now.AddDays(-1); // Initialise à la date et heure actuelles
            LastRefundDate = lastRefundDate; // Initialise à la date et heure actuelles
            LastEventData = DateTime.Now; // Initialise à la date et heure actuelles
            CustomerId = customerId;
            LoanManager = "N/A"; // Initialise à une chaîne vide
            LoanStatus = loanStatus;
            NewLoanId = "N/A"; // Initialise à une chaîne vide
            IsWriteOffLoan = false; // Initialise à false
            IsDeliquentLoan = false; // Initialise à false
            IsCurrentLoan = isCurrentLoan; // Initialise à true
            MaturityDate = maturityDate;
            OrganizationId = "1";
            BranchId = branchId;
            BankId = bankId;
            CustomerName = memberName;
            LoanType = loanType;
            BranchCode = branchcode;
            LoanDuration = loanDuration;
            LoanJourneyStatus = "Normal";
            LoanTarget = "Individual";
            LoanCategory = "Main_Loan";
            VatRate = loanAmount >= 2000000 ? 19.25m : 0.0m;
            IsUpload = true;
            AccountNumber = accountNumber;
            DeliquentInterest = deliquentInterest;
            AdvancedPaymentDays = advancedPaymentDays;
            DeliquentDays = deliquentDays;
            AdvancedPaymentAmount = advancedPaymentAmount;
            DeliquentAmount = deliquentAmount;
            DeliquentStatus = delinquentStatus;
            IsCurrentLoan = delinquentStatus == LoanDeliquentStatus.Current.ToString();
            IsDeliquentLoan= delinquentStatus== LoanDeliquentStatus.Delinquent.ToString();
            LastDeliquecyProcessedDate = BaseUtilities.UtcNowToDoualaTime();

        }


    }
    public class LoanDtoExport
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
        public decimal DeliquentInterest { get; set; }
        public decimal DueAmount { get; set; }
        public decimal AccrualInterest { get; set; }
        public decimal LastCalculatedInterest { get; set; }
        public decimal AccrualInterestPaid { get; set; }
        public decimal TotalPrincipalPaid { get; set; }
        public decimal Tax { get; set; }
        public decimal TaxPaid { get; set; }
        public decimal FeePaid { get; set; }
        public string ProductName { get; set; }
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
        public virtual LoanApplication LoanApplication { get; set; }
        public string? LoanJourneyStatus { get; set; }
        public decimal VatRate { get; set; }
        public string? LoanTarget { get; set; }//Employee, Government, Group, Company, Individual etc
        public string? LoanCategory { get; set; }//Main_Loan OR Special_Saving_Facilities
        public string? AccountNumber { get; set; }
        public bool IsUpload { get; set; }
        public decimal Saving { get; set; }
        public decimal Deposit { get; set; }
        public decimal PShare { get; set; }
        public decimal OShare { get; set; }
        public decimal Salary { get; set; }


        public string DeliquentStatus { get; set; }
        public int AdvancedPaymentDays { get; set; }
        public int DeliquentDays { get; set; }
        public decimal AdvancedPaymentAmount { get; set; }
        public decimal DeliquentAmount { get; set; }
        public bool StopInterestCalculation { get; set; } = false;
        public string? StoppedBy { get; set; } = "Normal";
        public DateTime? DateInterestWastStoped { get; set; } = DateTime.MinValue;
        public DateTime? LastDeliquecyProcessedDate { get; set; } // Nullable to support unprocessed loans

    }
    public class BackgroundServiceState
    {
        public bool IsRunning { get; set; }
        public DateTime LastSuccessfulRun { get; set; }
        public string LastErrorMessage { get; set; }
    }
}