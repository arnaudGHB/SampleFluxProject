using CBS.TransactionManagement.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.MongoDBObjects
{

    public class AccountToAccountTransfer
    {
        public string Id { get; set; }
        public List<MakeAccountPostingCommand> MakeAccountPostingCommands { get; set; }
        public string MemberName { get; set; }
        public string MemberReference { get; set; }
        public string TransactionType { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string BulkExecutionCode { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ExternalBranchName { get; set; }
        public string Narration { get; set; }
        public string ExternalBranchcode { get; set; }
        public string TransactionReferenceId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Charges { get; set; }
        public string UserFullName { get; set; }
        public UserInfoToken UserInfoToken { get; set; }
        public string? BranchCode { get; set; }
        public LoanRefundCollectionAlpha LoanRefundCollectionAlpha { get; set; }
        public List<LoanRefundCollection> LoanRefundCollections { get; set; }

        public AccountToAccountTransfer()
        {
            MakeAccountPostingCommands=new List<MakeAccountPostingCommand>();
            UserInfoToken=new UserInfoToken();
            LoanRefundCollections=new List<LoanRefundCollection>();
            LoanRefundCollectionAlpha=new LoanRefundCollectionAlpha();
        }
    }
    public class MakeAccountPostingCommand
    {
        public string FromAccountNumber { get; set; }
        public string ToAccountNumber { get; set; }
        public string? MemberReference { get; set; }
        public string ToProductId { get; set; }
        public string ToProductName { get; set; }
        public string OperationType { get; set; }
        public string ExternalBranchCode { get; set; }
        public string ExternalBranchId { get; set; }
        public bool IsInterBranchTransaction { get; set; }
        public string FromProductId { get; set; }
        public string FromProductName { get; set; }
        public string ProductType { get; set; }
        public string Naration { get; set; }
        public List<AmountCollection>? AmountCollection { get; set; }
        public List<AmountEventCollection>? AmountEventCollections { get; set; }
        public LoanRefundCollectionAlpha LoanRefundCollectionAlpha { get; set; }
        public List<LoanRefundCollection> LoanRefundCollections { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionReferenceId { get; set; }
        public MakeAccountPostingCommand()
        {
            AmountCollection=new List<AmountCollection>();
            AmountEventCollections=new List<AmountEventCollection>();
            LoanRefundCollectionAlpha=new LoanRefundCollectionAlpha();
            LoanRefundCollections=new List<LoanRefundCollection>();
        }
       
    }
    public class LoanRefundCollection
    {
        public string EventAttributeName { get; set; }
        public decimal Amount { get; set; }
        public string Naration { get; set; }
    }
    public class LoanRefundCollectionAlpha
    {
        public string VatAccountNumber { get; set; }
        public string InterestAccountNumber { get; set; }
        public string AmountAccountNumber { get; set; }
        public decimal AmountVAT { get; set; }
        public string VatNaration { get; set; }
        public string InterestNaration { get; set; }
        public decimal AmountInterest { get; set; }
        public decimal AmountCapital { get; set; }
    }

















    //public class AccountToAccountTransfer
    //{
    //    public string Id { get; set; }
    //    public List<MakeAccountPostingCommand> MakeAccountPostingCommands { get; set; }
    //    public string MemberName { get; set; }
    //    public string MemberReference { get; set; }
    //    public string TransactionType { get; set; }
    //    public string BranchId { get; set; }
    //    public string BranchName { get; set; }
    //    public string Status { get; set; }
    //    public string ErrorMessage { get; set; }
    //    public DateTime StartTime { get; set; }
    //    public DateTime EndTime { get; set; }
    //    public string ExternalBranchName { get; set; }
    //    public string Narration { get; set; }
    //    public string ExternalBranchcode { get; set; }
    //    public string TransactionReferenceId { get; set; }
    //    public string BulkExecutionCode { get; set; }
    //    public decimal TotalAmount { get; set; }
    //    public decimal Charges { get; set; }
    //    public string UserFullName { get; set; }
    //    public UserInfoToken UserInfoToken { get; set; }
    //    public string? BranchCode { get; set; }

    //}

    //public class AmountEventCollection
    //{

    //    public string? EventCode { get; set; }   // Generated from rule
    //    public decimal Amount { get; set; }
    //    public string? Naration { get; set; }
    //}
    //public class AmountCollection
    //{
    //    public string? EventAttributeName { get; set; }
    //    public string? LiaisonEventName { get; set; }
    //    public decimal Amount { get; set; }
    //    public bool IsPrincipal { get; set; }
    //    public bool IsInterBankOperationPrincipalCommission { get; set; }
    //    public bool IsInterBankOperationCommission { get; set; }
    //    public bool HasPaidCommissionByCash { get; set; }
    //    public string? Naration { get; set; }

    //    public string GetOperationEventCode(string operation)
    //    {
    //        return operation + "@" + this.EventAttributeName;
    //    }
    //    public string GetLiaisonEventCode(string operation)
    //    {
    //        return operation + "@" + this.LiaisonEventName;
    //    }
    //    public string? GetComissionPrameter(string operation)
    //    {
    //        if (this.EventAttributeName == "DestinationBranchCommission_Account")
    //        {
    //            return operation + "@Commission_Account";
    //        }
    //        else
    //        {
    //            return operation + "@Commission_Account";
    //        }

    //    }
    //    public bool CheckCommissionAccountType()
    //    {
    //        return this.EventAttributeName == "DestinationBranchCommission_Account" || this.EventAttributeName == "Commission_Account";


    //    }
    //}
    //public class LoanRefundCollection
    //{
    //    public string EventAttributeName { get; set; }
    //    public decimal Amount { get; set; }
    //    public string Naration { get; set; }
    //}
    //public class LoanRefundCollectionAlpha
    //{
    //    public string VatAccountNumber { get; set; }
    //    public string InterestAccountNumber { get; set; }
    //    public string AmountAccountNumber { get; set; }
    //    public double AmountVAT { get; set; }
    //    public string VatNaration { get; set; }
    //    public string InterestNaration { get; set; }
    //    public double AmountInterest { get; set; }
    //    public double AmountCapital { get; set; }
    //}

    //public class MakeAccountPostingCommand
    //{
    //    public string TransactionReferenceId { get; set; }
    //    public string AccountNumber { get; set; }
    //    public string? MemberReference { get; set; }
    //    public string ToProductId { get; set; }
    //    public string ToProductName { get; set; }
    //    public string ExternalBranchCode { get; set; }
    //    public string ExternalBranchId { get; set; }
    //    public bool IsInterBranchTransaction { get; set; }
    //    public string FromProductId { get; set; }//Members_Account 
    //    public string ProductType { get; set; }//NewLoan ,OldLoan Or SavingProduct
    //    public string FromProductName { get; set; }
    //    public List<AmountCollection>? AmountCollection { get; set; }
    //    public List<AmountEventCollection>? AmountEventCollections { get; set; }
    //    public List<LoanRefundCollection> LoanRefundCollections { get; set; }

    //    public LoanRefundCollectionAlpha LoanRefundCollectionAlpha { get; set; }
    //    public DateTime TransactionDate { get; set; }
    //    public string ToAccountNumber { get; set; }
    //    public string Naration { get; set; }
    //    public string OperationType { get; set; }

    //    public decimal GetPrincipalAmount()
    //    {
    //        //return this.AmountCollection.Sum(amt => amt.Amount);
    //        return this.AmountCollection.Where(x => x.IsPrincipal == true).FirstOrDefault().Amount;
    //    }
    //}

}
