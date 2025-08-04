using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Commands;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Services.NormalCashOutP
{
    public interface INormalCashoutServices
    {
        Task<ServiceResponse<PaymentReceiptDto>> Cashout(AddBulkOperationWithdrawalCommand requests, DateTime accountingDate, Config config);
    }
}