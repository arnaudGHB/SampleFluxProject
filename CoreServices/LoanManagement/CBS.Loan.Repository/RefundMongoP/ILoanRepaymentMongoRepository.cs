using CBS.NLoan.Common.MongoGenericRepository;
using CBS.NLoan.Data.MongoEntity.RefundP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Repository.RefundMongoP
{
    public interface ILoanRepaymentMongoRepository : IMongoGenericRepository<LoanRepaymentMongo>
    {
        Task<IEnumerable<LoanRepaymentMongo>> GetByBranchIdAsync(string branchId);
        Task<IEnumerable<LoanRepaymentMongo>> GetBySalaryCodeAsync(string salaryCode);
        Task<IEnumerable<LoanRepaymentMongo>> GetByExecutionDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<LoanRepaymentMongo>> GetByLoanIdAsync(string loanId);
        Task<IEnumerable<LoanRepaymentMongo>> GetBySalaryCodeAndStatusAsync(string salaryCode, string status);
    }
}
