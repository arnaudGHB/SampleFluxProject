using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BudgetManagement.Repository;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Common;

using CBS.BudgetManagement.Helper;
using CBS.BudgetManagement.MediatR.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using CBS.BudgetManagement.Domain;

namespace CBS.BudgetManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update an BudgetCategory based on UpdateBudgetCategoryCommand.
    /// </summary>
    public class UpdateBudgetCategoryCommandHandler : IRequestHandler<UpdateBudgetCategoryCommand, ServiceResponse<BudgetCategoryDto>>
    {
        private readonly IBudgetCategoryRepository _BudgetCategoryRepository; // Repository for accessing BudgetCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateBudgetCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<BudgetManagementContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateBudgetCategoryCommandHandler.
        /// </summary>
        /// <param name="BudgetCategoryRepository">Repository for BudgetCategory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateBudgetCategoryCommandHandler(
            IBudgetCategoryRepository BudgetCategoryRepository,
            IMapper mapper,
            ILogger<UpdateBudgetCategoryCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _BudgetCategoryRepository = BudgetCategoryRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateBudgetCategoryCommand to update an BudgetCategory.
        /// </summary>
        /// <param name="request">The UpdateBudgetCategoryCommand containing updated BudgetCategory data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetCategoryDto>> Handle(UpdateBudgetCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the BudgetCategory entity to be updated from the repository
                var existingBudgetCategory = await _BudgetCategoryRepository.FindAsync(request.Id);

                // Step 2: Check if the BudgetCategory entity exists
                if (existingBudgetCategory != null)
                {
                    // Step 3: Update BudgetCategory entity properties with values from the request
                 
                    // Step 4: Use the repository to update the existing BudgetCategory entity
                    _BudgetCategoryRepository.Update(existingBudgetCategory);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"BudgetCategory '{existingBudgetCategory.Name}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var BudgetCategoryDto = _mapper.Map<BudgetCategoryDto>(existingBudgetCategory);

                    // Step 9: Return the updated BudgetCategoryDto with a 200 status code
                    return ServiceResponse<BudgetCategoryDto>.ReturnResultWith200(BudgetCategoryDto);
                }
                else
                {
                    // Step 10: If the BudgetCategory entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"BudgetCategory '{request.Name}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<BudgetCategoryDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating BudgetCategory: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<BudgetCategoryDto>.Return500(errorMessage);
            }
        }
    }
}
