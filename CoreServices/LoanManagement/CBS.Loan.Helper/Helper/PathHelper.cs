using Microsoft.Extensions.Configuration;

namespace CBS.NLoan.Helper.Helper
{
    public class PathHelper
    {

        public IConfiguration _configuration;

        public PathHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string DocumentPath
        {
            get
            {
                return _configuration["DocumentPath"];
            }
        }
        public string UploadPath
        {
            get
            {
                return _configuration["UploadPath"];
            }
        }
        public string LoanDocumentPath
        {
            get
            {
                return _configuration["LoanDocumentPath"];
            }
        }

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
                return _configuration["Identity:UserName"];
            }
        }
        public string Password
        {
            get
            {
                return _configuration["Identity:Password"];
            }
        }
        public string IdentityBaseUrl
        {
            get
            {
                return _configuration["Identity:IdentityBaseUrl"];
            }
        }
        public string LoginURL
        {
            get
            {
                return _configuration["Identity:LoginURL"];
            }
        }
        //
        public string AuthenthicationUrl
        {
            get
            {
                return _configuration["Identity:AuthenthicationUrl"];
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
        public string SMSE
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
        public string CustomerUrlGet
        {
            get
            {
                return _configuration["Customer:CustomerUrlGet"];
            }
        }
        public string BankConfigurationBaseUrl
        {
            get
            {
                return _configuration["BankConfiguration:BankConfigurationBaseUrl"];
            }
        }
        public string BranchGetBranchUrlEndPointUrl
        {
            get
            {
                return _configuration["BankConfiguration:BranchGetBranchUrlEndPointUrl"];
            }
        }
        public string EconomicActivitiesEndPointURL
        {
            get
            {
                return _configuration["BankConfiguration:EconomicActivitiesEndPointURL"];
            }
        }
        public string CustomerBaseUrl
        {
            get
            {
                return _configuration["Customer:CustomerBaseUrl"];
            }
        }
        public string AccountingAccoutTypeCreateURL
        {
            get
            {
                return _configuration["Accounting:AccountingAccoutTypeCreateURL"];
            }
        }
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
        public string SMSEnvironment
        {
            get
            {
                return _configuration["SMS:SMSEnvironment"];
            }
        }
        //
        public string SendSingleSMSURL
        {
            get
            {
                return _configuration["SMS:SendSingleSMSURL"];
            }
        }
        public string OpenAndCloseOfTheDayURL
        {
            get
            {
                return _configuration["Accounting:OpenAndCloseOfTheDayURL"];
            }
        }
        public string LoanApprovalPostingURL
        {
            get
            {
                return _configuration["Accounting:LoanApprovalPostingURL"];
            }
        }
        public string AccountingTransferPostingURL
        {
            get
            {
                return _configuration["Accounting:AccountingTransferPostingURL"];
            }
        }
        public string GetAccountByCustomerIDURL
        {
            get
            {
                return _configuration["Transaction:GetAccountByCustomerIDURL"];
            }
        }
        public string TransactionBaseUrl
        {
            get
            {
                return _configuration["Transaction:TransactionBaseUrl"];
            }
        }
        public string BlockOrUnblockAccountUrl
        {
            get
            {
                return _configuration["Transaction:BlockOrUnblockAccountUrl"];
            }
        }
        public string BlockListOfAccountBalanceUrl
        {
            get
            {
                return _configuration["Transaction:BlockListOfAccountBalanceUrl"];
            }
        }
        public string AddLoanAccountUrl
        {
            get
            {
                return _configuration["Transaction:AddLoanAccountUrl"];
            }
        }
        public string GenerateTemporalOTPCodeUrl
        {
            get
            {
                return _configuration["Identity:GenerateTemporalOTPCodeUrl"];
            }
        }
        public string VerifyTemporalOTPCodeUrl
        {
            get
            {
                return _configuration["Identity:VerifyTemporalOTPCodeUrl"];
            }
        }
        public string UpdateLoanAccountBalanceUrl
        {
            get
            {
                return _configuration["Transaction:UpdateLoanAccountBalanceUrl"];
            }
        }
        public string MakeDisburstmentUrl
        {
            get
            {
                return _configuration["Transaction:MakeDisburstmentUrl"];
            }
        }
        public string CreditLoanAccountUrl
        {
            get
            {
                return _configuration["Transaction:CreditLoanAccountUrl"];
            }
        }
        public string AccountingBaseURL
        {
            get
            {
                return _configuration["Accounting:AccountingBaseURL"];
            }
        }
    }
}
