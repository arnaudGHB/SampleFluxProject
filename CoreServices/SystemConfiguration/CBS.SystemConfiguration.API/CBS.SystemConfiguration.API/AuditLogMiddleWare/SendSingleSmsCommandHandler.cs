using AutoMapper;
using CBS.Communication.Common;
using CBS.Communication.Data;
using CBS.Communication.Domain;
using CBS.Communication.Helper;
using CBS.Communication.Helper.Helper;
using CBS.Communication.MediatR.Sms.Commands;
using MediatR;

namespace CBS.Communication.MediatR.Sms.Handlers
{
    public class SendSingleSmsCommandHandler : IRequestHandler<SendSingleSmsCommand, ServiceResponse<SendSingleSmsResponse>>
    {

        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<SendSingleSmsCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;
        /// <summary>
        /// Constructor for initializing the AddDepartmentCommandHandler.
        /// </summary>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public SendSingleSmsCommandHandler(
            IMapper mapper,
            ILogger<SendSingleSmsCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            PathHelper pathHelper)
        {

            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _UserInfoToken = userInfoToken;
            _PathHelper = pathHelper;
        }


        public async Task<ServiceResponse<SendSingleSmsResponse>> Handle(SendSingleSmsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.Recipient = BaseUtilities.Add237Prefix(request.Recipient);

                SendSingleSmsSpecificationRequest sendSingleSms = new()
                {
                    Recipient = request.Recipient,
                    MessageBody = request.MessageBody,
                    Sender = request.SenderService

                };

                var sendSingleSmsResult = await SmsHelper.SendSingleSMS(_PathHelper.SMSOtherSinglePath, _PathHelper.SMSAPI_KEY, sendSingleSms);


                return ServiceResponse<SendSingleSmsResponse>.ReturnResultWith200(sendSingleSmsResult);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Department: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<SendSingleSmsResponse>.Return500(e);
            }
        }
    }
}
