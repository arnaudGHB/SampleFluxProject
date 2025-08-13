using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CBS.TransactionManagement.Data
{
    public class Transaction : BaseEntity
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public decimal OriginalDepositAmount { get; set; }
        public string? ExternalReference { get; set; }
        public bool IsExternalOperation { get; set; }
        public string? ExternalApplicationName { get; set; }
        public string? Currency { get; set; }
        public decimal Debit { get; set; }
        public string? AccountType { get; set; }
        public decimal Credit { get; set; }
        public string AccountId { get; set; }
        public string? AccountNumber { get; set; }
        public string TransactionType { get; set; }
        public string OperationType { get; set; }//Debit or Credit
        public string Status { get; set; } = "PENDING";
        public string TransactionReference { get; set; }
        public decimal Tax { get; set; }
        public string Operation { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal SourceBranchCommission { get; set; }
        public decimal HeadOfficeCommission { get; set; }
        public decimal CamCCULCommission { get; set; }
        public decimal FluxAndPTMCommission { get; set; }

        public DateTime AccountingDate { get; set; }
        public decimal DestinationBranchCommission { get; set; }
        public string Note { get; set; }
        public string? SenderAccountId { get; set; }
        public string? ReceiverAccountId { get; set; }
        public string? DepositorIDNumber { get; set; }
        public string? DepositerTelephone { get; set; }
        public bool IsDepositDoneByAccountOwner { get; set; }
        public string? DepositorName { get; set; }
        public string? DepositorIDIssueDate { get; set; }
        public string? DepositorIDExpiryDate { get; set; }
        public string? DepositorIDNumberPlaceOfIssue { get; set; }
        public string? DepositerNote { get; set; }
        public bool IsInterBrachOperation { get; set; }
        public string SourceBrachId { get; set; }
        public string DestinationBrachId { get; set; }
        public decimal Balance { get; set; }
        public string ProductId { get; set; }
        public decimal Fee { get; set; }
        public decimal WithrawalFormCharge { get; set; }
        public decimal OperationCharge { get; set; }
        public decimal WithdrawalChargeWithoutNotification { get; set; }
        public decimal CloseOfAccountCharge { get; set; }
        public string FeeType { get; set; }
        public string? SourceType { get; set; }
        public string BankId { get; set; }
        public string? CustomerId { get; set; }
        public string BranchId { get; set; }
        public string TellerId { get; set; }
        public string? ReceiptTitle { get; set; }
        public bool IsSWS { get; set; }
        public string? CheckNumber { get; set; }
        public string? CheckName { get; set; }

        public virtual ICollection<TellerOperation> TellerOperations { get; set; }
        public virtual Teller Teller { get; set; }
        public virtual Account Account { get; set; }

    }
}