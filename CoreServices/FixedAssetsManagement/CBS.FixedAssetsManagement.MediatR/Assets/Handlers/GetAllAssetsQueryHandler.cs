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
    /// Handles the retrieval of all Assets based on the GetAllAssetsQuery.
    /// </summary>
    public class GetAllAssetsQueryHandler : IRequestHandler<GetAllAssetsQuery, ServiceResponse<List<AssetDto>>>
    {
        private readonly IAssetRepository _assetRepository; // Repository for accessing Asset data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAssetsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllAssetsQueryHandler.
        /// </summary>
        /// <param name="assetRepository">Repository for Asset data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAssetsQueryHandler(
            IAssetRepository assetRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllAssetsQueryHandler> logger)
        {
            _assetRepository = assetRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAssetsQuery to retrieve all Assets.
        /// </summary>
        /// <param name="request">The GetAllAssetsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AssetDto>>> Handle(GetAllAssetsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all Asset entities from the repository
                var entities = await _assetRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of assets
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Assets returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<AssetDto>>.ReturnResultWith200(_mapper.Map<List<AssetDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Assets: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<AssetDto>>.Return500(e, "Failed to retrieve Assets");
            }
        }
    }
}
