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
    /// Handles the retrieval of AssetTransfer based on the GetAssetTransferQuery.
    /// </summary>
    public class GetAssetTransferQueryHandler : IRequestHandler<GetAssetTransferQuery, ServiceResponse<AssetTransferDto>>
    {
        private readonly IAssetTransferRepository _assetTransferRepository; // Repository for accessing AssetTransfer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAssetTransferQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAssetTransferQueryHandler.
        /// </summary>
        /// <param name="assetTransferRepository">Repository for AssetTransfer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetAssetTransferQueryHandler(
            IAssetTransferRepository assetTransferRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetAssetTransferQueryHandler> logger)
        {
            _assetTransferRepository = assetTransferRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAssetTransferQuery to retrieve an AssetTransfer.
        /// </summary>
        /// <param name="request">The GetAssetTransferQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AssetTransferDto>> Handle(GetAssetTransferQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the AssetTransfer entity from the repository
                var entity = await _assetTransferRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"AssetTransfer with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetTransferDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "AssetTransfer returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var assetTransferDto = _mapper.Map<AssetTransferDto>(entity);
                return ServiceResponse<AssetTransferDto>.ReturnResultWith200(assetTransferDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve AssetTransfer: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AssetTransferDto>.Return500(e, "Failed to retrieve AssetTransfer");
            }
        }
    }
}
