using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.Resource;
using CBS.TransactionManagement.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository
{
    public class MemberAccountSituationListingx : List<MembersAccountSummaryDto>
    {
        public string MemberName { get; set; }
        public string MemberReference { get; set; }
        public string BranchCode { get; set; }
        public decimal Saving { get; set; }
        public decimal PreferenceShare { get; set; }
        public decimal Share { get; set; }
        public decimal Deposit { get; set; }
        public decimal Loan { get; set; }
        public decimal Gav { get; set; }
        public decimal DailyCollection { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal NetBalance { get; set; }
        public PaginationMetadata PaginationMetadata { get; private set; }

        public int Skip { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public MemberAccountSituationListingx()
        {
        }

        public MemberAccountSituationListingx(List<MembersAccountSummaryDto> items, int count, int skip, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            Skip = skip;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public async Task<MemberAccountSituationListing> Create(IQueryable<Account> source, int skip, int pageSize)
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
            var dtoPageList = new MemberAccountSituationListing(dtoList, count, skip, pageSize);
            return dtoPageList;
        }

        public async Task<int> GetCount(IQueryable<Account> source)
        {
            return await source.AsNoTracking().CountAsync();
        }

        public async Task<List<MembersAccountSummaryDto>> GetDtos(IQueryable<Account> source, int skip, int pageSize)
        {
            var entities = await source
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .Select(c => new MembersAccountSummaryDto
                {
                    MemberName = c.CustomerName,
                    MemberReference = c.CustomerId,
                    BranchCode = c.BranchCode,
                    Saving = c.AccountType == AccountType.Saving.ToString() ? c.Balance : 0,
                    PreferenceShare = c.AccountType == AccountType.PreferenceShare.ToString() ? c.Balance : 0,
                    Share = c.AccountType == AccountType.MemberShare.ToString() ? c.Balance : 0,
                    Deposit = c.AccountType == AccountType.Deposit.ToString() ? c.Balance : 0,
                    Loan = c.AccountType == AccountType.Loan.ToString() ? c.Balance : 0,
                    Gav = c.AccountType == AccountType.Gav.ToString() ? c.Balance : 0,
                    DailyCollection = c.AccountType == AccountType.DailyCollection.ToString() ? c.Balance : 0,
                    TotalBalance = c.Balance,
                    NetBalance = c.Balance - (c.AccountType == AccountType.Loan.ToString() ? c.Balance : 0),
                    PaginationMetadata = PaginationMetadata
                })
                .ToListAsync();

            return entities;
        }
    }


    public class MemberAccountSituationListing : List<MembersAccountSummaryDto>
    {
       
        public int Skip { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public PaginationMetadata PaginationMetadata { get; private set; }
        public MemberAccountSituationListing()
        {
        }

        public MemberAccountSituationListing(List<MembersAccountSummaryDto> items, int count, int skip, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            Skip = skip;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public async Task<MemberAccountSituationListing> Create(IQueryable<Account> source, int skip, int pageSize)
        {

            var count = await source.AsNoTracking().CountAsync();
            PaginationMetadata = new PaginationMetadata
            {
                PageSize = pageSize,
                Skip = skip,
                TotalCount = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            };
            var dtoList = await GetDtos(source, skip, pageSize);
            var dtoPageList = new MemberAccountSituationListing(dtoList, count, skip, pageSize);
            return dtoPageList;

        }

        private async Task<List<MembersAccountSummaryDto>> GetDtos(IQueryable<Account> source, int skip, int pageSize)
        {
            var entities = await source
                .GroupBy(a => new { a.CustomerId, a.CustomerName, a.BranchCode })
                .Select(g => new MembersAccountSummaryDto
                {
                    MemberName = g.Key.CustomerName,
                    MemberReference = g.Key.CustomerId,
                    BranchCode = g.Key.BranchCode,
                    Saving = g.Where(a => a.AccountType == AccountType.Saving.ToString()).Sum(a => a.Balance),
                    PreferenceShare = g.Where(a => a.AccountType == AccountType.PreferenceShare.ToString()).Sum(a => a.Balance),
                    Share = g.Where(a => a.AccountType == AccountType.MemberShare.ToString()).Sum(a => a.Balance),
                    Deposit = g.Where(a => a.AccountType == AccountType.Deposit.ToString()).Sum(a => a.Balance),
                    Loan = g.Where(a => a.AccountType == AccountType.Loan.ToString()).Sum(a => a.Balance),
                    Gav = g.Where(a => a.AccountType == AccountType.Gav.ToString()).Sum(a => a.Balance),
                    DailyCollection = g.Where(a => a.AccountType == AccountType.DailyCollection.ToString()).Sum(a => a.Balance),
                    TotalBalance = g.Sum(a => a.Balance),
                    NetBalance = g.Sum(a => a.Balance) - g.Where(a => a.AccountType == AccountType.Loan.ToString()).Sum(a => a.Balance), 
                    PaginationMetadata= PaginationMetadata
                })
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return entities;
        }
    }
}
