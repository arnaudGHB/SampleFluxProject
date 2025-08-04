using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.Receipts.Payments
{
    public class PaymentProcessingRequest
    {
        public List<TransactionDto> Transactions { get; set; }
        public List<PaymentDetailObject> PaymentDetails { get; set; }
        public CurrencyNotesRequest NotesRequest { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalCharges { get; set; }
        public DateTime AccountingDay { get; set; }
        public string PortalUsed { get; set; }
        public string OperationTypeGrouping { get; set; }
        public string OperationType { get; set; }
        public string MemberName { get; set; }
        public string ServiceType { get; set; }
        public string SourceOfRequest { get; set; }
    }

}
