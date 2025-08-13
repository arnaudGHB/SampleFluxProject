using CBS.TransactionManagement.Data.Entity.Receipts.Details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.Receipts.Payments
{
    public class PaymentReceipt:BaseEntity
    {
        public string Id { get; set; }
        public string? MemberName { get; set; }
        public string? MemberReference { get; set; }
        public decimal Amount { get; set; }
        public decimal Charges { get; set; }
        public decimal TotalAmount { get; set; }
        public string? DepositorName { get; set; }
        public string? DepositorPhone { get; set; }
        public string? DepositorCNI { get; set; }
        public string? AmountInWord { get; set; }
        public string? ReceiptTitle { get; set; }
        public string? CashierName { get; set; }
        public string? TillName { get; set; }
        public string? TellerId { get; set; }
        public string? ServiceType { get; set; }//Loan_Disburstment, Loan_Repayment, Cash_In, Cash_Out, Transfer, Other_CashIn, Other_Payments, Momo_Cash_Collection
        public string? OperationType { get; set; }//Cash_Operation, None_Cash_Operation
        public string? OperationTypeGrouping { get; set; }//Cash_In, Cash_Out, Transfer, Others
        public DateTime AccountingDay { get; set; }
        public DateTime Date { get; set; }
        public string? InternalReferenceNumber { get; set; }
        public string? ExternalReferenceNumber { get; set; }
        public string SourceOfRequest { get; set; }//Daily_Collection_Service, GAV, C_Money, Teller, BackOffice
        public string PortalUsed { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public int Note10000 { get; set; } = 0;
        public int Note5000 { get; set; } = 0;
        public int Note2000 { get; set; } = 0;
        public int Note1000 { get; set; } = 0;
        public int Note500 { get; set; } = 0;
        public int Coin500 { get; set; } = 0;
        public int Coin100 { get; set; } = 0;
        public int Coin50 { get; set; } = 0;
        public int Coin25 { get; set; } = 0;
        public int Coin10 { get; set; } = 0;
        public int Coin5 { get; set; } = 0;
        public int Coin1 { get; set; } = 0;
        public virtual ICollection<PaymentDetail> PaymentDetails  { get; set; }
    }

}
