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

namespace CBS.TransactionManagement.MediatR.Commands
{
    public class AddBulkOperationWithdrawalCommand : IRequest<ServiceResponse<PaymentReceiptDto>>
    {
        public List<BulkOperation> BulkOperations { get; set; }
        public string OperationType { get; set; }
        public bool IsCashOperation { get; set; }
        public string? Id { get; set; }

    }
}
