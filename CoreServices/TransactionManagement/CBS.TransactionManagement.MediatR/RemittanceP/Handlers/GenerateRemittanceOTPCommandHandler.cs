using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.RemittanceP.Commands;
using CBS.TransactionManagement.Repository.RemittanceP;
using CBS.TransactionManagement.Data.Dto.User;
using CBS.TransactionManagement.MediatR.OTP.Commands;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data.Dto.OTP;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Handlers
{
    /// <summary>
    /// Handles the command to delete a Remittance based on GenerateRemittanceOTPCommand.
    /// </summary>
    public class GenerateRemittanceOTPCommandHandler : IRequestHandler<GenerateRemittanceOTPCommand, ServiceResponse<TempOTPDto>>
    {
        private readonly ILogger<GenerateRemittanceOTPCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _mediator; // AutoMapper for object mapping.

        /// <summary>
        /// Constructor for initializing the GenerateRemittanceOTPCommandHandler.
        /// </summary>
        /// <param name="RemittanceRepository">Repository for Remittance data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GenerateRemittanceOTPCommandHandler(IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<GenerateRemittanceOTPCommandHandler> logger
, IUnitOfWork<TransactionContext> uow,
IMediator mediator)
        {
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
            this._mediator=mediator;
        }

        public async Task<ServiceResponse<TempOTPDto>> Handle(GenerateRemittanceOTPCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Remittance entity with the specified ID exists
                var generateTemporalOTPCommand = new GenerateTemporalOTPCommand { Id = request.RemittanceReference };
                var resultgenerateTemporalOTPCommand = await _mediator.Send(generateTemporalOTPCommand);

                if (resultgenerateTemporalOTPCommand != null)
                {
                    var response = resultgenerateTemporalOTPCommand.Data;

                    // Message for the receiver
                    string receiverMsg = $"Your OTP for cashout is {response.Otp}. It expires in {response.ExpireDate.Subtract(BaseUtilities.UtcNowToDoualaTime()).Minutes} minutes. Please provide this OTP to the cashier to complete your transaction.";
                    var sendSMSPICallCommand = new SendSMSPICallCommand { messageBody = receiverMsg, recipient = request.ReceiverPhoneNumber };
                    var resultSMS = await _mediator.Send(sendSMSPICallCommand);

                    return ServiceResponse<TempOTPDto>.ReturnResultWith200(response, $"OTP {response.Otp} sent successfully to member {request.ReceiverPhoneNumber}");
                }

                return ServiceResponse<TempOTPDto>.Return403(resultgenerateTemporalOTPCommand.Message);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while processing OTP generation: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                     errorMessage,
                     request,
                     HttpStatusCodeEnum.InternalServerError,
                     LogAction.Remittance,
                     LogLevelInfo.Error);
                return ServiceResponse<TempOTPDto>.Return500(e);
            }
        }
    }

}
