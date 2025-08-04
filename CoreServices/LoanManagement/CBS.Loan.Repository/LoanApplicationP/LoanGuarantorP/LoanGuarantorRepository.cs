using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanApplicationP.LoanGuarantorP
{

    public class LoanGuarantorRepository : GenericRepository<LoanGuarantor, LoanContext>, ILoanGuarantorRepository
    {
        public LoanGuarantorRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
