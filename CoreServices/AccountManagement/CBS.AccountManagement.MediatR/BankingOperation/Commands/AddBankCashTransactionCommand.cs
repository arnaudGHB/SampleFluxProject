using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class AddBankCashOutTransactionCommand : IRequest<ServiceResponse<bool>>
    {
        
              public string Id { get; set; }
        public string FromAccountId { get; set; }
        public string Balance { get; set; }
        public string ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public CurrencyNotesRequest CurrencyNotesRequest { get; set; }
        public string ReferenceId { get; set; }
        public string TransactionType { get; set; }
        public string BankTransactionReference { get; set; }
        public string Description { get; set; }
        public string FileUpload { get; set; }
        public string ValueDate { get; set; }

        public BankTransaction SetBankTransaction(AddBankCashOutTransactionCommand command, string banktransactionId)
        {
            BankTransaction model = new BankTransaction();
            model.TransactionType = command.TransactionType;
            model.Id = banktransactionId;
            model.FromAccountId = command.FromAccountId;
            model.ToAccountId = command.ToAccountId;
            model.Balance = command.Balance;
            model.Amount= command.Amount;
            model.ReferenceId = command.ReferenceId;
            model.BankTransactionReference = command.BankTransactionReference;
            model.Description = command.Description;
            model.FileUpload = command.FileUpload;
            model.ValueDate = command.ValueDate;
            return model;
        }

        public string getTransactionNaration(string fromAccount, string toAccount)
        {
            return $"BankCashOut Operation ref:{this.ReferenceId} amount :{this.Amount} form {fromAccount} to {toAccount}";
        }
    }
    public class AddBankCashInForTransactionCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public string Balance { get; set; }
        public decimal Amount { get; set; }
        public string ReferenceId { get; set; }

        public string TransactionType { get; set; }
        public CurrencyNotesRequest Notes { get; set; }
        public string FromAccountId { get;  set; }
        public string ToAccountId { get;  set; }

        public BankTransaction SetBankTransaction(AddBankCashInForTransactionCommand command, string banktransactionId)
        {
            BankTransaction model = new BankTransaction();
            model.TransactionType = command.TransactionType;
            model.Id = banktransactionId;
            model.FromAccountId = command.ToAccountId;
            model.Balance = command.Balance;
            model.Amount = command.Amount;
            model.ReferenceId = command.ReferenceId;
 
            return model;
        }
    }

 
}
