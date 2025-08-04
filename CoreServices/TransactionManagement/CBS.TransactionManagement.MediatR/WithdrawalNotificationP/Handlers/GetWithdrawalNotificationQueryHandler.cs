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

    public class GetWithdrawalNotificationQueryHandler : IRequestHandler<GetWithdrawalNotificationQuery, ServiceResponse<WithdrawalNotificationDto>>
    {
        private readonly IWithdrawalNotificationRepository _WithdrawalNotificationRepository; // Repository for accessing WithdrawalNotification data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetWithdrawalNotificationQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetWithdrawalNotificationQueryHandler.
        /// </summary>
        /// <param name="WithdrawalNotificationRepository">Repository for WithdrawalNotification data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetWithdrawalNotificationQueryHandler(
            IWithdrawalNotificationRepository WithdrawalNotificationRepository,
            IMapper mapper,
            ILogger<GetWithdrawalNotificationQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _WithdrawalNotificationRepository = WithdrawalNotificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetWithdrawalNotificationQuery to retrieve a specific WithdrawalNotification.
        /// </summary>
        /// <param name="request">The GetWithdrawalNotificationQuery containing WithdrawalNotification ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<WithdrawalNotificationDto>> Handle(GetWithdrawalNotificationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the WithdrawalNotification entity with the specified ID from the repository, including the associated account
                var entity = await _WithdrawalNotificationRepository.FindBy(a => a.Id == request.Id).Include(a => a.Account).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the WithdrawalNotification entity to WithdrawalNotificationDto and return it with a success response
                    var WithdrawalNotificationDto = _mapper.Map<WithdrawalNotificationDto>(entity);
                    return ServiceResponse<WithdrawalNotificationDto>.ReturnResultWith200(WithdrawalNotificationDto);
                }
                else
                {
                    // If the WithdrawalNotification entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("WithdrawalNotification not found.");
                    return ServiceResponse<WithdrawalNotificationDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting WithdrawalNotification: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<WithdrawalNotificationDto>.Return500(e);
            }
        }
    }

}
