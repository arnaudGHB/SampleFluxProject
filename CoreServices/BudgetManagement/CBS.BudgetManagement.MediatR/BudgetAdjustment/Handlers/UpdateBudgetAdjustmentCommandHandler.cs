using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.Repository;
using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using CBS.BudgetManagement.MediatR.Commands;
using CBS.BudgetManagement.Domain;


namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a BudgetAdjustment based on UpdateBudgetAdjustmentCommand.
    /// </summary>
    public class UpdateBudgetAdjustmentCommandHandler : IRequestHandler<UpdateBudgetAdjustmentCommand, ServiceResponse<BudgetAdjustmentDto>>
    {
        private readonly IBudgetAdjustmentRepository _budgetAdjustmentRepository;
        private readonly IBudgetPlanRepository _budgetPlanRepository;
        private readonly ILogger<UpdateBudgetAdjustmentCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<BudgetManagementContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the UpdateBudgetAdjustmentCommandHandler.
        /// </summary>
        public UpdateBudgetAdjustmentCommandHandler(
            IBudgetAdjustmentRepository budgetAdjustmentRepository,
            IBudgetPlanRepository budgetPlanRepository,
            ILogger<UpdateBudgetAdjustmentCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<BudgetManagementContext> uow,
            UserInfoToken userInfoToken)
        {
            _budgetAdjustmentRepository = budgetAdjustmentRepository;
            _budgetPlanRepository = budgetPlanRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the UpdateBudgetAdjustmentCommand to update a BudgetAdjustment.
        /// </summary>
        public async Task<ServiceResponse<BudgetAdjustmentDto>> Handle(UpdateBudgetAdjustmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Check if the BudgetAdjustment exists
                var existingAdjustment = await _budgetAdjustmentRepository.FindAsync(request.Id);
                if (existingAdjustment == null)
                {
                    string notFoundMessage = $"BudgetAdjustment with ID {request.Id} not found.";
                    _logger.LogError(notFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, notFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<BudgetAdjustmentDto>.Return404(notFoundMessage);
                }

                // Step 2: Check if the associated BudgetPlan exists
                var existingBudgetPlan = await _budgetPlanRepository.FindAsync(request.BudgetPlanId);
                if (existingBudgetPlan == null)
                {
                    string budgetPlanNotFoundMessage = $"BudgetPlan with ID {request.BudgetPlanId} not found.";
                    _logger.LogError(budgetPlanNotFoundMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, budgetPlanNotFoundMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<BudgetAdjustmentDto>.Return404(budgetPlanNotFoundMessage);
                }

                // Step 3: Update the BudgetAdjustment properties
                _mapper.Map(request, existingAdjustment);

                // Step 4: Update the entity in the repository
                _budgetAdjustmentRepository.Update(existingAdjustment);

                // Step 5: Save changes
                await _uow.SaveAsync();

                // Step 6: Map the updated entity to DTO
                var updatedAdjustmentDto = _mapper.Map<BudgetAdjustmentDto>(existingAdjustment);

                // Step 7: Log successful update
                string successMessage = $"BudgetAdjustment with ID {request.Id} successfully updated.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 8: Return successful response with updated DTO
                return ServiceResponse<BudgetAdjustmentDto>.ReturnResultWith200(updatedAdjustmentDto);
            }
            catch (Exception e)
            {
                // Step 9: Log error and return 500 Internal Server Error response
                string errorMessage = $"Error occurred while updating BudgetAdjustment: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<BudgetAdjustmentDto>.Return500(e);
            }
        }
    }
}