using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;

namespace CBS.AccountManagement.MediatR
{
    public class Booking
    {
        public decimal Amount { get; set; }
        public bool IsAuxilaryTransaction { get; private set; }
        public string ExternalBranchId { get; private set; }
        public decimal AmountDifference { get;   set; }
        public string OperationType { get; set; }
        public string MemberReference { get; set; }
        public string TransactionReferenceId { get;   set; }
        public string Naration { get; set; }
        public string FromAccountNumber { get; set; }
        public string FromAccountName { get; set; }
        public string ToAccountNumber { get; set; }
        public string ToAccountName { get; set; }

      
        public Booking(decimal Amount, decimal AmountDifference, string TransactionReferenceId,string Naration,string OperationType,string branchId)
        {
            this.OperationType = OperationType; 
            this.Amount = Amount;
            this.AmountDifference = AmountDifference;
            this.TransactionReferenceId = TransactionReferenceId;
            this.Naration = Naration;
            this.ExternalBranchId = branchId;
   
        }
        public Booking(string naration, string MemberReference, string TransactionReferenceId, AccountOperationType operationType, Data.Account fromAccount, Data.Account toAccount, decimal Amount,string branchId,bool isAuxilaryTransaction = false, bool isInterBranchTransaction = false, string ExternalBranchId = "not set")
        {
            this.FromAccountNumber = fromAccount.AccountNumber;
            this.FromAccountName = fromAccount.AccountName;
            this.ToAccountNumber = toAccount.AccountNumber;
            this.ToAccountName = toAccount.AccountName;
            this.TransactionReferenceId = TransactionReferenceId;
            this.Amount = Amount;
            this.IsAuxilaryTransaction = isAuxilaryTransaction;
            this.ExternalBranchId = isAuxilaryTransaction ? ExternalBranchId:branchId ;
            this.OperationType = operationType.ToString().ToUpper();
            this.MemberReference = MemberReference;
            this.Naration = naration;
            this.MemberReference = MemberReference;

        }
   
    }
    public class CashMovementAccount
    {
        public string BookingDirection { get; set; }
        public Data.Account SourceAccount { get; set; }
        public Data.Account DestinationAccount { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountDifference { get; set; }
        public string Nature { get; set; }
    }
}