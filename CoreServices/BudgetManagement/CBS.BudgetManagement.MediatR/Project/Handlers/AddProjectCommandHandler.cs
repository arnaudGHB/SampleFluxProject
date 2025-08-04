using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.BudgetManagement.Common;

using CBS.BudgetManagement.Repository;
using AutoMapper;
using CBS.BudgetManagement.MediatR.Commands;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Domain;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Project based on AddProjectCommand.
    /// </summary>
    public class AddProjectCommandHandler : IRequestHandler<AddProjectCommand, ServiceResponse<ProjectDto>>
    {
        private readonly IProjectRepository _ProjectRepository;
 
        private readonly ILogger<AddProjectCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<BudgetManagementContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddProjectCommandHandler(
            IProjectRepository ProjectRepository,
 
            ILogger<AddProjectCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _ProjectRepository = ProjectRepository;
 
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<ProjectDto>> Handle(AddProjectCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if an Project with the same serial number already exists
                var existingProject = await _ProjectRepository.FindBy(x => x.ProjectName == request.ProjectName).FirstOrDefaultAsync();
                string message = $"Project '{request.ProjectName}' created successfully.";

                if (existingProject != null)
                {
                    message = $"Project with '{request.ProjectName}' already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<ProjectDto>.Return409(message);
                }
 

                // Map the request to an Project entity
                var Project = _mapper.Map<Project>(request);
                Project.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "BDT");


                // Add the new Project to the repository
                _ProjectRepository.Add(Project);
                await _uow.SaveAsync();

                // Log successful creation of the Project
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the Project entity back to a DTO for response
                var ProjectDto = _mapper.Map<ProjectDto>(Project);
                return ServiceResponse<ProjectDto>.ReturnResultWith200(ProjectDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while adding Project: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<ProjectDto>.Return500(msg);
            }
        }
    }
}
