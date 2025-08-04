using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.AccountingRuleP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.AccountingRuleP
{

    public class AccountingRuleRepository : GenericRepository<AccountingRule, LoanContext>, IAccountingRuleRepository
    {
        public AccountingRuleRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
