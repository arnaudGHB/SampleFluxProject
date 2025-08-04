using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.CommiteeP.LoanCommeteeMemberP
{

    public class LoanCommeteeMemberRepository : GenericRepository<LoanCommiteeMember, LoanContext>, ILoanCommeteeMemberRepository
    {
        public LoanCommeteeMemberRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
