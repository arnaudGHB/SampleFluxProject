using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.FixedAssetsManagement.MediatR.Queries;
using CBS.FixedAssetsManagement.Repository;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Domain;
using CBS.FixedAssetsManagement.Data;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of DepreciationMethod based on the GetDepreciationMethodQuery.
    /// </summary>
    public class GetDepreciationMethodQueryHandler : IRequestHandler<GetDepreciationMethodQuery, ServiceResponse<DepreciationMethodDto>>
    {
        private readonly IDepreciationMethodRepository _depreciationMethodRepository; // Repository for accessing DepreciationMethod data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDepreciationMethodQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetDepreciationMethodQueryHandler.
        /// </summary>
        /// <param name="depreciationMethodRepository">Repository for DepreciationMethod data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDepreciationMethodQueryHandler(
            IDepreciationMethodRepository depreciationMethodRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetDepreciationMethodQueryHandler> logger)
        {
            _depreciationMethodRepository = depreciationMethodRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDepreciationMethodQuery to retrieve a DepreciationMethod.
        /// </summary>
        /// <param name="request">The GetDepreciationMethodQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DepreciationMethodDto>> Handle(GetDepreciationMethodQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the DepreciationMethod entity from the repository
                var entity = await _depreciationMethodRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"DepreciationMethod with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<DepreciationMethodDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "DepreciationMethod returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var depreciationMethodDto = _mapper.Map<DepreciationMethodDto>(entity);
                return ServiceResponse<DepreciationMethodDto>.ReturnResultWith200(depreciationMethodDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve DepreciationMethod: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<DepreciationMethodDto>.Return500(e, "Failed to retrieve DepreciationMethod");
            }
        }
    }
}
