using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.RemittanceP
{
    public interface IRemittanceCashInServices
    {
        Task<ServiceResponse<PaymentReceiptDto>> RemittanceCashIn(AddBulkOperationDepositCommand requests, DateTime accountingDate, Config config);
    }
}