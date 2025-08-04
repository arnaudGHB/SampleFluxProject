using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.FeeRangeP
{

    public class FeeRangeRepository : GenericRepository<FeeRange, LoanContext>, IFeeRangeRepository
    {
        public FeeRangeRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
