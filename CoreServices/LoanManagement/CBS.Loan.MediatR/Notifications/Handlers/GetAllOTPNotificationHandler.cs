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
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllOTPNotificationHandler : IRequestHandler<GetAllOTPNotificationQuery, ServiceResponse<List<OTPNotificationDto>>>
    {
        private readonly IOTPNotificationRepository _OTPNotificationRepository; // Repository for accessing OTPNotifications data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllOTPNotificationHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOTPNotificationQueryHandler.
        /// </summary>
        /// <param name="OTPNotificationRepository">Repository for OTPNotifications data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllOTPNotificationHandler(
            IOTPNotificationRepository OTPNotificationRepository,
            IMapper mapper, ILogger<GetAllOTPNotificationHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _OTPNotificationRepository = OTPNotificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllOTPNotificationQuery to retrieve all OTPNotifications.
        /// </summary>
        /// <param name="request">The GetAllOTPNotificationQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<OTPNotificationDto>>> Handle(GetAllOTPNotificationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all OTPNotifications entities from the repository
                var entities = await _OTPNotificationRepository.AllIncluding(x=>x.LoanApplication).Where(x => !x.IsDeleted)
                   .ToListAsync();
                return ServiceResponse<List<OTPNotificationDto>>.ReturnResultWith200(_mapper.Map<List<OTPNotificationDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all OTPNotifications: {e.Message}");
                return ServiceResponse<List<OTPNotificationDto>>.Return500(e, "Failed to get all OTPNotifications");
            }
        }
    }
}
