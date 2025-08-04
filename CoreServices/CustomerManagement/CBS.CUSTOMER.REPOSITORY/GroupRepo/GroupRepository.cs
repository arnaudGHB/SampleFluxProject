using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.Groups;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using Microsoft.EntityFrameworkCore;

namespace CBS.CUSTOMER.REPOSITORY.GroupRepo
{

    public class GroupRepository : GenericRepository<Group, POSContext>, IGroupRepository
    {
        private readonly IPropertyMappingService _propertyMappingService;

        public GroupRepository(IUnitOfWork<POSContext> unitOfWork, IPropertyMappingService propertyMappingService = null) : base(unitOfWork)
        {
            _propertyMappingService = propertyMappingService;
        }

        public async Task<GroupsList> GetGroupsAsync(GroupResource groupResource)
        {
            // Define the base query for groups
            IQueryable<Group> query = All.AsNoTracking().Where(x => x.IsDeleted == false);

            // Apply sorting based on the provided order by property
            query = query.ApplySort(groupResource.OrderBy, _propertyMappingService.GetPropertyMapping<GroupDto, Group>()).OrderBy(x => x.GroupName);

            // Filter groups based on SearchQuery if provided
            if (!string.IsNullOrWhiteSpace(groupResource.SearchQuery) && groupResource.SearchQuery.ToLower() != "all")
            {
                string searchQueryLower = groupResource.SearchQuery.ToLower();
                query = query.Where(g =>
                    (g.GroupName != null && g.GroupName.ToLower().Contains(searchQueryLower)) ||
                    (g.RegistrationNumber != null && g.RegistrationNumber.ToLower().Contains(searchQueryLower)) ||
                    (g.TaxPayerNumber != null && g.TaxPayerNumber.ToLower().Contains(searchQueryLower))); // Assuming GroupType has a TypeName property
            }

            // Create and return the paginated list of groups
            var groupsList = new GroupsList();
            return await groupsList.Create(query, groupResource.Skip, groupResource.PageSize);
        }

        public async Task<GroupsList> GetGroupsAsyncByBranch(GroupResource groupResource)
        {
            // Define the base query for groups
            IQueryable<Group> query = FindBy(x=>x.BranchId== groupResource.BranchId &&x.IsDeleted==false).AsNoTracking().Where(x => x.IsDeleted == false);

            // Apply sorting based on the provided order by property
            query = query.ApplySort(groupResource.OrderBy, _propertyMappingService.GetPropertyMapping<GroupDto, Group>()).OrderBy(x => x.GroupName);

            // Filter groups based on SearchQuery if provided
            if (!string.IsNullOrWhiteSpace(groupResource.SearchQuery) && groupResource.SearchQuery.ToLower() != "all")
            {
                string searchQueryLower = groupResource.SearchQuery.ToLower();
                query = query.Where(g =>
                    (g.GroupName != null && g.GroupName.ToLower().Contains(searchQueryLower)) ||
                    (g.RegistrationNumber != null && g.RegistrationNumber.ToLower().Contains(searchQueryLower)) ||
                    (g.TaxPayerNumber != null && g.TaxPayerNumber.ToLower().Contains(searchQueryLower))); // Assuming GroupType has a TypeName property
            }

            // Create and return the paginated list of groups
            var groupsList = new GroupsList();
            return await groupsList.Create(query, groupResource.Skip, groupResource.PageSize);
        }

    }
    // IQueryable<DATA.Entity.Customer> query = FindBy(x => x.BranchId == customerResource.BranchId && x.IsDeleted == false).AsNoTracking();

}
