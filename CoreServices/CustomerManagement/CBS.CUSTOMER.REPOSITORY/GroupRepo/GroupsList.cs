using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Dto.Groups;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.HELPER.Helper;
using Microsoft.EntityFrameworkCore;

namespace CBS.CUSTOMER.REPOSITORY.GroupRepo
{


    public class GroupsList : List<GroupDto>
    {
        public GroupsList()
        {
        }

        public int Skip { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public PaginationMetadata PaginationMetadata;

        public GroupsList(List<GroupDto> items, int count, int skip, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            Skip = skip;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public async Task<GroupsList> Create(IQueryable<Group> source, int skip, int pageSize)
        {
            var count = await GetCount(source);
            PaginationMetadata = new PaginationMetadata
            {
                PageSize = pageSize,
                Skip = skip,
                TotalCount = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            };
            var dtoList = await GetDtos(source, skip, pageSize);
            var dtoPageList = new GroupsList(dtoList, count, skip, pageSize);
            return dtoPageList;
        }

        public async Task<int> GetCount(IQueryable<Group> source)
        {
            return await source.AsNoTracking().CountAsync();
        }

        public async Task<List<GroupDto>> GetDtos(IQueryable<Group> source, int skip, int pageSize)
        {
            var entities = await source
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .Select(c => new GroupDto
                {
                    GroupId = c.GroupId,
                    GroupName = c.GroupName,
                    GroupTypeId = c.GroupTypeId,
                    RegistrationNumber = c.RegistrationNumber,
                    TaxPayerNumber = c.TaxPayerNumber,
                    DateOfEstablishment = c.DateOfEstablishment,
                    PhotoSource = c.PhotoSource,
                    GroupLeaderId = c.GroupLeaderId,
                    Active = c.Active,
                    GroupType = c.GroupType, 
                    PaginationMetadata = PaginationMetadata
                })
                .ToListAsync();

            return entities;
        }


    }


}
