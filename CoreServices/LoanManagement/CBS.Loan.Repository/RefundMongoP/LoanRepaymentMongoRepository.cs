using System;
using System.Collections.Generic;
using CBS.NLoan.Domain.MongoDBContext.DBConnection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.NLoan.Data.MongoEntity.RefundP;
using MongoDB.Driver;
using CBS.NLoan.Common.MongoGenericRepository;

namespace CBS.NLoan.Repository.RefundMongoP
{
    public class LoanRepaymentMongoRepository : MongoGenericRepository<LoanRepaymentMongo>, ILoanRepaymentMongoRepository
    {
        public LoanRepaymentMongoRepository(IMongoDbConnection mongoDbConnection)
            : base(mongoDbConnection)
        {
        }

        public async Task<IEnumerable<LoanRepaymentMongo>> GetByBranchIdAsync(string branchId)
        {
            var filter = Builders<LoanRepaymentMongo>.Filter.Eq(x => x.BranchId, branchId);
            return await FindByAsync(filter);
        }

        public async Task<IEnumerable<LoanRepaymentMongo>> GetBySalaryCodeAsync(string salaryCode)
        {
            var filter = Builders<LoanRepaymentMongo>.Filter.Eq(x => x.SalaryCode, salaryCode);
            return await FindByAsync(filter);
        } 
        
        public async Task<IEnumerable<LoanRepaymentMongo>> GetBySalaryCodeAndStatusAsync(string salaryCode,string status)
        {
            var filter = Builders<LoanRepaymentMongo>.Filter.And(
                Builders<LoanRepaymentMongo>.Filter.Eq(x => x.SalaryCode, salaryCode),
                Builders<LoanRepaymentMongo>.Filter.Eq(x => x.Status, status));
            return await FindByAsync(filter);
        }

        public async Task<IEnumerable<LoanRepaymentMongo>> GetByExecutionDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<LoanRepaymentMongo>.Filter.And(
                Builders<LoanRepaymentMongo>.Filter.Gte(x => x.ExecutionDate, startDate),
                Builders<LoanRepaymentMongo>.Filter.Lte(x => x.ExecutionDate, endDate));

            return await FindByAsync(filter);
        }

        public async Task<IEnumerable<LoanRepaymentMongo>> GetByLoanIdAsync(string loanId)
        {
            var filter = Builders<LoanRepaymentMongo>.Filter.Eq(x => x.LoanId, loanId);
            return await FindByAsync(filter);
        }
    }
}
