using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data.Dto
{
    /// <summary>
    /// Represents a transaction record.
    /// </summary>
    public class TransactionTrackerDto
    {
        public string Id { get; set; }
        public string CommandDataType { get; set; }
        public string CommandJsonObject { get; set; }
        public string TransactionReferenceId { get; set; }
        public bool HasPassed { get; set; }
        public int NumberOfRetry { get; set; }
        public DateTime DatePassed { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? DestinationUrl { get; set; }
        public string? SourceUrl { get; set; }
        public string? UserFullName{ get; set; }
        public string? BranchId { get; set; }
        public string? BranchCode { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public static class CommandDataType
    {
        public const string AddTransferEventCommand = "AddTransferEventCommand";
        public const string CashInitializationCommand = "CashInitializationCommand";
        public const string AddTransferToNonMemberEventCommand = "AddTransferToNonMemberEventCommand";
        public const string AddTransferWithdrawalEventCommand = "AddTransferWithdrawalEventCommand";
        public const string AutoPostingEventCommand = "AutoPostingEventCommand";
        public const string ClosingOfMemberAccountCommand = "ClosingOfMemberAccountCommand";
        public const string DailyCollectionConfirmationPostingEventCommand = "DailyCollectionConfirmationPostingEventCommand";
        public const string DailyCollectionMonthlyCommisionEventCommand = "DailyCollectionMonthlyCommisionEventCommand";
        public const string DailyCollectionMonthlyPayableEventCommand = "DailyCollectionMonthlyPayableEventCommand";
        public const string DailyCollectionPostingEventCommand = "DailyCollectionPostingEventCommand";
        public const string LoanRefundPostingCommand = "LoanRefundPostingCommand";
        public const string MakeBulkAccountPostingCommand = "MakeBulkAccountPostingCommand";
        public const string LoanApprovalPostingCommand = "LoanApprovalPostingCommand";
        public const string LoanDisbursementPostingCommand = "LoanDisbursementPostingCommand";
        public const string MakeAccountPostingCommand = "MakeAccountPostingCommand";
        public const string ReverseAccountingEntryCommand = "ReverseAccountingEntryCommand";
        public const string ManualPostingEventCommand = "ManualPostingEventCommand";
        public const string MobileMoneyCollectionOperationCommand = "MobileMoneyCollectionOperationCommand";
        public const string MobileMoneyManagementPostingCommand = "MobileMoneyManagementPostingCommand";
        public const string MobileMoneyOperationCommand = "MobileMoneyOperationCommand";
        public const string OpeningOfDayEventCommand = "OpeningOfDayEventCommand";
 

    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Data
    {
        public List<TransactionTrackerDto> items { get; set; }
        public int totalCount { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public bool hasPreviousPage { get; set; }
        public bool hasNextPage { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public string commandDataType { get; set; }
        public string commandJsonObject { get; set; }
        public string transactionReferenceId { get; set; }
        public bool hasPassed { get; set; }
        public int numberOfRetry { get; set; }
        public DateTime datePassed { get; set; }
        public DateTime transactionDate { get; set; }
        public string destinationUrl { get; set; }
        public string sourceUrl { get; set; }
        public DateTime createdDate { get; set; }
    }

    public class TransactionTrack
    {
        public Data data { get; set; }
        public List<object> errors { get; set; }
        public int statusCode { get; set; }
        public string statusDescription { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }



}
