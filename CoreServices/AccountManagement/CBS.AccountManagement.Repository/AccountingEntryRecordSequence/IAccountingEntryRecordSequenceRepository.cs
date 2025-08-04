using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;

namespace CBS.AccountManagement.Repository
{
    public interface IAccountingEntryRecordSequenceRepository : IGenericRepository<AccountingEntryRecordSequence>
    {
        Task<string> GetNextSequenceValueAsync(string sequenceName, string branchCode);
    }
}