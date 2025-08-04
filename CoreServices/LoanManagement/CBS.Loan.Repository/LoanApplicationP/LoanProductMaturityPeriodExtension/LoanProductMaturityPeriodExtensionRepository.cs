using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP
{

    public class LoanProductMaturityPeriodExtensionRepository : GenericRepository<LoanProductMaturityPeriodExtension, LoanContext>, ILoanProductMaturityPeriodExtensionRepository
    {
        public LoanProductMaturityPeriodExtensionRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
