using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.HolyDayP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.HolyDayRecurringP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Repository.HolyDayP
{


    public class HolyDayRepository : GenericRepository<HolyDay, TransactionContext>, IHolyDayRepository
    {
        private readonly ILogger<HolyDayRepository> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IUnitOfWork<TransactionContext> _unitOfWork;
        private readonly IHolyDayRecurringRepository _holyDayRecurringRepository;

        public HolyDayRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<HolyDayRepository> logger = null, UserInfoToken userInfoToken = null, IHolyDayRecurringRepository holyDayRecurringRepository = null) : base(unitOfWork)
        {
            _logger = logger;
            _userInfoToken = userInfoToken;
            _unitOfWork = unitOfWork;
            _holyDayRecurringRepository = holyDayRecurringRepository;
        }

        /// <summary>
        /// Checks if a given date is a holiday, including recurring holidays like weekends and branch-specific holidays.
        /// Defaults to a normal operation day if no holiday configuration exists or no matching holiday is found.
        /// </summary>
        /// <param name="accountingDay">The date to check.</param>
        /// <returns>True if it's a holiday, false otherwise.</returns>
        public async Task<bool> IsHoliday(DateTime accountingDay)
        {
            try
            {
                // Step 1: Check if the day is a fixed holiday (from HolyDay entity)
                var holiday = await All.AsNoTracking()
                    .Where(h => accountingDay >= h.DateFrom && accountingDay <= h.DateTo && h.IsActive &&
                                (h.IsCentralisedConfiguration || h.BranchId == _userInfoToken.BranchID))
                    .FirstOrDefaultAsync();

                // If a matching holiday is found, return true
                if (holiday != null)
                {
                    return true;
                }

                // Step 2: Handle recurring holidays like weekends or configured lockdowns
                var recurringHolidayExists = await IsRecurringHoliday(accountingDay);
                if (recurringHolidayExists)
                {
                    return true;
                }

                // If no holiday is found and no error occurs, return false (normal operation day)
                return false;
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions with additional details
                string unexpectedErrorMessage = $"Unexpected error while checking holiday status for branch {_userInfoToken.BankCode} " +
                                                $"on {accountingDay:yyyy-MM-dd}.";
                _logger.LogError(ex, unexpectedErrorMessage);
                throw new InvalidOperationException(unexpectedErrorMessage, ex);
            }
        }

        /// <summary>
        /// Checks if the given date is a recurring holiday (weekends, lockdowns, etc.) based on the admin configuration.
        /// </summary>
        /// <param name="accountingDay">The date to check.</param>
        /// <returns>True if it's a recurring holiday, false otherwise.</returns>
        private async Task<bool> IsRecurringHoliday(DateTime accountingDay)
        {
            // Fetch the recurring holiday configurations (weekends, recurring lockdowns, etc.)
            var  status = await _holyDayRecurringRepository.IsRecurringHoliday(accountingDay);
            return status;
        }
    }



}
