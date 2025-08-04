using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data.Entity.HolyDayP;

namespace CBS.TransactionManagement.Repository.HolyDayP
{
    public interface IHolyDayRepository : IGenericRepository<HolyDay>
    {
        Task<bool> IsHoliday(DateTime accountingDay);
    }
}
