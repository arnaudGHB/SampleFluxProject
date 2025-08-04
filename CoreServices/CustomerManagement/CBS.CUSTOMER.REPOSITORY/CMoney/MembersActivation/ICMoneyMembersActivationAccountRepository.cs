using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;


namespace CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation
{
    public interface ICMoneyMembersActivationAccountRepository : IGenericRepository<CMoneyMembersActivationAccount>
    {
        Task<CMoneyMembersActivationAccountsList> GetCustomersAsync(CustomerResource customerResource);
        Task<CMoneyMembersActivationAccountsList> GetCustomersAsyncByBranch(CustomerResource customerResource);

    }
}
