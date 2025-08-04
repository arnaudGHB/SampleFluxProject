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
    public class GetAllUsersNotificationQueryHandler : IRequestHandler<GetAllUsersNotificationQuery, ServiceResponse<List<UsersNotificationDto>>>
    {
        private readonly IUserNotificationRepository _userNotificationRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllUsersNotificationQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllUsersNotificationQueryHandler(
            IUserNotificationRepository userNotificationRepository,
            IMapper mapper, ILogger<GetAllUsersNotificationQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _userInfoToken = userInfoToken;
            // Assign provided dependencies to local variables.
            _userNotificationRepository = userNotificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllUsersNotificationQueryHandler to retrieve all OperationEventName.
        /// </summary>
        /// <param name="request">The GetAllOperationEventNameQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<UsersNotificationDto>>> Handle(GetAllUsersNotificationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (_userInfoToken.IsHeadOffice)
                {
                    var entities = await _userNotificationRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                    return ServiceResponse<List<UsersNotificationDto>>.ReturnResultWith200(_mapper.Map<List<UsersNotificationDto>>(entities));

                }
                else
                {

                    var entitiesAccounts = await _userNotificationRepository.All.Where(x => x.BranchId.Equals(_userInfoToken.BranchId) || x.TempData == _userInfoToken.BranchId && x.IsDeleted.Equals(false)).ToListAsync();
                    return ServiceResponse<List<UsersNotificationDto>>.ReturnResultWith200(_mapper.Map<List<UsersNotificationDto>>(entitiesAccounts));


                }
                // Retrieve all OperationEvent entities from the repository
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all OperationEventNameDto: {e.Message}");
                return ServiceResponse<List<UsersNotificationDto>>.Return500(e, "Failed to get all OperationEventName");
            }
        }
    }
    public class GetAllUsersNotificationByBranchQueryHandler : IRequestHandler<GetAllUsersNotificationByBranchQuery, ServiceResponse<List<UsersNotificationDto>>>
    {
        private readonly IUserNotificationRepository _userNotificationRepository; // Repository for accessing OperationEvent data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllUsersNotificationByBranchQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetAllOperationEventNameQueryHandler.
        /// </summary>
        /// <param name="OperationEventNameRepository">Repository for OperationEventName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllUsersNotificationByBranchQueryHandler(
            IUserNotificationRepository userNotificationRepository,
            IMapper mapper, ILogger<GetAllUsersNotificationByBranchQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _userInfoToken = userInfoToken;
            // Assign provided dependencies to local variables.
            _userNotificationRepository = userNotificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllUsersNotificationQueryHandler to retrieve all OperationEventName.
        /// </summary>
        /// <param name="request">The GetAllOperationEventNameQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<UsersNotificationDto>>> Handle(GetAllUsersNotificationByBranchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (_userInfoToken.IsHeadOffice)
                {
                    var entities = await _userNotificationRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                    return ServiceResponse<List<UsersNotificationDto>>.ReturnResultWith200(_mapper.Map<List<UsersNotificationDto>>(entities));

                }
                else
                {

                    var entitiesAccounts = await _userNotificationRepository.All.Where(x => x.BranchId.Equals(_userInfoToken.BranchId) || x.TempData== _userInfoToken.BranchId && x.IsDeleted.Equals(false)).ToListAsync();
                    return ServiceResponse<List<UsersNotificationDto>>.ReturnResultWith200(_mapper.Map<List<UsersNotificationDto>>(entitiesAccounts));

                }
                // Retrieve all OperationEvent entities from the repository
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all OperationEventNameDto: {e.Message}");
                return ServiceResponse<List<UsersNotificationDto>>.Return500(e, "Failed to get all OperationEventName");
            }
        }
    }
}