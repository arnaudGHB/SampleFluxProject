using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Commands
{
    public class AddBulkOperationWithdrawalCommand : IRequest<ServiceResponse<PaymentReceiptDto>>
    {
        public List<BulkOperation> BulkOperations { get; set; }
        public string OperationType { get; set; }
        public bool IsCashOperation { get; set; }
        public string? Id { get; set; }
        public string? ReceiverCNI { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverCNIDateOfIssue { get; set; }
        public string? ReceiverCNIDateOfExpiration { get; set; }
        public string? ReceiverCNIPlcaceOfIssue { get; set; }
        public string? OTP { get; set; }
        public string? SenderName { get; set; }
        public string? SenderPhoneNumber { get; set; }
        public string? ReceiverPhoneNumber { get; set; }
        public string? SenderSecretCode { get; set; }
        public string? SenderAddress { get; set; }
        public string? ReceiverAddress { get; set; }
        public decimal RemittanceAmount { get; set; }
        public DateTime? RemittanceDate { get; set; }

    }
}
