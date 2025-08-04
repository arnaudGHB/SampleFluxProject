using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP
{

    public class LoanProductRepaymentCycleRepository : GenericRepository<LoanProductRepaymentCycle, LoanContext>, ILoanProductRepaymentCycleRepository
    {
        public LoanProductRepaymentCycleRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
