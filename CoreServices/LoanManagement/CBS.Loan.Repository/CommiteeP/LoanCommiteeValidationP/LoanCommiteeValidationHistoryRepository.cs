using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.CommiteeP.LoanCommiteeValidationP
{

    public class LoanCommiteeValidationHistoryRepository : GenericRepository<LoanCommiteeValidationHistory, LoanContext>, ILoanCommiteeValidationHistoryRepository
    {
        public LoanCommiteeValidationHistoryRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
