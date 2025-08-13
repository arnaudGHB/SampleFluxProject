using CBS.TransactionManagement.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Helper
{
    public enum WithdrawalNotificationStatus
    {
        Completed,
        Scheduled,
        InGracePeriod,
        Expired
    }
    public enum HolidayType
    {
        PublicHoliday,  // Fixed public holidays like New Year's Day
        Weekend,        // Weekends (e.g., Saturday and Sunday)
        Recurring,      // Recurring holidays (e.g., every Monday)
        SpecialEvent,   // Special events like company anniversaries
        Lockdown
    }
    /// <summary>
    /// Enum for Product Types
    /// </summary>
    public enum ProductType
    {
        Teller,
        OrdinaryAccount,
        Remittance
    }
    public enum PushNotificationTitle
    {
        CASH_IN,
        CASH_OUT,
        LOAN_REPAYMENT,
        LOAN_REPAYMENT_MOMO_CASH,
        CASH_IN_MOMO_CASH,
        TRANSFER
    }

    public enum CommandDataType
    {
        AddTransferEventCommand,
        AddTransferToNonMemberEventCommand,
        AddTransferWithdrawalEventCommand,
        AutoPostingEventCommand,
        ClosingOfMemberAccountCommand,
        DailyCollectionConfirmationPostingEventCommand,
        DailyCollectionMonthlyCommisionEventCommand,
        DailyCollectionMonthlyPayableEventCommand,
        DailyCollectionPostingEventCommand,
        MakeNonCashAccountAdjustmentCommand,
        MakeBulkAccountPostingCommand,
        LoanApprovalPostingCommand,
        LoanDisbursementPostingCommand,
        LoanDisbursementRefinancingPostingCommand,
        MakeAccountPostingCommand,
        ReverseAccountingEntryCommand,
        ManualPostingEventCommand,
        MobileMoneyCollectionOperationCommand,
        MobileMoneyManagementPostingCommand,
        MobileMoneyOperationCommand,
        OpeningOfDayEventCommand,
        MobileMoneyTopUpCommand,
        MakeTransferPosting,
        InternalAutoPostingCommand,
        CashInitializationCommand,
        AddAccountingAccountType,
        AddSalaryAccountToAccountTransferCommand,
        RemittanceAccountingPostingCommand
    }

    public enum RecurrencePattern
    {
        None,           // No recurrence
        Daily,          // Every day
        Weekly,         // Weekly recurrence (e.g., every Monday)
        Monthly,        // Monthly recurrence
        Yearly,         // Yearly recurrence
    }


    public enum CashOperationType
    {
        CashIn,
        RemittanceIn,
        RemittanceOut,
        CashOut,
        LoanRepayment,
        OtherCashIn,
        OtherCashOut,
        MobileMoneyCashIn,
        NewMember,
        MobileMoneyCashOut,
        OrangeMoneyCashOut,
        OrangeMoneyCashIn,
        LoanDisbursementFee,
        SubscriptionFee,
        LoanFee,
        CloseOfDayPrimaryTill,
        OpenOfDayPrimaryTill,
        OpenOfDaySubTill,
        CloseOfDaySubTill,
        CashReplenishmentSubTill,
        CashReplenishmentPrimaryTill
    }
    public enum SMSTypes
    {
        Subscription,
        Saving,
        Claim,
        Cashout
    }
    public enum GAVOperationType
    {
        TRANSFER,
        CASHIN,
        CASHOUT,
        BILL_PAYMENT,
        AIRTIME,
        MERCHANT_PURCHASE,
        CARDLESS_WITHDRAWAL,
        REMITTANCE
    }

    public enum TellerSources
    {
        Virtual_Teller_MTN,
        Virtual_Teller_Orange,
        Virtual_Teller_GAV,
        Members_Account,
        Members_Activation,
        Physical_Teller,
        Virtual_Teller_Momo_cash_Collection,
        DailyCollector, MemberCommission
    }
    public enum LegalForms
    {
        Physical_Person,
        Moral_Person

    }
    public enum ServiceTypes
    {
        ClientMicroService,
        LoanMicroService,
        AccountMicroService,
        ClaimMicroService,
        CLOSE_OF_DAY_PRIMARY_TELLER,
        CLOSE_OF_DAY_SUB_TELLER,
        CLOSE_OF_DAY_ACCOUNTANT,
        OPEN_OF_DAY_PRIMARY_TELLER,
        OPEN_OF_DAY_SUB_TELLER,
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
    public enum DepositType
    {
        Local_Cash,
        Local_Check,
        Inter_Branch_Cash,
        Inter_Branch_Check,
    }
    public enum BulkOperationType
    {
        CASH,
        CHECK,
    }
    public enum InterOperationType
    {
        Local_Cash,
        Local_Check,
        Inter_Branch_Cash,
        Inter_Branch_Check,
    }

    public enum TransferType
    {
        Local,
        Inter_Branch,
        Self,
        Incoming_International,
        Outgoing_International
    }
    public enum RemittanceTransferType
    {
        Local,
        Incoming_International,
        Outgoing_International
    }
    public enum TPPtransferType
    {
        GimacPayment,
        Local,
        InterBank
    }
    //
    public enum CashInCashOutTransfer
    {

        Withdrawal,
        Transfer,
        Deposit,
        Cmoney_SWN

    }
    public enum OperationType
    {
        Debit,
        Credit,
        Withdrawal,
        Transfer, Both_Cash_NoneCash,
        Deposit, Cash, NoneCash

    }
    public enum OperationAccountType
    {
        Teller,
        Saving_Product
    }
    public enum TellerType
    {
        PhysicalTeller,
        MobileMoneyMTN,
        MomocashCollectionMTN,
        MobileMoneyORANGE,
        MomocashCollectionOrange,
        OtherVirtualTeller,
        DailyCollector,
        NoneCashTeller
    }

    public enum OperationSourceType
    {
        TTP,
        Web_Portal, BackOffice_Operation, Physical_Teller,
        Job
    }
    public enum FeeOperationType
    {
        MemberShip,  // Membership or Member Registration
        Operation,   // Normal cash operation
        CMoney,      // C-Money operation
        Gav,         // Gav operation
        MobileMoney, // Mobile Money operation
        OrangeMoney,  // Orange Money operation
        CivilServants,
        PrivateInstitutions,
        Internal,
        DailyServers,
    }

    public enum OtherCashInSourceType
    {
        Cash_Collected,
        Member_Account
    }
    public enum OtherCashInType
    {
        Income,
        Expense
    }
    public enum LoanApplicationTypes
    {
        Normal,
        Refinancing,
        Reschedule,
        Restructure
    }
    public enum LoanRepaymentType
    {
        LoanRepaymentMomocashCollection,
        LocalAccount,
        Cash
    }

    public enum RemittanceTypes
    {
        WesternUnion,
        MoneyGram,
        Ria,
        OFX,
        MPesa,
        Payoneer,
        WorldRemit,
        TrustSoftCredit
    }
    public enum ChargeType
    {
        Inclussive,
        Exclussive,

    }
    public enum OperationPrefix
    {
        Withdrawal = 1,
        Deposit,
        Transfer,
        TTP_Transfer,
        LR_Deposit,
        CashIn_Loan_Repayment,
        Loan_Disbursement,
        Loan_Disbursement_Refinance,
        WithdrawalRequest,
        OtherCashIn_Expense,
        OtherCashIn_Income,
        OtherCashIn_Income_Withdrawal,
        TTP_Withdrawal,
        Loan_Accrual_Interest,
        Deposit_Loan_Repayment,
        MMC_In,
        MMC_Out,
        OMC_In,
        OMC_Out,
        TTP_Transfer_GAV,
        TTP_Transfer_CMoney,
        MomoKash_Collection,
        MomoKash_Collection_Loan_Repayment,
        Expense,
        Income,
        Cash_W_Remittance,
        Cash_In_Remittance,
        Remittance,
        Vault_Operation_Change,
        Cash_Change_Operation_SubTill,
        Cash_Change_Operation_PrimaryTill,
        Salary,
        Other // Default
    }


    public enum TransactionType
    {
        CASH_RECEPTION,
        DEPOSIT,
        CASHIN_LOAN_REPAYMENT,
        CASHIN_LOAN_REPAYMENT_STANDING_ORDER,
        LR_DEPOSIT,
        WITHDRAWAL,
        WITHDRAWAL_MMK,
        WITHDRAWAL_MOBILEMONEY,
        CASH_WITHDRAWAL,
        CASH_W_Remittance,
        CASH_IN, CASH_IN_MMK,
        CASH_IN_Remittance,
        CASH_REVERSAL,
        CASH_IN_MEMBER_SUB_FEE,
        CASH_IN_FEE,
        FEE, MomokcashCollection, MomokcashCollection_Loan_Repayment,
        FEE_Remittance,
        CASH_IN_LOAN_FEE,
        SALARY_TRANSFER_CHARGES,
        OTHER_CASH_IN, MobileMoney, OrangeMoney,
        NONE_CASH_CASH_IN,
        NONE_CASH_WITHDRAWAL,
        OTHER_CASH_PAYMENT,
        AuxillaryToBranch, HeadOfficeToBranch, BranchToBranch, BranchToHeadOffice,
        WITHDRAWALREQUEST,
        WITHDRAWAL_REQUEST_FORM_FEE, CASHIN_FORM_FEE, CASH_REPLENISHMENT,
        MobileMoney_TopUp, OrangeMoney_TopUp,
        Loan_Accrual_Interest,
        Loan_Disbursement_Refinance,
        Loan_Disbursement,
        TRANSFER,
        SALARY_TRANSFER,
        TTP_TRANSFER, Migration, Reversal, TTP_TRANSFER_GAV, TTP_TRANSFER_CMNEY,
        TTP_WITHDRAWAL,
        OPENOFDAY, CLOSEOFDAY,
        OtherCashIn_Income, OtherCashIn_Expense, OtherCashIn_Income_Withdrawal, MMC_IN, MMC_OUT, OMC_IN, OMC_OUT

    }
    public enum AppType
    {
        MobileApp,
        ThirdPartyApp
    }

    public enum ServiceType
    {
        GAV,
        CMONEY
    }
    public enum DirectionType
    {

        AuxillaryToBranch, HeadOfficeToBranch, BranchToBranch, BranchToHeadOffice
    }
    //public enum OperationEventAttributeTypes
    //{
    //    Cash_Deposit_Operations,
    //    Initial_Deposit_Operations,
    //    Check_Deposit_Operations,
    //    Withdrawal_Operations,
    //    Transfer_Operations,
    //    Management_Operations,
    //    Clossing_Of_Account_Operations,
    //    Openning_Of_Account_Operations,
    //    Teller_Operation_Account,
    //}

    public enum OperationEventRubbriqueName
    {
        Interest,
        Fee,
        Commision,
        Amount,
        Saving_Interest_Account,
        Saving_Interest_Expense_Account,
        Principal_Saving_Account,
        Management_Fee_Account,
        Closing_Fee_Account,
        Other_Fee_Account,
        //Withdrawal_Fee_Account,
        Reopening_Fee_Account,
        Transfer_Fee_Account,
        Transfer_Principal_Account,
        Loan_Principal_Account,
        Loan_VAT_Account,
        Loan_Interest_Account,
        Loan_Penalty_Account,
        Saving_Fee_Account,

        CashIn_Commission_Account,
        CashOut_Commission_Account,
        HeadOfficeShareCashInCommission,
        FluxAndPTMShareCashInCommission,
        CamCCULShareCashInCommission,
        HeadOfficeShareCashOutCommission,
        FluxAndPTMShareCashOutCommission,
        CamCCULShareCashOutCommission,


        HeadOfficeShareTransferCommission,
        FluxAndPTMShareTransferCommission,
        CamCCULShareTransferCommission,

        HeadOfficeShareCMoneyTransferCommission,
        FluxAndPTMShareCMoneyTransferCommission,
        CamCCULShareCMoneyTransferCommission,
        SourceCMoneyTransferCommission,
        ChartOfAccountIdDestinationCMoneyTransferCommission,

        Liasson_Account,
    }

    public enum OperationEventRubbriqueNameForLoan
    {

        Loan_Principal_Account,
        Loan_VAT_Account,
        Loan_Interest_Recieved_Account,
        Loan_Penalty_Account,
        Loan_Fee_Account,
        Loan_WriteOff_Account,
        Loan_Provisioning_Account,
        Loan_Transit_Account,
        Loan_Product,
    }
    public enum SharingWithPartner
    {
        SourceBrachCommission_Account,
        DestinationBranchCommission_Account,
        HeadOfficeShareCashInCommission,
        FluxAndPTMShareCashInCommission,
        CamCCULShareCashInCommission,
        HeadOfficeShareCashOutCommission,
        FluxAndPTMShareCashOutCommission,
        CamCCULShareCashOutCommission,

        TRUST_SOFT_CREDIT_SHARING,
        C_MONWY_SHARING,

    }
    public enum CloseOfDayStatus
    {
        OOD,
        NEGATIVE_OOD,
        POSITIVE_OOD,
        OK,
        CLOSED,
        NEGATIVE_COD,
        POSITIVE_COD,
        Under_Review,
    }
    public enum CloseOfDayActions
    {
        Closed,
        Await_Primary_Teller_Confirmation,
        Await_Accountant_Confirmation,
        Under_Review,
        Not_Confirmed, Open_Of_The_Day
    }
    public enum EventCode
    {
        Cash_To_Vault,
        Vault_To_Cash,
        Subteller_Cash_To_PrimaryTeller,
        Cashout_To_Primary_Teller,
    }


    public enum Events
    {
        None,
        EntryFee,
        ChargeOfDeposit, MomokcashCollection,
        Charge_Of_Saving_Withdrawal_Form,
        Inter_Branch_Commission,
        ChargeOfTransfer,
        LoanFee,
        LoanFee_Refinancing,
        Loan_Refinancing,
        WithDrawalCharges_Fee,
        WithDrawalCharges_Tax,
        WithDrawalAmount,
        ManagementFee,
        AccountClossingFee,
        InitialDeposit,
        Interest, Deposit, Withdrawal, Transfer,
    }
    public enum TransactionStatus
    {
        PENDING,
        CANCELLED,
        FAILED,
        COMPLETED
    }

    public enum ManagementFeeFrequency
    {
        DAILY,
        WEEKLY,
        MONTHLY,
        YEARLY,
        NONE
    }

    public enum PostingFrequency
    {
        DAILY,
        WEEKLY,
        MONTHLY,
        YEARLY,
        NONE
    }
    public enum MemberType
    {
        Physical_Person,
        Moral_Person
    }
    public enum OperationFeeType
    {
        MemberShip,
        OperationFee
    }
    public enum MembershipApprovalStatus
    {
        Awaits_Validation,
        Approved
    }
    public enum Currency
    {
        XAF,
        USD,
    }

    public enum InterestCalculationFrequency
    {
        DAILY,
        WEEKLY,
        MONTHLY,
        YEARLY,
        NONE
    }

    public enum AccountStatus
    {
        Inprogress,
        Inactive,
        Active,
        Approved,
        approved,
        Blocked,
        Closed,
        Open
    }
    public enum AccountType
    {
        PreferenceShare,
        MemberShare,
        Deposit,
        Saving, Salary,
        Loan,
        Atm,
        Gav,
        DailyCollection,
        WesternUnion,
        MoneyGram,
        Ria,
        OFX,
        MPesa,
        Payoneer,
        WorldRemit,
        Membership,
        MobileMoneyMTN,
        MobileMoneyORANGE,
        Teller,
        MomocashCollectionMTN,
        MomocashCollectionOrange,
    }

    public enum SalaryExecution
    {
        PreferenceShare, 
        Charges, 
        NetSalary,
        MemberShare,
        Deposit,
        Saving, 
        Salary,
        Loan,
       
    }



    public enum Status
    {
        Approved,
        Successful, Failed,
        Pending,
        Rejected, Treated, Completed, Validated, Withdrawn, Paid

    }

    public enum AgeCategoryStatus
    {
        Major,
        Minor

    }

    
    public enum SalaryProcessingStatus
    {
        Processed,
        Extracted,
        Approved,
        Completed,
        Validated, Extraction

    }
    public enum FileCategory
    {
        SalaryModelExtraction,
        SalaryProcessing, SalaryAnalysisExtract
    }
    //FileCategory
    public enum XAFDenomination
    {
        Note10000,
        Note5000,
        Note2000,
        Note1000,
        Note500,
        Coin500,
        Coin100,
        Coin50,
        Coin25,
        Coin10,
        Coin5,
        Coin1
    }
    /// <summary>
    /// Enumeration representing various HTTP status codes and their corresponding descriptions.
    /// </summary>
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
        StandingOrder = 504,
        AutoAccountCreation = 505
    }

    /// <summary>
    /// Represents the various actions that can be logged in the application.
    /// </summary>
    public enum LogAction
    {
        CashCeillingCashOutRequest,
        GetLoanRepaymentOrderTypes,
        CashCeillingCashOutValidation,
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
        LoanApprovalPostingCommand,

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
        CashReplenishment_57,
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
        VaultCashMovement,
        VaultTransfer,
        DinominationValidationError,
        VaultUpdate,
        VaultCreation,
        VaultRetrieval,
        CashInitialization,
        CashCeilingRequestRetrieval,
        CashCeilingRequestDeletion,
        VaultInitialization,
        CashChangeOperationSubTill,
        VaultOperationChange,
        TellerOperationChange,
        CashChangeOperationPrimaryTill,
        RetrieveCashChangeHistory,
        RetrieveCashChangeById,
        AccountingDayOpen,
        ValidateProvisioningHistoriesForAccountingOpenOfDay,
        AccountingDayClose,
        TillValidationForAccountingDayClose,
        AccountingDayAction,
        CashRequisitionSubteller,
        SalaryUpload,
        QuerySalaryUpload,
        SalaryFileUploading,
        SalaryAnalysis,
        GetSalaryAnalysisResult,
        DeleteSalaryAnalysisResult,
        ActivateDeactivateSalaryFile,
        DeleteFileUpload,
        SalaryAnalysisRetrivalOfSalaryMembers,
        CashInitializationAccountingPosting,
        PushNotificationFailed,
        PushNotificationCompleted,
        PushNotificationInitiated,
        MemberNoneCashOperationRequest,
        MemberNoneCashOperationDelete,
        GetMemberNoneCashOperations,
        GetMemberNoneCashOperationById,
        NoneCashOperationValidation,
        MobileMoneyTopUpError,
        ValidateMobileMoneyCashTopup,
        CloseAccountingDayOverall,
        CloseAccountingDayError,
        CloseAccountingDayBranch,
        UpdateSavingProduct,
        AddAccountingAccountType,
        MomocashCollectionDepositProcessed,
        MomocashCollectionLoanRepayment,
        AccountingDayRetrieved,
        TellerCloseDay,
        TillOpen,
        TillBalanceQuery,
        AnalysedSalaryFileUpload,
        SalaryProcessing,
        ReversalProcessed,
        ApprovedReversalRequest,
        ReversalValidation,
        SkipAutoAccountCreation,
        AutoAccountCreation,
        LoanFeePayment,
        SalaryStandingOrderLoanRefund,
        SalaryFileProcessing,
        SalaryPaymentProcessing,
        GetBranchLoansForSalaryAnalysis,
        FailedSalaryExecutionMogoRollback,
        GetAllCustomersByTransactionService,
        MigrationProcessing,
        TransactionDeletionForMigration,
        AccountDeletionForMigration,
        AccountInsertionForMigration,
        TransactionInsertionForMigration,
        AccounBalanceMigrationCompletion,
        AccountBalanceMigrationProcessing,
        AccountBalanceMigrationQueueProcessing,
        AccountBalanceMigrationQueueDequeue,
        AccountBalanceMigrationQueueEnqueue,
        AccountBalanceMigrationCompletion,
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


    public enum TermDepositDurations
    {
        DAILY,
        MONTHLY,
        WEEKLY, NONE

    }
}
