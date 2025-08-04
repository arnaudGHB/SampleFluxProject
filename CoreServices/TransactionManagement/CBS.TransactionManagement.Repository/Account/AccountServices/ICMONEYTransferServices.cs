using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Repository.AccountServices
{
    public interface ICMONEYTransferServices
    {
        IMediator _mediator { get; set; }
        Task<TransferCharges> CalculateTransferCharges(decimal amount, string productid, FeeOperationType feeOperationType);
        Task<TransactionDto> TransferCMONEY(Teller teller, Account tellerAccount, TransferTTP request, string Reference, Account senderAccount, Account receiverAccount, CustomerDto senderCustomer, CustomerDto receiverCustomer, string sourceType, string TPPtransferType);
    }
}