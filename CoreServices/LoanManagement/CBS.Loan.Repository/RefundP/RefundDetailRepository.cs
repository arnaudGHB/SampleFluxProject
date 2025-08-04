using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.RefundP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.RefundP
{

    public class RefundDetailRepository : GenericRepository<RefundDetail, LoanContext>, IRefundDetailRepository
    {
        public RefundDetailRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
