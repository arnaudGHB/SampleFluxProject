using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Notifications.Commands;
using CBS.NLoan.Repository.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.Notifications.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateOTPNotificationHandler : IRequestHandler<UpdateOTPNotificationCommand, ServiceResponse<OTPNotificationDto>>
    {

        private readonly IOTPNotificationRepository _OTPNotificationRepository; // Repository for accessing OTPNotification data.
        private readonly ILogger<UpdateOTPNotificationHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateOTPNotificationCommandHandler.
        /// </summary>
        /// <param name="OTPNotificationRepository">Repository for OTPNotification data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateOTPNotificationHandler(
            IOTPNotificationRepository OTPNotificationRepository,
            ILogger<UpdateOTPNotificationHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null)
        {
            _OTPNotificationRepository = OTPNotificationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateOTPNotificationCommand to update a OTPNotification.
        /// </summary>
        /// <param name="request">The UpdateOTPNotificationCommand containing updated OTPNotification data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OTPNotificationDto>> Handle(UpdateOTPNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the OTPNotification entity to be updated from the repository
                var existingOTPNotification = await _OTPNotificationRepository.FindAsync(request.Id);

                // Check if the OTPNotification entity exists
                if (existingOTPNotification != null)
                {
                    // Update OTPNotification entity properties with values from the request
                   _mapper.Map(request, existingOTPNotification);
                    // Use the repository to update the existing OTPNotification entity
                    _OTPNotificationRepository.Update(existingOTPNotification);
                   // await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<OTPNotificationDto>.ReturnResultWith200(_mapper.Map<OTPNotificationDto>(existingOTPNotification));
                    _logger.LogInformation($"OTPNotification status was successfully updated.");
                    return response;
                }
                else
                {
                    // If the OTPNotification entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"OTP notification {request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<OTPNotificationDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating OTPNotification: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<OTPNotificationDto>.Return500(e);
            }
        }
    }

}
