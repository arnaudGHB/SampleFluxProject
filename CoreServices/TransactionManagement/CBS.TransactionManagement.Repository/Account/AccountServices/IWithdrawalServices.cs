using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data.Entity.RemittanceP;
using CBS.TransactionManagement.Dto;
using MediatR;

namespace CBS.TransactionManagement.Repository.AccountServices
{
    public interface IWithdrawalServices
    {
        IMediator _mediator { get; set; }
        Task<TransactionDto> Withdrawal(Teller teller, Account tellerAccount, BulkDeposit request, bool isInterBanchOperation, string sourceBranchId, string destinationBranchId, decimal customCharge, string Reference, Account customerAccount,Config config,bool IsOtherCashInOperation, DateTime accountingDate,bool isRemittance,Remittance remittance);
    }
}