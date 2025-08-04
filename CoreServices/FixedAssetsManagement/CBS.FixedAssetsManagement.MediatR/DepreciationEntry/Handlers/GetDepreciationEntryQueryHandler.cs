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
    /// Handles the retrieval of DepreciationEntry based on the GetDepreciationEntryQuery.
    /// </summary>
    public class GetDepreciationEntryQueryHandler : IRequestHandler<GetDepreciationEntryQuery, ServiceResponse<DepreciationEntryDto>>
    {
        private readonly IDepreciationEntryRepository _depreciationEntryRepository; // Repository for accessing DepreciationEntry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDepreciationEntryQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetDepreciationEntryQueryHandler.
        /// </summary>
        /// <param name="depreciationEntryRepository">Repository for DepreciationEntry data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDepreciationEntryQueryHandler(
            IDepreciationEntryRepository depreciationEntryRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetDepreciationEntryQueryHandler> logger)
        {
            _depreciationEntryRepository = depreciationEntryRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDepreciationEntryQuery to retrieve a DepreciationEntry.
        /// </summary>
        /// <param name="request">The GetDepreciationEntryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DepreciationEntryDto>> Handle(GetDepreciationEntryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the DepreciationEntry entity from the repository
                var entity = await _depreciationEntryRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"DepreciationEntry with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<DepreciationEntryDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "DepreciationEntry returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var depreciationEntryDto = _mapper.Map<DepreciationEntryDto>(entity);
                return ServiceResponse<DepreciationEntryDto>.ReturnResultWith200(depreciationEntryDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve DepreciationEntry: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<DepreciationEntryDto>.Return500(e, "Failed to retrieve DepreciationEntry");
            }
        }
    }
}
