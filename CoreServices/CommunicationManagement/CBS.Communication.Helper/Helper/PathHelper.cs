using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;

namespace CBS.Communication.Helper.Helper
{
    public class PathHelper
    {

        public IConfiguration _configuration;

        public PathHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetCMoneyMembersActivationByIdPath
        {
            get
            {
                return _configuration["CustomerService:GetCMoneyMembersActivationByIdPath"];
            }
        }
        public string CustomerBaseUrl
        {
            get
            {
                return _configuration["CustomerService:CustomerBaseUrl"];
            }
        }
        public string DocumentPath
        {
            get
            {
                return _configuration["DocumentPath"];
            }
        }

        public string EmailSmtpUserName
        {
            get
            {
                return _configuration["EmailSettings:UserName"];
            }
        }


        public string EmailSmtpPassword
        {
            get
            {
                return _configuration["EmailSettings:Password"];
            }
        }

        public string EmailSmtpHost
        {
            get
            {
                return _configuration["EmailSettings:Host"];
            }
        }

        public string EmailSmtpPort
        {
            get
            {
                return _configuration["EmailSettings:Port"];
            }
        }


        public string SMSOtherSinglePath
        {
            get
            {
                return _configuration["SMSSettings:OtherSingleSMSAPIBaseEdnPoint"];
            }
        }
        
        public string SMSAPI_KEY
        {
            get
            {
                return _configuration["SMSSettings:SMSAPIKEY"];
            }
        } 
        
        
        public string SMSOtherMultiplePath
        {
            get
            {
                return _configuration["SMSSettings:OtherMultipleSMSAPIBaseEdnPoint"];
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

        public string OnlineAuthenthicationUrl
        {
            get
            {
                return _configuration["Authentication:OnlineAuthenthicationUrl"];
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


        public string MomokashSmsUrl
        {
            get
            {
                return _configuration["Momokash:SmsUrl"];
            }
        }

        public string MomokaskServiceName
        {
            get
            {
                return _configuration["Momokash:ServiceName"];
            }
        }

        public string CreateAccountUrl
        {
            get
            {
                return _configuration["TransactionManagement:CreateAccountUrl"];
            }
        } 
        
        public string GoogleCloudPlatforms
        {
            get
            {
                return _configuration["Firebase:GoogleCloudPlatforms"];
            }
        } 
        
        public string ServiceAccountFilePath
        {
            get
            {
                return _configuration["Firebase:ServiceAccountFilePath"];
            }
        }
        
        public string FcmEndpoint
        {
            get
            {
                return _configuration["Firebase:FcmEndpoint"];
            }
        }

    
    }
}
