namespace CBS.AccountManagement.Data
{
    public enum CashReplishmentRequestStatus
    {
        Pending,
        Awaiting_Branch_Transfer,
        Awaiting_Branch_CashClearing,
        Awaiting_Bank_CashOut,
        CashInFusion,
        Approved,
        PendingApproval,
        RedirectToBranchBCO,
        RedirectToBranchBD,
        RedirectToBranchBTB,
        Rejected,
        Awaiting_uploaded_bank_deposit_receipt,

        Completed
    }
}