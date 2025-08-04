using CBS.NLoan.Data.Entity.LoanApplicationP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.LoanCalculatorHelper.DeliquencyCalculations
{
    public interface IDelinquencyService
    {
        Task ProcessLoanAsync(Loan loan);
        Task ProcessAllLoansAsync();
    }
}
