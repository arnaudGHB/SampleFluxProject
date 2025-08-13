using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;
using CBS.TransactionManagement.Queries.WithdrawalNotificationP;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;

namespace CBS.TransactionManagement.Handlers.WithdrawalNotificationP
{

    public class GetWithdrawalNotificationMobileAppQueryHandler : IRequestHandler<GetWithdrawalNotificationMobileAppQuery, ServiceResponse<WithdrawalNotificationAndriodDto>>
    {
        private readonly IWithdrawalNotificationRepository _WithdrawalNotificationRepository; // Repository for accessing WithdrawalNotification data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetWithdrawalNotificationMobileAppQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetWithdrawalNotificationMobileAppQueryHandler.
        /// </summary>
        /// <param name="WithdrawalNotificationRepository">Repository for WithdrawalNotification data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetWithdrawalNotificationMobileAppQueryHandler(
            IWithdrawalNotificationRepository WithdrawalNotificationRepository,
            IMapper mapper,
            ILogger<GetWithdrawalNotificationMobileAppQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _WithdrawalNotificationRepository = WithdrawalNotificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetWithdrawalNotificationMobileAppQuery to retrieve a specific WithdrawalNotification.
        /// </summary>
        /// <param name="request">The GetWithdrawalNotificationMobileAppQuery containing WithdrawalNotification ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<WithdrawalNotificationAndriodDto>> Handle(GetWithdrawalNotificationMobileAppQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the most recent WithdrawalNotification based on NotificationDate
                var entity = await _WithdrawalNotificationRepository
                    .FindBy(a => a.AccountNumber == request.AccountNUmber && a.CustomerId == request.CustomerReference)
                    .OrderByDescending(a => a.NotificationDate)
                    .FirstOrDefaultAsync();

                if (entity != null)
                {
                    // Determine status based on NotificationDate, DateOfIntendedWithdrawal, GracePeriodDate, and IsWithdrawalDone
                    string status;
                    DateTime today = DateTime.Now;

                    if (entity.IsWithdrawalDone)
                    {
                        status = WithdrawalNotificationStatus.Completed.ToString();
                    }
                    else if (today < entity.DateOfIntendedWithdrawal)
                    {
                        status = WithdrawalNotificationStatus.Scheduled.ToString();
                    }
                    else if (today >= entity.DateOfIntendedWithdrawal && today <= entity.GracePeriodDate)
                    {
                        status = WithdrawalNotificationStatus.InGracePeriod.ToString();
                    }
                    else
                    {
                        status = WithdrawalNotificationStatus.Expired.ToString();
                    }

                    // Map the WithdrawalNotification entity to WithdrawalNotificationAndriodDto and set additional properties
                    var dto = _mapper.Map<WithdrawalNotificationAndriodDto>(entity);
                    dto.IsExpired = status == WithdrawalNotificationStatus.Expired.ToString();
                    dto.IsGracePeriod = status == WithdrawalNotificationStatus.InGracePeriod.ToString();
                    dto.Status = status;

                    return ServiceResponse<WithdrawalNotificationAndriodDto>.ReturnResultWith200(dto);
                }
                else
                {
                    // If the WithdrawalNotification entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("WithdrawalNotification not found.");
                    return ServiceResponse<WithdrawalNotificationAndriodDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting WithdrawalNotification: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<WithdrawalNotificationAndriodDto>.Return500(e);
            }
        }

    }

}
