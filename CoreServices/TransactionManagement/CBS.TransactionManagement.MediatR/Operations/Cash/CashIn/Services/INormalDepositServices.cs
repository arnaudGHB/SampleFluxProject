using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;
using MediatR;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services
{
    public interface INormalDepositServices
    {
        IMediator _mediator { get; set; }

        Task<ServiceResponse<PaymentReceiptDto>> MakeDeposit(AddBulkOperationDepositCommand requests, DateTime accountingDate, Config config);
    }
}