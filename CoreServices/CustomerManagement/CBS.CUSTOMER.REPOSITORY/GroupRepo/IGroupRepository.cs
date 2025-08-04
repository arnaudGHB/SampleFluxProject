using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Resources;

namespace CBS.CUSTOMER.REPOSITORY.GroupRepo
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        Task<GroupsList> GetGroupsAsync(GroupResource groupResource);
        Task<GroupsList> GetGroupsAsyncByBranch(GroupResource groupResource);
    }
}
