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
    /// Handles the retrieval of all AssetDisposals based on the GetAllAssetDisposalsQuery.
    /// </summary>
    public class GetAllAssetDisposalsQueryHandler : IRequestHandler<GetAllAssetDisposalsQuery, ServiceResponse<List<AssetDisposalDto>>>
    {
        private readonly IAssetDisposalRepository _assetDisposalRepository; // Repository for accessing AssetDisposal data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAssetDisposalsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllAssetDisposalsQueryHandler.
        /// </summary>
        /// <param name="assetDisposalRepository">Repository for AssetDisposal data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAssetDisposalsQueryHandler(
            IAssetDisposalRepository assetDisposalRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllAssetDisposalsQueryHandler> logger)
        {
            _assetDisposalRepository = assetDisposalRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAssetDisposalsQuery to retrieve all AssetDisposals.
        /// </summary>
        /// <param name="request">The GetAllAssetDisposalsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AssetDisposalDto>>> Handle(GetAllAssetDisposalsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all AssetDisposal entities from the repository
                var entities = await _assetDisposalRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of asset disposals
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Asset disposals returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<AssetDisposalDto>>.ReturnResultWith200(_mapper.Map<List<AssetDisposalDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Asset Disposals: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<AssetDisposalDto>>.Return500(e, "Failed to retrieve Asset Disposals");
            }
        }
    }
}
