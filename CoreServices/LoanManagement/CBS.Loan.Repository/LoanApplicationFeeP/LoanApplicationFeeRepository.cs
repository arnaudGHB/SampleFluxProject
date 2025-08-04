using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanApplicationFeeP
{

    public class LoanApplicationFeeRepository : GenericRepository<LoanApplicationFee, LoanContext>, ILoanApplicationFeeRepository
    {
        public LoanApplicationFeeRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
