using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Data.Entity.ChangeManagement;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Repository
{
    public interface IPrimaryTellerProvisioningHistoryRepository : IGenericRepository<PrimaryTellerProvisioningHistory>
    {
        void CashInByDinomination(decimal amount, CurrencyNotesRequest request, string tellerId, DateTime accountingDate, string Reference);
        Task<bool> CashOutByDinomination(decimal amount, CurrencyNotesRequest request, string tellerProvitionHistoryId, DateTime accountingDate,string Reference);
        Task<PrimaryTellerProvisioningHistory> CheckIfPrimaryTellerIsOpened(DateTime accountingDate);
        PrimaryTellerProvisioningHistory CloseDay(CurrencyNotesRequest request, PrimaryTellerProvisioningHistory _tellerHistory);
        List<TillStatusReportDto> GetAllTillsByBranchId(string branchId, DateTime dateTimeFrom, DateTime dateTimeTo);
        PrimaryTellerProvisioningHistory OpenDay(CurrencyNotesRequest request, PrimaryTellerProvisioningHistory _tellerHistory);
        Task<PrimaryTellerProvisioningHistory> GetLastUpdatedRecordForPrimaryProvisioningHistory(string tellerId);
        void CashOutByDinomination(decimal amount, CurrencyNotesRequest request, PrimaryTellerProvisioningHistory _tellerHistory);
        Task<CashChangeHistory> ManageCashChangeAsync(CashChangeDataCarrier changeManagement);
    }
}
