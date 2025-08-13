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

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands
{
    public class AddBulkOperationDepositCommand : IRequest<ServiceResponse<PaymentReceiptDto>>
    {
        public List<BulkOperation> BulkOperations { get; set; }
        public string DepositType { get; set; }//Normal, LoanFeePayment, Disbursment, LoanRepayment
        public string? Period { get; set; }
        public string OperationType { get; set; }
        public bool IsCashOperation { get; set; }
        public string? Id { get; set; }
        public bool IsPartialRequest { get; set; }
        public AddBulkOperationDepositCommand()
        {
            DepositType = "Normal";
        }
    }
}
