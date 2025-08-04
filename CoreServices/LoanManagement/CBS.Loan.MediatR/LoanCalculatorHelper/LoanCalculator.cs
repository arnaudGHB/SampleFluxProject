using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.RefundP;
using CBS.NLoan.Helper.Helper;

namespace CBS.NLoan.MediatR.LoanCalculatorHelper
{

    public class LoanCalculator
    {
        public static List<LoanAmortization> GenerateAmortizationSchedule(LoanParameters loanParameter)
        {
            List<LoanAmortization> amortizationSchedule = new List<LoanAmortization>();

            decimal amount = loanParameter.Amount;
            decimal interestRate = loanParameter.InterestRate;
            int loanDuration = loanParameter.LoanDuration;
            string loanDurationType = loanParameter.LoanDurationType;
            string repaymentCycle = loanParameter.RepaymentCycle;
            DateTime repaymentStartDate = loanParameter.RepaymentStartDate;
            string interestCalculationPeriod = loanParameter.InterestCalculationPeriod;
            string amortizationType = loanParameter.AmortizationType;

            decimal monthlyInterestRate = interestRate / 100;
            int numberOfInstallments = CalculateTotalPayments(loanDuration, repaymentCycle);
            int periodsPassed = 0;
            decimal remainingBalance = amount;
            decimal vatRate = loanParameter.VatRate / 100; // VAT rate on 19.25%

            DateTime currentPaymentDate = repaymentStartDate;

            while (remainingBalance > 0 && periodsPassed < numberOfInstallments)
            {
                decimal interestPayment = remainingBalance * monthlyInterestRate;
                decimal principalPayment = 0;

                if (amortizationType == "Constant_Amortization")
                {
                    principalPayment = amount / numberOfInstallments;
                }
                else if (amortizationType == "Constant_Annuity")
                {
                    decimal monthlyPayment = CalculateMonthlyPayment(amount, monthlyInterestRate, numberOfInstallments);
                    principalPayment = monthlyPayment - interestPayment;
                }

                if (remainingBalance < principalPayment)
                {
                    principalPayment = remainingBalance;
                }

                decimal vat = interestPayment * vatRate;
                // Calculate VAT on 19.25% of the interest

                var amortization = new LoanAmortization
                {
                    Sno = periodsPassed + 1,
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    Amount = amount,
                    BegginingBalance = periodsPassed == 0 ? amount : remainingBalance, // Adjust beginning balance calculation
                    Principal = principalPayment,
                    Interest = interestPayment,
                    PreviouseInterest = 0,
                    Tax = vat,
                    TotalDue = principalPayment + interestPayment + vat,
                    Balance = remainingBalance - principalPayment, // Calculate end balance
                    Fee = loanParameter.Fee,
                    Annuity = principalPayment + interestPayment + vat,
                    Penalty = loanParameter.Penalty,
                    Paid = 0,
                    Due = principalPayment + interestPayment + vat,
                    DateOfPayment = currentPaymentDate,
                    Status = "Pending",
                    LoanId = loanParameter.LoanId == null ? "N/A" : loanParameter.LoanId.ToString(),
                    MethodOfPayment = "Unknown",
                    Description = $"Installment {periodsPassed + 1}",
                    IsStructured = false,
                    LoanStructuringStatus = "N/A",
                    IsCompleted = false,
                    PrincipalPaid = 0,
                    InterestPaid = 0,
                    PenaltyPaid = 0,
                    TaxPaid = 0,
                    PreviousInstallmentDue = false,
                    BranchId = loanParameter.BranchId,
                    BankId = loanParameter.BankId,
                    NextPaymentDate = IncrementPaymentDate(currentPaymentDate, repaymentCycle)
                };

                amortizationSchedule.Add(amortization);

                remainingBalance -= principalPayment;
                periodsPassed++;
                currentPaymentDate = IncrementPaymentDate(currentPaymentDate, repaymentCycle);
            }

            return amortizationSchedule;
        }
        public static decimal CalculateInterestUpfront(decimal loanAmount, decimal annualInterestRate, int loanDurationInMonths)
        {
            // Formula: Simple Interest Calculation
            decimal interestAmount = loanAmount * (annualInterestRate / 100) * (loanDurationInMonths / 12.0m);
            return interestAmount;
        }
        public static List<LoanAmortization> GenerateAmortizationScheduleLoan(LoanParameters loanParameter)
        {
            List<LoanAmortization> amortizationSchedule = new List<LoanAmortization>();

            decimal amount = loanParameter.Amount;
            decimal interestRate = loanParameter.InterestRate;
            int loanDuration = loanParameter.LoanDuration;
            string loanDurationType = loanParameter.LoanDurationType;
            string repaymentCycle = loanParameter.RepaymentCycle;
            DateTime repaymentStartDate = loanParameter.RepaymentStartDate;
            string interestCalculationPeriod = loanParameter.InterestCalculationPeriod;
            string amortizationType = loanParameter.AmortizationType;

            decimal monthlyInterestRate = interestRate / 100;
            int numberOfInstallments = CalculateTotalPayments(loanDuration, repaymentCycle);
            int periodsPassed = 0;
            decimal remainingBalance = amount;
            decimal vatRate = loanParameter.VatRate / 100; // VAT rate on 19.25%

            DateTime currentPaymentDate = repaymentStartDate;

            while (remainingBalance > 0 && periodsPassed < numberOfInstallments)
            {
                decimal interestPayment = 0;
                decimal principalPayment = 0;

                if (amortizationType == "Constant_Amortization")
                {
                    principalPayment = amount / numberOfInstallments;
                }
                else if (amortizationType == "Constant_Annuity")
                {
                    decimal monthlyPayment = CalculateMonthlyPayment(amount, monthlyInterestRate, numberOfInstallments);
                    principalPayment = monthlyPayment - interestPayment;
                }

                if (remainingBalance < principalPayment)
                {
                    principalPayment = remainingBalance;
                }

                decimal vat = interestPayment * vatRate;
                // Calculate VAT on 19.25% of the interest

                var amortization = new LoanAmortization
                {
                    Sno = periodsPassed + 1,
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    Amount = amount,
                    BegginingBalance = periodsPassed == 0 ? amount : remainingBalance, // Adjust beginning balance calculation
                    Principal = principalPayment,
                    Interest = interestPayment,
                    PreviouseInterest = 0,
                    Tax = vat,
                    TotalDue = principalPayment + interestPayment + vat,
                    Balance = remainingBalance - principalPayment, // Calculate end balance
                    Fee = loanParameter.Fee,
                    Annuity = principalPayment + interestPayment + vat,
                    Penalty = loanParameter.Penalty,
                    Paid = 0,
                    Due = principalPayment + interestPayment + vat,
                    DateOfPayment = currentPaymentDate,
                    Status = "Pending",
                    LoanId = loanParameter.LoanId == null ? "N/A" : loanParameter.LoanId.ToString(),
                    MethodOfPayment = "Unknown",
                    Description = $"Installment {periodsPassed + 1}",
                    IsStructured = false,
                    LoanStructuringStatus="N/A",
                    IsCompleted = false,
                    PrincipalPaid = 0,
                    InterestPaid = 0,
                    PenaltyPaid = 0,
                    TaxPaid = 0,
                    PreviousInstallmentDue = false,
                    BranchId = loanParameter.BranchId,
                    BankId = loanParameter.BankId,
                    NextPaymentDate = IncrementPaymentDate(currentPaymentDate, repaymentCycle)
                };

                amortizationSchedule.Add(amortization);

                remainingBalance -= principalPayment;
                periodsPassed++;
                currentPaymentDate = IncrementPaymentDate(currentPaymentDate, repaymentCycle);
            }

            return amortizationSchedule;
        }

