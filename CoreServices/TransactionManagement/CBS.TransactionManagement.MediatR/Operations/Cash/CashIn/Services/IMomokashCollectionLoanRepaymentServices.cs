using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.NewFolder
{
    public interface IMomokashCollectionLoanRepaymentServices
    {
        Task<ServiceResponse<PaymentReceiptDto>> LoanRepaymentMomokashCollection(AddBulkOperationDepositCommand requests, DateTime accountingDate);
    }
}