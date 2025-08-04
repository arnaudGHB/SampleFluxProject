using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.SMS;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.SMS.Command;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.SMS.Handler
{
    public class SendSMSPICallCommandHandler : IRequestHandler<SendSMSPICallCommand, ServiceResponse<SMSDto>>
    {
        private readonly ILogger<SendSMSPICallCommandHandler> _logger; // Logger for logging handler actions and errors.
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
        public SendSMSPICallCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<SendSMSPICallCommandHandler> logger,
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
        public async Task<ServiceResponse<SMSDto>> Handle(SendSMSPICallCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.senderService = _pathHelper.SmsServiceName;
                // Serialize the command request
                string serializedRequest = JsonConvert.SerializeObject(request);
                if (_userInfoToken.Token==null)
                {
                    _userInfoToken.Token = request.Token;
                }
                // Call the sms API
                var serviceResponse = await APICallHelper.SMSAPICalls<ServiceResponse<SMSDto>>(_pathHelper, _userInfoToken.Token, serializedRequest, _pathHelper.SendSingleSMSURL);

                // Return the response
                return ServiceResponse<SMSDto>.ReturnResultWith200(serviceResponse.Data);
            }
            catch (Exception e)
            {
                // Log the error
                string errorMessage = $"Error occurred while sending SMS via SMS API: {e.Message}";
                _logger.LogError(errorMessage);

                // Log the audit error
                await LogAuditError(request, errorMessage);

                // Return a 500 Internal Server Error response with the error message
                return ServiceResponse<SMSDto>.Return500(e, errorMessage);
            }
        }

        private async Task LogAuditError(SendSMSPICallCommand request, string errorMessage)
        {
            // Serialize the request data
            string serializedRequest = JsonConvert.SerializeObject(request);

            // Log the audit error
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), serializedRequest, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
        }

    }
}
