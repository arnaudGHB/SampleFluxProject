using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanApplicationP.RescheduleLoanP
{

    public class RescheduleLoanRepository : GenericRepository<RescheduleLoan, LoanContext>, IRescheduleLoanRepository
    {
        public RescheduleLoanRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
