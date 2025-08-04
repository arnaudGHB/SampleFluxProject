using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data.Entity.HolyDayP;
using CBS.TransactionManagement.Data.Entity.HolyDayRecurringP;

namespace CBS.TransactionManagement.Repository.HolyDayRecurringP
{
    public interface IHolyDayRecurringRepository : IGenericRepository<HolyDayRecurring>
    {
        Task<bool> IsRecurringHoliday(DateTime accountingDay);
    }
}
