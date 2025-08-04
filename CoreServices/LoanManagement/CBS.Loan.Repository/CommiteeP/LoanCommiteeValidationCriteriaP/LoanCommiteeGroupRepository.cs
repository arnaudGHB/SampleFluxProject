using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.CommiteeP.LoanCommiteeGroupP
{

    public class LoanCommiteeGroupRepository : GenericRepository<LoanCommiteeGroup, LoanContext>, ILoanCommiteeGroupRepository
    {
        public LoanCommiteeGroupRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
