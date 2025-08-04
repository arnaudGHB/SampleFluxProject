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
    /// Handles the retrieval of AssetType based on the GetAssetTypeQuery.
    /// </summary>
    public class GetAssetTypeQueryHandler : IRequestHandler<GetAssetTypeQuery, ServiceResponse<AssetTypeDto>>
    {
        private readonly IAssetTypeRepository _assetTypeRepository; // Repository for accessing AssetType data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAssetTypeQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAssetTypeQueryHandler.
        /// </summary>
        /// <param name="assetTypeRepository">Repository for AssetType data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAssetTypeQueryHandler(
            IAssetTypeRepository assetTypeRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAssetTypeQueryHandler> logger)
        {
            _assetTypeRepository = assetTypeRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAssetTypeQuery to retrieve an AssetType.
        /// </summary>
        /// <param name="request">The GetAssetTypeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AssetTypeDto>> Handle(GetAssetTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the AssetType entity from the repository
                var entity = await _assetTypeRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"AssetType with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetTypeDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "AssetType returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var assetTypeDto = _mapper.Map<AssetTypeDto>(entity);
                return ServiceResponse<AssetTypeDto>.ReturnResultWith200(assetTypeDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve AssetType: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AssetTypeDto>.Return500(e, "Failed to retrieve AssetType");
            }
        }
    }
}
