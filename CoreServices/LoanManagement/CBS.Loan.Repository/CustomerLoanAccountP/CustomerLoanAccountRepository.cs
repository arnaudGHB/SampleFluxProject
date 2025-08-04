using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.CustomerLoanAccountP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.CustomerLoanAccountP
{

    public class CustomerLoanAccountRepository : GenericRepository<CustomerLoanAccount, LoanContext>, ICustomerLoanAccountRepository
    {
        public CustomerLoanAccountRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
