using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.Commands
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class SimulateLoanInstallementQuery : IRequest<ServiceResponse<List<LoanAmortizationDto>>>
    {
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public int LoanDuration { get; set; } // Represents the duration value (int)
        public string LoanDurationType { get; set; } //Represents the duration type (Months, Days, Weeks, Years)
        public string RepaymentCycle { get; set; }//Daily, Weekly, Biweekly, Monthly, Bimonthly, Quarterly, Every4Months, SemiAnnual, Every9Months, Yearly, LumpSum
        public DateTime RepaymentStartDate { get; set; }
        public string InterestCalculationPeriod { get; set; }//Daily, Weekly, Monthly, Yearly etc
        public decimal Penalty { get; set; }
        public decimal Tax { get; set; }
        public decimal Fee { get; set; }
        public decimal VatRate { get; set; }
        public string? LoanId { get; set; }
        public string? BranchId { get; set; }
        public int NumberOfInstallments { get; set; }
        public string? BankId { get; set; }
        public bool IsSimulation { get; set; }
        public string? LoanApplicationId { get; set; }
        public string AmortizationType { get; set; }

    }
}

