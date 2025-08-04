using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CUSTOMER.HELPER.Helper;
using Microsoft.EntityFrameworkCore;

namespace CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation
{


    public class CMoneyMembersActivationAccountsList : List<GetAllCMoneyMembersAccounts>
    {
        public CMoneyMembersActivationAccountsList()
        {
        }

        public int Skip { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public PaginationMetadata PaginationMetadata;

        public CMoneyMembersActivationAccountsList(List<GetAllCMoneyMembersAccounts> items, int count, int skip, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            Skip = skip;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public async Task<CMoneyMembersActivationAccountsList> Create(IQueryable<CMoneyMembersActivationAccount> source, int skip, int pageSize)
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
            var dtoPageList = new CMoneyMembersActivationAccountsList(dtoList, count, skip, pageSize);
            return dtoPageList;
        }

        public async Task<int> GetCount(IQueryable<CMoneyMembersActivationAccount> source)
        {
            return await source.AsNoTracking().CountAsync();
        }

        public async Task<List<GetAllCMoneyMembersAccounts>> GetDtos(IQueryable<CMoneyMembersActivationAccount> source, int skip, int pageSize)
        {
            var entities = await source
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .Select(c => new GetAllCMoneyMembersAccounts
                {
                    CustomerId = c.CustomerId,
                    SecretQuestion = c.SecretQuestion,
                    SecretAnswer = c.SecretAnswer,
                    PaginationMetadata = PaginationMetadata,
                    Name=c.Name,
                    BranchCode=c.BranchCode, 
                    LoginId=c.LoginId,
                    PhoneNumber=c.PhoneNumber,
                    ActivatedBy=c.ActivatedBy,
                    ActivatingBranchCode=c.ActivatingBranchCode,
                    ActivatingBranchId=c.ActivatingBranchId,
                    ActivatingBranchName=c.ActivatingBranchName,
                    ActivationDate=c.ActivationDate,
                    BranchId=c.BranchId,
                    DeactivationReason=c.DeactivationReason,
                    Id=c.Id,
                    DefaultPin=c.DefaultPin,
                    IsActive=c.IsActive,
                    IsSubcribed=c.IsSubcribed,
                    Language=c.Language,
                    LastPaymentDate=c.LastPaymentDate,
                    LastSubcriptionRenewalDate=c.LastSubcriptionRenewalDate,
                    FailedAttempts=c.FailedAttempts,
                    HasChangeDefaultPin=c.HasChangeDefaultPin,
                    LastPaymentAmount=c.LastPaymentAmount,
                    LastLoginDate=c.LastLoginDate
                })
                .ToListAsync();
            return entities;
        }

    }


}
