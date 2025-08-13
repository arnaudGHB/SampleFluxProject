using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.NewFolder
{
    public interface ILoanProcessingFeeServices
    {
        Task<ServiceResponse<PaymentReceiptDto>> LoanProcessingFeePayment(AddBulkOperationDepositCommand request, DateTime accountingDate, Config config);
    }
}