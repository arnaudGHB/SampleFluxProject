using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.FeeP.FeeP
{

    public class FeeRepository : GenericRepository<Fee, LoanContext>, IFeeRepository
    {
        public FeeRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
