using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanApplicationP.LoanAmortizationP
{

    public class LoanAmortizationRepository : GenericRepository<LoanAmortization, LoanContext>, ILoanAmortizationRepository
    {
        public LoanAmortizationRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
