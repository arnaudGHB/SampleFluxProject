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
    public class AddBudgetItemCommandHandler : IRequestHandler<AddBudgetItemCommand, ServiceResponse<BudgetItemDto>>
    {
        private readonly IBudgetItemRepository _budgetItemRepository;
        private readonly IBudgetPlanRepository _budgetPlanRepository;
        private readonly IBudgetCategoryRepository _budgetCategoryRepository;
        private readonly ILogger<AddBudgetItemCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<BudgetManagementContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddBudgetItemCommandHandler(
            IBudgetItemRepository budgetItemRepository,
            IBudgetPlanRepository budgetPlanRepository,
            IBudgetCategoryRepository budgetCategoryRepository,
            ILogger<AddBudgetItemCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<BudgetManagementContext> uow,
            UserInfoToken userInfoToken)
        {
            _budgetItemRepository = budgetItemRepository;
            _budgetPlanRepository = budgetPlanRepository;
            _budgetCategoryRepository = budgetCategoryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        public async Task<ServiceResponse<BudgetItemDto>> Handle(AddBudgetItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Check if the BudgetPlan exists
                var existingBudgetPlan = await _budgetPlanRepository.FindAsync(request.BudgetPlanId);
                if (existingBudgetPlan == null)
                {
                    string errorMessage = $"BudgetPlan with ID {request.BudgetPlanId} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<BudgetItemDto>.Return404(errorMessage);
                }

                // Step 2: Check if the BudgetCategory exists, create if not
                var budgetCategory = await _budgetCategoryRepository.FindAsync(request.BudgetCategoryId);
                if (budgetCategory == null)
                {
                    budgetCategory = new BudgetCategory
                    {
                        Id = request.BudgetCategoryId,
                        Name = request.BudgetCategoryName
                    };
                    _budgetCategoryRepository.Add(budgetCategory);
                }

                // Step 3: Create new BudgetItem entity
                var newBudgetItem = _mapper.Map<BudgetItem>(request);
                newBudgetItem.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "BI");

                // Step 4: Add the new BudgetItem to the repository
                _budgetItemRepository.Add(newBudgetItem);

                // Step 5: Save changes
                await _uow.SaveAsync();

                // Step 6: Map the new entity to DTO
                var budgetItemDto = _mapper.Map<BudgetItemDto>(newBudgetItem);

                // Step 7: Log successful creation
                string successMessage = $"BudgetItem with ID {newBudgetItem.Id} successfully created.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 8: Return successful response with created DTO
                return ServiceResponse<BudgetItemDto>.ReturnResultWith200(budgetItemDto);
            }
            catch (Exception e)
            {
                // Step 9: Log error and return 500 Internal Server Error response
                string errorMessage = $"Error occurred while creating BudgetItem: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<BudgetItemDto>.Return500(e);
            }
        }
    }
}