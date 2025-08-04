using AutoMapper;
using CBS.AccountingManagement.MediatR.Commands;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a OperationEventName based on UpdateOperationEventNameCommand.
    /// </summary>
    public class UpdateUserNotificationCommandHandler : IRequestHandler<UpdateUserNotificationCommand, ServiceResponse<UsersNotificationDto>>
    {
        private readonly IUserNotificationRepository _userNotificationRepository; // Repository for accessing OperationEventName data.
        private readonly ILogger<UpdateUserNotificationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateUserNotificationCommandHandler(
            IUserNotificationRepository userNotificationRepository,
            ILogger<UpdateUserNotificationCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _userNotificationRepository = userNotificationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateOperationEventNameCommand to update a OperationEventName.
        /// </summary>
        /// <param name="request">The UpdateOperationEventNameCommand containing updated OperationEventName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UsersNotificationDto>> Handle(UpdateUserNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the OperationEventName entity to be updated from the repository
                var userModel = await _userNotificationRepository.FindAsync(request.Id);

                // Check if the OperationEventName entity exists
                if (userModel != null)
                {
                    // Update OperationEventName entity properties with values from the request
                    userModel.Action= request.Action;
                    userModel.BankId= request.ActionId;
                    userModel.ActionId= request.ActionId;
                    userModel = _mapper.Map(request, userModel);
                    // Use the repository to update the existing OperationEventName entity
                    _userNotificationRepository.Update(userModel);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UsersNotificationDto>.ReturnResultWith200(_mapper.Map<UsersNotificationDto>(userModel));
                    _logger.LogInformation($"OperationEventName {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the OperationEventName entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UsersNotificationDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating UsersNotification: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UsersNotificationDto>.Return500(e);
            }
        }
    }
}