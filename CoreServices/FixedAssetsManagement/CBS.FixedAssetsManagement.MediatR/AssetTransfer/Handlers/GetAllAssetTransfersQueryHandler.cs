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
    /// Handles the retrieval of all Asset Transfers based on the GetAllAssetTransfersQuery.
    /// </summary>
    public class GetAllAssetTransfersQueryHandler : IRequestHandler<GetAllAssetTransfersQuery, ServiceResponse<List<AssetTransferDto>>>
    {
        private readonly IAssetTransferRepository _assetTransferRepository; // Repository for accessing Asset Transfer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAssetTransfersQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllAssetTransfersQueryHandler.
        /// </summary>
        /// <param name="assetTransferRepository">Repository for Asset Transfer data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAssetTransfersQueryHandler(
            IAssetTransferRepository assetTransferRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllAssetTransfersQueryHandler> logger)
        {
            _assetTransferRepository = assetTransferRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAssetTransfersQuery to retrieve all Asset Transfers.
        /// </summary>
        /// <param name="request">The GetAllAssetTransfersQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AssetTransferDto>>> Handle(GetAllAssetTransfersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all Asset Transfer entities from the repository
                var entities = await _assetTransferRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of asset transfers
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Asset Transfers returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<AssetTransferDto>>.ReturnResultWith200(_mapper.Map<List<AssetTransferDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Asset Transfers: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<AssetTransferDto>>.Return500(e, "Failed to retrieve Asset Transfers");
            }
        }
    }
}
