using CBS.AccountManagement.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountingRuleEntry : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public bool IsPrimaryEntry { get; set; }
        public string EventCode { get; set; }
        public string  AccountingRuleEntryName { get; set; }
        public string BookingDirection { get; set; }
        public string ? OperationEventAttributeId { get; set; }
        public string?  ProductAccountingBookId { get; set; }
        public string ?  DeterminationAccountId { get; set; }
        public bool? HasManagementAccountId { get; set; }
        public string ? BalancingAccountId { get; set; }
        public string BankId { get; set; }

        public string Description { get; set; } = string.Empty;
        public virtual OperationEventAttributes? OperationEventAttribute { get; set; }
        public virtual ProductAccountingBook? ProductAccountingBook { get; set; }
        public virtual Data.ChartOfAccountManagementPosition? DeterminationAccount { get; set; }
        public virtual  Data.ChartOfAccountManagementPosition? BalancingAccount { get; set; }

        public bool IsLiaison()
        {
            return this.EventCode.Contains("Liasson_Account");
        }
    }
}
