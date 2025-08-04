using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Repository
{
    public interface IDailyTellerRepository : IGenericRepository<DailyTeller>
    {
        Task<DailyTeller> GetAnActivePrimaryTellerForTheDate();
        Task<DailyTeller> GetThePrimaryTellerOfTheDay(string tellerId);
        Task<DailyTeller> GetAnActiveTellerForTheDate(string tellerId);
        Task<DailyTeller> GetAnActiveSubTellerForTheDate();
        Task<DailyTeller> GetAnActiveSubTellerForTheDate3PP(string tellerid);
    }
}
