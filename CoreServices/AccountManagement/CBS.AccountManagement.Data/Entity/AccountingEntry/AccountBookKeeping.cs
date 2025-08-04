using CBS.AccountManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountBookKeeping : BaseEntity
    {
        public string Id { get; set; }
        public string? Name { get; set; } // Key={"Principal","Revenus","Charges","TAX","Penalty","EntryFee"}
        public bool? IsDebit { get; set; } //true if debit, false if credit
        public string? ChartOfAccountId { get; set; } //ChartOfAccountId 
        public string? AccountTypeId { get; set; }
        public string? OperationEventAttributeId { get; set; }
        public string? EventCode { get; set; }
        public bool? IsPrincipal { get; set; }
        public virtual OperationEventAttributes? OperationEventAttribute { get; set; }
        public virtual CashMovementTracker? ChartOfAccount { get; set; }
        public virtual AccountType? AccountType { get; set; }
    }
}
