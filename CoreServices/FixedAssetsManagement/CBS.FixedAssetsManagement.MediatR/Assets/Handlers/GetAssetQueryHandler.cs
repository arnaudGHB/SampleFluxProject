using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.MediatR.Queries;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of Asset based on the GetAssetQuery.
    /// </summary>
    public class GetAssetQueryHandler : IRequestHandler<GetAssetQuery, ServiceResponse<AssetDto>>
    {
        private readonly IAssetRepository _assetRepository; // Repository for accessing Asset data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAssetQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAssetQueryHandler.
        /// </summary>
        /// <param name="assetRepository">Repository for Asset data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetAssetQueryHandler(
            IAssetRepository assetRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetAssetQueryHandler> logger)
        {
            _assetRepository = assetRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAssetQuery to retrieve an Asset.
        /// </summary>
        /// <param name="request">The GetAssetQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AssetDto>> Handle(GetAssetQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Asset entity from the repository
                var entity = await _assetRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"Asset with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "Asset returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var assetDto = _mapper.Map<AssetDto>(entity);
                return ServiceResponse<AssetDto>.ReturnResultWith200(assetDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Asset: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AssetDto>.Return500(e, "Failed to retrieve Asset");
            }
        }
    }
}
