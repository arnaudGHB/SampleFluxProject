using AutoMapper;
using CBS.Communication.Helper.Helper;
using CBS.Communication.MediatR.Email.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.Communication.MediatR.Email.Handlers
{
    /// <summary>
    /// Handles the command to send emails.
    /// </summary>
    public class EmailCommandHandler : IRequestHandler<SendEmailSpecificationCommand, ServiceResponse<bool>>
    {
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<EmailCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly PathHelper _pathHelper; // AutoMapper for object mapping.
        /// <summary>
        /// Constructor for initializing the SendEmailCommandHandler.
        /// </summary>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public EmailCommandHandler(
            IMapper mapper,
            ILogger<EmailCommandHandler> logger,
            PathHelper pathHelper
)
        {
            _mapper = mapper;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the SendEmailCommand to notify customer.
        /// </summary>
        /// <param name="request">The AddClaimPolicyCommand containing ClaimPolicy data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(SendEmailSpecificationCommand request, CancellationToken cancellationToken)
        {
            string message = null;
            try
            {
                var emailrequest = new SendEmailSpecification()
                {
                    UserName = _pathHelper.EmailSmtpUserName,
                    Password = _pathHelper.EmailSmtpPassword,
                    Port = Convert.ToInt32(_pathHelper.EmailSmtpPort),
                    Host = _pathHelper.EmailSmtpHost,
                    IsEnableSSL = true,
                    Attachments = request.Attachments,
                    Body = request.Body,
                    CCAddress = request.CCAddress,
                    FromAddress = request.FromAddress,
                    Subject = request.Subject,
                    ToAddress = request.ToAddress
                };

                EmailHelper.SendEmail(emailrequest);

                return ServiceResponse<bool>.ReturnResultWith200(true);

            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                message = $"Error occurred while connecting to remote identity server: {e.Message}";
                _logger.LogError(message);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
