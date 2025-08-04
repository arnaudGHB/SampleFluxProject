using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.CollateralP
{

    public class LoanProductCollateralRepository : GenericRepository<LoanProductCollateral, LoanContext>, ILoanProductCollateralRepository
    {
        public LoanProductCollateralRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
