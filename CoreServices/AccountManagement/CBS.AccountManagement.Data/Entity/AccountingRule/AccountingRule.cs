using CBS.AccountManagement.Data.Entity;

namespace CBS.AccountManagement.Data;

public class AccountingRule : BaseEntity
{
    public string Id { get; set; }
    public string SystemDescription { get; set; }
    public string? RuleName { get; set; } //Loan Operation xxx
    public bool? IsDoubleValidationNeeded { get; set; }
    public string MFI_ChartOfAccountId { get; set; }
    public string BookingDirection { get; set; }

    public string System_Id { get; set; }
    public string EntryType { get; set; }
    public string LevelOfExecution { get; set; }
    public ICollection<ChartOfAccountManagementPosition>? ChartOfAccounts { get; set; }

}

