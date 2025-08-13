using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.DailyStatisticBoard
{
    public class GeneralDailyDashboardDto
    {
        public string Id { get; set; }
        public string BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? BranchCode { get; set; }
        public int NumberOfCashIn { get; set; }
        public int NumberOfCashOut { get; set; }
        public decimal TotalCashInAmount { get; set; }
        public decimal TotalCashOutAmount { get; set; }
        public int NewMembers { get; set; }
        public int ClosedAccounts { get; set; }
        public int ActiveAccounts { get; set; }
        public int DormantAccounts { get; set; }
        public int NumberOfInterBranchCashIn { get; set; }
        public int NumberOfInterBranchCashOut { get; set; }
        public decimal VolumeOfInterBranchCashIn { get; set; }
        public decimal VolumeOfInterBranchCashOut { get; set; }
        public decimal LoanDisbursements { get; set; }
        public decimal LoanRepayments { get; set; }
        public decimal ServiceFeesCollected { get; set; }
        public decimal InterestPaid { get; set; }
        public decimal Vat { get; set; }
        public decimal Penalties { get; set; }

        public decimal DailyExpenses { get; set; }
        public decimal OrdinaryShares { get; set; }
        public decimal PreferenceShares { get; set; }
        public decimal Savings { get; set; }
        public decimal Deposits { get; set; }
        public decimal CashInHand57 { get; set; }
        public decimal CashInHand56 { get; set; }
        public decimal MTNMobileMoney { get; set; }
        public decimal MobileMoneyCashOut { get; set; }
        public int NumberOfCashOutMTN { get; set; }

        public int NumberOfCashOutOrange { get; set; }
        public int NumberOfCashInMTN { get; set; }
        public int NumberOfLoanFee { get; set; }
        public int NumberOfLoanDisbursementFee { get; set; }
        public int NumberOfCashInOrange { get; set; }
        public decimal OrangeMoneyCashOut { get; set; }

        public decimal OrangeMoney { get; set; }
        public decimal DailyCollectionCashOut { get; set; }
        public decimal DailyCollectionCashIn { get; set; }
        public int NumberOfDailyCollectionCashOut { get; set; }
        public int NumberOfDailyCollectionCashIn { get; set; }

        public decimal MomocashCollection { get; set; }
        public decimal Transfer { get; set; }
        public int NumberOfTransfer { get; set; }
        public decimal PrimaryTillOpenOfDayBalance { get; set; }

        public decimal SubTillTillOpenOfDayBalance { get; set; }
        public DateTime Date { get; set; }
        public DateTime AccountingDate { get; set; }
        public decimal SubTillBalance { get; set; }
        public decimal PrimaryTillBalance { get; set; }
        public decimal CashReplenishmentSubTill { get; set; }
        public decimal CashReplenishmentPrimaryTill { get; set; }
        public int NumberOfBranches { get; set; }
    }

}

/// <summary>
/// Represents a cash operation performed at a branch, including details like branch info, amount, fees, and operation type.
/// </summary>
public class CashOperation
{
    /// <summary>
    /// Gets or sets the branch identifier.
    /// </summary>
    public string BranchId { get; set; }

    /// <summary>
    /// Gets or sets the operation amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the fee associated with the operation.
    /// </summary>
    public decimal Fee { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the operation is inter-branch.
    /// </summary>
    public bool IsInterBranch { get; set; }

    /// <summary>
    /// Gets or sets the branch name.
    /// </summary>
    public string BranchName { get; set; }

    /// <summary>
    /// Gets or sets the branch code.
    /// </summary>
    public string BranchCode { get; set; }

    /// <summary>
    /// Gets or sets the open-of-day reference identifier.
    /// </summary>
    public string OpenOfDayReference { get; set; }

    /// <summary>
    /// Gets or sets the teller identifier.
    /// </summary>
    public string TellerId { get; set; }

    /// <summary>
    /// Gets or sets the type of the cash operation.
    /// </summary>
    public CashOperationType OperationType { get; set; }

    /// <summary>
    /// Gets or sets the logging action associated with the operation.
    /// </summary>
    public LogAction LogAction { get; set; }

    /// <summary>
    /// Gets or sets the provisioning history for the sub-teller involved in the operation.
    /// </summary>
    public SubTellerProvioningHistory SubTellerProvioningHistory { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CashOperation"/> class with specified details.
    /// </summary>
    /// <param name="branchId">The branch identifier.</param>
    /// <param name="amount">The transaction amount.</param>
    /// <param name="fee">The transaction fee.</param>
    /// <param name="branchName">The branch name.</param>
    /// <param name="branchCode">The branch code.</param>
    /// <param name="operationType">The type of operation.</param>
    /// <param name="logAction">The log action associated with the operation.</param>
    /// <param name="openOfDayReference">The open-of-day reference identifier.</param>
    /// <param name="tellerId">The teller identifier.</param>
    /// <param name="subTellerProvioningHistory">The sub-teller provisioning history.</param>
    public CashOperation(
        string branchId,
        decimal amount,
        decimal fee,
        string branchName,
        string branchCode,
        CashOperationType operationType,
        LogAction logAction,
        string openOfDayReference,
        string tellerId,
        SubTellerProvioningHistory subTellerProvioningHistory)
    {
        BranchId = branchId;
        Amount = amount;
        Fee = fee;
        BranchName = branchName;
        BranchCode = branchCode;
        OperationType = operationType;
        LogAction = logAction;
        OpenOfDayReference = openOfDayReference;
        TellerId = tellerId;

        // Initialize SubTellerProvisioningHistory with a new instance if null.
        SubTellerProvioningHistory = subTellerProvioningHistory ?? new SubTellerProvioningHistory();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CashOperation"/> class with simplified details.
    /// </summary>
    /// <param name="branchId">The branch identifier.</param>
    /// <param name="amount">The transaction amount.</param>
    /// <param name="fee">The transaction fee.</param>
    /// <param name="branchName">The branch name.</param>
    /// <param name="branchCode">The branch code.</param>
    /// <param name="operationType">The type of operation.</param>
    /// <param name="logAction">The log action associated with the operation.</param>
    /// <param name="subTellerProvioningHistory">The sub-teller provisioning history.</param>
    public CashOperation(
        string branchId,
        decimal amount,
        decimal fee,
        string branchName,
        string branchCode,
        CashOperationType operationType,
        LogAction logAction,
        SubTellerProvioningHistory subTellerProvioningHistory)
    {
        BranchId = branchId;
        Amount = amount;
        Fee = fee;
        BranchName = branchName;
        BranchCode = branchCode;
        OperationType = operationType;
        LogAction = logAction;

        // Initialize SubTellerProvisioningHistory with a new instance if null.
        SubTellerProvioningHistory = subTellerProvioningHistory ?? new SubTellerProvioningHistory();
    }

    /// <summary>
    /// Returns a string representation of the cash operation for debugging and logging purposes.
    /// </summary>
    /// <returns>A string containing the cash operation details.</returns>
    public override string ToString()
    {
        return $"BranchId: {BranchId}, BranchName: {BranchName}, BranchCode: {BranchCode}, Amount: {Amount}, Fee: {Fee}, OperationType: {OperationType}, LogAction: {LogAction}";
    }
}

