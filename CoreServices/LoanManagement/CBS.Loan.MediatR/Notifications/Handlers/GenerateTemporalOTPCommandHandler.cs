using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.CustomerAccountDto;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CustomerP.Command;
using CBS.NLoan.MediatR.CustomerP.Query;
using CBS.NLoan.MediatR.Notifications.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.NLoan.MediatR.Notifications.Handler
{
    public class GenerateTemporalOTPCommandHandler : IRequestHandler<GenerateTemporalOTPCommand, ServiceResponse<TempOTPDto>>
    {
        private readonly ILogger<GenerateTemporalOTPCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddAccountingEventCommandHandler.
        /// </summary>
        /// <param name="AccountingEventRepository">Repository for AccountingEvent data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GenerateTemporalOTPCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<GenerateTemporalOTPCommandHandler> logger,
            PathHelper pathHelper,
            IMediator mediator)
        {

            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddAccountingEventCommand to add a new AccountingEvent.
        /// </summary>
        /// <param name="request">The AddAccountingEventCommand containing AccountingEvent data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TempOTPDto>> Handle(GenerateTemporalOTPCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                string serializedRequest = JsonConvert.SerializeObject(request);
                var serviceResponse = await APICallHelper.PostRequest<ServiceResponse<TempOTPDto>>(_pathHelper.IdentityBaseUrl, _userInfoToken.Token, serializedRequest, _pathHelper.GenerateTemporalOTPCodeUrl);
                if (serviceResponse.StatusCode != 200)
                {
                    string errorMessage = $"Generating Temporal OTP Failed: {serviceResponse.Message}";
                    _logger.LogError(errorMessage);
                    await LogAuditError(request, errorMessage);
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }
                return serviceResponse;

            }
            catch (Exception e)
            {
                // Log the error
                string errorMessage = $"Error occure while Generating Temporal OTP: {e.Message}";
                _logger.LogError(errorMessage);

                // Log the audit error
                await LogAuditError(request, errorMessage);

                // Return a 500 Internal Server Error response with the error message
                return ServiceResponse<TempOTPDto>.Return500(errorMessage);
            }
        }

        private async Task LogAuditError(GenerateTemporalOTPCommand request, string errorMessage)
        {
            // Serialize the request data
            string serializedRequest = JsonConvert.SerializeObject(request);

            // Log the audit error
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), serializedRequest, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
        }

    }
}
