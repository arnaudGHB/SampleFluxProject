using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.LoanTermP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanTermP
{

    public class LoanTermRepository : GenericRepository<LoanTerm, LoanContext>, ILoanTermRepository
    {
        public LoanTermRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
