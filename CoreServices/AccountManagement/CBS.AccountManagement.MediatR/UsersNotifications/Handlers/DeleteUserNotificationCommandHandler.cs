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
    /// Handles the command to add a new OperationEventName.
    /// </summary>
    public class DeleteUserNotificationCommandHandler : IRequestHandler<DeleteUserNotificationCommand, ServiceResponse<bool>>
    {
        private readonly IUserNotificationRepository _userNotificationRepository; // Repository for accessing OperationEventName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DeleteUserNotificationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;


        /// <summary>
        /// Constructor for initializing the DeleteOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteUserNotificationCommandHandler(
            IUserNotificationRepository userNotificationRepository, IMapper mapper,
            ILogger<DeleteUserNotificationCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _userNotificationRepository = userNotificationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteOperationEventNameCommand to delete a OperationEventName.
        /// </summary>
        /// <param name="request">The DeleteOperationEventNameCommand containing OperationEventName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteUserNotificationCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the OperationEventName entity with the specified ID exists
                var userModel = await _userNotificationRepository.FindAsync(request.Id);
                if (userModel == null)
                {
                    errorMessage = $"usernotifiction with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                userModel.IsDeleted = true;

                _userNotificationRepository.Update(userModel);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting OperationEventName: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}