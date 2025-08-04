using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.CustomerProfileP
{

    public class CustomerProfileRepository : GenericRepository<CustomerProfile, LoanContext>, ICustomerProfileRepository
    {
        public CustomerProfileRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
