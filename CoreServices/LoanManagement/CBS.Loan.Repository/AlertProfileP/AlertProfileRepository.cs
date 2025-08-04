using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.AlertProfileP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.AlertProfileP
{

    public class AlertProfileRepository : GenericRepository<AlertProfile, LoanContext>, IAlertProfileRepository
    {
        public AlertProfileRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
