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
    /// Handles the retrieval of AssetDisposal based on the GetAssetDisposalQuery.
    /// </summary>
    public class GetAssetDisposalQueryHandler : IRequestHandler<GetAssetDisposalQuery, ServiceResponse<AssetDisposalDto>>
    {
        private readonly IAssetDisposalRepository _assetDisposalRepository; // Repository for accessing AssetDisposal data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAssetDisposalQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAssetDisposalQueryHandler.
        /// </summary>
        /// <param name="assetDisposalRepository">Repository for AssetDisposal data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetAssetDisposalQueryHandler(
            IAssetDisposalRepository assetDisposalRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetAssetDisposalQueryHandler> logger)
        {
            _assetDisposalRepository = assetDisposalRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAssetDisposalQuery to retrieve AssetDisposal.
        /// </summary>
        /// <param name="request">The GetAssetDisposalQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AssetDisposalDto>> Handle(GetAssetDisposalQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the AssetDisposal entity from the repository
                var entity = await _assetDisposalRepository.FindAsync(request.Id);

                // Step 2: Check if entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"AssetDisposal with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetDisposalDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "AssetDisposal returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var assetDisposalDto = _mapper.Map<AssetDisposalDto>(entity);
                return ServiceResponse<AssetDisposalDto>.ReturnResultWith200(assetDisposalDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve AssetDisposal: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AssetDisposalDto>.Return500(e, "Failed to retrieve AssetDisposal");
            }
        }
    }
}
