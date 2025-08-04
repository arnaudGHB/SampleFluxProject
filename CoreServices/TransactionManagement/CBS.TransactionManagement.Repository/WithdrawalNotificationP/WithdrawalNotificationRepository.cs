using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.WithdrawalNotificationP
{
   
    public class WithdrawalNotificationRepository : GenericRepository<WithdrawalNotification, TransactionContext>, IWithdrawalNotificationRepository
    {
        private readonly ILogger<WithdrawalNotificationRepository> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        public WithdrawalNotificationRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<WithdrawalNotificationRepository> logger, UserInfoToken userInfoToken) : base(unitOfWork)
        {
            _logger=logger;
            _userInfoToken=userInfoToken;
        }
        public async Task<WithdrawalNotification> GetWithdrawalNotification(string customerId, string accountNumber, decimal amount)
        {
            // Retrieve withdrawal notifications for the specified customer and account number, filtering those that are paid and within the intended withdrawal and grace period dates
            var notifications = await FindBy(x => x.CustomerId == customerId && x.AccountNumber == accountNumber && x.DateOfIntendedWithdrawal.Date <= DateTime.Now.Date && x.GracePeriodDate.Date >= DateTime.Now.Date && x.IsWithdrawalDone == false).ToListAsync();

            // Initialize an empty withdrawal notification object
            var validNotification = new WithdrawalNotification();

            // Get today's date
            DateTime today = DateTime.Today;

            // Iterate through the list of notifications
            foreach (var notification in notifications)
            {
                // Check if today's date is between DateOfIntendedWithdrawal and GracePeriodDate and if the notification is not expired and is paid
                if (today >= notification.DateOfIntendedWithdrawal && today <= notification.GracePeriodDate && !notification.IsExpired)
                {
                    validNotification = notification; // Set the valid notification
                    break; // Exit the loop as we found a valid notification
                }
            }

            // Check if a valid notification was found
            if (validNotification.Id != null)
            {
                // Check if the required amount matches the entered amount
                if (validNotification.AmountRequired != amount)
                {
                    var errorMessage = $"Amount to withdraw from saving must be equal to amount entered from the form. Requested amount: {BaseUtilities.FormatCurrency(validNotification.AmountRequired)}";
                    _logger.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw an exception with the error message
                }
                if (validNotification.ApprovalStatus == Status.Pending.ToString())
                {
                    var errorMessage = $"Notification of {BaseUtilities.FormatCurrency(validNotification.AmountRequired)} is still pending validation. Please contact member service for validation";
                    _logger.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw an exception with the error message

                }
                if (!validNotification.IsNotificationPaid)
                {
                    var errorMessage = $"The withdrawal notification of {BaseUtilities.FormatCurrency(validNotification.AmountRequired)} is still pending payment. Please contact the cash desk to complete the payment.";
                    _logger.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw an exception with the error message
                }

                if (validNotification.GracePeriodDate.Date == BaseUtilities.UtcNowToDoualaTime().Date)
                {
                    if (!validNotification.IsNotificationPaid)
                    {
                        var errorMessage = $"Notification of {BaseUtilities.FormatCurrency(validNotification.AmountRequired)} must be paid. Click on pay withdrawal notification charges to pay this notification.";
                        _logger.LogError(errorMessage); // Log error message
                        throw new InvalidOperationException(errorMessage); // Throw an exception with the error message

                    }
                }
                return validNotification; // Return the valid notification
            }

            return null; // Return null if no valid notification is found within the specified period
        }
        public void UpdateWithdrawalNotification(WithdrawalNotification withdrawalNotification, Transaction transaction, Teller teller, DateTime accountingDate)
        {

            withdrawalNotification.IsWithdrawalDone = true;
            withdrawalNotification.DateWithdrawalWasDone = transaction.CreatedDate;
            withdrawalNotification.TellerId = transaction.TellerId;
            withdrawalNotification.TellerCaise = teller.Name;
            withdrawalNotification.TellerName = _userInfoToken.FullName;
            withdrawalNotification.IsNotificationPaid = true;
            withdrawalNotification.DateWithdrawalWasDone = accountingDate;
            withdrawalNotification.ServiceClearName = _userInfoToken.FullName;
            withdrawalNotification.TransactionReference = transaction.TransactionReference;
            Update(withdrawalNotification);
        }
    }
}
