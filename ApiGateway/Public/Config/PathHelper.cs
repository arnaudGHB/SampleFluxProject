using Microsoft.Extensions.Configuration;


namespace CBS.Gateway.API
{
    public class PathHelper
    {
        
        public IConfiguration _configuration;

        public PathHelper(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
     
        public string LoggerPath
        {
            get
            {
                return _configuration["LoogerSetting:LoggerPath"];
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
        public string IdentityServerEndPoint
        {
            get
            {
                return _configuration["Authentication:IdentityServerEndPoint"];
            }
        }
        public string AuthenthicationEndPoint
        {
            get
            {
                return _configuration["Authentication:AuthenthicationEndPoint"];
            }
        }

    }
}
