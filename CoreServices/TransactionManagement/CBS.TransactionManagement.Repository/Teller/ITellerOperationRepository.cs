using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Repository
{
    public interface ITellerOperationRepository : IGenericRepository<TellerOperation>
    {
        Task<List<TellerOperationGL>> GetTellerLastOperations(string tellerId, DateTime accountingDate);
    }
}
