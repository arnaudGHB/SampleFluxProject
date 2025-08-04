namespace CBS.BudgetManagement.Helper
{
    public enum SMSTypes
    {
        Subscription,
        Saving,
        Claim,
        Cashout
    }

    public enum ServiceTypes
    {
        ClientMicroService,
        LoanMicroService,
        AccountMicroService,
        ClaimMicroService
    }

    public enum SubscriptionStatus
    {
        Awaiting_Customer_Momo_Validation,
        Unsubscrbed,
        Failed, Subscribed, ReSubcription,
        Unsubscribed,
        ReSubscription
    }

    public enum ResultStatus
    {
        Ok,
        Failed
    }
}