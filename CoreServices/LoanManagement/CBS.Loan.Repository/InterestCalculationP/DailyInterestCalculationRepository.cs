using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.InterestCalculationP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.InterestCalculationP
{

    public class DailyInterestCalculationRepository : GenericRepository<DailyInterestCalculation, LoanContext>, IDailyInterestCalculationRepository
    {
        public DailyInterestCalculationRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
