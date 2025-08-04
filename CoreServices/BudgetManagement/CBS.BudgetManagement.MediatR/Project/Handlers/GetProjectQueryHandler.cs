using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BudgetManagement.MediatR.Queries;
using CBS.BudgetManagement.Repository;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Data;

using System.Threading;
using System.Threading.Tasks;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of Project based on the GetProjectQuery.
    /// </summary>
    public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ServiceResponse<ProjectDto>>
    {
        private readonly IProjectRepository _ProjectRepository; // Repository for accessing Project data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetProjectQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetProjectQueryHandler.
        /// </summary>
        /// <param name="ProjectRepository">Repository for Project data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public GetProjectQueryHandler(
            IProjectRepository ProjectRepository,
            UserInfoToken userInfoToken,
            IMapper mapper, ILogger<GetProjectQueryHandler> logger)
        {
            _ProjectRepository = ProjectRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetProjectQuery to retrieve an Project.
        /// </summary>
        /// <param name="request">The GetProjectQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ProjectDto>> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Project entity from the repository
                var entity = await _ProjectRepository.FindAsync(request.Id);

                // Step 2: Check if the entity exists
                if (entity == null)
                {
                    string notFoundMessage = $"Project with ID '{request.Id}' not found.";
                    _logger.LogWarning(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<ProjectDto>.Return404(notFoundMessage);
                }

                // Step 3: Log success and return the mapped entity as a DTO
                string successMessage = "Project returned successfully.";
                _logger.LogInformation(successMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                var ProjectDto = _mapper.Map<ProjectDto>(entity);
                return ServiceResponse<ProjectDto>.ReturnResultWith200(ProjectDto);
            }
            catch (Exception e)
            {
                // Step 4: Log error and return a 500 Internal Server Error response
                string errorMessage = $"Failed to retrieve Project: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<ProjectDto>.Return500(e, "Failed to retrieve Project");
            }
        }
    }
}
