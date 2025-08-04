using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Notifications.Commands;
using CBS.NLoan.Repository.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.Notifications.Handlers
{
    /// <summary>
    /// Handles the command to delete a Loan based on DeleteLoanCommand.
    /// </summary>
    public class DeleteOTPNotificationHandler : IRequestHandler<DeleteOTPNotificationCommand, ServiceResponse<bool>>
    {
        private readonly IOTPNotificationRepository _OTPNotificationRepository; // Repository for accessing OTPNotification data.
        private readonly ILogger<DeleteOTPNotificationHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteOTPNotificationCommandHandler.
        /// </summary>
        /// <param name="OTPNotificationRepository">Repository for OTPNotification data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteOTPNotificationHandler(
            IOTPNotificationRepository OTPNotificationRepository, IMapper mapper,
            ILogger<DeleteOTPNotificationHandler> logger, IUnitOfWork<LoanContext> uow)
        {
            _OTPNotificationRepository = OTPNotificationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteOTPNotificationCommand to delete a OTPNotification.
        /// </summary>
        /// <param name="request">The DeleteOTPNotificationCommand containing OTPNotification ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteOTPNotificationCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the OTPNotification entity with the specified ID exists
                var existingOTPNotification = await _OTPNotificationRepository.FindAsync(request.Id);
                if (existingOTPNotification == null)
                {
                    errorMessage = $"OTPNotification with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingOTPNotification.IsDeleted = true;
                _OTPNotificationRepository.Update(existingOTPNotification);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting OTPNotification: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
