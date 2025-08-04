using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;

namespace CBS.BankMGT.Helper
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
                return _configuration["SMSSettings:SendSMS"];
            }
        }
        public string SMSSenderName
        {
            get
            {
                return _configuration["SMSSettings:SMSSenderName"];
            }
        }

        public string DocumentUploadPath
        {
            get
            {
                return _configuration["DocumentUploadPath"];
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
        public string BankLogoPath
        {
            get
            {
                return _configuration["BankLogo:BankLogoPath"];
            }
        }
        public string BranchLogoPath
        {
            get
            {
                return _configuration["BankLogo:BranchLogoPath"];
            }
        }
        public string LogoBaseUrl
        {
            get
            {
                return _configuration["Logo:LogoBaseUrl"];
            }
        }
    }
}
