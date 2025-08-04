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
    /// Handles the command to add a new BudgetCategory based on AddBudgetCategoryCommand.
    /// </summary>
    public class AddBudgetCategoryCommandHandler : IRequestHandler<AddBudgetCategoryCommand, ServiceResponse<BudgetCategoryDto>>
    {
        private readonly IBudgetCategoryRepository _BudgetCategoryRepository;
 
        private readonly ILogger<AddBudgetCategoryCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<BudgetManagementContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public AddBudgetCategoryCommandHandler(
            IBudgetCategoryRepository BudgetCategoryRepository,
 
            ILogger<AddBudgetCategoryCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _BudgetCategoryRepository = BudgetCategoryRepository;
 
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ServiceResponse<BudgetCategoryDto>> Handle(AddBudgetCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if an BudgetCategory with the same serial number already exists
                var existingBudgetCategory = await _BudgetCategoryRepository.FindBy(x => x.Name == request.Name).FirstOrDefaultAsync();
                string message = $"BudgetCategory '{request.Name}' created successfully.";

                if (existingBudgetCategory != null)
                {
                    message = $"BudgetCategory with '{request.Name}' already exists.";
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<BudgetCategoryDto>.Return409(message);
                }
                // Map the request to an BudgetCategory entity
                var BudgetCategory = _mapper.Map<BudgetCategory>(request);
                BudgetCategory.Id = BaseUtilities.GenerateInsuranceUniqueNumber(15, "BDT");
                // Add the new BudgetCategory to the repository
                _BudgetCategoryRepository.Add(BudgetCategory);
                await _uow.SaveAsync();
                // Log successful creation of the BudgetCategory
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                // Map the BudgetCategory entity back to a DTO for response
                var BudgetCategoryDto = _mapper.Map<BudgetCategoryDto>(BudgetCategory);
                return ServiceResponse<BudgetCategoryDto>.ReturnResultWith200(BudgetCategoryDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                string msg = $"Error occurred while adding BudgetCategory: {e.Message}";
                _logger.LogError(msg);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<BudgetCategoryDto>.Return500(msg);
            }
        }
    }
}
