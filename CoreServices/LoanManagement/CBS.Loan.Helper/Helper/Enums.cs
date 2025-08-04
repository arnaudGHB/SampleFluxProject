namespace CBS.NLoan.Helper.Helper
{
    public enum SMSTypes
    {
        Subscription,
        Saving,
        Claim,
        Cashout
    }
   
   
    public enum HttpStatusCodeEnum
    {
        /// <summary>
        /// 200 OK: The request has succeeded.
        /// </summary>
        OK = 200,

        /// <summary>
        /// 201 Created: The request has been fulfilled and a new resource has been created.
        /// </summary>
        Created = 201,

        /// <summary>
        /// 204 No Content: The server successfully processed the request and is not returning any content.
        /// </summary>
        NoContent = 204,

        /// <summary>
        /// 400 Bad Request: The server could not understand the request due to invalid syntax.
        /// </summary>
        BadRequest = 400,

        /// <summary>
        /// 401 Unauthorized: The request requires user authentication.
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// 403 Forbidden: The server understood the request, but refuses to authorize it.
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 404 Not Found: The server can not find the requested resource.
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// 409 Conflict: The request could not be completed due to a conflict with the current state of the target resource.
        /// </summary>
        Conflict = 409,

        /// <summary>
        /// 500 Internal Server Error: The server has encountered a situation it doesn't know how to handle.
        /// </summary>
        InternalServerError = 500,

        /// <summary>
        /// 502 Bad Gateway: The server, while acting as a gateway or proxy, received an invalid response from the upstream server.
        /// </summary>
        BadGateway = 502,

        /// <summary>
        /// 503 Service Unavailable: The server is not ready to handle the request, often due to being overloaded or down for maintenance.
        /// </summary>
        ServiceUnavailable = 503,
        StandingOrder = 504
    }

    /// <summary>
    /// Represents the various actions that can be logged in the application.
    /// </summary>
    public enum LogAction
    {
        /// <summary>
        /// Indicates that a new resource has been created.
        /// </summary>
        Create,
        /// <summary>
        /// Indicates that a new resource has been created.
        /// </summary>
        AccountingMapping,
        /// <summary>
        /// Indicates that an existing resource has been updated.
        /// </summary>
        Update,
        GeneralDailyDashboard,
        /// <summary>
        /// Indicates that a resource has been deleted.
        /// </summary>
        Delete,

        /// <summary>
        /// Indicates that a resource has been read or retrieved.
        /// </summary>
        Read,

        Remittance,

        /// <summary>
        /// Indicates that a file or data has been downloaded.
        /// </summary>
        Download,

        /// <summary>
        /// Indicates that a file or data has been uploaded.
        /// </summary>
        Upload,

        /// <summary>
        /// Indicates that a user has logged in to the system.
        /// </summary>
        Login,

        /// <summary>
        /// Indicates that an accounting posting has been made.
        /// </summary>
        AccountingPosting,

        /// <summary>
        /// Indicates that a user has logged out of the system.
        /// </summary>
        Logout,

        /// <summary>
        /// Indicates that a user has registered for an account.
        /// </summary>
        Register,

        /// <summary>
        /// Indicates that a user has changed their password.
        /// </summary>
        ChangePassword,

        /// <summary>
        /// Indicates that a user has requested a password reset.
        /// </summary>
        ResetPassword,

        /// <summary>
        /// Indicates that an account has been locked due to failed login attempts.
        /// </summary>
        AccountLock,

        /// <summary>
        /// Indicates that an account has been unlocked.
        /// </summary>
        AccountUnlock,
        MemberRegistration,
        /// <summary>
        /// Indicates that a user has updated their profile information.
        /// </summary>
        ProfileUpdate,

        /// <summary>
        /// Indicates that funds have been transferred between accounts.
        /// </summary>
        Transfer,

        /// <summary>
        /// Indicates that funds have been transferred between accounts.
        /// </summary>
        Transfer3PP,
        /// <summary>
        /// Indicates that funds have been transferred between accounts.
        /// </summary>
        TransferCMONEY,

        /// <summary>
        /// Indicates that a transaction has failed.
        /// </summary>
        TransactionFailed,

        /// <summary>
        /// Indicates that a transaction has been successful.
        /// </summary>
        TransactionSuccess,

        /// <summary>
        /// Indicates that a user session has started.
        /// </summary>
        SessionStart,

        /// <summary>
        /// Indicates that a user session has ended.
        /// </summary>
        SessionEnd,

        /// <summary>
        /// Indicates that notifications have been sent to users.
        /// </summary>
        NotificationSent,

        /// <summary>
        /// Indicates that permissions have been granted to a user.
        /// </summary>
        PermissionGranted,

        /// <summary>
        /// Indicates that permissions have been revoked from a user.
        /// </summary>
        PermissionRevoked,

        /// <summary>
        /// Indicates that data has been exported from the application.
        /// </summary>
        DataExport,

        /// <summary>
        /// Indicates that data has been imported into the application.
        /// </summary>
        DataImport,

        /// <summary>
        /// Indicates that a user has logged in using two-factor authentication.
        /// </summary>
        TwoFactorLogin,

        /// <summary>
        /// Indicates that a user has completed an onboarding process.
        /// </summary>
        OnboardingComplete,

        /// <summary>
        /// Indicates that a user has submitted feedback or a support ticket.
        /// </summary>
        FeedbackSubmitted,

        /// <summary>
        /// Indicates that a user has updated their notification preferences.
        /// </summary>
        NotificationPreferencesUpdated,

        /// <summary>
        /// Indicates that a user has requested account deletion.
        /// </summary>
        AccountDeletionRequested,

        /// <summary>
        /// Indicates that a user has accepted terms and conditions.
        /// </summary>
        TermsAccepted,

        /// <summary>
        /// Indicates that an administrator has made changes to system settings.
        /// </summary>
        SettingsUpdated,

        /// <summary>
        /// Indicates that a user has subscribed to a service.
        /// </summary>
        SubscriptionCreated,

        /// <summary>
        /// Indicates that a user has unsubscribed from a service.
        /// </summary>
        SubscriptionCanceled,

        /// <summary>
        /// Indicates that a customer has made an account inquiry.
        /// </summary>
        AccountInquiry,

        /// <summary>
        /// Indicates that a customer has requested a withdrawal.
        /// </summary>
        WithdrawalRequested,

        /// <summary>
        /// Indicates that a withdrawal has been processed.
        /// </summary>
        WithdrawalProcessed,

        /// <summary>
        /// Indicates that a customer has made a deposit.
        /// </summary>
        DepositMade,

        /// <summary>
        /// Indicates that a deposit has been processed.
        /// </summary>
        DepositProcessed,
        AccountStatistics,
        /// <summary>
        /// Indicates accounting event posting.
        /// </summary>
        AccountingEventPosting,
        OtherCashIn,
        OtherCashOut,
        /// <summary>
        /// Indicates that a customer has requested a statement printout.
        /// </summary>
        StatementPrintRequested,

        /// <summary>
        /// Indicates that a checkbook has been ordered.
        /// </summary>
        CheckbookOrdered,


        /// <summary>
        /// Indicates that a loan application form has been provided to a customer.
        /// </summary>
        LoanApplicationFormProvided,

        /// <summary>
        /// Indicates that a customer has made a currency exchange.
        /// </summary>
        CurrencyExchangeMade,

        /// <summary>
        /// Indicates that a safe deposit box has been accessed.
        /// </summary>
        SafeDepositBoxAccessed,

        /// <summary>
        /// Indicates that a user has processed a new account opening request.
        /// </summary>
        NewAccountOpeningProcessed,

        /// <summary>
        /// Indicates that a user has updated a customer's contact information.
        /// </summary>
        CustomerContactInfoUpdated,

        /// <summary>
        /// Indicates that a user has provided financial advice to a customer.
        /// </summary>
        FinancialAdviceProvided,

        /// <summary>
        /// Indicates that a customer has made a request for a financial product.
        /// </summary>
        FinancialProductRequestMade,

        /// <summary>
        /// Indicates that a customer has made a payment on a loan or account.
        /// </summary>
        PaymentReceived,

        /// <summary>
        /// Indicates that a customer has submitted documentation for verification.
        /// </summary>
        DocumentationSubmitted,

        /// <summary>
        /// Indicates that a customer has inquired about a credit card application.
        /// </summary>
        CreditCardInquiry,

        /// <summary>
        /// Indicates that a credit card application has been submitted.
        /// </summary>
        CreditCardApplicationSubmitted,

        /// <summary>
        /// Indicates that a customer has requested a product demonstration.
        /// </summary>
        ProductDemonstrationRequested,

        /// <summary>
        /// Indicates that a customer has expressed interest in a new banking service.
        /// </summary>
        ServiceInterestExpressed,

        /// <summary>
        /// Indicates that a customer has filed a complaint.
        /// </summary>
        ComplaintFiled,

        /// <summary>
        /// Indicates that a customer has provided feedback on service.
        /// </summary>
        ServiceFeedbackReceived,

        /// <summary>
        /// Indicates that a user has escalated a customer issue to management.
        /// </summary>
        CustomerIssueEscalated,

        /// <summary>
        /// Indicates that a cash withdrawal operation has failed.
        /// </summary>
        CashWithdrawalFailed,

        /// <summary>
        /// Indicates that a cash deposit operation has failed.
        /// </summary>
        CashDepositFailed,

        /// <summary>
        /// Indicates that a cash transaction has been flagged for review.
        /// </summary>
        CashTransactionFlagged,

        /// <summary>
        /// Indicates that a fund transfer operation has failed.
        /// </summary>
        TransferFailed,

        /// <summary>
        /// Indicates that a fund transfer operation has succeeded.
        /// </summary>
        TransferSuccess,

        /// <summary>
        /// Indicates that a transaction has been reversed.
        /// </summary>
        Reversal,

        /// <summary>
        /// Indicates that a loan application has been submitted.
        /// </summary>
        LoanApplicationSubmitted,

        /// <summary>
        /// Indicates that a loan has been disbursed to a customer.
        /// </summary>
        LoanDisbursement,

        /// <summary>
        /// Indicates that a loan repayment has been made by a customer.
        /// </summary>
        LoanRepayment,
        /// <summary>
        /// Indicates that a loan Fee to be paid for a loan application.
        /// </summary>
        LoanFeeDeposit,

        /// <summary>
        /// Indicates that a loan has been approved.
        /// </summary>
        LoanApproved,

        /// <summary>
        /// Indicates that a loan application has been rejected.
        /// </summary>
        LoanApplicationRejected,

        /// <summary>
        /// Indicates that a loan has been extended.
        /// </summary>
        LoanExtended,

        /// <summary>
        /// Indicates that a loan has been rescheduled.
        /// </summary>
        LoanRescheduled,

        /// <summary>
        /// Indicates that a loan has been closed.
        /// </summary>
        LoanClosed,

        /// <summary>
        /// Indicates that a loan has been flagged for review.
        /// </summary>
        LoanFlagged,

        /// <summary>
        /// Indicates that a customer has requested information about their loan.
        /// </summary>
        LoanInquiryRequested,

        /// <summary>
        /// Indicates that a customer has received a loan statement.
        /// </summary>
        LoanStatementReceived,
        /// <summary>
        /// Indicates that a customer has made a Mobile Money deposit.
        /// </summary>
        MTNOrOrangeOper,

        /// <summary>
        /// Indicates that a Mobile Money withdrawal has been processed.
        /// </summary>
        MobileMoneyWithdrawal,

        /// <summary>
        /// Indicates that a customer has made an Orange Money deposit.
        /// </summary>
        OrangeMoneyDeposit,

        /// <summary>
        /// Indicates that an Orange Money withdrawal has been processed.
        /// </summary>
        OrangeMoneyWithdrawal,
        /// <summary>
        /// Indicates that a customer has made a Mobile Money or Orange Money operation (deposit/withdrawal).
        /// </summary>
        MTNorOrangeMoneyOperation,
        /// <summary>
        /// Indicates that the accounting day has been opened.
        /// </summary>
        OpenOfAccountingDay,

        /// <summary>
        /// Indicates that the accounting day has been closed.
        /// </summary>
        ClosedOfAccountingDay,

        /// <summary>
        /// Indicates that the tills have been opened.
        /// </summary>
        OpenOfTills,

        /// <summary>
        /// Indicates that the tills have been closed.
        /// </summary>
        ClosedOfTills,

        /// <summary>
        /// Indicates that a cash requisition has been made.
        /// </summary>
        CashRequisitions,
        GetMembersByMareicules,
        StandingOrder,
        Branch,
        MemberActivation,
        MemberPinValidation,
        GetMembersByTelephonNumber,
        GetMemberByMemberReference,
        GetAllLoans,
        GetCustomerLoans,
        GetMembersLoans,
        TransactionTrackerAccounting,
        DeleteTransactionTrackerAccounting,
        PaginatedTransactionTrackerAccounting,
        GetLastXTransactionTrackerAccounting,
        CalculateTransferCharges,
        LoanApproval,
        BlockAccount,
        ReleaseBlockedAccount,
        VaultOperationOut,
        VaultOperationTransferCash,
        VaultOperationCashin,
        WithdrawalNotification,
        LoanApplicationRejection,
        LoanApplicationApproval,
        LoanApplicationRejectionAttempt,
        LoanGuarantor,
        DelinquencyServiceEnd,
        DelinquencyServiceRun,
        DelinquencyServiceError,
        DelinquencyServiceStop,
        DelinquencyServiceStart,
        LoanDeliquencyProcessingError,
        LoanDeliquencyProcessingSuccess,
        LoanApplicationSearch,
        LoanSORepaymentCompleted,
        LoanSORepaymentFailed,
        LoanSORepaymentAttempted,
    }

    public enum LogLevelInfo
    {
        Information,   // General informational messages
        Warning,       // Potential issues or important events
        Error,         // Error events that might still allow the application to continue running
        Critical,      // Serious errors that require immediate attention
        Debug,         // Detailed information for debugging purposes
        Trace          // Fine-grained informational events for tracking the flow of the application
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
    public enum LoanPeriode
    {
        Normal,
        Grace,
        OverDue,
    }
    public enum TransactionStatus
    {
        Pending,
        Validaded,
        Abandoned,
        Delete,
        Postponed,
        Rejected,
        Disburse,
        Pay,
    }
    public enum GuarantyType
    {
        Collateral,
        Goods,
        None,
    }

    public enum LoanSORepayment
    {
        Successful,
        Failed,
        Pending
    }
}
