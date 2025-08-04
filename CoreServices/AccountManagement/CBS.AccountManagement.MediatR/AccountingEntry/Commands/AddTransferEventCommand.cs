using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public  class AddTransferEventCommand : IRequest<ServiceResponse<bool>> 
    {

        public string FromProductId { get; set; }
        public string ToProductId { get; set; }
        public string? FromMemberReference { get; set; }

        public string TransactionReferenceId { get; set; }
        public bool IsInterBranchTransaction { get; set; }
        public string ExternalBranchId { get; set; }
        public string ExternalBranchCode { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<TransferAmountCollection> AmountCollection { get; set; }
 


        public decimal GetPrincipalAmount()
        {
            return this.AmountCollection.Where(x=>x.IsPrincipal==true).FirstOrDefault().Amount;
        }
        public string? GetOperationEventCode(string productId)
        {
            return productId + "@Principal_Saving_Account";
        }

        public class TransferAmountCollection
        {
            public string EventAttributeName { get; set; }
            public string LiaisonEventName { get; set; }

            public decimal Amount { get; set; }
            public bool IsPrincipal { get; set; }
            public string WhichSourceAccountPaysTheCharges { get; set; }
            public string Naration { get; set; }
            public TransferAmountCollection()
            {
                WhichSourceAccountPaysTheCharges = "None";
            }
            public bool IsNone()
            {
                return this.WhichSourceAccountPaysTheCharges == "None";
            }
            public bool IsSource()
            {
                return this.WhichSourceAccountPaysTheCharges == "Source";
            }
            public bool IsDestination()
            {
                return this.WhichSourceAccountPaysTheCharges == "Destination";
            }
            public string? GetOperationEventCode(string fromProductName)
            {
                return fromProductName + "@" + this.EventAttributeName;
            }

          
            public string? GetLiaisonEventCode(string fromProductName)
            {
                return fromProductName + "@" + "Liasson_Account";
            }
        }
    }
}
