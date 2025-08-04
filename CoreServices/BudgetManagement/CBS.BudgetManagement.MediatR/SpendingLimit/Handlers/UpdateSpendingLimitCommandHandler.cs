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
    /// Handles the command to update an SpendingLimit based on UpdateSpendingLimitCommand.
    /// </summary>
    public class UpdateSpendingLimitCommandHandler : IRequestHandler<UpdateSpendingLimitCommand, ServiceResponse<SpendingLimitDto>>
    {
        private readonly ISpendingLimitRepository _SpendingLimitRepository; // Repository for accessing SpendingLimit data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateSpendingLimitCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<BudgetManagementContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateSpendingLimitCommandHandler.
        /// </summary>
        /// <param name="SpendingLimitRepository">Repository for SpendingLimit data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateSpendingLimitCommandHandler(
            ISpendingLimitRepository SpendingLimitRepository,
            IMapper mapper,
            ILogger<UpdateSpendingLimitCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _SpendingLimitRepository = SpendingLimitRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateSpendingLimitCommand to update an SpendingLimit.
        /// </summary>
        /// <param name="request">The UpdateSpendingLimitCommand containing updated SpendingLimit data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SpendingLimitDto>> Handle(UpdateSpendingLimitCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the SpendingLimit entity to be updated from the repository
                var existingSpendingLimit = await _SpendingLimitRepository.FindAsync(request.Id);

                // Step 2: Check if the SpendingLimit entity exists
                if (existingSpendingLimit != null)
                {
                    // Step 3: Update SpendingLimit entity properties with values from the request
                 
                    // Step 4: Use the repository to update the existing SpendingLimit entity
                    _SpendingLimitRepository.Update(existingSpendingLimit);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"SpendingLimit '{existingSpendingLimit.FiscalYearId}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var SpendingLimitDto = _mapper.Map<SpendingLimitDto>(existingSpendingLimit);

                    // Step 9: Return the updated SpendingLimitDto with a 200 status code
                    return ServiceResponse<SpendingLimitDto>.ReturnResultWith200(SpendingLimitDto);
                }
                else
                {
                    // Step 10: If the SpendingLimit entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"SpendingLimit '{request.FiscalYearId}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<SpendingLimitDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating SpendingLimit: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<SpendingLimitDto>.Return500(errorMessage);
            }
        }
    }
}
