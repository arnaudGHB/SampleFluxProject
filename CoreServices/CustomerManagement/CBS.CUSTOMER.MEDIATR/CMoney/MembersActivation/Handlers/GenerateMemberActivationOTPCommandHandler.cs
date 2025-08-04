using AutoMapper;
using BusinessServiceLayer.Objects.SmsObject;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.OTP;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.OTP.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Command
{
    /// <summary>
    /// Handles the command to generate an OTP for C-MONEY member activation.
    /// </summary>
    public class GenerateMemberActivationOTPCommandHandler : IRequestHandler<GenerateMemberActivationOTPCommand, ServiceResponse<TempOTPDto>>
    {
        private readonly ILogger<GenerateMemberActivationOTPCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IMediator _mediator;
        private readonly SmsHelper _smsHelper;
        private readonly UserInfoToken _userInfoToken;

        public GenerateMemberActivationOTPCommandHandler(
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<GenerateMemberActivationOTPCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            IMediator mediator,
            SmsHelper smsHelper)
        {
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _mediator = mediator;
            _smsHelper = smsHelper;
        }

        /// <summary>
        /// Handles the generation and sending of OTP for C-MONEY member activation.
        /// </summary>
        public async Task<ServiceResponse<TempOTPDto>> Handle(GenerateMemberActivationOTPCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;
            string otpmessage= string.Empty;
            try
            {
                // Generate OTP using another command
                var generateOTPCommand = new GenerateTemporalOTPCommand { Id = request.CustomerId };
                var otpResponse = await _mediator.Send(generateOTPCommand);

                if (otpResponse == null || otpResponse.StatusCode != 200)
                {
                    errorMessage = otpResponse?.Message ?? "Failed to generate OTP.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.Forbidden,
                        LogAction.CMoneyMemberAccountActivation,
                        LogLevelInfo.Warning
                    );
                    return ServiceResponse<TempOTPDto>.Return403(errorMessage);
                }

                var otpData = otpResponse.Data;

                // Prepare SMS message
                string smsMessage = $"TSC:OTP-{otpData.Otp}.";
                request.PhoneNumber= BaseUtilities.Add237Prefix(request.PhoneNumber);
                var smsRequest = new SubSmsRequestDto
                {
                    Message = smsMessage,
                    Msisdn = request.PhoneNumber,
                    Token = _userInfoToken.Token
                };
                // Log success and audit
                string successMessage = $"TSC:OTP-{otpData.Otp} sent successfully to {request.PhoneNumber}.";
                otpmessage=successMessage;
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.CMoneyMemberAccountActivation,
                    LogLevelInfo.Information
                );
                // Send SMS
                await _smsHelper.SendSms(smsRequest);

               

                return ServiceResponse<TempOTPDto>.ReturnResultWith200(otpData, successMessage);
            }
            catch (Exception ex)
            {
                errorMessage = $"Error occurred while generating OTP for CustomerId {request.CustomerId}. OTP message: {otpmessage}. Error: {ex.Message}";
                _logger.LogError(errorMessage);

                // Log and audit error
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.CMoneyMemberAccountActivation,
                    LogLevelInfo.Error
                );

                return ServiceResponse<TempOTPDto>.Return500(errorMessage);
            }
        }
    }

}
