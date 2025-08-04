using AutoMapper;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Notifications.Queries;
using CBS.NLoan.Repository.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.Notifications.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetOTPNotificationHandler : IRequestHandler<GetOTPNotificationQuery, ServiceResponse<OTPNotificationDto>>
    {
        private readonly IOTPNotificationRepository _OTPNotificationRepository; // Repository for accessing OTPNotification data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetOTPNotificationHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetOTPNotificationQueryHandler.
        /// </summary>
        /// <param name="OTPNotificationRepository">Repository for OTPNotification data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetOTPNotificationHandler(
            IOTPNotificationRepository OTPNotificationRepository,
            IMapper mapper,
            ILogger<GetOTPNotificationHandler> logger)
        {
            _OTPNotificationRepository = OTPNotificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetOTPNotificationQuery to retrieve a specific OTPNotification.
        /// </summary>
        /// <param name="request">The GetOTPNotificationQuery containing OTPNotification ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OTPNotificationDto>> Handle(GetOTPNotificationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the OTPNotification entity with the specified ID from the repository
                var entity = await _OTPNotificationRepository.AllIncluding(x=>x.LoanApplication).FirstOrDefaultAsync(x=>x.Id==request.Id);
                if (entity != null)
                {
                    // Map the OTPNotification entity to OTPNotificationDto and return it with a success response
                    var OTPNotificationDto = _mapper.Map<OTPNotificationDto>(entity);
                    return ServiceResponse<OTPNotificationDto>.ReturnResultWith200(OTPNotificationDto);
                }
                else
                {
                    // If the OTPNotification entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("OTPNotification not found.");
                    return ServiceResponse<OTPNotificationDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting OTPNotification: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<OTPNotificationDto>.Return500(e);
            }
        }
    }

}
