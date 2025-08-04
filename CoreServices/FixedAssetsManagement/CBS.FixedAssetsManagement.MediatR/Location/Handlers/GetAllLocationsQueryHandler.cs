using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.MediatR.Queries;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Locations based on the GetAllLocationsQuery.
    /// </summary>
    public class GetAllLocationsQueryHandler : IRequestHandler<GetAllLocationsQuery, ServiceResponse<List<LocationDto>>>
    {
        private readonly ILocationRepository _locationRepository; // Repository for accessing Location data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLocationsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllLocationsQueryHandler.
        /// </summary>
        /// <param name="locationRepository">Repository for Location data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLocationsQueryHandler(
            ILocationRepository locationRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllLocationsQueryHandler> logger)
        {
            _locationRepository = locationRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLocationsQuery to retrieve all Locations.
        /// </summary>
        /// <param name="request">The GetAllLocationsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LocationDto>>> Handle(GetAllLocationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all Location entities from the repository
                var entities = await _locationRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of locations
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Locations returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<LocationDto>>.ReturnResultWith200(_mapper.Map<List<LocationDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Locations: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<LocationDto>>.Return500(e, "Failed to retrieve Locations");
            }
        }
    }
}
