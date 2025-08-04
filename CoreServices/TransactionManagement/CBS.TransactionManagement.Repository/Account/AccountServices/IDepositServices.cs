using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data.Entity.RemittanceP;
using CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP;
using CBS.TransactionManagement.Dto;
using MediatR;

namespace CBS.TransactionManagement.Repository.AccountServices
{
    public interface IDepositServices
    {
        IMediator _mediator { get; set; }
        Task<TransactionDto> DepositWithdrawalFormFee(Teller teller, Account tellerAccount, WithdrawalNotification withdrawal, List<CurrencyNotesDto> currencyNotes, bool IsInterBranchOperation, string sourceBranchId, string destinationBranchId, string Reference, Account customerAccount,string Name, bool IsMomocashCollection, bool isChargeIclussive, DateTime accountingDate);
        Task<TransactionDto> Deposit(Teller teller, Account tellerAccount, BulkDeposit request, bool isInterBanchOperation, string sourceBranchId, string destinationBranchId, decimal customCharge, string Reference, Account customerAccount, Config config,string Name, bool IsMomocashCollection, DateTime accountingDate, bool isRemittance, Remittance remittance);
    }
}