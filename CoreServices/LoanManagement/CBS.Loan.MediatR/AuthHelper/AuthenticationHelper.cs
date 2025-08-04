using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.Helper.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.AuthHelper
{
    public static class AuthenticationHelper
    {
        public static async Task<bool> AuthenticateAsync(
            PathHelper pathHelper,
            ILogger logger,
            TokenStorageService tokenStorageService)
        {
            try
            {
                AuthRequest auth = new AuthRequest(pathHelper.UserName, pathHelper.Password);
                string stringifyData = JsonConvert.SerializeObject(auth);

                logger.LogInformation("Authentication started at {StartTime} for loans microservice.", DateTime.Now);

                var serviceResponse = await APICallHelper.AuthenthicationAuto<ServiceResponse<LoginDto>>(pathHelper, stringifyData);

                if (serviceResponse.StatusCode == 200 && serviceResponse.Data != null)
                {
                    logger.LogInformation("Authentication succeeded at {Time}.", DateTime.Now);

                    // Store token and user details in TokenStorageService
                    tokenStorageService.SetToken(serviceResponse.Data.bearerToken, serviceResponse.Data.firstName);
                    logger.LogInformation("Token and user details stored successfully.");

                    return true;
                }
                else
                {
                    logger.LogWarning("Authentication failed at {Time}. Status Code: {StatusCode}", DateTime.Now, serviceResponse.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during authentication.");
                return false;
            }
        }
    }

}
