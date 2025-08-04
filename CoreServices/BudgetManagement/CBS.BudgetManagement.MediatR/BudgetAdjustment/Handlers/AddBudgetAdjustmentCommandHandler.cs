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

public class AddBudgetAdjustmentCommandHandler : IRequestHandler<AddBudgetAdjustmentCommand, ServiceResponse<BudgetAdjustmentDto>>
{
    private readonly IBudgetAdjustmentRepository _budgetAdjustmentRepository;
    private readonly IBudgetPlanRepository _budgetPlanRepository;
    private readonly ILogger<AddBudgetAdjustmentCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<BudgetManagementContext> _uow;
    private readonly UserInfoToken _userInfoToken;

    public AddBudgetAdjustmentCommandHandler(
        IBudgetAdjustmentRepository budgetAdjustmentRepository,
        IBudgetPlanRepository budgetPlanRepository,
        ILogger<AddBudgetAdjustmentCommandHandler> logger,
        UserInfoToken userInfoToken,
        IMapper mapper,
        IUnitOfWork<BudgetManagementContext> uow)
    {
        _budgetAdjustmentRepository = budgetAdjustmentRepository;
        _budgetPlanRepository = budgetPlanRepository;
        _logger = logger;
        _userInfoToken = userInfoToken;
        _mapper = mapper;
        _uow = uow;
    }

    public async Task<ServiceResponse<BudgetAdjustmentDto>> Handle(AddBudgetAdjustmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if the BudgetPlan exists
            var existingBudgetPlan = await _budgetPlanRepository.FindAsync(request.BudgetPlanId);
            if (existingBudgetPlan == null)
            {
                string message = $"Budget Plan with ID '{request.BudgetPlanId}' does not exist.";
                _logger.LogError(message);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                return ServiceResponse<BudgetAdjustmentDto>.Return404(message);
            }

            // Map the request to a BudgetAdjustment entity
            var budgetAdjustment = _mapper.Map<BudgetAdjustment>(request);
            budgetAdjustment.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "ADJ");

            // Add the new BudgetAdjustment to the repository
            _budgetAdjustmentRepository.Add(budgetAdjustment);
            await _uow.SaveAsync();

            // Log successful creation of the BudgetAdjustment
            string successMessage = $"Budget Adjustment '{budgetAdjustment.Id}' created successfully.";
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

            // Map the BudgetAdjustment entity back to a DTO for response
            var budgetAdjustmentDto = _mapper.Map<BudgetAdjustmentDto>(budgetAdjustment);
            return ServiceResponse<BudgetAdjustmentDto>.ReturnResultWith200(budgetAdjustmentDto);
        }
        catch (Exception e)
        {
            // Log error and return 500 Internal Server Error response with error message
            string errorMsg = $"Error occurred while adding Budget Adjustment: {e.Message}";
            _logger.LogError(errorMsg);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMsg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
            return ServiceResponse<BudgetAdjustmentDto>.Return500(errorMsg);
        }
    }
}