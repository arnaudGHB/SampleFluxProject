using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.WithdrawalNotificationP
{
    public interface IWithdrawalNotificationRepository : IGenericRepository<WithdrawalNotification>
    {
        Task<WithdrawalNotification> GetWithdrawalNotification(string customerId, string accountNumber, decimal amount);
        void UpdateWithdrawalNotification(WithdrawalNotification withdrawalNotification, Transaction transaction, Teller teller, DateTime accountingDate);
    }
}
