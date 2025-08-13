using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository
{

    public class DailyTellerRepository : GenericRepository<DailyTeller, TransactionContext>, IDailyTellerRepository
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<DailyTellerRepository> _logger; // Logger for logging handler actions and errors.
        private readonly ITellerRepository _tellerRepository;
        public DailyTellerRepository(IUnitOfWork<TransactionContext> unitOfWork, UserInfoToken userInfoToken = null, ILogger<DailyTellerRepository> logger = null, ITellerRepository tellerRepository = null) : base(unitOfWork)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _tellerRepository = tellerRepository;
        }
        // Method to check teller provision for current user
        public async Task<DailyTeller> GetAnActivePrimaryTellerForTheDate()
        {
            // Find the first active primary teller for the current user with active teller status
            var dailyTeller = await FindBy(t => t.UserId == _userInfoToken.Id && t.Status && t.IsDeleted == false && t.IsPrimary && t.BranchId==_userInfoToken.BranchID)
                                                                    .Include(x => x.Teller)
                                                                    .Where(t => t.Teller.ActiveStatus && t.IsPrimary) // Ensure teller's ActiveStatus is true
                                                                    .FirstOrDefaultAsync();

            // Check if an active primary teller exists
            if (dailyTeller == null)
            {
                var errorMessage = "Today, your account does not have teller privileges. Please contact your primary teller to request authorization for teller access.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            return dailyTeller; // Return the active primary teller.
        }

        public async Task<DailyTeller> GetAnActiveSubTellerForTheDate()
        {
            // Fetch the sub-teller record for the current user
            var dailyTeller = await FindBy(t => t.UserId == _userInfoToken.Id && t.IsPrimary == false && t.IsDeleted == false)
                                    .Include(x => x.Teller)
                                    .FirstOrDefaultAsync();

            // Check if the user is designated as a sub-teller
            if (dailyTeller == null)
            {
                var errorMessage = "You are not currently assigned as a sub-teller. Please reach out to your primary teller or the administrator to request the necessary permissions.";
                _logger.LogError(errorMessage); // Log the error
                throw new InvalidOperationException(errorMessage); // Throw exception with user-friendly message
            }
            // Check if the sub-teller is active for the current day
            else if (!dailyTeller.Status)
            {
                var errorMessage = "Your sub-teller account has been deactivated for today’s operations. Please contact your primary teller or the branch supervisor for assistance.";
                _logger.LogWarning(errorMessage); // Log the warning
                throw new InvalidOperationException(errorMessage); // Throw exception with user-friendly message
            }
            // Check if the teller's till is active
            else if (!dailyTeller.Teller.ActiveStatus)
            {
                var errorMessage = "Your assigned teller's till is currently inactive. Please consult with the branch supervisor or system administrator to resolve this issue.";
                _logger.LogWarning(errorMessage); // Log the warning
                throw new InvalidOperationException(errorMessage); // Throw exception with user-friendly message
            }

            return dailyTeller; // Return the active sub-teller object if all checks pass
        }

        public async Task<DailyTeller> GetAnActiveSubTellerForTheDate3PP(string tellerid)
        {
            var dailyTeller = await FindBy(t => t.TellerId == tellerid && t.Status && t.IsPrimary == false && t.IsDeleted == false).Include(x => x.Teller)
                                                                    .Where(t => t.Teller.ActiveStatus) // Ensure teller's ActiveStatus is true
                                                                    .FirstOrDefaultAsync();

            if (dailyTeller == null)
            {
                var errorMessage = $"Your account is not currently designated as a sub-teller. Please contact your primary teller to obtain the necessary permissions.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            return dailyTeller; // Return teller provision.
        }
        // Method to check teller provision for current user
        public async Task<DailyTeller> GetAnActiveTellerForTheDate(string tellerId)
        {
            var dailyTeller = await FindBy(t => t.TellerId == tellerId).Include(x => x.Teller)
                                                                    .Where(t => t.Teller.ActiveStatus) // Ensure teller's ActiveStatus is true
                                                                    .FirstOrDefaultAsync();


            // Check if teller provision exists
            if (dailyTeller == null)
            {
                var errorMessage = $"Teller not programed for today. Contact your primary teller to add said teller in operation.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Check if teller provision exists
            if (!dailyTeller.Status)
            {
                var errorMessage = $"Teller is not active. Contact your primary teller to activate teller.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            return dailyTeller; // Return teller provision.
        }

        public async Task<DailyTeller> GetThePrimaryTellerOfTheDay(string tellerId)
        {
            var dailyTeller = await FindBy(t => t.TellerId == tellerId &&
                                                 t.IsPrimary).Include(x => x.Teller)
                                                                    .Where(t => t.Teller.ActiveStatus) // Ensure teller's ActiveStatus is true
                                                                    .FirstOrDefaultAsync();


            // Check if teller provision exists
            if (dailyTeller == null)
            {
                var errorMessage = $"NO primary teller is configured for your branch.. Contact your administrator.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            return dailyTeller; // Return teller provision.
        }



    }
}
