using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.TaxP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.TaxP
{

    public class TaxRepository : GenericRepository<Tax, LoanContext>, ITaxRepository
    {
        public TaxRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
