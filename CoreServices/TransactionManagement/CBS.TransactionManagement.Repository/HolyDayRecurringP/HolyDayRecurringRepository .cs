using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.HolyDayP;
using CBS.TransactionManagement.Data.Entity.HolyDayRecurringP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.HolyDayRecurringP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Repository.HolyDayP
{


    public class HolyDayRecurringRepository : GenericRepository<HolyDayRecurring, TransactionContext>, IHolyDayRecurringRepository
    {
        private readonly ILogger<HolyDayRecurringRepository> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IUnitOfWork<TransactionContext> _unitOfWork;
        public HolyDayRecurringRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<HolyDayRecurringRepository> logger = null, UserInfoToken userInfoToken = null) : base(unitOfWork)
        {
            _logger = logger;
            _userInfoToken = userInfoToken;
            _unitOfWork = unitOfWork;
        }



        /// <summary>
        /// Checks if the given date is a recurring holiday (weekends, lockdowns, etc.) based on the admin configuration.
        /// </summary>
        /// <param name="accountingDay">The date to check.</param>
        /// <returns>True if it's a recurring holiday, false otherwise.</returns>
        public async Task<bool> IsRecurringHoliday(DateTime accountingDay)
        {
            // Fetch the recurring holiday configurations
            var recurringHolidays = await All
                .AsNoTracking()
                .Where(r => r.IsActive &&
                            (r.IsGlobal || r.BranchId == _userInfoToken.BranchID))
                .ToListAsync();

            foreach (var holiday in recurringHolidays)
            {
                if (!Enum.TryParse(holiday.HolidayType, true, out HolidayType holidayType))
                    continue;

                switch (holidayType)
                {
                    case HolidayType.Weekend:
                        if (IsWeekend(accountingDay))
                            return true;
                        break;

                    case HolidayType.Recurring:
                        if (HandleRecurringHoliday(holiday, accountingDay))
                            return true;
                        break;

                    case HolidayType.Lockdown:
                        if (MatchesRecurringDay(holiday.RecurringDay, accountingDay))
                            return true;
                        break;

                    case HolidayType.SpecialEvent:
                        if (MatchesRecurringDay(holiday.RecurringDay, accountingDay))
                            return true;
                        break;

                    case HolidayType.PublicHoliday:
                        // Implement additional logic for public holidays if needed
                        break;

                    default:
                        break;
                }
            }

            return false;
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private bool HandleRecurringHoliday(HolyDayRecurring holiday, DateTime accountingDay)
        {
            if (!Enum.TryParse(holiday.RecurrencePattern, true, out RecurrencePattern recurrencePattern))
                return false;

            switch (recurrencePattern)
            {
                case RecurrencePattern.Weekly:
                    return MatchesRecurringDay(holiday.RecurringDay, accountingDay);
                case RecurrencePattern.Monthly:
                    return accountingDay.Day == holiday.DayOfMonth;
                case RecurrencePattern.Yearly:
                    return accountingDay.Month == holiday.Month && accountingDay.Day == holiday.DayOfMonth;
                case RecurrencePattern.Daily:
                    return true;
                default:
                    return false;
            }
        }

        private bool MatchesRecurringDay(string? recurringDays, DateTime accountingDay)
        {
            if (string.IsNullOrWhiteSpace(recurringDays))
                return false;

            return recurringDays.Split(',')
                .Select(day => Enum.TryParse(day.Trim(), out DayOfWeek parsedDay) ? parsedDay : (DayOfWeek?)null)
                .Where(day => day.HasValue)
                .Contains(accountingDay.DayOfWeek);
        }
    }



}
