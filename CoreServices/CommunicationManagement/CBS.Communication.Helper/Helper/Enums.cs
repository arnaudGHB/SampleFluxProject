using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Communication.Helper.Helper
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

    public enum CommunicationPreference
    {
        Email,
        SMS,
        Notification
    }

    public enum GenderType
    {
        Male,
        Female,
        Other
    }

    public enum AccountType
    {
        Savings,
        Current,
        FixedDeposit,
        Loan
    }

    public enum CurrencyType
    {
        USD,
        EUR,
        GBP,
        JPY,
        Other
    }

    public enum KYCStatus
    {
        Pending,
        Verified,
        Rejected
    }

    public enum FATCAStatus
    {
        Compliant,
        NonCompliant
    }

    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer,
        Payment,
        Other
    }

    public enum LoanStatus
    {
        Approved,
        Disbursed,
        Repaid,
        Defaulted,
        Closed
    }

    public enum RelationshipType
    {
        Spouse,
        Child,
        Parent,
        Sibling,
        Other
    }

    public enum CustomerProfileType
    {
        Group,
        Organisation,
        Individual
    }
}
