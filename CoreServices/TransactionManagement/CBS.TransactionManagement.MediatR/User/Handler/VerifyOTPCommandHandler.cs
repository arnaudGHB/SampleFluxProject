using AutoMapper;
using CBS.TransactionManagement.Data.Dto.User;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.User.Command;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.TransactionManagement.User.Handler
{
    public class VerifyOTPCommandHandler : IRequestHandler<VerifyOTPCommand, ServiceResponse<OTPDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly ILogger<VerifyOTPCommandHandler> _logger; // Logger for logging handler actions and errors.
        public VerifyOTPCommandHandler(
            IMapper mapper,
            UserInfoToken UserInfoToken,
            ILogger<VerifyOTPCommandHandler> logger,
            PathHelper pathHelper = null)

        {
            _userInfoToken = UserInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        public async Task<ServiceResponse<OTPDto>> Handle(VerifyOTPCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                string stringifyData = JsonConvert.SerializeObject(request);
                var customerData = await APICallHelper.PostData<ServiceResponse<OTPDto>>(_pathHelper.IdentityBaseUrl, _pathHelper.VerifyOTPUrl, stringifyData, _userInfoToken.Token);
                return customerData;
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while Verifying OTP: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<OTPDto>.Return500(errorMessage);
            }
        }
    }
}

