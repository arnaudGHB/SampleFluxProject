using CBS.NLoan.Data.Entity.LoanApplicationP;

namespace CBS.NLoan.MediatR.LoanCalculatorHelper.InterestCalculationService
{
    public interface IInterestForLoanCalculationServices
    {
        Task CalculateDailyInterestAndUpdateAsync(CancellationToken stoppingToken);
    }
}