using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.PeriodP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.PeriodP
{

    public class PeriodRepository : GenericRepository<Period, LoanContext>, IPeriodRepository
    {
        public PeriodRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
