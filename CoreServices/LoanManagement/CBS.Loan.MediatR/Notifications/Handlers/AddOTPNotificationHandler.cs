using AutoMapper;
using Azure;
using CBS.APICaller.Helper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.Data.Entity.Notifications;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CustomerP.Command;
using CBS.NLoan.MediatR.CustomerP.Query;
using CBS.NLoan.MediatR.Notifications.Commands;
using CBS.NLoan.MediatR.SMS.Command;
using CBS.NLoan.Repository.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.Notifications.Handlers
{
    public class AddOTPNotificationHandler : IRequestHandler<AddOTPNotificationCommand, ServiceResponse<OTPNotificationDto>>
    {
        private readonly IOTPNotificationRepository _otpNotificationRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddOTPNotificationHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _unitOfWork;
        private readonly IMediator _mediator;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the AddOTPNotificationHandler.
        /// </summary>
        public AddOTPNotificationHandler(
            IOTPNotificationRepository otpNotificationRepository,
            IMapper mapper,
            ILogger<AddOTPNotificationHandler> logger,
            IUnitOfWork<LoanContext> unitOfWork,
            IMediator mediator = null,
            UserInfoToken userInfoToken = null)
        {
            _otpNotificationRepository = otpNotificationRepository ?? throw new ArgumentNullException(nameof(otpNotificationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mediator = mediator;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddOTPNotificationCommand to add a new OTPNotification.
        /// </summary>
        public async Task<ServiceResponse<OTPNotificationDto>> Handle(AddOTPNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Map the AddOTPNotificationCommand to a OTPNotification entity
                var otpNotificationEntity = _mapper.Map<OTPNotification>(request);
                // Convert UTC to local time and set it in the entity
                otpNotificationEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);

                otpNotificationEntity.Id = BaseUtilities.GenerateUniqueNumber();
                otpNotificationEntity.InitiatedBy = _userInfoToken.FullName;
                otpNotificationEntity.Status = OTPStatuses.Expired.ToString();
                otpNotificationEntity.InitializedDate = otpNotificationEntity.CreatedDate;




                // Call external services using Mediator
                var customerPICallCommand = new GetCustomerCallCommand { CustomerId = request.CustomerId };
                var resultCustomer = await _mediator.Send(customerPICallCommand);

                if (resultCustomer.StatusCode == 200)
                {

                    var generateTemporalOTPCommand = new GenerateTemporalOTPCommand { Id = request.CustomerId };
                    var resultgenerateTemporalOTPCommand = await _mediator.Send(generateTemporalOTPCommand);

                    if (resultgenerateTemporalOTPCommand != null)
                    {
                        var response = resultgenerateTemporalOTPCommand.Data;
                        otpNotificationEntity.OPTCode = response.Otp;
                        string msg = $"TSC:Loan Approval:OTP{otpNotificationEntity.OPTCode}";
                        var sendSMSPICallCommand = new SendSMSPICallCommand { messageBody = msg, recipient = resultCustomer.Data.phone };
                        var resultSMS = await _mediator.Send(sendSMSPICallCommand);
                        var otpNotificationDto = _mapper.Map<OTPNotificationDto>(otpNotificationEntity);
                        _otpNotificationRepository.Add(otpNotificationEntity);
                        await _unitOfWork.SaveAsync();
                        return ServiceResponse<OTPNotificationDto>.ReturnResultWith200(otpNotificationDto, $"OTP {response.Otp} send successfully to member {resultCustomer.Data.phone}");

                    }
                    return ServiceResponse<OTPNotificationDto>.Return403(resultgenerateTemporalOTPCommand.Message);


                }
                return ServiceResponse<OTPNotificationDto>.Return403(resultCustomer.Message);

            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving OTPNotification: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<OTPNotificationDto>.Return500(errorMessage);
            }
        }
    }

}
