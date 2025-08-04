


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
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.Handlers.AccountingDayOpening
{
    /// <summary>
    /// Command handler for opening accounting days, supporting both centralized and branch-specific operations.
    /// </summary>
    public class OpenOfAccountingDayCommandHandler : IRequestHandler<OpenOfAccountingDayCommand, ServiceResponse<List<CloseOrOpenAccountingDayResultDto>>>
    {
        private readonly ILogger<OpenOfAccountingDayCommandHandler> _logger; // Logger for actions and errors
        private readonly UserInfoToken _userInfoToken; // User info for auditing
        private readonly IAccountingDayRepository _accountingDayRepository; // Repository for accounting day operations
        private readonly IMediator _mediator; // Mediator for CQRS operations
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenOfAccountingDayCommandHandler"/> class.
        /// </summary>
        /// <param name="userInfoToken">User information for auditing.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        /// <param name="accountingDayRepository">Repository for accessing accounting day data.</param>
        /// <param name="mediator">Mediator for handling dependent commands.</param>
        /// <param name="utilityServicesRepository">Repository for retrieving utility data.</param>
        public OpenOfAccountingDayCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<OpenOfAccountingDayCommandHandler> logger,
            IAccountingDayRepository accountingDayRepository,
            IMediator mediator,
            IAPIUtilityServicesRepository utilityServicesRepository)
        {
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accountingDayRepository = accountingDayRepository ?? throw new ArgumentNullException(nameof(accountingDayRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _utilityServicesRepository = utilityServicesRepository ?? throw new ArgumentNullException(nameof(utilityServicesRepository));
        }

        /// <summary>
        /// Handles the `OpenOfAccountingDayCommand` to open accounting days for branches or centralized operations.
        /// </summary>
        /// <param name="request">The command containing opening details.</param>
        /// <param name="cancellationToken">Cancellation token for request cancellation.</param>
        /// <returns>A service response with the results of the operation.</returns>
        public async Task<ServiceResponse<List<CloseOrOpenAccountingDayResultDto>>> Handle(OpenOfAccountingDayCommand request, CancellationToken cancellationToken)
        {
            var successMessage = string.Empty;
            var logDetails = string.Empty;

            try
            {
                // Retrieve branches based on the operation type (centralized or branch-specific)
                List<BranchDto> branches = request.IsCentraliseOpening
                    ? await _utilityServicesRepository.GetBranches()
                    : new List<BranchDto>
                    {
                    new BranchDto
                    {
                        id = request.Branches.FirstOrDefault()?.BranchId,
                        name = request.Branches.FirstOrDefault()?.BranchName,
                        branchCode = request.Branches.FirstOrDefault()?.BranchCode
                    }
                    };

                // Perform the operation to open accounting days
                var resultDtos = await _accountingDayRepository.OpenAccountingDayForBranches(request.Date, request.Branches, request.IsCentraliseOpening, branches);

                // Build detailed logging information
                int serialNumber = 1;
                foreach (var result in resultDtos)
                {
                    logDetails += $"[{serialNumber}] Branch: {result.BranchName} (Code: {result.BranchCode}), Success: {result.IsSuccess}, Message: {result.Message} | ";
                    serialNumber++;
                }

                if (!string.IsNullOrWhiteSpace(logDetails))
                {
                    successMessage = $"Details: {logDetails.TrimEnd(' ', '|')} By {_userInfoToken.FullName}";
                }

                // Handle single branch scenarios with conditional logging
                if (resultDtos.Count == 1)
                {
                    var isSuccess = resultDtos.FirstOrDefault()?.IsSuccess ?? false;

                    if (!isSuccess)
                    {
                        _logger.LogWarning(successMessage);
                        await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.AccountingDayOpen, LogLevelInfo.Warning);
                        return ServiceResponse<List<CloseOrOpenAccountingDayResultDto>>.Return403(resultDtos, successMessage);
                    }

                    _logger.LogInformation(successMessage);
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.AccountingDayOpen, LogLevelInfo.Information);
                    return ServiceResponse<List<CloseOrOpenAccountingDayResultDto>>.ReturnResultWith200(resultDtos, successMessage);
                }

                // Log results for multi-branch operations
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.AccountingDayOpen, LogLevelInfo.Information);

                // Return success response
                return ServiceResponse<List<CloseOrOpenAccountingDayResultDto>>.ReturnResultWith200(resultDtos, successMessage);
            }
            catch (Exception ex)
            {
                // Construct and log error message
                var errorMessage = request.IsCentraliseOpening
                    ? $"Error during centralized accounting day opening: {ex.Message}"
                    : $"Error opening accounting day for {request.Branches?.Count ?? 0} branch(es): {ex.Message}";

                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingDayOpen, LogLevelInfo.Error);

                // Return failure response
                return ServiceResponse<List<CloseOrOpenAccountingDayResultDto>>.Return500(errorMessage);
            }
        }
    }

}

