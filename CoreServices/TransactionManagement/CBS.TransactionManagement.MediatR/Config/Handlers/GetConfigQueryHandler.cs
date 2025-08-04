using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all TempAccount based on the GetAllTempAccountQuery.
    /// </summary>
    public class GetConfigQueryHandler : IRequestHandler<GetConfigQuery, ServiceResponse<ConfigDto>>
    {
        private readonly IConfigRepository _ConfigRepository; // Repository for accessing Config data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetConfigQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllTempAccountQueryHandler.
        /// </summary>
        /// <param name="TempAccountRepository">Repository for Config data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetConfigQueryHandler(
            IConfigRepository ConfigRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetConfigQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _ConfigRepository = ConfigRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTempAccountQuery to retrieve all Config.
        /// </summary>
        /// <param name="request">The GetAllTempAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ConfigDto>> Handle(GetConfigQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Config entities from the repository
                var entities = await _ConfigRepository.FindAsync(request.Id);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "System configs returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<ConfigDto>.ReturnResultWith200(_mapper.Map<ConfigDto>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get Config: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get Config: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<ConfigDto>.Return500(e, "Failed to get Config");
            }
        }

    }
}
