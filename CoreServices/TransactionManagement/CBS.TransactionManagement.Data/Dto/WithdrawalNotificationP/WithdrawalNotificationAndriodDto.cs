using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP
{
    public class WithdrawalNotificationAndriodDto
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public DateTime NotificationDate { get; set; }
        public DateTime DateOfIntendedWithdrawal { get; set; }
        public DateTime GracePeriodDate { get; set; }
        public decimal AmountRequired { get; set; }
        public decimal Charge { get; set; }
        public bool IsExpired { get; set; }
        public bool IsGracePeriod { get; set; }
        public string Status { get; set; }
    }
}
