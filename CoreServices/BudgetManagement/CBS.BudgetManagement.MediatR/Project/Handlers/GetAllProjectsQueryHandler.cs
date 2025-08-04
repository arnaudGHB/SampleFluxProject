using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BudgetManagement.MediatR.Queries;
using CBS.BudgetManagement.Repository;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Projects based on the GetAllProjectsQuery.
    /// </summary>
    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectQuery, ServiceResponse<List<ProjectDto>>>
    {
        private readonly IProjectRepository _ProjectRepository; // Repository for accessing Project data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllProjectsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllProjectsQueryHandler.
        /// </summary>
        /// <param name="ProjectRepository">Repository for Project data access.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllProjectsQueryHandler(
            IProjectRepository ProjectRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllProjectsQueryHandler> logger)
        {
            _ProjectRepository = ProjectRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllProjectsQuery to retrieve all Projects.
        /// </summary>
        /// <param name="request">The GetAllProjectsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ProjectDto>>> Handle(GetAllProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve all Project entities from the repository
                var entities = await _ProjectRepository.All.ToListAsync(cancellationToken);

                // Step 2: Log the successful retrieval of Projects
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Projects returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 3: Return the result with a successful response
                return ServiceResponse<List<ProjectDto>>.ReturnResultWith200(_mapper.Map<List<ProjectDto>>(entities));
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Projects: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<ProjectDto>>.Return500(e, "Failed to retrieve Projects");
            }
        }
    }
}
