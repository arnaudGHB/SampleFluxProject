using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.LoanApplicationP.LoanDeliquencyConfigurationP
{

    public class LoanDeliquencyConfigurationRepository : GenericRepository<LoanDeliquencyConfiguration, LoanContext>, ILoanDeliquencyConfigurationRepository
    {
        public LoanDeliquencyConfigurationRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
