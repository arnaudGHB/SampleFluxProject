using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using Microsoft.EntityFrameworkCore;


namespace CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation
{

   

    public class CMoneyMembersActivationAccountRepository : GenericRepository<CMoneyMembersActivationAccount, POSContext>, ICMoneyMembersActivationAccountRepository
    {
        private readonly IPropertyMappingService _propertyMappingService;

        public CMoneyMembersActivationAccountRepository(IUnitOfWork<POSContext> unitOfWork, IPropertyMappingService propertyMappingService = null) : base(unitOfWork)
        {
            _propertyMappingService = propertyMappingService;
        }

        public async Task<CMoneyMembersActivationAccountsList> GetCustomersAsync(CustomerResource customerResource)
        {
            // Define the base query for customers
            IQueryable<CMoneyMembersActivationAccount> query = All.AsNoTracking().Where(x => x.IsDeleted == false);


            // Apply sorting based on the provided order by property
            query = query.ApplySort(customerResource.OrderBy, _propertyMappingService.GetPropertyMapping<CMoneyMembersActivationAccountDto, CMoneyMembersActivationAccount>()).OrderBy(x => x.Name);

            // Filter customers based on SearchQuery if provided
            if (!string.IsNullOrWhiteSpace(customerResource.SearchQuery) && customerResource.SearchQuery.ToLower() != "all")
            {
                string searchQueryLower = customerResource.SearchQuery.ToLower();
                query = query.Where(c =>
                    (c.Name != null && c.Name.ToLower().Contains(searchQueryLower)) ||
                    (c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(searchQueryLower)) ||
                    (c.LoginId != null && c.LoginId.ToLower().Contains(searchQueryLower)) ||
                    (c.BranchCode != null && c.BranchCode.ToLower().Contains(searchQueryLower)) ||
                    (c.CustomerId != null && c.CustomerId.ToLower().Contains(searchQueryLower)));
            }
            var customersList = new CMoneyMembersActivationAccountsList();
            return await customersList.Create(query, customerResource.Skip, customerResource.PageSize);
        }

        public async Task<CMoneyMembersActivationAccountsList> GetCustomersAsyncByBranch(CustomerResource customerResource)
        {
            // Define the base query for customers
            IQueryable<CMoneyMembersActivationAccount> query = FindBy(x => x.BranchId == customerResource.BranchId && x.IsDeleted == false).AsNoTracking();

            // Apply sorting based on the provided order by property

            query = query.ApplySort(customerResource.OrderBy, _propertyMappingService.GetPropertyMapping<CMoneyMembersActivationAccountDto, CMoneyMembersActivationAccount>()).OrderBy(x => x.Name);

            // Filter customers based on SearchQuery if provided
            if (!string.IsNullOrWhiteSpace(customerResource.SearchQuery) && customerResource.SearchQuery.ToLower() != "all")
            {
                string searchQueryLower = customerResource.SearchQuery.ToLower();
                query = query.Where(c =>
                    (c.Name != null && c.Name.ToLower().Contains(searchQueryLower)) ||
                    (c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(searchQueryLower)) ||
                    (c.LoginId != null && c.LoginId.ToLower().Contains(searchQueryLower)) ||
                    (c.BranchCode != null && c.BranchCode.ToLower().Contains(searchQueryLower)) ||
                    (c.CustomerId != null && c.CustomerId.ToLower().Contains(searchQueryLower)));
            }


            // Create and return the paginated list of customers
            var customersList = new CMoneyMembersActivationAccountsList();
            return await customersList.Create(query, customerResource.Skip, customerResource.PageSize);
        }


    }
}
