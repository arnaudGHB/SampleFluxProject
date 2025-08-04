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
    public class AddUserNotificationCommandHandler : IRequestHandler<AddUserNotificationCommand, ServiceResponse<UsersNotificationDto>>
    {
        private readonly IUserNotificationRepository _userNotificationRepository; // Repository for accessing OperationEventName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddUserNotificationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the AddOperationEventNameCommandHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddUserNotificationCommandHandler(
            IUserNotificationRepository userNotificationRepository,
            IMapper mapper,
            ILogger<AddUserNotificationCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _userNotificationRepository = userNotificationRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken= userInfoToken;
        }

        /// <summary>
        /// Handles the AddOperationEventNameCommand to add a new OperationEventName.
        /// </summary>
        /// <param name="request">The AddOperationEventNameCommand containing OperationEventName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UsersNotificationDto>> Handle(AddUserNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
  

                // Map the AddOperationEventNameAttributesCommand to a OperationEventNameAttributes entity
                var UserModel = _mapper.Map<UsersNotification>(request);

                UserModel =(UsersNotification) UserModel.SetOperationEventEntity(UserModel, _userInfoToken);
                // Add the new OperationEventName entity to the repository
                UserModel.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "Notif");
                _userNotificationRepository.Add(UserModel);
                await _uow.SaveAsync();
              
                // Map the OperationEventName entity to OperationEventNameDto and return it with a success response
                var UserModelDto = _mapper.Map<UsersNotificationDto>(UserModel);
                return ServiceResponse<UsersNotificationDto>.ReturnResultWith200(UserModelDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving UsersNotification: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UsersNotificationDto>.Return500(e);
            }
        }
    }
}