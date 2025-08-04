using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.MediatR.Queries;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Domain;
using CBS.FixedAssetsManagement.Data;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of Location based on the GetLocationQuery.
    /// </summary>
    public class GetLocationQueryHandler : IRequestHandler<GetLocationQuery, ServiceResponse<LocationDto>>
    {
        private readonly ILocationRepository _locationRepository; // Repository for accessing Location data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLocationQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetLocationQueryHandler.
        /// </summary>
        /// <param name="locationRepository">Repository for Location data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLocationQueryHandler(
            ILocationRepository locationRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetLocationQueryHandler> logger)
        {
            _locationRepository = locationRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLocationQuery to retrieve a Location.
        /// </summary>
        /// <param name="request">The GetLocationQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LocationDto>> Handle(GetLocationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Location entity from the repository
                var entity = await _locationRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"Location with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<LocationDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "Location returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var locationDto = _mapper.Map<LocationDto>(entity);
                return ServiceResponse<LocationDto>.ReturnResultWith200(locationDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Location: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<LocationDto>.Return500(e, "Failed to retrieve Location");
            }
        }
    }
}
