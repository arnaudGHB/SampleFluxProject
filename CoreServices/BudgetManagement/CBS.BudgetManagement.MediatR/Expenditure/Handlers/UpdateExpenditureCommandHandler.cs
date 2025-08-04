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
    /// Handles the command to update an Expenditure based on UpdateExpenditureCommand.
    /// </summary>
    public class UpdateExpenditureCommandHandler : IRequestHandler<UpdateExpenditureCommand, ServiceResponse<ExpenditureDto>>
    {
        private readonly IExpenditureRepository _ExpenditureRepository; // Repository for accessing Expenditure data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateExpenditureCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<BudgetManagementContext> _uow; // Unit of Work for managing transactions.
        private readonly UserInfoToken _userInfoToken; // User information token for audit logging.

        /// <summary>
        /// Constructor for initializing the UpdateExpenditureCommandHandler.
        /// </summary>
        /// <param name="ExpenditureRepository">Repository for Expenditure data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="userInfoToken">User info token for audit logging.</param>
        /// <param name="uow">Unit of Work for managing transactions.</param>
        public UpdateExpenditureCommandHandler(
            IExpenditureRepository ExpenditureRepository,
            IMapper mapper,
            ILogger<UpdateExpenditureCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<BudgetManagementContext> uow)
        {
            _ExpenditureRepository = ExpenditureRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateExpenditureCommand to update an Expenditure.
        /// </summary>
        /// <param name="request">The UpdateExpenditureCommand containing updated Expenditure data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ExpenditureDto>> Handle(UpdateExpenditureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Expenditure entity to be updated from the repository
                var existingExpenditure = await _ExpenditureRepository.FindAsync(request.Id);

                // Step 2: Check if the Expenditure entity exists
                if (existingExpenditure != null)
                {
                    // Step 3: Update Expenditure entity properties with values from the request
                 
                    // Step 4: Use the repository to update the existing Expenditure entity
                    _ExpenditureRepository.Update(existingExpenditure);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"Expenditure '{existingExpenditure.Description}' updated successfully.";
                    _logger.LogInformation(msg);

                    // Step 7: Audit log the update action
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Map the updated entity to a DTO for the response
                    var ExpenditureDto = _mapper.Map<ExpenditureDto>(existingExpenditure);

                    // Step 9: Return the updated ExpenditureDto with a 200 status code
                    return ServiceResponse<ExpenditureDto>.ReturnResultWith200(ExpenditureDto);
                }
                else
                {
                    // Step 10: If the Expenditure entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"Expenditure '{request.BudgetItemId}' was not found.";
                    _logger.LogError(errorMessage);

                    // Step 11: Audit log the failed update attempt
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<ExpenditureDto>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Expenditure: {ex.Message}";
                _logger.LogError(errorMessage);

                // Step 13: Audit log the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<ExpenditureDto>.Return500(errorMessage);
            }
        }
    }
}
