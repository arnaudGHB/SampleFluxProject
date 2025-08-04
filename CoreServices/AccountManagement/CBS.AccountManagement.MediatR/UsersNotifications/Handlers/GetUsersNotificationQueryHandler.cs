using AutoMapper;
using CBS.AccountingManagement.MediatR.Commands;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all OperationEvent based on the GetAllOperationEventNameQuery.
    /// </summary>
    public class GetUsersNotificationQueryHandler : IRequestHandler<GetUsersNotificationQuery, ServiceResponse<UsersNotificationDto>>
    {
        private readonly IUserNotificationRepository _userNotificationRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetUsersNotificationQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="userNotificationRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetUsersNotificationQueryHandler(
            IUserNotificationRepository userNotificationRepository,
            IMapper mapper, ILogger<GetUsersNotificationQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _userNotificationRepository = userNotificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetOperationEventNameQuery to retrieve a specific OperationEventName.
        /// </summary>
        /// <param name="request">The GetOperationEventNameQuery containing OperationEventName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UsersNotificationDto>> Handle(GetUsersNotificationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the OperationEventName entity with the specified ID from the repository
                var entity = await _userNotificationRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "OperationEventName has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<UsersNotificationDto>.Return404(message);
                    }
                    else
                    {
                        // Map the OperationEventName entity to OperationEventNameDto and return it with a success response
                        var OperationEventNameDto = _mapper.Map<UsersNotificationDto>(entity);
                        return ServiceResponse<UsersNotificationDto>.ReturnResultWith200(OperationEventNameDto);
                    }

                }
                else
                {
                    // If the OperationEventName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("OperationEventName not found.");
                    return ServiceResponse<UsersNotificationDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting OperationEventName: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<UsersNotificationDto>.Return500(e);
            }
        }
    }
}