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
    /// Handles the command to update an BudgetPlan based on UpdateBudgetPlanCommand.
    /// </summary>
    public class UpdateBudgetPlanCommandHandler : IRequestHandler<UpdateBudgetPlanCommand, ServiceResponse<BudgetPlanDto>>
    {
        private readonly IBudgetPlanRepository _BudgetPlanRepository; // Repository for accessing BudgetPlan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateBudgetPlanCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<BudgetManagementContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateBudgetPlanCommandHandler.
        /// </summary>
        /// <param name="BudgetPlanRepository">Repository for BudgetPlan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateBudgetPlanCommandHandler(
            IBudgetPlanRepository BudgetPlanRepository,
            IMapper mapper,
            ILogger<UpdateBudgetPlanCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _BudgetPlanRepository = BudgetPlanRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateBudgetPlanCommand to update an BudgetPlan.
        /// </summary>
        /// <param name="request">The UpdateBudgetPlanCommand containing updated BudgetPlan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetPlanDto>> Handle(UpdateBudgetPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the BudgetPlan entity to be updated from the repository
                var existingBudgetPlan = await _BudgetPlanRepository.FindAsync(request.FiscalYearId);

                // Step 2: Check if the BudgetPlan entity exists
                if (existingBudgetPlan != null)
                {
                    // Step 3: Update BudgetPlan entity properties with values from the request
                 
                    // Step 4: Use the repository to update the existing BudgetPlan entity
                    _BudgetPlanRepository.Update(existingBudgetPlan);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"BudgetPlan '{existingBudgetPlan.FiscalYearId}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var BudgetPlanDto = _mapper.Map<BudgetPlanDto>(existingBudgetPlan);

                    // Step 9: Return the updated BudgetPlanDto with a 200 status code
                    return ServiceResponse<BudgetPlanDto>.ReturnResultWith200(BudgetPlanDto);
                }
                else
                {
                    // Step 10: If the BudgetPlan entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"BudgetPlan '{request.FiscalYearId}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<BudgetPlanDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating BudgetPlan: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<BudgetPlanDto>.Return500(errorMessage);
            }
        }
    }
}
