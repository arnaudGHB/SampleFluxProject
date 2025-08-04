using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Queries.WithdrawalNotificationP;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;

namespace CBS.TransactionManagement.Handlers.WithdrawalNotificationP
{
    public class GetAllWithdrawalNotificationByCustomerIdQueryHandler : IRequestHandler<GetAllWithdrawalNotificationByCustomerIdQuery, ServiceResponse<List<WithdrawalNotificationDto>>>
    {
        private readonly IWithdrawalNotificationRepository _WithdrawalNotificationRepository; // Repository for accessing WithdrawalNotifications data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllWithdrawalNotificationByCustomerIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllWithdrawalNotificationByCustomerIdQueryHandler.
        /// </summary>
        /// <param name="WithdrawalNotificationRepository">Repository for WithdrawalNotifications data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllWithdrawalNotificationByCustomerIdQueryHandler(
            IWithdrawalNotificationRepository WithdrawalNotificationRepository,
            IMapper mapper,
            ILogger<GetAllWithdrawalNotificationByCustomerIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _WithdrawalNotificationRepository = WithdrawalNotificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllWithdrawalNotificationByCustomerIdQuery to retrieve all WithdrawalNotifications by Customer ID.
        /// </summary>
        /// <param name="request">The GetAllWithdrawalNotificationByCustomerIdQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<WithdrawalNotificationDto>>> Handle(GetAllWithdrawalNotificationByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all WithdrawalNotification entities for the specified Customer ID from the repository
                var entities = await _WithdrawalNotificationRepository
                    .FindBy(x => x.CustomerId == request.Id) // Filter by Customer ID
                    .Where(x => !x.IsDeleted) // Exclude deleted entities
                    .Include(a => a.Account) // Include related Account entities
                    .ToListAsync(); // Execute the query and convert results to a list

                // Map the entities to WithdrawalNotificationDto objects and return a success response
                return ServiceResponse<List<WithdrawalNotificationDto>>.ReturnResultWith200(_mapper.Map<List<WithdrawalNotificationDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all WithdrawalNotifications: {e.Message}");
                return ServiceResponse<List<WithdrawalNotificationDto>>.Return500(e, "Failed to get all WithdrawalNotifications");
            }
        }
    }
}
