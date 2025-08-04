using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.FundingLineP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.FundingLineP
{

    public class FundingLineRepository : GenericRepository<FundingLine, LoanContext>, IFundingLineRepository
    {
        public FundingLineRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
