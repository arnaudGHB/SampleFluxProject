using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Repository
{
    public interface ITellerRepository : IGenericRepository<Teller>
    {
        Task<Teller> RetrieveTeller(DailyTeller tellerProvision);
        Task<Teller> GetTellerByCode(string applicationCode, string branchCode, string branchName);
        Task<bool> CheckIfTransferIsWithinTellerScope(string accessCode, decimal amount);
        Task<Teller> GetPrimaryTeller(string BranchId);
        Task<Teller> GetTeller(string tellerId);
        Task<Teller> GetTellerByOperationType(string SourceType);
        Task<Teller> GetTellerByType(string tellerType);
        Task<bool> ValidateTellerLimites(decimal requestedAmount, decimal dinominationTotal, Teller teller);
        Task<bool> CheckTellerOperationalRights(Teller teller, string operationType, bool isCashOperation);
        Task<Teller> GetTellerByType(string tellerType, string branchid);
        Task<Teller> GetTellerByOperationType(string SourceType, string branchid);
    }
}
