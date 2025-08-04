using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanPurposeP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanPurposeP
{

    public class LoanPurposeRepository : GenericRepository<LoanPurpose, LoanContext>, ILoanPurposeRepository
    {
        public LoanPurposeRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
