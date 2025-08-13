using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository
{
   
    public class TransactionRepository : GenericRepository<Transaction, TransactionContext>, ITransactionRepository
    {
        public TransactionRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {
            
        }
        public void AddTransaction(TransactionDto transactionDto)
        {
            var transaction = MapToTransaction(transactionDto);
            Add(transaction);
            
        }
        private Transaction MapToTransaction(TransactionDto dto)
        {
            return new Transaction
            {
                Id = dto.Id,
                Amount = dto.Amount,
                OriginalDepositAmount = dto.OriginalDepositAmount,
                ExternalReference = dto.ExternalReference,
                IsExternalOperation = dto.IsExternalOperation,
                ExternalApplicationName = dto.ExternalApplicationName,
                Currency = dto.Currency,
                Debit = dto.Debit,
                Credit = dto.Credit,
                AccountId = dto.AccountId,
                AccountNumber = dto.AccountNumber,
                TransactionType = dto.TransactionType,
                OperationType = dto.OperationType,
                Status = dto.Status,
                TransactionReference = dto.TransactionReference,
                Tax = dto.Tax,
                Operation = dto.Operation,
                PreviousBalance = dto.PreviousBalance,
                SourceBranchCommission = dto.SourceBranchCommission,
                DestinationBranchCommission = dto.DestinationBranchCommission,
                Note = dto.Note,
                SenderAccountId = dto.SenderAccountId,
                ReceiverAccountId = dto.ReceiverAccountId,
                DepositorIDNumber = dto.DepositorIDNumber,
                DepositerTelephone = dto.DepositerTelephone,
                IsDepositDoneByAccountOwner = dto.IsDepositDoneByAccountOwner,
                DepositorName = dto.DepositorName,
                DepositorIDIssueDate = dto.DepositorIDIssueDate,
                DepositorIDExpiryDate = dto.DepositorIDExpiryDate,
                DepositorIDNumberPlaceOfIssue = dto.DepositorIDNumberPlaceOfIssue,
                DepositerNote = dto.DepositerNote,
                IsInterBrachOperation = dto.IsInterBrachOperation,
                SourceBrachId = dto.SourceBrachId,
                DestinationBrachId = dto.DestinationBrachId,
                Balance = dto.Balance,
                ProductId = dto.ProductId,
                Fee = dto.Fee,
                WithrawalFormCharge = dto.WithrawalFormCharge,
                OperationCharge = dto.OperationCharge,
                WithdrawalChargeWithoutNotification = dto.WithdrawalChargeWithoutNotification,
                CloseOfAccountCharge = dto.CloseOfAccountCharge,
                FeeType = dto.FeeType,
                SourceType = dto.SourceType,
                BankId = dto.BankId,
                CustomerId = dto.CustomerId,
                BranchId = dto.BranchId,
                TellerId = dto.TellerId,
                ReceiptTitle = dto.ReceiptTitle, 
              
            };
        }
    }
}