        public static int CalculateTotalPayments(int durationInMonths, string repaymentCycle)
        {
            int numberOfInstallments = 0;

            switch (repaymentCycle.ToLower())
            {
                case "daily":
                    numberOfInstallments = durationInMonths * 30;
                    break;
                case "weekly":
                    numberOfInstallments = durationInMonths * 4;
                    break;
                case "biweekly":
                    numberOfInstallments = durationInMonths * 2;
                    break;
                case "monthly":
                    numberOfInstallments = durationInMonths;
                    break;
                case "bimonthly":
                    numberOfInstallments = durationInMonths / 2;
                    break;
                case "quarterly":
                    numberOfInstallments = durationInMonths / 3;
                    break;
                case "every4months":
                    numberOfInstallments = durationInMonths / 4;
                    break;
                case "semiannual":
                    numberOfInstallments = durationInMonths / 6;
                    break;
                case "every9months":
                    numberOfInstallments = durationInMonths / 9;
                    break;
                case "yearly":
                    numberOfInstallments = durationInMonths / 12;
                    break;
                case "lumpsum":
                    numberOfInstallments = 1;
                    break;
                default:
                    throw new ArgumentException("Invalid repayment cycle.");
            }

            return numberOfInstallments;
        }


        private static decimal CalculateMonthlyPayment(decimal amount, decimal monthlyInterestRate, int totalPeriods)
        {
            decimal numerator = amount * monthlyInterestRate * (decimal)Math.Pow(1 + (double)monthlyInterestRate, totalPeriods);
            decimal denominator = (decimal)Math.Pow(1 + (double)monthlyInterestRate, totalPeriods) - 1;

            return numerator / denominator;
        }

        private static DateTime IncrementPaymentDate(DateTime currentPaymentDate, string repaymentCycle)
        {
            switch (repaymentCycle.ToLower())
            {
                case "daily":
                    return currentPaymentDate.AddDays(1);
                case "weekly":
                    return currentPaymentDate.AddDays(7);
                case "biweekly":
                    return currentPaymentDate.AddDays(14);
                case "monthly":
                    return currentPaymentDate.AddMonths(1);
                case "bimonthly":
                    return currentPaymentDate.AddMonths(2);
                case "quarterly":
                    return currentPaymentDate.AddMonths(3);
                case "every4months":
                    return currentPaymentDate.AddMonths(4);
                case "semiannual":
                    return currentPaymentDate.AddMonths(6);
                case "every9months":
                    return currentPaymentDate.AddMonths(9);
                case "yearly":
                    return currentPaymentDate.AddYears(1);
                case "lumpsum":
                    return currentPaymentDate.AddMonths(1); // Not sure about LumpSum cycle, using monthly as a placeholder
                default:
                    throw new ArgumentException("Invalid repayment cycle specified.");
            }
        }
    }

}
