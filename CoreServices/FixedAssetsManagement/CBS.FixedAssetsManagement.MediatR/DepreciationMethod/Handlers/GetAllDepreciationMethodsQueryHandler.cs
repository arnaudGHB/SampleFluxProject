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
    /// Handles the retrieval of all Depreciation Methods based on the GetAllDepreciationMethodsQuery.
    /// </summary>
    public class GetAllDepreciationMethodsQueryHandler : IRequestHandler<GetAllDepreciationMethodsQuery, ServiceResponse<List<DepreciationMethodDto>>>
    {
        private readonly IDepreciationMethodRepository _depreciationMethodRepository; // Repository for accessing Depreciation Method data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDepreciationMethodsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllDepreciationMethodsQueryHandler.
        /// </summary>
        /// <param name="depreciationMethodRepository">Repository for Depreciation Method data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDepreciationMethodsQueryHandler(
            IDepreciationMethodRepository depreciationMethodRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllDepreciationMethodsQueryHandler> logger)
        {
            _depreciationMethodRepository = depreciationMethodRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllDepreciationMethodsQuery to retrieve all Depreciation Methods.
        /// </summary>
        /// <param name="request">The GetAllDepreciationMethodsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DepreciationMethodDto>>> Handle(GetAllDepreciationMethodsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all Depreciation Method entities from the repository
                var entities = await _depreciationMethodRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of depreciation methods
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Depreciation Methods returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<DepreciationMethodDto>>.ReturnResultWith200(_mapper.Map<List<DepreciationMethodDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Depreciation Methods: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<DepreciationMethodDto>>.Return500(e, "Failed to retrieve Depreciation Methods");
            }
        }
    }
}
