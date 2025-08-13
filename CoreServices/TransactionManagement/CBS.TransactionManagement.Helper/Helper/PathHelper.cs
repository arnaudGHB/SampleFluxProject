using Microsoft.Extensions.Configuration;
//using SixLabors.ImageSharp;

namespace CBS.TransactionManagement.Helper
{
    public class PathHelper
    {
        
        public IConfiguration _configuration;

        public PathHelper(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public string DocumentPath
        {
            get
            {
                return _configuration["DocumentPath"];
            }
        }

        public string VerifyTemporalOTPCodeUrl
        {
            get
            {
                return _configuration["Identity:VerifyTemporalOTPCodeUrl"];
            }
        }
        public string LoanApprovalPostingURL
        {
            get
            {
                return _configuration["Accounting:LoanApprovalPostingURL"];
            }
        }
        public string SourceBaseURL
        {
            get
            {
                return _configuration["AuditTrails:BaseUrl"];
            }
        }
        public string SourceUrl
        {
            get
            {
                return _configuration["AuditTrails:SourceUrl"];
            }
        }
        //
        public string SMSAPIBaseEdnPoint
        {
            get
            {
                return _configuration["SMSSettings:SMSAPIBaseEdnPoint"];
            }
        }
        public string KYCAPIBaseEdnPoint
        {
            get
            {
                return _configuration["KYCSettings:KYCAPIBaseEdnPoint"];
            }
        }
        public string KYCEndPointURL
        {
            get
            {
                return _configuration["KYCSettings:KYCEndPointURL"];
            }
        }
        public string SubscriptionBaseEndPoint
        {
            get
            {
                return _configuration["SubcriptionSettings:SubscriptionBaseEndPoint"];
            }
        }
        public string SubscriptionURL
        {
            get
            {
                return _configuration["SubcriptionSettings:SubscriptionURL"];
            }
        }
      
        public string SubscriptionCallBackURL
        {
            get
            {
                return _configuration["SubcriptionSettings:SubscriptionCallBackURL"];
            }
        }
        public string ComapanyName
        {
            get
            {
                return _configuration["SMSSettings:ComapanyName"];
            }
        }
        public string SpUserName
        {
            get
            {
                return _configuration["BankSettings:SpUserName"];
            }
        }

       
        public string URLToCompleteSubscription
        {
            get
            {
                return _configuration["WebSIteURL:URLToCompleteSubscription"];
            }
        }
        public string UserName
        {
            get
            {
                return _configuration["Authentication:UserName"];
            }
        }
        public string Password
        {
            get
            {
                return _configuration["Authentication:Password"];
            }
        }
        public string IdentityServerBaseUrl
        {
            get
            {
                return _configuration["Authentication:IdentityServerBaseUrl"];
            }
        }
        public string AuthenthicationUrl
        {
            get
            {
                return _configuration["Authentication:AuthenthicationUrl"];
            }
        }
        public string WebSIteURL
        {
            get
            {
                return _configuration["WebSiteSettings:WebSIteURL"];
            }
        }
        public int SubscriptionAmount
        {
            get
            {
                return Convert.ToInt32(_configuration["BankSettings:SubscriptionAmount"]);
            }
        }
        public string SpPassword
        {
            get
            {
                return _configuration["BankSettings:SpPassword"];
            }
        }
        public string SendSMSURL
        {
            get
            {
                return _configuration["BankSettings:SendSMS"];
            }
        }
        public string SMSSenderName
        {
            get
            {
                return _configuration["BankSettings:SMSSenderName"];
            }
        }

        public string UserProfilePath
        {
            get
            {
                return _configuration["UserProfilePath"];
            }
        }
        public string UserIDCardFrontSidePath
        {
            get
            {
                return _configuration["UserIDCardFrontSidePath"];
            }
        }
        public string UserIDCardBackSidePath
        {
            get
            {
                return _configuration["UserIDCardBackSidePath"];
            }
        }
        public string OpenAPIServiceType
        {
            get
            {
                return _configuration["OpenAPISettings:OpenAPIServiceType"];
            }
        }
        public string OpenAPIBaseEdnPoint
        {
            get
            {
                return _configuration["OpenAPISettings:OpenAPIBaseEdnPoint"];
            }
        }
        public string OpenAPISubscriptionURL
        {
            get
            {
                return _configuration["OpenAPISettings:OpenAPISubscriptionURL"];
            }
        }
        public string OpenAPISubscriptionCallBackURL
        {
            get
            {
                return _configuration["OpenAPISettings:OpenAPISubscriptionCallBackURL"];
            }
        }
        public string OpenAPIBankID
        {
            get
            {
                return _configuration["OpenAPISettings:OpenAPIBankID"];
            }
        }

        public string LoggerPath
        {
            get
            {
                return _configuration["LoogerSetting:LoggerPath"];
            }
        }
        public string FileUploadPath
        {
            get
            {
                return _configuration["LoogerSetting:FileUploadPath"];
            }
        }
        
        //VerifyOTPUrl
        //GenerateOTPUrl
        //Identity
        public string GenerateOTPUrl
        {
            get
            {
                return _configuration["Identity:GenerateOTPUrl"];
            }
        }
        public string IdentityBaseUrl
        {
            get
            {
                return _configuration["Identity:IdentityBaseUrl"];
            }
        }
        public string VerifyOTPUrl
        {
            get
            {
                return _configuration["Identity:VerifyOTPUrl"];
            }
        }
        
        public string CustomerUrlGet
        {
            get
            {
                return _configuration["Customer:CustomerUrlGet"];
            }
        }
        public string GetMembersWithMatriculeByBranch
        {
            get
            {
                return _configuration["Customer:GetMembersWithMatriculeByBranch"];
            }
        }
        public string GetMembers
        {
            get
            {
                return _configuration["Customer:GetMembers"];
            }
        }
        public string PinValidation
        {
            get
            {
                return _configuration["Customer:PinValidation"];
            }
        }
        //
        public string BankConfigurationBaseUrl
        {
            get
            {
                return _configuration["BankConfiguration:BankConfigurationBaseUrl"];
            }
        }
        public string GetAllBranches
        {
            get
            {
                return _configuration["BankConfiguration:GetAllBranches"];
            }
        }
        public string BranchGetBranchUrlEndPointUrl
        {
            get
            {
                return _configuration["BankConfiguration:BranchGetBranchUrlEndPointUrl"];
            }
        }
        public string CustomerBaseUrl
        {
            get
            {
                return _configuration["Customer:CustomerBaseUrl"];
            }
        }
        public string GetAllCustomerListings
        {
            get
            {
                return _configuration["Customer:GetAllCustomerListings"];
            }
        }
        public string CustomerByTelephoneBaseUrl
        {
            get
            {
                return _configuration["Customer:CustomerByTelephoneBaseUrl"];
            }
        }
        public string GenerateTemporalOTPCodeUrl
        {
            get
            {
                return _configuration["Identity:GenerateTemporalOTPCodeUrl"];
            }
        }
        public string ActivateMemberShip
        {
            get
            {
                return _configuration["Customer:ActivateMemberShip"];
            }
        }
        //
        public string AccountingAccoutTypeCreateURL
        {
            get
            {
                return _configuration["Accounting:AccountingAccoutTypeCreateURL"];
            }
        }
        public string MakeNonCashAccountAdjustmentCommandURL
        {
            get
            {
                return _configuration["Accounting:MakeNonCashAccountAdjustmentCommandURL"];
            }
        }
        
        //
        public string GetAccountNumberByIdURL
        {
            get
            {
                return _configuration["Accounting:GetAccountNumberByIdURL"];
            }
        }
        public string GetAccountNumberURL
        {
            get
            {
                return _configuration["Accounting:GetAccountNumberURL"];
            }
        }
        public string AccountingEventPostingURL
        {
            get
            {
                return _configuration["Accounting:AccountingEventPostingURL"];
            }
        }
        public string InternalAutoPostingEventURL
        {
            get
            {
                return _configuration["Accounting:InternalAutoPostingEventURL"];
            }
        }
        public string CashInitializationURL
        {
            get
            {
                return _configuration["Accounting:CashInitializationURL"];
            }
        }
        //
        public string SmsServiceName
        {
            get
            {
                return _configuration["SMS:SmsServiceName"];
            }
        }
        public string SMSBaseURL
        {
            get
            {
                return _configuration["SMS:SMSBaseURL"];
            }
        }
        public string SendSingleSMSURL
        {
            get
            {
                return _configuration["SMS:SendSingleSMSURL"];
            }
        }
        public string PushNotification
        {
            get
            {
                return _configuration["SMS:PushNotification"];
            }
        }
        
        public string OpenAndCloseOfTheDayURL
        {
            get
            {
                return _configuration["Accounting:OpenAndCloseOfTheDayURL"];
            }
        }
        public string AccountingPostingURL
        {
            get
            {
                return _configuration["Accounting:AccountingPostingURL"];
            }
        }
        public string MakeBulkAccountPostingURL
        {
            get
            {
                return _configuration["Accounting:MakeBulkAccountPostingURL"];
            }
        }
        public string AccountingPostingMomoCollectionURL
        {
            get
            {
                return _configuration["Accounting:AccountingPostingMomoCollectionURL"];
            }
        }
        public string MobileMoneyManagementURL
        {
            get
            {
                return _configuration["Accounting:MobileMoneyManagementURL"];
            }
        }
        public string AccountingPostingMomoMobileMoneyURL
        {
            get
            {
                return _configuration["Accounting:AccountingPostingMomoMobileMoneyURL"];
            }
        }
        public string AccountingTransferPostingURL
        {
            get
            {
                return _configuration["Accounting:AccountingTransferPostingURL"];
            }
        }
        public string AccountToAccountTransferByTransctionCodeURL
        {
            get
            {
                return _configuration["Accounting:AccountToAccountTransferByTransctionCodeURL"];
            }
        }
        public string MakeLoanDisbursementPostingURL
        {
            get
            {
                return _configuration["Accounting:MakeLoanDisbursementPostingURL"];
            }
        }
        public string LoanRefinancingPostingURL
        {
            get
            {
                return _configuration["Accounting:LoanRefinancingPostingURL"];
            }
        }
        public string AccountingPostingLoanRefundURL
        {
            get
            {
                return _configuration["Accounting:LoanRefundURL"];
            }
        }
        public string RefundURL
        {
            get
            {
                return _configuration["Loan:RefundURL"];
            }
        }
        public string GetCustomersLoans
        {
            get
            {
                return _configuration["Loan:GetCustomersLoans"];
            }
        }
        public string GetOpenLoansByBranch
        {
            get
            {
                return _configuration["Loan:GetOpenLoansByBranch"];
            }
        }
        //
        public string LoanSOBulkRepayment
        {
            get
            {
                return _configuration["Loan:LoanSOBulkRepayment"];
            }
        }
        

        public string GetLoanProductRepaymentOrder
        {
            get
            {
                return _configuration["Loan:GetLoanProductRepaymentOrder"];
            }
        }
        //MakeRemittanceCommand
        ///api/v1/AccountingEntry/MomoMobileMoney
        public string MomoMobileMoneyURL
        {
            get
            {
                return _configuration["Accounting:MomoMobileMoneyURL"];
            }
        }
        public string MakeRemittanceCommandURL
        {
            get
            {
                return _configuration["Accounting:MakeRemittanceCommandURL"];
            }
        }
        //MakeLoanDisbursementPostingURL
        public string AccountingEventAttributesCreateURL
        {
            get
            {
                return _configuration["Accounting:AccountingEventAttributesCreateURL"];
            }
        }
        public string AccountingBaseURL
        {
            get
            {
                return _configuration["Accounting:AccountingBaseURL"];
            }
        }
        public string LoanBaseURL
        {
            get
            {
                return _configuration["Loan:LoanBaseURL"];
            }
        }
        public string GetCustomerLoan
        {
            get
            {
                return _configuration["Loan:GetCustomerLoan"];
            }
        }
        
        public string AllLoansURL
        {
            get
            {
                return _configuration["Loan:AllLoansURL"];
            }
        }
        public string RepaymentURL
        {
            get
            {
                return _configuration["Loan:RepaymentURL"];
            }
        }
        public string LoanApplicationFeeURL
        {
            get
            {
                return _configuration["Loan:LoanApplicationFeeURL"];
            }
        }
    }
}
