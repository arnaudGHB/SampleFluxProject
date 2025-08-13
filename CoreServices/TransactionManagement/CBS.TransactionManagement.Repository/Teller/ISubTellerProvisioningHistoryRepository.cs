using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data.Entity.ChangeManagement;

namespace CBS.TransactionManagement.Repository
{
    public interface ISubTellerProvisioningHistoryRepository : IGenericRepository<SubTellerProvioningHistory>
    {
        SubTellerProvioningHistory CashInByDinomination(decimal amount, CurrencyNotesRequest request, string tellerId, DateTime accountingDate, string OpenOfDayReference);
        SubTellerProvioningHistory CashInByDinominationReplenisment(decimal amount, CurrencyNotesRequest request, string tellerId, DateTime accountingDate, string OpenOfDayReference);
        SubTellerProvioningHistory CashOutByDinomination(decimal amount, CurrencyNotesRequest request, string tellerId, DateTime accountingDate, string OpenOfDayReference);
        SubTellerProvioningHistory CashOutByDenomination(decimal amount, CurrencyNotesRequest request, string tellerId, DateTime accountingDate, decimal customerCharges, bool IsChargeInclussive, string OpenOfDayReference);
       
        Task<bool> CheckIfDayIsStillOpened(string tellerid, DateTime accountingDay);
        SubTellerProvioningHistory CashOutByDinomination(decimal amount, CurrencyNotesRequest request, SubTellerProvioningHistory _tellerHistory);
        SubTellerProvioningHistory CloseDay(CurrencyNotesRequest request, SubTellerProvioningHistory _tellerHistory);
        List<TillStatusReportDto> GetAllTillsByBranchId(string branchId, DateTime dateTimeFrom, DateTime dateTimeTo);
        SubTellerProvioningHistory OpenDay(CurrencyNotesRequest request, SubTellerProvioningHistory _tellerHistory);
        Task<SubTellerProvioningHistory> GetLastUpdatedRecordForSubTellerProvisioningHistory(string tellerId);
        SubTellerProvioningHistory CashInAndOutByDenomination(
            decimal amountToCashOut,
            decimal charges,
            CurrencyNotesRequest notesRequest,
            string tellerId,
            DateTime accountingDate,
            string openOfDayReference);
        Task<CashChangeHistory> ManageCashChangeAsync(CashChangeDataCarrier changeManagement);
    }
}