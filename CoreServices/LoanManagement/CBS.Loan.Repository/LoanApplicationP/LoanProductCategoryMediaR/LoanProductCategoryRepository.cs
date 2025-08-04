using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanApplicationP.LoanCycleP
{

    public class LoanProductCategoryRepository : GenericRepository<LoanProductCategory, LoanContext>, ILoanProductCategoryRepository
    {
        public LoanProductCategoryRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
