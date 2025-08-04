using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data.Entity.ChangeManagement;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Repository
{
    public interface ICashChangeHistoryRepository : IGenericRepository<CashChangeHistory>
    {
        void CreateChangeHistory(CashChangeDataCarrier changeManagement, CashChangeHistory changeHistory);
    }
}
