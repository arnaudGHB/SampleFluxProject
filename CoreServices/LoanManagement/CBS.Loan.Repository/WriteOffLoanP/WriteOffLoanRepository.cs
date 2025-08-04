using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.WriteOffLoanP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.WriteOffLoanP
{

    public class WriteOffLoanRepository : GenericRepository<WriteOffLoan, LoanContext>, IWriteOffLoanRepository
    {
        public WriteOffLoanRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
