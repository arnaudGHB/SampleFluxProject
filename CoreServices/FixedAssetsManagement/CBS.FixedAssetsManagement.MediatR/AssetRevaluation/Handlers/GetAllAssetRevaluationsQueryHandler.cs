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
    /// Handles the retrieval of all AssetRevaluations based on the GetAllAssetRevaluationsQuery.
    /// </summary>
    public class GetAllAssetRevaluationsQueryHandler : IRequestHandler<GetAllAssetRevaluationsQuery, ServiceResponse<List<AssetRevaluationDto>>>
    {
        private readonly IAssetRevaluationRepository _assetRevaluationRepository; // Repository for accessing AssetRevaluation data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAssetRevaluationsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllAssetRevaluationsQueryHandler.
        /// </summary>
        /// <param name="assetRevaluationRepository">Repository for AssetRevaluation data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAssetRevaluationsQueryHandler(
            IAssetRevaluationRepository assetRevaluationRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllAssetRevaluationsQueryHandler> logger)
        {
            _assetRevaluationRepository = assetRevaluationRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAssetRevaluationsQuery to retrieve all AssetRevaluations.
        /// </summary>
        /// <param name="request">The GetAllAssetRevaluationsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AssetRevaluationDto>>> Handle(GetAllAssetRevaluationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all AssetRevaluation entities from the repository
                var entities = await _assetRevaluationRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of asset revaluations
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Asset revaluations returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<AssetRevaluationDto>>.ReturnResultWith200(_mapper.Map<List<AssetRevaluationDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Asset Revaluations: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<AssetRevaluationDto>>.Return500(e, "Failed to retrieve Asset Revaluations");
            }
        }
    }
}
