using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Repository.AccountingDayOpening
{
    public interface IAccountingDayRepository : IGenericRepository<AccountingDay>
    {
        Task<bool> CloseAccountingDay(DateTime date, string branchId = null);
        DateTime GetCurrentAccountingDay(string branchId = null);
        Task<CloseOrOpenAccountingDayResultDto> AccountingDayAction(string id, string option);
        Task<List<AccountingDay>> OpenAccountingDays(DateTime date, bool isCentralized = false, List<string> branchIds = null);
        Task<bool> ReopenClosedAccountingDay(DateTime date, string branchId = null);
        Task<List<CloseOrOpenAccountingDayResultDto>> OpenAccountingDayForBranches(DateTime date, List<BranchListing> branches, bool isCentralized, List<BranchDto> branchDtos);
        Task<List<CloseOrOpenAccountingDayResultDto>> CloseAccountingDayForBranches(DateTime date, List<BranchListing> branches, bool isCentralized);
        Task<bool> DeleteAccountingDay(DateTime date, string branchId = null);
        Task<List<AccountingDayDto>> GetAccountingDays(DateTime date, List<BranchDto> branches, string queryParameter, bool byBranch);
    }
}
