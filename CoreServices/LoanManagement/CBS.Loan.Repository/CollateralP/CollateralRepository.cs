using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.CollateralP
{

    public class CollateralRepository : GenericRepository<Collateral, LoanContext>, ICollateralRepository
    {
        public CollateralRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
