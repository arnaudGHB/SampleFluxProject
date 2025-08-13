using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Data.Entity.Receipts.Payments;

namespace CBS.TransactionManagement.Repository.Receipts.Payments
{
    public interface IPaymentReceiptRepository : IGenericRepository<PaymentReceipt>
    {
        PaymentReceipt ProcessPaymentAsync(PaymentProcessingRequest request);
    }
}
