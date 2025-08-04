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
    /// Handles the retrieval of all Asset Types based on the GetAllAssetTypesQuery.
    /// </summary>
    public class GetAllAssetTypesQueryHandler : IRequestHandler<GetAllAssetTypesQuery, ServiceResponse<List<AssetTypeDto>>>
    {
        private readonly IAssetTypeRepository _assetTypeRepository; // Repository for accessing Asset Type data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAssetTypesQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllAssetTypesQueryHandler.
        /// </summary>
        /// <param name="assetTypeRepository">Repository for Asset Type data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAssetTypesQueryHandler(
            IAssetTypeRepository assetTypeRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllAssetTypesQueryHandler> logger)
        {
            _assetTypeRepository = assetTypeRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAssetTypesQuery to retrieve all Asset Types.
        /// </summary>
        /// <param name="request">The GetAllAssetTypesQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AssetTypeDto>>> Handle(GetAllAssetTypesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all Asset Type entities from the repository
                var entities = await _assetTypeRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of asset types
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Asset Types returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<AssetTypeDto>>.ReturnResultWith200(_mapper.Map<List<AssetTypeDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Asset Types: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<AssetTypeDto>>.Return500(e, "Failed to retrieve Asset Types");
            }
        }
    }
}
