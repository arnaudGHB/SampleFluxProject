using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.InterestCalculationP;

namespace CBS.NLoan.Repository.InterestCalculationP
{ 
    public interface IDailyInterestCalculationRepository : IGenericRepository<DailyInterestCalculation>
    {
    }
}
