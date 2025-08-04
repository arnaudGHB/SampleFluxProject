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
    /// Handles the command to delete a BudgetAdjustment based on DeleteBudgetAdjustmentCommand.
    /// </summary>
    public class DeleteBudgetAdjustmentCommandHandler : IRequestHandler<DeleteBudgetAdjustmentCommand, ServiceResponse<bool>>
    {
        private readonly IBudgetAdjustmentRepository _budgetAdjustmentRepository; // Repository for accessing BudgetAdjustment data.
        private readonly ILogger<DeleteBudgetAdjustmentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<BudgetManagementContext> _uow; // Unit of Work for managing database operations.
        private readonly UserInfoToken _userInfoToken; // User information for audit logging.

        /// <summary>
        /// Constructor for initializing the DeleteBudgetAdjustmentCommandHandler.
        /// </summary>
        /// <param name="budgetAdjustmentRepository">Repository for BudgetAdjustment data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for managing database operations.</param>
        /// <param name="userInfoToken">User information token for audit logging.</param>
        public DeleteBudgetAdjustmentCommandHandler(
            IBudgetAdjustmentRepository budgetAdjustmentRepository,
            ILogger<DeleteBudgetAdjustmentCommandHandler> logger,
            IUnitOfWork<BudgetManagementContext> uow,
            UserInfoToken userInfoToken)
        {
            _budgetAdjustmentRepository = budgetAdjustmentRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteBudgetAdjustmentCommand to delete a BudgetAdjustment.
        /// </summary>
        /// <param name="request">The DeleteBudgetAdjustmentCommand containing the ID of the BudgetAdjustment to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteBudgetAdjustmentCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Step 1: Check if the BudgetAdjustment entity with the specified ID exists
                var existingBudgetAdjustment = await _budgetAdjustmentRepository.FindAsync(request.Id);
                if (existingBudgetAdjustment == null)
                {
                    errorMessage = $"BudgetAdjustment with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(
                        _userInfoToken.Email,
                        LogAction.Delete.ToString(),
                        request,
                        errorMessage,
                        LogLevelInfo.Error.ToString(),
                        404,
                        _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Step 2: Soft delete the BudgetAdjustment
                existingBudgetAdjustment.IsDeleted = true;
                _budgetAdjustmentRepository.Update(existingBudgetAdjustment);

                // Step 3: Save changes in the unit of work
                await _uow.SaveAsync();

                // Step 4: Log successful deletion
                string successMessage = $"BudgetAdjustment with ID {request.Id} successfully deleted.";
                await APICallHelper.AuditLogger(
                    _userInfoToken.Email,
                    LogAction.Delete.ToString(),
                    request,
                    successMessage,
                    LogLevelInfo.Information.ToString(),
                    200,
                    _userInfoToken.Token);

                // Step 5: Return a successful response
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting BudgetAdjustment: {e.Message}";

                // Step 6: Log error and return a 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(
                    _userInfoToken.Email,
                    LogAction.Delete.ToString(),
                    request,
                    errorMessage,
                    LogLevelInfo.Error.ToString(),
                    500,
                    _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}