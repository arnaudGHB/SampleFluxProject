using CBS.TransactionManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{

    public class AmountCollection
    {
        public string EventAttributeName { get; set; }
        public string LiaisonEventName { get; set; }
        public decimal Amount { get; set; }  // Use decimal for monetary values
        public bool IsPrincipal { get; set; }
        public string Naration { get; set; }
        public bool IsInterBankOperationPrincipalCommission { get; set; }
        public bool IsInterBankOperationCommission { get; set; }
        public bool HasPaidCommissionByCash { get; set; }
        public AmountCollection()
        {
            LiaisonEventName = OperationEventRubbriqueName.Liasson_Account.ToString();
            HasPaidCommissionByCash = true;
        }
    }
  
    public class AmountEventCollection
    {
        public string EventCode { get; set; }
        public decimal Amount { get; set; }
        public string Naration { get; set; }
    }




    public class TransferAmountCollection
    {
        public string EventAttributeName { get; set; }
        public string LiaisonEventName { get; set; }
        public decimal Amount { get; set; }
        public bool IsPrincipal { get; set; }
        public string DirectionOfTransfer { get; set; }
        public TransferAmountCollection()
        {
            DirectionOfTransfer = "None";
            LiaisonEventName = "None";
        }

    }
}
