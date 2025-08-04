using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP
{

    public class LoanProductRepaymentOrderRepository : GenericRepository<LoanProductRepaymentOrder, LoanContext>, ILoanProductRepaymentOrderRepository
    {
        public LoanProductRepaymentOrderRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
