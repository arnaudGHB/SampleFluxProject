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
    /// Handles the retrieval of AssetRevaluation based on the GetAssetRevaluationQuery.
    /// </summary>
    public class GetAssetRevaluationQueryHandler : IRequestHandler<GetAssetRevaluationQuery, ServiceResponse<AssetRevaluationDto>>
    {
        private readonly IAssetRevaluationRepository _assetRevaluationRepository; // Repository for accessing AssetRevaluation data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAssetRevaluationQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAssetRevaluationQueryHandler.
        /// </summary>
        /// <param name="assetRevaluationRepository">Repository for AssetRevaluation data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetAssetRevaluationQueryHandler(
            IAssetRevaluationRepository assetRevaluationRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetAssetRevaluationQueryHandler> logger)
        {
            _assetRevaluationRepository = assetRevaluationRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAssetRevaluationQuery to retrieve AssetRevaluation.
        /// </summary>
        /// <param name="request">The GetAssetRevaluationQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AssetRevaluationDto>> Handle(GetAssetRevaluationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the AssetRevaluation entity from the repository
                var entity = await _assetRevaluationRepository.FindAsync(request.Id);

                // Step 2: Check if entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"AssetRevaluation with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<AssetRevaluationDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "AssetRevaluation returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var assetRevaluationDto = _mapper.Map<AssetRevaluationDto>(entity);
                return ServiceResponse<AssetRevaluationDto>.ReturnResultWith200(assetRevaluationDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve AssetRevaluation: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<AssetRevaluationDto>.Return500(e, "Failed to retrieve AssetRevaluation");
            }
        }
    }
}
