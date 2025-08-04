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
    /// Handles the retrieval of all Depreciation Entries based on the GetAllDepreciationEntriesQuery.
    /// </summary>
    public class GetAllDepreciationEntriesQueryHandler : IRequestHandler<GetAllDepreciationEntriesQuery, ServiceResponse<List<DepreciationEntryDto>>>
    {
        private readonly IDepreciationEntryRepository _depreciationEntryRepository; // Repository for accessing Depreciation Entry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDepreciationEntriesQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllDepreciationEntriesQueryHandler.
        /// </summary>
        /// <param name="depreciationEntryRepository">Repository for Depreciation Entry data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDepreciationEntriesQueryHandler(
            IDepreciationEntryRepository depreciationEntryRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllDepreciationEntriesQueryHandler> logger)
        {
            _depreciationEntryRepository = depreciationEntryRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDepreciationEntriesQuery to retrieve all Depreciation Entries.
        /// </summary>
        /// <param name="request">The GetAllDepreciationEntriesQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DepreciationEntryDto>>> Handle(GetAllDepreciationEntriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all Depreciation Entry entities from the repository
                var entities = await _depreciationEntryRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of depreciation entries
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Depreciation Entries returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<DepreciationEntryDto>>.ReturnResultWith200(_mapper.Map<List<DepreciationEntryDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Depreciation Entries: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<DepreciationEntryDto>>.Return500(e, "Failed to retrieve Depreciation Entries");
            }
        }
    }
}
