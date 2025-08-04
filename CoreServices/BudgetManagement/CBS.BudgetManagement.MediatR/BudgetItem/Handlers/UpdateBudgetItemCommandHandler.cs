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
    /// Handles the command to update an BudgetItem based on UpdateBudgetItemCommand.
    /// </summary>
    public class UpdateBudgetItemCommandHandler : IRequestHandler<UpdateBudgetItemCommand, ServiceResponse<BudgetItemDto>>
    {
        private readonly IBudgetItemRepository _BudgetItemRepository; // Repository for accessing BudgetItem data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateBudgetItemCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<BudgetManagementContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateBudgetItemCommandHandler.
        /// </summary>
        /// <param name="BudgetItemRepository">Repository for BudgetItem data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateBudgetItemCommandHandler(
            IBudgetItemRepository BudgetItemRepository,
            IMapper mapper,
            ILogger<UpdateBudgetItemCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _BudgetItemRepository = BudgetItemRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateBudgetItemCommand to update an BudgetItem.
        /// </summary>
        /// <param name="request">The UpdateBudgetItemCommand containing updated BudgetItem data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetItemDto>> Handle(UpdateBudgetItemCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the BudgetItem entity to be updated from the repository
                var existingBudgetItem = await _BudgetItemRepository.FindAsync(request.Id);

                // Step 2: Check if the BudgetItem entity exists
                if (existingBudgetItem != null)
                {
                    // Step 3: Update BudgetItem entity properties with values from the request
                 
                    // Step 4: Use the repository to update the existing BudgetItem entity
                    _BudgetItemRepository.Update(existingBudgetItem);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"BudgetItem '{existingBudgetItem.Description}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var BudgetItemDto = _mapper.Map<BudgetItemDto>(existingBudgetItem);

                    // Step 9: Return the updated BudgetItemDto with a 200 status code
                    return ServiceResponse<BudgetItemDto>.ReturnResultWith200(BudgetItemDto);
                }
                else
                {
                    // Step 10: If the BudgetItem entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"BudgetItem '{request.Description}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<BudgetItemDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating BudgetItem: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<BudgetItemDto>.Return500(errorMessage);
            }
        }
    }
}
