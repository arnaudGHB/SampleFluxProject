using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CBS.TransactionManagement.Repository.AccountingDayOpening
{

    public class AccountingDayRepository : GenericRepository<AccountingDay, TransactionContext>, IAccountingDayRepository
    {
        private readonly ILogger<AccountingDayRepository> _logger; // Logger for capturing log messages
        private readonly UserInfoToken _userInfoToken; // User information token for capturing user details
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of Work for transaction management
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository; // User information token for capturing user details
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvisioningHistoryRepository; // User information token for capturing user details

        // Constructor for initializing the repository
        public AccountingDayRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<AccountingDayRepository> logger = null, UserInfoToken userInfoToken = null, IUnitOfWork<TransactionContext> uow = null, IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository = null, ISubTellerProvisioningHistoryRepository subTellerProvisioningHistoryRepository = null)
            : base(unitOfWork) // Call base constructor with unitOfWork
        {
            _logger = logger; // Initialize logger
            _userInfoToken = userInfoToken; // Initialize user info token
            _uow = uow ?? unitOfWork; // Fallback to unitOfWork if uow is null
            _primaryTellerProvisioningHistoryRepository=primaryTellerProvisioningHistoryRepository;
            _subTellerProvisioningHistoryRepository=subTellerProvisioningHistoryRepository;
        }
        public async Task<bool> DeleteAccountingDay(DateTime date, string branchId = null)
        {
            try
            {
                // Query to find the accounting day for the given date and branch
                var accountingDay = FindBy(ad => ad.Date == date && (ad.BranchId == branchId || ad.IsCentralized) && !ad.IsClosed)
                    .FirstOrDefault();

                // Check if the accounting day exists and is not closed
                if (accountingDay == null)
                {
                    // Log error message
                    var errorMessage = $"No open accounting day found for branch {branchId ?? "centralized system"} on {date.ToShortDateString()}.";
                    _logger?.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw exception
                }

                // Remove the accounting day from the repository
                Remove(accountingDay);
                await _uow.SaveAsync(); // Save changes to the database asynchronously

                // Log success message
                _logger?.LogInformation($"Successfully deleted the accounting day for {(branchId == null ? "the centralized system" : $"branch {branchId}")} on {date.ToShortDateString()}.");
                return true; // Return success status
            }
            catch (Exception ex)
            {
                // Log exception
                var errorMessage = $"{ex}, Error deleting accounting day for {(branchId == null ? "the centralized system" : $"branch {branchId}")} on {date.ToShortDateString()}.";
                _logger?.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage); // Throw exception
            }
        }

        // Method to get the current accounting day for a specific branch or centrally
        public DateTime GetCurrentAccountingDay(string branchId = null)
        {
            // First, search for the open accounting day specific to the given branchId
            var accountingDay = FindBy(ad => ad.BranchId == branchId && !ad.IsClosed && !ad.IsDeleted)
                .OrderByDescending(ad => ad.Date.Date) // Order by date descending to get the most recent
                .FirstOrDefault(); // Get the first (most recent) result

            // If no open accounting day is found for the branchId, search for a centralized accounting day
            if (accountingDay == null)
            {
                accountingDay = FindBy(ad => ad.IsCentralized && !ad.IsClosed && !ad.IsDeleted)
                    .OrderByDescending(ad => ad.Date.Date) // Order by date descending to get the most recent
                    .FirstOrDefault(); // Get the first (most recent) result
            }

            // Check if no accounting day was found
            if (accountingDay == null)
            {
                // Log error message
                var errorMessage = $"No open accounting day found for branch {branchId ?? "centralized system"}.";
                _logger?.LogError(errorMessage); // Log error message
                throw new InvalidOperationException(errorMessage); // Throw exception
            }

            return accountingDay.Date; // Return the date of the current accounting day
        }

        /// <summary>
        /// Performs actions on an accounting day such as reopening, closing, or removing it.
        /// </summary>
        /// <param name="id">The ID of the accounting day.</param>
        /// <param name="option">The action to perform: "Reopened", "Close", or "Remove".</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        public async Task<CloseOrOpenAccountingDayResultDto> AccountingDayAction(string id, string option)
        {
            // Retrieve the accounting day record by ID.
            var existingDay = await FindAsync(id);

            if (existingDay == null)
            {
                // Log and throw an error if the accounting day is not found.
                var errorMessage = $"Accounting day with ID {id} was not found.";
                _logger?.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            string message = string.Empty;

            try
            {
                switch (option)
                {
                    case "Reopened":
                        // Reopen the accounting day.
                        existingDay.Note += $", Reopening of accounting day {existingDay.Date:dd/MM/yyyy} by {_userInfoToken.FullName}";
                        existingDay.IsClosed = false;
                        existingDay.ReOpenedDate = BaseUtilities.UtcNowToDoualaTime();
                        message = $"Reopening of accounting day {existingDay.Date:dd/MM/yyyy} by {_userInfoToken.FullName} was successful.";
                        break;

                    case "Close":
                        // Validate provisioning histories before closing.
                        var validationError = await ValidateProvisioningHistories(existingDay.Date, CloseOfDayStatus.CLOSED, existingDay.BranchId);
                        if (!string.IsNullOrEmpty(validationError))
                        {
                            _logger?.LogWarning(validationError);
                            throw new InvalidOperationException(validationError);
                        }

                        // Close the accounting day.
                        existingDay.Note += $", Closing of accounting day {existingDay.Date:dd/MM/yyyy} by {_userInfoToken.FullName}";
                        existingDay.IsClosed = true;
                        existingDay.ClosedAt = BaseUtilities.UtcNowToDoualaTime();
                        existingDay.ClosedBy = _userInfoToken.FullName;
                        message = $"Closing of accounting day {existingDay.Date:dd/MM/yyyy} by {_userInfoToken.FullName} was successful.";
                        break;

                    case "Remove":
                        // Mark the accounting day as deleted.
                        existingDay.Note += $", Deletion of accounting day {existingDay.Date:dd/MM/yyyy} by {_userInfoToken.FullName}";
                        existingDay.IsDeleted = true;
                        message = $"Deletion of accounting day {existingDay.Date:dd/MM/yyyy} by {_userInfoToken.FullName} was successful.";
                        break;

                    default:
                        // Handle invalid options.
                        var invalidOptionMessage = $"Invalid action option: {option}. Valid options are 'Reopened', 'Close', or 'Remove'.";
                        _logger?.LogWarning(invalidOptionMessage);
                        throw new ArgumentException(invalidOptionMessage);
                }

                // Update the accounting day record and save changes.
                Update(existingDay);
                await _uow.SaveAsync();

                // Log the successful operation.
                _logger?.LogInformation(message);
                await BaseUtilities.LogAndAuditAsync(message, existingDay, HttpStatusCodeEnum.OK, LogAction.AccountingDayAction, LogLevelInfo.Information);
            }
            catch (Exception ex)
            {
                // Handle and log unexpected exceptions.
                var errorMessage = $"An error occurred while performing '{option}' on accounting day {existingDay.Date:dd/MM/yyyy}: {ex.Message}";
                _logger?.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, existingDay, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingDayAction, LogLevelInfo.Error);
                throw;
            }

            // Return the operation result.
            return new CloseOrOpenAccountingDayResultDto
            {
                BranchCode = "", // Optionally populate branch details if available.
                BranchId = existingDay.BranchId,
                BranchName = "",
                IsSuccess = true,
                Message = message,
            };
        }

        public async Task<List<AccountingDayDto>> GetAccountingDays(DateTime date, List<BranchDto> branches, string queryParameter, bool byBranch)
        {
            // Define the base query to filter by date and exclude deleted records
            var query = FindBy(ad => ad.Date.Date == date.Date && !ad.IsDeleted);

            // Apply branch filtering if requested
            if (byBranch)
            {
                if (string.IsNullOrEmpty(queryParameter))
                {
                    throw new ArgumentException("Query parameter cannot be null or empty when filtering by branch.");
                }
                query = query.Where(ad => ad.BranchId == queryParameter);
            }

            // Apply additional filters based on the queryParameter for status (open, close, all)
            switch (queryParameter?.ToLower())
            {
                case "open":
                    query = query.Where(ad => !ad.IsClosed);
                    break;
                case "close":
                    query = query.Where(ad => ad.IsClosed);
                    break;
                case "all":
                    // No additional filter needed
                    break;
                default:
                    throw new ArgumentException("Invalid query parameter value for status.");
            }

            // Execute the query and get the list of AccountingDay entities
            var accountingDays = await query.ToListAsync();

            // Use a dictionary for quick branch lookup
            var branchDict = branches.ToDictionary(b => b.id, b => b);

            // Convert the entities to DTOs
            var accountingDayDtos = accountingDays.Select(ad =>
            {
                if (ad.BranchId == null)
                {
                    _logger?.LogWarning($"AccountingDay with Id {ad.Id} has a null BranchId.");
                }

                var branch = ad.BranchId != null && branchDict.ContainsKey(ad.BranchId)
                    ? branchDict[ad.BranchId]
                    : null;

                return new AccountingDayDto
                {
                    Id = ad.Id,
                    BranchId = ad.BranchId,
                    Date = ad.Date,
                    IsClosed = ad.IsClosed,
                    ClosedBy = ad.ClosedBy,
                    OpenedBy = ad.OpenedBy,
                    ClosedAt = ad.ClosedAt,
                    OpenedAt = ad.OpenedAt,
                    Note = ad.Note,
                    ReOpenedDate = ad.ReOpenedDate,
                    BranchCode = branch?.branchCode ?? "N/A",
                    BranchName = branch?.name ?? "N/A",
                    IsCentralized = ad.IsCentralized
                };
            }).ToList();

            // Log success message
            _logger?.LogInformation($"Successfully retrieved {accountingDayDtos.Count} accounting days for {date.ToShortDateString()} with query parameter '{queryParameter}'.");

            return accountingDayDtos; // Return the list of AccountingDayDto
        }



        // Method to open new accounting days for a list of branches or centralized system
        public async Task<List<AccountingDay>> OpenAccountingDays(DateTime date, bool isCentralized = false, List<string> branchIds = null)
        {
            var openedDays = new List<AccountingDay>(); // List to hold the newly opened accounting days

            if (branchIds == null || !branchIds.Any())
            {
                // If no branch IDs are provided, open a centralized accounting day
                var existingDay = FindBy(ad => ad.Date == date && ad.IsCentralized)
                    .FirstOrDefault();

                // Check if a centralized accounting day already exists
                if (existingDay != null)
                {
                    // Log error message
                    var errorMessage = $"An accounting day for centralized system on {date.ToShortDateString()} already exists.";
                    _logger?.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw exception
                }

                // Create a new centralized accounting day
                var accountingDay = new AccountingDay
                {
                    BranchId = null,
                    Date = date,
                    IsClosed = false,
                    IsCentralized = true
                };

                // Add the new accounting day to the repository
                Add(accountingDay);
                openedDays.Add(accountingDay); // Add to the list of opened days
            }
            else
            {
                // Open accounting days for each branch in the branchIds list
                foreach (var branchId in branchIds)
                {
                    var existingDay = FindBy(ad => ad.Date == date && ad.BranchId == branchId)
                        .FirstOrDefault();

                    // Check if the accounting day already exists for the branch
                    if (existingDay != null)
                    {
                        // Log error message
                        var errorMessage = $"An accounting day for branch {branchId} on {date.ToShortDateString()} already exists.";
                        _logger?.LogError(errorMessage); // Log error message
                        throw new InvalidOperationException(errorMessage); // Throw exception
                    }

                    // Create a new accounting day for the branch
                    var accountingDay = new AccountingDay
                    {
                        BranchId = branchId,
                        Date = date,
                        IsClosed = false,
                        IsCentralized = false
                    };

                    // Add the new accounting day to the repository
                    Add(accountingDay);
                    openedDays.Add(accountingDay); // Add to the list of opened days
                }
            }

            await _uow.SaveAsync(); // Save changes to the database asynchronously

            // Log success message
            _logger?.LogInformation($"Successfully opened accounting days for {(branchIds == null || !branchIds.Any() ? "the centralized system" : $"branches {string.Join(", ", branchIds)}")} on {date.ToShortDateString()}.");

            return openedDays; // Return the list of newly created accounting days
        }
        // Method to open new accounting days for a list of branches or centralized system

        /// <summary>
        /// Opens accounting days for a list of branches, with support for centralized and branch-specific modes.
        /// </summary>
        /// <param name="date">The accounting date to be opened.</param>
        /// <param name="branches">List of branches for branch-specific opening.</param>
        /// <param name="isCentralized">Flag indicating if the operation is centralized.</param>
        /// <param name="branchDtos">Additional branch details for validation.</param>
        /// <returns>List of results indicating success or failure for each branch.</returns>
        public async Task<List<CloseOrOpenAccountingDayResultDto>> OpenAccountingDayForBranches(DateTime date, List<BranchListing> branches, bool isCentralized, List<BranchDto> branchDtos)
        {
            var results = new List<CloseOrOpenAccountingDayResultDto>();
            var accountingDays = new List<AccountingDay>();
            var isCentralizedOpen = false;

            try
            {
                // Step 1: Validate provisioning histories to ensure no open history exists for the provided date.
                await ValidateProvisioningHistories(date, CloseOfDayStatus.OOD);

                // Step 2: Fetch all currently open accounting days.
                var openDays = FindBy(ad => !ad.IsClosed && !ad.IsDeleted).ToList();

                // Step 3: Handle centralized accounting day logic.
                if (isCentralized)
                {
                    await HandleCentralizedDayLogic(date, openDays, branchDtos, accountingDays);
                    isCentralizedOpen = true; // Centralized day successfully opened.
                    return results; // Exit after centralized operation.
                }

                // Step 4: Handle branch-specific accounting days.
                await HandleBranchSpecificDays(date, openDays, branches, branchDtos, accountingDays, results);

                // Step 5: Save new accounting days to the database.
                if (accountingDays.Any())
                {
                    AddRange(accountingDays);
                    await _uow.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while opening accounting days: {ex.Message}";
                _logger?.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingDayOpen, LogLevelInfo.Error);
                throw;
            }

            return results; // Return results for each branch.
        }

        /// <summary>
        /// Validates provisioning histories for open sub-tellers and primary tellers.
        /// Ensures all provisioning histories are closed before opening a new accounting day.
        /// </summary>
        private async Task ValidateProvisioningHistories(DateTime date, CloseOfDayStatus dayStatus)
        {
            var subTellerProvisioningHistories = await _subTellerProvisioningHistoryRepository.FindBy(x => x.OpenedDate.Value == date).ToListAsync();
            if (subTellerProvisioningHistories.Any(x => x.ClossedStatus == dayStatus.ToString()))
            {
                var errorMessage = GenerateProvisioningErrorMessage(subTellerProvisioningHistories);
                _logger?.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, date, HttpStatusCodeEnum.BadRequest, LogAction.ValidateProvisioningHistoriesForAccountingOpenOfDay, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }

            var primaryTellerProvisioningHistories = await _primaryTellerProvisioningHistoryRepository.FindBy(x => x.OpenedDate.Value == date).ToListAsync();
            if (primaryTellerProvisioningHistories.Any(x => x.ClossedStatus == dayStatus.ToString()))
            {
                var errorMessage = GenerateProvisioningErrorMessage(primaryTellerProvisioningHistories);
                _logger?.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, date, HttpStatusCodeEnum.BadRequest, LogAction.ValidateProvisioningHistoriesForAccountingOpenOfDay, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }
        }
        /// <summary>
        /// Validates provisioning histories for open sub-tellers and primary tellers.
        /// Ensures all provisioning histories are closed before opening a new accounting day.
        /// </summary>
        private async Task<string> ValidateProvisioningHistories(DateTime date, CloseOfDayStatus dayStatus,string branchid)
        {
            var subTellerProvisioningHistories = await _subTellerProvisioningHistoryRepository.FindBy(x => x.OpenedDate.Value == date && x.BranchId==branchid).ToListAsync();
            if (subTellerProvisioningHistories.Any(x => x.ClossedStatus != dayStatus.ToString()))
            {
                var errorMessage = GenerateProvisioningErrorMessage(subTellerProvisioningHistories);
                _logger?.LogError(errorMessage);
                return errorMessage;
            }

            var primaryTellerProvisioningHistories = await _primaryTellerProvisioningHistoryRepository.FindBy(x => x.OpenedDate.Value == date && x.BranchId==branchid).ToListAsync();
            if (primaryTellerProvisioningHistories.Any(x => x.ClossedStatus != dayStatus.ToString()))
            {
                var errorMessage = GenerateProvisioningErrorMessage(primaryTellerProvisioningHistories);
                _logger?.LogError(errorMessage);
                return errorMessage;
            }
            return null;
        }




        /// <summary>
        /// Generates an error message for open provisioning histories.
        /// </summary>
        private string GenerateProvisioningErrorMessage(IEnumerable<dynamic> provisioningHistories)
        {
            var openBranchDays = string.Join(", ", provisioningHistories.Select(b => $"{b.BranchCode}-{b.BranchName} (Opened on {b.OpenDate:dd/MM/yyyy})"));
            return $"Operation aborted: The following branch-specific accounting days are still open and must be closed: {openBranchDays}.";
        }

        /// <summary>
        /// Handles the logic for opening a centralized accounting day.
        /// </summary>
        private async Task HandleCentralizedDayLogic(DateTime date, List<AccountingDay> openDays, List<BranchDto> branchDtos, List<AccountingDay> accountingDays)
        {
            // Step 1: Validate no centralized accounting day is currently open.
            if (openDays.Any(ad => ad.IsCentralized))
            {
                var centralizedDayInfo = string.Join(", ", openDays.Where(ad => ad.IsCentralized).Select(cd => $"Centralized Day (Opened on {cd.Date:dd/MM/yyyy})"));
                var errorMessage = $"Operation aborted: The following centralized day(s) are still open: {centralizedDayInfo}.";
                _logger?.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.AccountingDayOpen, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }

            // Step 2: Ensure no branch-specific accounting days are open that conflict with centralized day.
            var openBranchDays = openDays
                .Where(ad => !ad.IsCentralized)
                .Join(branchDtos.Select(b => new { BranchId = b.id.ToString(), b.branchCode, b.name }),
                    ad => ad.BranchId.ToString(),
                    b => b.BranchId,
                    (ad, b) => new { BranchCode = b.branchCode, BranchName = b.name, OpenDate = ad.Date })
                .ToList();

            if (openBranchDays.Any())
            {
                var branchesWithOpenDays = string.Join(", ", openBranchDays.Select(b => $"{b.BranchCode}-{b.BranchName} (Opened on {b.OpenDate:dd/MM/yyyy})"));
                var errorMessage = $"Operation aborted: The following branch-specific days are still open: {branchesWithOpenDays}.";
                _logger?.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.BadRequest, LogAction.AccountingDayOpen, LogLevelInfo.Warning);
                throw new InvalidOperationException(errorMessage);
            }

            // Step 3: Create and add the centralized accounting day.
            var centralizedDay = new AccountingDay
            {
                Date = date,
                Id = BaseUtilities.GenerateUniqueNumber(),
                IsCentralized = true,
                OpenedBy = _userInfoToken?.FullName,
                OpenedAt = BaseUtilities.UtcNowToDoualaTime(),
                Note = $"Centralized opening by {_userInfoToken.FullName}."
            };
            accountingDays.Add(centralizedDay);
            var successMessage = $"Successfully opened centralized accounting day on {date:dd/MM/yyyy}.";
            _logger?.LogInformation(successMessage);
            await BaseUtilities.LogAndAuditAsync(successMessage, centralizedDay, HttpStatusCodeEnum.OK, LogAction.AccountingDayOpen, LogLevelInfo.Information);
        }

        /// <summary>
        /// Handles the logic for opening branch-specific accounting days.
        /// </summary>
        private async Task HandleBranchSpecificDays(DateTime date, List<AccountingDay> openDays, List<BranchListing> branches, List<BranchDto> branchDtos, List<AccountingDay> accountingDays, List<CloseOrOpenAccountingDayResultDto> results)
        {
            foreach (var branch in branches)
            {
                var result = new CloseOrOpenAccountingDayResultDto
                {
                    BranchId = branch.BranchId,
                    BranchCode = branch.BranchCode,
                    BranchName = branch.BranchName
                };

                try
                {
                    var branchOpenDays = openDays.Where(ad => ad.BranchId == branch.BranchId).ToList();

                    if (branchOpenDays.Any())
                    {
                        // Error if branch has an open day.
                        var errorMessage = $"Branch {branch.BranchCode} has an open day for {branchOpenDays.First().Date:dd/MM/yyyy}.";
                        _logger?.LogError(errorMessage);
                        await BaseUtilities.LogAndAuditAsync(errorMessage, branch, HttpStatusCodeEnum.BadRequest, LogAction.AccountingDayOpen, LogLevelInfo.Warning);
                        result.IsSuccess = false;
                        result.Message = errorMessage;
                    }
                    else
                    {
                        // Create and add branch-specific accounting day.
                        var accountingDay = new AccountingDay
                        {
                            Date = date,
                            Id = BaseUtilities.GenerateUniqueNumber(),
                            BranchId = branch.BranchId,
                            IsCentralized = false,
                            OpenedBy = _userInfoToken?.FullName,
                            Note = $"Opened by {_userInfoToken.FullName} for branch {branch.BranchCode}.",
                            OpenedAt = BaseUtilities.UtcNowToDoualaTime()
                        };
                        accountingDays.Add(accountingDay);
                        var successMessage = $"Successfully opened accounting day for branch {branch.BranchCode}.";
                        _logger?.LogInformation(successMessage);
                        await BaseUtilities.LogAndAuditAsync(successMessage, accountingDay, HttpStatusCodeEnum.OK, LogAction.AccountingDayOpen, LogLevelInfo.Information);
                        result.IsSuccess = true;
                        result.Message = successMessage;
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Error opening accounting day for branch {branch.BranchCode}: {ex.Message}";
                    _logger?.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, branch, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingDayOpen, LogLevelInfo.Error);
                    result.IsSuccess = false;
                    result.Message = errorMessage;
                }
                results.Add(result);
            }
        }



        public async Task<List<CloseOrOpenAccountingDayResultDto>> OpenAccountingDayForBranchesxx(DateTime date, List<BranchListing> branches, bool isCentralized, List<BranchDto> branchDtos)
        {
            var results = new List<CloseOrOpenAccountingDayResultDto>();
            var accountingDays = new List<AccountingDay>();
            var isCentralizedOpen = false;

            try
            {
               

                // 1. Check if any accounting day is still open
                var openDays = FindBy(ad => !ad.IsClosed && !ad.IsDeleted).ToList();

                // 2. If it's a centralized operation, first check if any centralized day is already open
                if (isCentralized)
                {
                    // Check if any centralized accounting day is still open
                    if (openDays.Any(ad => ad.IsCentralized))
                    {
                        var centralizedOpenDays = openDays
                            .Where(ad => ad.IsCentralized)
                            .Select(ad => new { ad.Date })
                            .ToList();

                        var centralizedDayInfo = string.Join(", ", centralizedOpenDays.Select(cd => $"Centralized Day (Opened on {cd.Date:dd/MM/yyyy})"));
                        var errorMessage = $"Operation aborted: The following centralized accounting day(s) are still open and must be closed before opening a new centralized day: {centralizedDayInfo}.";
                        _logger?.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }

                    // Check if any branch-specific days are still open, preventing the centralized day from being opened
                    var openBranchDays = openDays
                        .Where(ad => !ad.IsCentralized) // Branch-specific days only
                        .Join(branchDtos.Select(b => new { BranchId = b.id.ToString(), b.branchCode, b.name }),
                        ad => ad.BranchId.ToString(),
                        b => b.BranchId,
                        (ad, b) => new { BranchCode = b.branchCode, BranchName = b.name, OpenDate = ad.Date })
                        .ToList();

                    if (openBranchDays.Any())
                    {
                        var branchesWithOpenDays = string.Join(", ", openBranchDays.Select(b => $"{b.BranchCode}-{b.BranchName} (Opened on {b.OpenDate:dd/MM/yyyy})"));
                        var errorMessage = $"Operation aborted: The following branch-specific accounting days are still open and must be closed before opening a centralized day: {branchesWithOpenDays}.";
                        _logger?.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }

                    // Proceed to open centralized day
                    var centralizedDay = new AccountingDay
                    {
                        Date = date,
                        Id = BaseUtilities.GenerateUniqueNumber(),
                        BranchId = null, // Centralized, no specific branch
                        IsClosed = false,
                        IsCentralized = true,
                        OpenedBy = _userInfoToken?.FullName,
                        OpenedAt = BaseUtilities.UtcNowToDoualaTime(),
                        Note = $"Centralized opening of accounting day by {_userInfoToken.FullName} for the centralized system."
                    };
                    accountingDays.Add(centralizedDay);
                    isCentralizedOpen = true;

                    if (accountingDays.Any())
                    {
                        AddRange(accountingDays);
                        await _uow.SaveAsync();
                    }

                    _logger?.LogInformation($"Successfully opened the centralized accounting day on {date:dd/MM/yyyy}.");
                    return results; // Exit after centralized operation is completed
                }

                // 3. If not centralized, check branch-specific accounting days
                var openBranchDaysForBranches = openDays
                    .Where(ad => !ad.IsCentralized)
                    .Join(branchDtos.Select(b => new { BranchId = b.id.ToString(), b.branchCode, b.name }),
                    ad => ad.BranchId.ToString(),
                    b => b.BranchId,
                    (ad, b) => new { BranchCode = b.branchCode, BranchName = b.name, OpenDate = ad.Date })
                    .ToList();

                // Notify the user if any branch-specific days are open
                if (openBranchDaysForBranches.Any())
                {
                    var branchesWithOpenDays = string.Join(", ", openBranchDaysForBranches.Select(b => $"{b.BranchCode}-{b.BranchName} (Opened on {b.OpenDate:dd/MM/yyyy})"));
                    _logger?.LogWarning($"The following branches have days still open: {branchesWithOpenDays}. They need to be closed before proceeding.");
                }

                // 4. Process each branch individually
                foreach (var branch in branches)
                {
                    var result = new CloseOrOpenAccountingDayResultDto
                    {
                        BranchId = branch.BranchId,
                        BranchCode = branch.BranchCode,
                        BranchName = branch.BranchName
                    };

                    try
                    {
                        var branchOpenDays = openDays.Where(ad => ad.BranchId == branch.BranchId && !ad.IsCentralized).ToList();

                        if (branchOpenDays.Any())
                        {
                            var pendingOpenDay = branchOpenDays.First();
                            var errorMessage = $"Branch {branch.BranchCode} has an open day for {pendingOpenDay.Date:dd/MM/yyyy}. Please close the pending day first.";
                            _logger?.LogError(errorMessage);
                            result.IsSuccess = false;
                            result.Message = errorMessage;
                        }
                        else
                        {
                            var accountingDay = new AccountingDay
                            {
                                Date = date,
                                Id = BaseUtilities.GenerateUniqueNumber(),
                                BranchId = branch.BranchId,
                                IsClosed = false,
                                IsCentralized = false,
                                OpenedBy = _userInfoToken?.FullName,
                                Note = $"Branch-specific opening of accounting day by {_userInfoToken.FullName} for branch: {branch.BranchCode}-{branch.BranchName}",
                                OpenedAt = BaseUtilities.UtcNowToDoualaTime()
                            };
                            accountingDays.Add(accountingDay);

                            _logger?.LogInformation($"Successfully opened the accounting day for branch {branch.BranchCode} on {date:dd/MM/yyyy}.");

                            result.IsSuccess = true;
                            result.Message = $"Successfully opened the accounting day. [Day: {date:dd/MM/yyyy}, Code: {branch.BranchCode}, Branch: {branch.BranchName}]";
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"{ex.Message}, Error opening accounting day for branch {branch.BranchCode} on {date:dd/MM/yyyy}.";
                        _logger?.LogError(errorMessage);

                        result.IsSuccess = false;
                        result.Message = errorMessage;
                    }
                    results.Add(result);
                }

                // 5. Save branch-specific accounting day records
                if (accountingDays.Any())
                {
                    AddRange(accountingDays);
                    await _uow.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError($"An error occurred while opening accounting days: {ex.Message}");
                throw;
            }

            return results;
        }
        public async Task<List<CloseOrOpenAccountingDayResultDto>> CloseAccountingDayForBranchesxx(DateTime date, List<BranchListing> branches, bool isCentralized)
        {
            var results = new List<CloseOrOpenAccountingDayResultDto>();
            var accountingDaysToUpdate = new List<AccountingDay>();
            var isCentralizedClosed = false;

            try
            {
                // Check if centralized and no branches are provided
                if (isCentralized && !branches.Any())
                {
                    var result = new CloseOrOpenAccountingDayResultDto
                    {
                        BranchId = null, // Since it's centralized, there's no specific branch
                        BranchCode = null,
                        BranchName = "Centralized System"
                    };

                    try
                    {
                        // Find an open centralized accounting day for the specified date
                        var existingCentralizedDay = FindBy(ad => ad.Date.Date == date.Date && ad.IsCentralized && !ad.IsClosed).FirstOrDefault();
                        if (existingCentralizedDay != null)
                        {
                            // Close the existing centralized accounting day
                            existingCentralizedDay.IsClosed = true;
                            existingCentralizedDay.ClosedBy = _userInfoToken?.FullName;
                            existingCentralizedDay.ClosedAt = BaseUtilities.UtcNowToDoualaTime();
                            accountingDaysToUpdate.Add(existingCentralizedDay);
                            isCentralizedClosed = true; // Mark that the centralized day is closed
                            existingCentralizedDay.Note += $", Closing of centralise accounting day by {_userInfoToken.FullName}\n";
                            result.IsSuccess = true;
                            result.Message = "Successfully closed the centralized accounting day.";
                        }
                        else
                        {
                            var errorMessage = $"No open centralized accounting day found on {date.ToShortDateString()}.";
                            _logger?.LogError(errorMessage);

                            result.IsSuccess = false;
                            result.Message = errorMessage;
                        }
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"{ex.Message}, Error closing centralized accounting day on {date.ToShortDateString()}.";
                        _logger?.LogError(errorMessage);

                        result.IsSuccess = false;
                        result.Message = errorMessage;
                    }

                    results.Add(result);
                }
                else
                {
                    // Process each branch if branches are provided or if centralized and branches exist
                    foreach (var branch in branches)
                    {
                        var result = new CloseOrOpenAccountingDayResultDto
                        {
                            BranchId = branch.BranchId,
                            BranchCode = branch.BranchCode,
                            BranchName = branch.BranchName
                        };

                        try
                        {
                            if (isCentralized && !isCentralizedClosed)
                            {
                                var existingCentralizedDay = FindBy(ad => ad.Date.Date == date.Date && ad.IsCentralized && !ad.IsClosed).FirstOrDefault();
                                if (existingCentralizedDay != null)
                                {
                                    existingCentralizedDay.IsClosed = true;
                                    existingCentralizedDay.ClosedBy = _userInfoToken?.FullName;
                                    existingCentralizedDay.ClosedAt = BaseUtilities.UtcNowToDoualaTime();
                                    accountingDaysToUpdate.Add(existingCentralizedDay);
                                    isCentralizedClosed = true;
                                }
                            }

                            var accountingDay = FindBy(ad => ad.Date.Date == date.Date &&
                                                             ad.BranchId == branch.BranchId &&
                                                             ((ad.IsCentralized == isCentralized) ||
                                                             (!ad.IsCentralized && ad.BranchId == branch.BranchId)))
                                                  .FirstOrDefault();

                            if (accountingDay == null)
                            {
                                accountingDay = FindBy(ad => (ad.BranchId == branch.BranchId || ad.IsCentralized) && !ad.IsClosed)
                                    .OrderByDescending(ad => ad.Date)
                                    .FirstOrDefault();

                                if (accountingDay != null)
                                {
                                    accountingDay.IsClosed = true;
                                    accountingDay.ClosedBy = _userInfoToken?.FullName;
                                    accountingDay.ClosedAt = BaseUtilities.UtcNowToDoualaTime();
                                    accountingDaysToUpdate.Add(accountingDay);

                                    _logger?.LogInformation($"Successfully closed the accounting day for {(branch.BranchCode == null ? "the centralized system" : $"branch {branch.BranchCode}")} on {accountingDay.Date.ToShortDateString()}.");

                                    result.IsSuccess = true;
                                    result.Message = "Successfully closed the accounting day.";
                                }
                                else
                                {
                                    var errorMessage = $"No open accounting day found for branch {branch.BranchCode ?? branch.BranchId.ToString() ?? "centralized system"} on {date.ToShortDateString()}.";
                                    _logger?.LogError(errorMessage);

                                    result.IsSuccess = false;
                                    result.Message = errorMessage;
                                    results.Add(result);
                                    continue;
                                }
                            }
                            else
                            {
                                accountingDay.IsClosed = true;
                                accountingDay.ClosedBy = _userInfoToken?.FullName;
                                accountingDay.ClosedAt = BaseUtilities.UtcNowToDoualaTime();
                                accountingDaysToUpdate.Add(accountingDay);

                                _logger?.LogInformation($"Successfully closed the accounting day for {(branch.BranchCode == null ? "the centralized system" : $"branch {branch.BranchCode}")} on {date.ToShortDateString()}.");

                                result.IsSuccess = true;
                                result.Message = "Successfully closed the accounting day.";
                            }
                        }
                        catch (Exception ex)
                        {
                            var errorMessage = $"{ex.Message}, Error closing accounting day for {(branch.BranchCode == null ? "the centralized system" : $"branch {branch.BranchCode}")} on {date.ToShortDateString()}.";
                            _logger?.LogError(errorMessage);

                            result.IsSuccess = false;
                            result.Message = errorMessage;
                        }

                        results.Add(result);
                    }
                }

                if (accountingDaysToUpdate.Any())
                {
                    UpdateRange(accountingDaysToUpdate);
                    await _uow.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError($"An error occurred while closing accounting days: {ex.Message}");
            }

            return results;
        }


        /// <summary>
        /// Closes accounting days for a list of branches, with support for centralized and branch-specific modes.
        /// Ensures all provisioning histories are closed before proceeding.
        /// </summary>
        /// <param name="date">The accounting date to be closed.</param>
        /// <param name="branches">List of branches for branch-specific closing.</param>
        /// <param name="isCentralized">Flag indicating if the operation is centralized.</param>
        /// <returns>List of results indicating success or failure for each branch.</returns>
        public async Task<List<CloseOrOpenAccountingDayResultDto>> CloseAccountingDayForBranches(DateTime date, List<BranchListing> branches, bool isCentralized)
        {
            var results = new List<CloseOrOpenAccountingDayResultDto>();
            var accountingDaysToUpdate = new List<AccountingDay>();
            var isCentralizedClosed = false;

            try
            {
            

                // Handle centralized accounting day closure if no branches are provided.
                if (isCentralized && !branches.Any())
                {
                    // Validate that all provisioning histories are closed before proceeding.
                    await ValidateProvisioningHistories(date, CloseOfDayStatus.CLOSED);
                    await CloseCentralizedAccountingDay(date, results, accountingDaysToUpdate);
                }
                else
                {
                    // Process each branch for closing accounting day.
                    foreach (var branch in branches)
                    {
                        var result = new CloseOrOpenAccountingDayResultDto
                        {
                            BranchId = branch.BranchId,
                            BranchCode = branch.BranchCode,
                            BranchName = branch.BranchName
                        };

                        try
                        {
                            var errorMessage = await ValidateProvisioningHistories(date, CloseOfDayStatus.CLOSED, branch.BranchId);
                            // Validate if all tills are closed for the branch.
                            if (errorMessage==null)
                            {
                                // Proceed to close accounting day for the branch.
                                await CloseBranchAccountingDay(date, branch, isCentralized, results, accountingDaysToUpdate, isCentralizedClosed);
                            }
                            else
                            {
                                // Log and audit error if tills are open.
                                result.IsSuccess = false;
                                result.Message = errorMessage;
                                await BaseUtilities.LogAndAuditAsync(
                                    result.Message,
                                    branch,
                                    HttpStatusCodeEnum.BadRequest,
                                    LogAction.TillValidationForAccountingDayClose,
                                    LogLevelInfo.Warning);
                                results.Add(result);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Handle exceptions for branch-specific closure.
                            var errorMessage = $"Error closing accounting day for branch {branch.BranchCode}: {ex.Message}";
                            _logger?.LogError(errorMessage);
                            await BaseUtilities.LogAndAuditAsync(
                                errorMessage,
                                branch,
                                HttpStatusCodeEnum.InternalServerError,
                                LogAction.AccountingDayClose,
                                LogLevelInfo.Error);

                            result.IsSuccess = false;
                            result.Message = errorMessage;
                            results.Add(result);
                        }
                    }
                }

                // Persist changes to the database.
                if (accountingDaysToUpdate.Any())
                {
                    UpdateRange(accountingDaysToUpdate);
                    await _uow.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions during the operation.
                var errorMessage = $"An error occurred while closing accounting days: {ex.Message}";
                _logger?.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    null,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingDayClose,
                    LogLevelInfo.Error);
            }

            return results;
        }

        /// <summary>
        /// Closes the centralized accounting day for the specified date.
        /// </summary>
        private async Task CloseCentralizedAccountingDay(DateTime date, List<CloseOrOpenAccountingDayResultDto> results, List<AccountingDay> accountingDaysToUpdate)
        {
            var result = new CloseOrOpenAccountingDayResultDto
            {
                BranchId = null, // No branch for centralized
                BranchCode = null,
                BranchName = "Centralized System"
            };

            try
            {
                // Find an open centralized accounting day for the specified date.
                var existingCentralizedDay = FindBy(ad => ad.Date.Date == date.Date && ad.IsCentralized && !ad.IsClosed).FirstOrDefault();
                if (existingCentralizedDay != null)
                {
                    // Close the existing centralized accounting day.
                    existingCentralizedDay.IsClosed = true;
                    existingCentralizedDay.ClosedBy = _userInfoToken?.FullName;
                    existingCentralizedDay.ClosedAt = BaseUtilities.UtcNowToDoualaTime();
                    accountingDaysToUpdate.Add(existingCentralizedDay);

                    existingCentralizedDay.Note += $", Closing of centralized accounting day by {_userInfoToken.FullName}";
                    result.IsSuccess = true;
                    result.Message = "Successfully closed the centralized accounting day.";

                    await BaseUtilities.LogAndAuditAsync(result.Message, existingCentralizedDay, HttpStatusCodeEnum.OK, LogAction.AccountingDayClose, LogLevelInfo.Information);
                }
                else
                {
                    var errorMessage = $"No open centralized accounting day found on {date.ToShortDateString()}.";
                    _logger?.LogError(errorMessage);
                    result.IsSuccess = false;
                    result.Message = errorMessage;

                    await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.NotFound, LogAction.AccountingDayClose, LogLevelInfo.Warning);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"{ex.Message}, Error closing centralized accounting day on {date.ToShortDateString()}.";
                _logger?.LogError(errorMessage);
                result.IsSuccess = false;
                result.Message = errorMessage;

                await BaseUtilities.LogAndAuditAsync(errorMessage, null, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingDayClose, LogLevelInfo.Error);
            }

            results.Add(result);
        }

       

        /// <summary>
        /// Closes the accounting day for a specific branch.
        /// </summary>
        private async Task CloseBranchAccountingDay(DateTime date, BranchListing branch, bool isCentralized, List<CloseOrOpenAccountingDayResultDto> results, List<AccountingDay> accountingDaysToUpdate, bool isCentralizedClosed)
        {
            var result = new CloseOrOpenAccountingDayResultDto
            {
                BranchId = branch.BranchId,
                BranchCode = branch.BranchCode,
                BranchName = branch.BranchName
            };

            try
            {
                // Close centralized day if applicable.
                if (isCentralized && !isCentralizedClosed)
                {
                    var existingCentralizedDay = FindBy(ad => ad.Date.Date == date.Date && ad.IsCentralized && !ad.IsClosed).FirstOrDefault();
                    if (existingCentralizedDay != null)
                    {
                        existingCentralizedDay.IsClosed = true;
                        existingCentralizedDay.ClosedBy = _userInfoToken?.FullName;
                        existingCentralizedDay.ClosedAt = BaseUtilities.UtcNowToDoualaTime();
                        accountingDaysToUpdate.Add(existingCentralizedDay);
                        isCentralizedClosed = true;
                    }
                }

                // Find and close the branch-specific accounting day.
                var accountingDay = FindBy(ad => ad.Date.Date == date.Date && ad.BranchId == branch.BranchId && !ad.IsClosed).FirstOrDefault();
                if (accountingDay == null)
                {
                    var errorMessage = $"No open accounting day found for branch {branch.BranchCode} on {date.ToShortDateString()}.";
                    _logger?.LogError(errorMessage);
                    result.IsSuccess = false;
                    result.Message = errorMessage;

                    await BaseUtilities.LogAndAuditAsync(errorMessage, branch, HttpStatusCodeEnum.NotFound, LogAction.AccountingDayClose, LogLevelInfo.Warning);
                }
                else
                {
                    accountingDay.IsClosed = true;
                    accountingDay.ClosedBy = _userInfoToken?.FullName;
                    accountingDay.ClosedAt = BaseUtilities.UtcNowToDoualaTime();
                    accountingDaysToUpdate.Add(accountingDay);

                    result.IsSuccess = true;
                    result.Message = "Successfully closed the accounting day.";
                    await BaseUtilities.LogAndAuditAsync(result.Message, accountingDay, HttpStatusCodeEnum.OK, LogAction.AccountingDayClose, LogLevelInfo.Information);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = $"{ex.Message}, Error closing accounting day for branch {branch.BranchCode}.";
                _logger?.LogError(errorMessage);
                result.IsSuccess = false;
                result.Message = errorMessage;

                await BaseUtilities.LogAndAuditAsync(errorMessage, branch, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingDayClose, LogLevelInfo.Error);
            }

            results.Add(result);
        }



        // Method to close an open accounting day for a single branch or centralized system
        public async Task<bool> CloseAccountingDay(DateTime date, string branchId = null)
        {
            try
            {
                // Query to find the open accounting day for the given date and branch
                var accountingDay = FindBy(ad => ad.Date == date && (ad.BranchId == branchId || ad.IsCentralized) && !ad.IsClosed)
                    .FirstOrDefault();

                // Check if an open accounting day was found
                if (accountingDay == null)
                {
                    // Log error message
                    var errorMessage = $"No open accounting day found for branch {branchId ?? "centralized system"} on {date.ToShortDateString()}.";
                    _logger?.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw exception
                }

                // Mark the accounting day as closed
                accountingDay.IsClosed = true;
                accountingDay.ClosedBy = _userInfoToken?.FullName; // Capture the name of the user who closed the day
                accountingDay.ClosedAt = DateTime.Now; // Capture the time when the day was closed

                // Update the accounting day in the repository
                Update(accountingDay);
                await _uow.SaveAsync(); // Save changes to the database asynchronously

                // Log success message
                _logger?.LogInformation($"Successfully closed the accounting day for {(branchId == null ? "the centralized system" : $"branch {branchId}")} on {date.ToShortDateString()}.");
                return true; // Return success status
            }
            catch (Exception ex)
            {
                // Log exception
                var errorMessage = $"{ex}, Error closing accounting day for {(branchId == null ? "the centralized system" : $"branch {branchId}")} on {date.ToShortDateString()}.";
                _logger?.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage); // Throw exception
            }
        }

        // Method to reopen a closed accounting day for a single branch or centralized system
        public async Task<bool> ReopenClosedAccountingDay(DateTime date, string branchId = null)
        {
            try
            {
                // Query to find the closed accounting day for the given date and branch
                var accountingDay = FindBy(ad => ad.Date == date && (ad.BranchId == branchId || ad.IsCentralized) && ad.IsClosed)
                    .FirstOrDefault();

                // Check if a closed accounting day was found
                if (accountingDay == null)
                {
                    // Log error message
                    var errorMessage = $"No closed accounting day found for branch {branchId ?? "centralized system"} on {date.ToShortDateString()}.";
                    _logger?.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw exception
                }

                // Mark the accounting day as reopened
                accountingDay.IsClosed = false;
                accountingDay.ClosedBy = null; // Clear the name of the user who closed the day
                accountingDay.ClosedAt = null; // Clear the time when the day was closed

                // Update the accounting day in the repository
                Update(accountingDay);
                await _uow.SaveAsync(); // Save changes to the database asynchronously

                // Log success message
                _logger?.LogInformation($"Successfully reopened the accounting day for {(branchId == null ? "the centralized system" : $"branch {branchId}")} on {date.ToShortDateString()}.");
                return true; // Return success status
            }
            catch (Exception ex)
            {
                // Log exception
                _logger?.LogError(ex, "Error reopening the accounting day.");
                var errorMessage = $"{ex}, Error closing accounting day for {(branchId == null ? "the centralized system" : $"branch {branchId}")} on {date.ToShortDateString()}.";
                _logger?.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage); // Throw exception
            }
        }
    }

}
