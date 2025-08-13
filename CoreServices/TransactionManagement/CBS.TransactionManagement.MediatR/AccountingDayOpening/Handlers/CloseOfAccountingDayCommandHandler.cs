using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.Commands.AccountingDayOpening;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace CBS.TransactionManagement.Handlers.AccountingDayOpening
{
    public class CloseOfAccountingDayCommandHandler : IRequestHandler<CloseOfAccountingDayCommand, ServiceResponse<List<CloseOrOpenAccountingDayResultDto>>>
    {
        private readonly ILogger<CloseOfAccountingDayCommandHandler> _logger; // Logger for handling logs and audit information
        private readonly UserInfoToken _userInfoToken; // User information for logging and auditing
        private readonly IAccountingDayRepository _accountingDayRepository; // Repository for accounting day data access

        /// <summary>
        /// Constructor for initializing the CloseOfAccountingDayCommandHandler.
        /// </summary>
        /// <param name="userInfoToken">User information for logging and auditing.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="accountingDayRepository">Repository for accessing accounting day data.</param>
        public CloseOfAccountingDayCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<CloseOfAccountingDayCommandHandler> logger,
            IAccountingDayRepository accountingDayRepository = null)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _accountingDayRepository = accountingDayRepository;
        }

        /// <summary>
        /// Handles the CloseOfAccountingDayCommand to close the accounting day for the specified branches.
        /// </summary>
        /// <param name="request">The CloseOfAccountingDayCommand containing date and branch details.</param>
        /// <param name="cancellationToken">Cancellation token for handling request cancellation.</param>
        /// <returns>A ServiceResponse with the results of the operation.</returns>
        public async Task<ServiceResponse<List<CloseOrOpenAccountingDayResultDto>>> Handle(CloseOfAccountingDayCommand request, CancellationToken cancellationToken)
        {
            string logReference = BaseUtilities.GenerateUniqueNumber();  // Generate unique log reference
            string logDetails = string.Empty;  // Accumulate detailed branch-specific logging

            try
            {
                // 1. Close accounting day for the specified branches
                var closeAccountingDayResultDtos = await _accountingDayRepository.CloseAccountingDayForBranches(request.Date, request.Branches, request.IsCentraliseOpening);

                // 2. Build detailed success log information
                int serialNumber = 1;
                foreach (var result in closeAccountingDayResultDtos)
                {
                    var branchMessage = $"[{serialNumber}] Branch: {result.BranchName}, Branch Code: {result.BranchCode}, Success: {result.IsSuccess}, Message: {result.Message}";
                    logDetails += branchMessage + " | ";  // Append each branch's result
                    serialNumber++;

                    // 3. Log success or failure for each branch individually
                    var branchLogMessage = result.IsSuccess
                        ? $"Successfully closed accounting day for branch '{result.BranchName}' (Code: {result.BranchCode})."
                        : $"Failed to close accounting day for branch '{result.BranchName}' (Code: {result.BranchCode}): {result.Message}";

                    LogLevelInfo logLevel = result.IsSuccess ? LogLevelInfo.Information : LogLevelInfo.Error;

                    await BaseUtilities.LogAndAuditAsync(branchLogMessage, request, result.IsSuccess ? HttpStatusCodeEnum.OK : HttpStatusCodeEnum.InternalServerError, LogAction.CloseAccountingDayBranch, logLevel, logReference);
                }

                // 4. Customize the final success message based on whether centralization is used
                var successMessage = request.IsCentraliseOpening
                    ? "Centralized closing of the accounting day was successful for all branches. "
                    : $"Closing of the accounting day was successful for {request.Branches.Count} branches. ";

                // Include branch-specific details if available
                successMessage += !string.IsNullOrEmpty(logDetails)
                    ? $"Details: {logDetails.TrimEnd(' ', '|')}"
                    : "No detailed branch-specific information is available.";

                // Log the overall success message
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.CloseAccountingDayOverall, LogLevelInfo.Information, logReference);

                // Return a successful response with branch-specific results
                return ServiceResponse<List<CloseOrOpenAccountingDayResultDto>>.ReturnResultWith200(closeAccountingDayResultDtos, successMessage);
            }
            catch (Exception e)
            {
                // 5. Construct a detailed error message with exception details
                var errorMessage = request.IsCentraliseOpening
                    ? $"Error occurred during the centralized closing of the accounting day for all branches: {e.Message}"
                    : $"Error occurred while closing the accounting day for {request.Branches.Count} branches: {e.Message}";

                // Log and audit the error
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CloseAccountingDayError, LogLevelInfo.Error, logReference);

                // Return an error response with the constructed error message
                return ServiceResponse<List<CloseOrOpenAccountingDayResultDto>>.Return500(errorMessage);
            }
        }


    }

}
