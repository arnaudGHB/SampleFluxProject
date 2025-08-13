using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Commands;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Services.RemittanceP
{
    public interface IRemittanceCashoutServices
    {
        Task<ServiceResponse<PaymentReceiptDto>> RemittanceCashout(AddBulkOperationWithdrawalCommand requests, DateTime accountingDate, Config config);
    }
}