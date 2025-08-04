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
    /// Handles the command to add a new BudgetPlan based on AddBudgetPlanCommand.
    /// </summary>
    public class AddBudgetPlanCommandHandler : IRequestHandler<AddBudgetPlanCommand, ServiceResponse<BudgetPlanDto>>
    {
        private readonly IBudgetPlanRepository _BudgetPlanRepository;
        private readonly ILogger<AddBudgetPlanCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<BudgetManagementContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public AddBudgetPlanCommandHandler(
            IBudgetPlanRepository BudgetPlanRepository,
 
            ILogger<AddBudgetPlanCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _BudgetPlanRepository = BudgetPlanRepository;
 
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }
        public async Task<ServiceResponse<BudgetPlanDto>> Handle(AddBudgetPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if an BudgetPlan with the same serial number already exists
                var existingBudgetPlan = await _BudgetPlanRepository.FindBy(x => x.FiscalYearId == request.FiscalYearId).FirstOrDefaultAsync();
                string message = $"BudgetPlan '{request.FiscalYearId}' created successfully.";

                if (existingBudgetPlan != null)
                {
                    message = $"BudgetPlan with '{request.FiscalYearId}' already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<BudgetPlanDto>.Return409(message);
                }
 

                // Map the request to an BudgetPlan entity
                var BudgetPlan = _mapper.Map<BudgetPlan>(request);
                BudgetPlan.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "BDT");


                // Add the new BudgetPlan to the repository
                _BudgetPlanRepository.Add(BudgetPlan);
                await _uow.SaveAsync();

                // Log successful creation of the BudgetPlan
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the BudgetPlan entity back to a DTO for response
                var BudgetPlanDto = _mapper.Map<BudgetPlanDto>(BudgetPlan);
                return ServiceResponse<BudgetPlanDto>.ReturnResultWith200(BudgetPlanDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while adding BudgetPlan: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<BudgetPlanDto>.Return500(msg);
            }
        }
    }
}
