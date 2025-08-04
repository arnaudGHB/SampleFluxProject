using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.AccountingEntry.Commands;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class MakeAccountPostingCommand : IRequest<ServiceResponse<bool>>,IPosting
    {
        public string TransactionReferenceId { get; set; }
        public string AccountNumber { get; set; }
        public string? MemberReference { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string OperationType { get; set; }
        public string ExternalBranchCode { get; set; }
        public string ExternalBranchId { get; set; }
        public bool IsInterBranchTransaction { get; set; }
        public string Source { get; set; }//Members_Account, Physical_Teller,Vitual_Teller
        public List<AmountCollection>? AmountCollection { get; set; }
        public List<AmountEventCollection>? AmountEventCollections { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal GetPrincipalAmount()
        {
            //return this.AmountCollection.Sum(amt => amt.Amount);
            return this.AmountCollection.Where(x=>x.IsPrincipal==true).FirstOrDefault().Amount;
        }

        internal bool IsRemittance()
        {
            return this.TransactionReferenceId.Contains("BWRM");
        }
    }

  


    public enum TellerSources
    {
        Virtual_Teller_MTN,
        Virtual_Teller_Orange,
        Virtual_Teller_GAV,
        Members_Account,
        Physical_Teller
    }
    public class AmountCollection
    {

        public string? EventAttributeName { get; set; }
        public string? LiaisonEventName { get; set; }
        public decimal Amount { get; set; }
        public bool IsPrincipal { get; set; }
        public bool IsInterBankOperationPrincipalCommission { get; set; }
        public bool IsInterBankOperationCommission { get;  set; }
        public bool HasPaidCommissionByCash { get;  set; }
        public string? Naration { get; set; }

        public string GetOperationEventCode(string operation)
        {
            return operation + "@" + this.EventAttributeName;
        }
        public string GetLiaisonEventCode(string operation)
        {
            return operation + "@" + this.LiaisonEventName;
        }



        public string? GetComissionPrameter(string operation)
        {
            if (this.EventAttributeName == "DestinationBranchCommission_Account")
            {
                return operation + "@Commission_Account";
            }
            else
            {
                return operation + "@Commission_Account";
            }

        }



        public bool CheckCommissionAccountType()
        {
            return this.EventAttributeName == "DestinationBranchCommission_Account" ||  this.EventAttributeName == "Commission_Account";


        }
    }

    public class OtherEntry
    {
        public string EventCode { get; set; }
        public decimal Amount { get; set; }

        public bool CheckIfOtherEntry(OtherEntry OtherEntry)
        {
            return OtherEntry.Amount == 0;
        }

    }

    public class MakeBulkAccountPostingCommand : IRequest<ServiceResponse<bool>>
    {
       
        public List<MakeAccountPostingCommand> MakeAccountPostingCommands { get; set; }


    }
}