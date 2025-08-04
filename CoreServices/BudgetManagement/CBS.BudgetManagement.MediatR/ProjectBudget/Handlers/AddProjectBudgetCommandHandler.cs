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
    /// Handles the command to add a new ProjectBudget based on AddProjectBudgetCommand.
    /// </summary>
    public class AddProjectBudgetCommandHandler : IRequestHandler<AddProjectBudgetCommand, ServiceResponse<ProjectBudgetDto>>
    {
        private readonly IProjectBudgetRepository _ProjectBudgetRepository;
 
        private readonly ILogger<AddProjectBudgetCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<BudgetManagementContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddProjectBudgetCommandHandler(
            IProjectBudgetRepository ProjectBudgetRepository,
 
            ILogger<AddProjectBudgetCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _ProjectBudgetRepository = ProjectBudgetRepository;
 
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<ProjectBudgetDto>> Handle(AddProjectBudgetCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if an ProjectBudget with the same serial number already exists
                var existingProjectBudget = await _ProjectBudgetRepository.FindBy(x => x.FiscalYearId == request.FiscalYearId).FirstOrDefaultAsync();
                string message = $"ProjectBudget '{request.FiscalYearId}' created successfully.";

                if (existingProjectBudget != null)
                {
                    message = $"ProjectBudget with '{request.FiscalYearId}' already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<ProjectBudgetDto>.Return409(message);
                }
 

                // Map the request to an ProjectBudget entity
                var ProjectBudget = _mapper.Map<ProjectBudget>(request);
                ProjectBudget.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "BDT");


                // Add the new ProjectBudget to the repository
                _ProjectBudgetRepository.Add(ProjectBudget);
                await _uow.SaveAsync();

                // Log successful creation of the ProjectBudget
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the ProjectBudget entity back to a DTO for response
                var ProjectBudgetDto = _mapper.Map<ProjectBudgetDto>(ProjectBudget);
                return ServiceResponse<ProjectBudgetDto>.ReturnResultWith200(ProjectBudgetDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while adding ProjectBudget: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<ProjectBudgetDto>.Return500(msg);
            }
        }
    }
}
