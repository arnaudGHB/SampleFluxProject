using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanApplicationP.DisburstLoan
{

    public class DisburstedLoanRepository : GenericRepository<DisburstedLoan, LoanContext>, IDisburstedLoanRepository
    {
        public DisburstedLoanRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
