using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;
using MediatR;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.NewFolder
{
    public interface ILoanRepaymentOperationServices
    {
        IMediator _mediator { get; set; }

        Task<ServiceResponse<PaymentReceiptDto>> LoanRepayment(AddBulkOperationDepositCommand requests, DateTime accountingDate, Config config);
    }
}