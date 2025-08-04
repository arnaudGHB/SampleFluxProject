using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.PenaltyP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.PenaltyP
{

    public class PenaltyRepository : GenericRepository<Penalty, LoanContext>, IPenaltyRepository
    {
        public PenaltyRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
