using CBS.SystemConfiguration.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CBS.SystemConfiguration.Helper
{
    public class PathHelper
    {
        public IConfiguration _configuration;

        public PathHelper(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        #region Initial system pathelper

        public string CountryPath
        {
            get
            {
                return _configuration["CountryPath"];
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
        public string FileUploadEndpointURL
        {
            get
            {
                return _configuration["FileUploadServer:IdentityServerBaseUrl"] + _configuration["FileUploadServer:FileUpload"];
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
        #endregion

        /// <summary>
        /// Endpoint to read client reference information during the creation of customer account.
        /// </summary>
        public string CustomerManagement_GetCustomerReferenceInfo
        {
            get
            {
                return _configuration["CustomerManagement:GetCustomerReferenceInfoEndpoint"];
            }
        }
        /// <summary>
        /// Endpoint to get branch information by BrancId.
        /// </summary>
        public string BankManagement_GetBranchInfoEndPoint
        {
            get
            {
                var model =  _configuration["BankManagement:GetBranchInfoEndpoint"];
                return model;
            }
        }
        public string BankManagement_BankCode
        {
            get
            {
                var model = _configuration["BankManagement:BankCode"];
                return model;
            }
        }
  
        /// <summary>
        /// Endpoint to get branch information by BrancId.
        /// </summary>
        public string BankManagementBaseUrl
        {
            get
            {
                var model = _configuration["BankManagement:BaseUrl"] ;
                return model;
            }
        }
        /// <summary>
        /// Endpoint to get all branch information.
        /// </summary>
        public string BankManagement_GetAllBranch
        {
            get
            {
                var model = _configuration["BankManagement:BaseUrl"] + _configuration["BankManagement:GetAllBranch"];
                return model;
            }
        }
        public string TransactionManagement_GetAllTransaction
        {
            get
            {
                var model = _configuration["TransactionManagement:BaseUrl"] + _configuration["TransactionManagement:GetAllTransactionInfoEndpoint"];
                return model;
            }
        }
        public string BankManagement_GetAllBranchInfo
        {
            get
            {
                var model = _configuration["BankManagement:BaseUrl"] + _configuration["BankManagement:GetAllBranchInfoEndpoint"];
                return model;
            }
        }
        public string BankManagement_GetAllBranchInfoGWl
        {
            get
            {
                var model = _configuration["BankManagement:BaseUrl"] + _configuration["BankManagement:GetBranchEndpoint"];
                return model;
            }
        }
        /// <summary>
        /// Endpoint to read transaction Call back end point during cashreplenishment request.
        /// </summary>
        public string TellerProvisioning_EndPoint
        {
            get
            {
                var model = _configuration["TellerProvisioning:BaseUrl"] + _configuration["TellerProvisioning:GetPrimaryEndpoint"];
                return model;
            }
        }
        public string TransferFromTellerToNonMember_EventAttributIdForTransit
        {
            get
            {
                var model = _configuration["TransferFromTellerToNonMember:EventAttributIdForTransit"];
                return model;
            }
        }
        public string TransferFromTellerToNonMember_EventAttributIdForRevenue
        {
            get
            {
                var model = _configuration["TransferFromTellerToNonMember:EventAttributIdForRevenue"];
                return model;
            }
        }

        public string TransferFromTellerToNonMember_EventAttributIdForCashInHand
        {
            get
            {
                var model = _configuration["TransferFromTellerToNonMember:EventAttributIdForCashInHand"];
                return model;
            }
        }
        public string TransferFromMemberToNonMember_EventAttributIdForRevenue
        {
            get
            {
                var model = _configuration["TransferFromTellerToNonMember:EventAttributIdForRevenue"];
                return model;
            }
        }
        

        public string TransferFromMemberToNonMember_EventAttributIdForTransit
        {
            get
            {
                var model = _configuration["TransferFromTellerToNonMember:EventAttributIdForTransit"];
                return model;
            }
        }
        public string WithdrawalTransferFundToNonMemberViaTeller_EventAttributIdForTransit
        {
            get
            {
                var model = _configuration["WithdrawalTransferFundToNonMemberViaTeller:EventAttributIdForTransit"];
                return model;
            }
        }
        public string WithdrawalTransferFundToNonMemberViaTeller_EventAttributIdForCashInHand
        {
            get
            {
                var model = _configuration["WithdrawalTransferFundToNonMemberViaTeller:EventAttributIdForCashInHand"];
                return model;
            }
        }

        /// <summary>
        /// Endpoint to read client reference information during the creation of customer account.
        /// </summary>
        public string FileUpload_MigrationPath
        {
            get
            {
                return _configuration["FileUpload:MigrationPath"];
            }
        }


        /// <summary>
        /// Endpoint to read client reference information during the creation of customer account.
        /// </summary>
        public string FileUpload_MigrationValidatedPath
        {
            get
            {
                return _configuration["FileUpload:MigrationValidated"];
            }
        }
        /// <summary>
        /// Endpoint to read client reference information during the creation of customer account.
        /// </summary>
        public string DomainName
        {
            get
            {
                return _configuration["FileUpload:DomainName"];
            }
        }


        public string BankTransactionCashINAndCashOutURL
        {
            get
            {
                ///
                var model = _configuration["Transaction:BankTransactionCashINAndCashOutURL"];
                return model;
            }
        }

    }
}