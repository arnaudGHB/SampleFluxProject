using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using CBS.TransactionManagement.Repository.AccountingDayOpening;

namespace CBS.TransactionManagement.Handlers
{


    /// <summary>
    /// Handles the request to retrieve a specific Teller's Account Balance based on the query provided.
    /// </summary>
    public class GetTellerAccountBalanceQueryHandler : IRequestHandler<GetTellerAccountBalanceQuery, ServiceResponse<TillOpenAndCloseOfDayDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITellerRepository _tellerRepository;

        private readonly IMapper _mapper;
        private readonly ILogger<GetTellerAccountBalanceQueryHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvisioningHistoryRepository;
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;

        /// <summary>
        /// Constructor for initializing the GetTellerAccountBalanceQueryHandler.
        /// </summary>
        /// <param name="accountRepository">The repository for accessing account data.</param>
        /// <param name="userInfoToken">The token containing user information.</param>
        /// <param name="mapper">The mapper for converting between objects.</param>
        /// <param name="logger">The logger for logging information and errors.</param>
        /// <param name="dailyTellerRepository">The repository for accessing daily teller data.</param>
        /// <param name="tellerOperationRepository">The repository for accessing teller operations.</param>
        /// <param name="subTellerProvisioningHistoryRepository">The repository for accessing sub-teller provisioning history.</param>
        /// <param name="primaryTellerProvisioningHistoryRepository">The repository for accessing primary teller provisioning history.</param>
        public GetTellerAccountBalanceQueryHandler(
            IAccountRepository accountRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetTellerAccountBalanceQueryHandler> logger,
            IDailyTellerRepository dailyTellerRepository,
            ITellerOperationRepository tellerOperationRepository,
            ISubTellerProvisioningHistoryRepository subTellerProvisioningHistoryRepository,
            IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository,
            ITellerRepository tellerRepository,
            IAccountingDayRepository accountingDayRepository)
        {
            // Assign dependencies to private fields, throwing exceptions if null to ensure required services are provided
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dailyTellerRepository = dailyTellerRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _subTellerProvisioningHistoryRepository = subTellerProvisioningHistoryRepository;
            _primaryTellerProvisioningHistoryRepository = primaryTellerProvisioningHistoryRepository;
            _tellerRepository = tellerRepository;
            _accountingDayRepository = accountingDayRepository;
        }

        /// <summary>
        /// Handles the query to retrieve a Teller's Account Balance. 
        /// Determines whether to process a primary teller's closing, a specific teller's balance, or opening operations for a teller.
        /// </summary>
        /// <param name="request">The query containing the teller's account balance request parameters.</param>
        /// <param name="cancellationToken">A cancellation token to handle request termination.</param>
        /// <returns>A service response containing the teller's account balance details.</returns>
        public async Task<ServiceResponse<TillOpenAndCloseOfDayDto>> Handle(GetTellerAccountBalanceQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;

            try
            {
                // Log entry message for better debugging
                string logMessage = "Processing Teller Account Balance Query.";
                _logger.LogInformation(logMessage);
                await BaseUtilities.LogAndAuditAsync(logMessage, request, HttpStatusCodeEnum.OK, LogAction.TillBalanceQuery, LogLevelInfo.Information);

                // Check if the request is specifically to close the day for the primary teller
                if (request.IsCloseOfDayPrimaryTeller)
                {
                    logMessage = "Handling request for Primary Teller's Close of Day.";
                    _logger.LogInformation(logMessage);
                    return await HandlePrimaryTellerCloseOfDay(request);
                }

                // Check if the request is targeting a specific teller account
                if (request.HasValue)
                {
                    logMessage = "Handling request for a specific Teller's Account Balance.";
                    _logger.LogInformation(logMessage);
                    return await HandleSpecificTellerAccount(request);
                }

                // Determine whether to process opening operations for a Primary or Sub Teller
                if (request.IsPrimary)
                {
                    logMessage = "Handling request for Primary Teller's Open Day.";
                    _logger.LogInformation(logMessage);
                    return await HandlePrimaryTellerOpenDay(request);
                }
                else
                {
                    logMessage = "Handling request for Sub Teller Operations.";
                    _logger.LogInformation(logMessage);
                    return await HandleSubTellerOperations(request);
                }
            }
            catch (Exception e)
            {
                // Construct a detailed error message
                errorMessage = $"An error occurred while retrieving the teller's account balance: {e.Message}";

                // Log the error details
                _logger.LogError(errorMessage, e);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.TillBalanceQuery, LogLevelInfo.Error);

                // Return a service response with a 500 error
                return ServiceResponse<TillOpenAndCloseOfDayDto>.Return500(e);
            }
        }


        ///// <summary>
        ///// Handles the closing of the day for the primary teller.
        ///// </summary>
        ///// <param name="request">The query containing the request parameters.</param>
        ///// <returns>A service response containing the closing balance information.</returns>
        //private async Task<ServiceResponse<TillOpenAndCloseOfDayDto>> HandlePrimaryTellerCloseOfDay(GetTellerAccountBalanceQuery request)
        //{
        //    string msg = "Entering HandlePrimaryTellerCloseOfDay method.";
        //    _logger.LogError(msg);

        //    // Retrieve the active primary teller for the date
        //    var dailyTeller = await _dailyTellerRepository.GetAnActivePrimaryTellerForTheDate();
        //    msg = $"Successfully retrieved the daily teller: {JsonSerializer.Serialize(dailyTeller)}.";
        //    _logger.LogError(msg);

        //    // Retrieve the account associated with the primary teller
        //    var entity = await _accountRepository.FindBy(x => x.TellerId == dailyTeller.TellerId).FirstOrDefaultAsync();
        //    msg = "Attempting to get the teller account.";
        //    _logger.LogError(msg);

        //    if (entity != null)
        //    {
        //        msg = $"Retrieved Teller Account: {JsonSerializer.Serialize(entity)}.";
        //        _logger.LogError(msg);

        //        // Retrieve all accounts linked to the opening of day reference
        //        var entityAll = await _accountRepository
        //            .FindBy(x => x.OpenningOfDayReference == entity.OpenningOfDayReference && x.IsTellerAccount && x.BranchId == _userInfoToken.BranchID)
        //            .ToListAsync();

        //        msg = $"Found accounts linked to opening of day reference: {entity.OpenningOfDayReference}. Accounts: {JsonSerializer.Serialize(entityAll)}.";
        //        _logger.LogError(msg);

        //        // Check if any of the accounts are still open
        //        var openAccounts = entityAll.Where(x => x.OpenningOfDayStatus.Contains(CloseOfDayStatus.OOD.ToString())).ToList();
        //        msg = $"Filtered accounts that are still open (OOD status): {JsonSerializer.Serialize(openAccounts)}.";
        //        _logger.LogError(msg);

        //        // Log accounting day information
        //        var accountingDay = DateTime.Now; // Assuming today's date is the accounting day
        //        msg = $"Current accounting day: {accountingDay:yyyy-MM-dd}.";
        //        _logger.LogError(msg);

        //        if (openAccounts.Any(x => x.Id != entity.Id))
        //        {
        //            // Log an error if there are still open tills and return a 403 response
        //            var openTills = string.Join(", ", openAccounts.Select(x => $"{x.AccountName} (Open Date: {x.OpenningOfDayDate:yyyy-MM-dd})")); // Collecting open till names and open dates
        //            msg = $"Some till(s) are still open: {openTills}. Please close all open tills before proceeding to close the operational day.";
        //            _logger.LogError(msg);
        //            return ServiceResponse<TillOpenAndCloseOfDayDto>.Return403(msg);
        //        }

        //        msg = "All accounts are closed or matched the current entity. Proceeding to map provisioning history.";
        //        _logger.LogError(msg);

        //        int i = 0;
        //        var provisioningHistoryDto = new List<TillOpenAndCloseOfDayDto>();
        //        var options = new JsonSerializerOptions
        //        {
        //            ReferenceHandler = ReferenceHandler.Preserve,  // To handle circular references
        //            WriteIndented = true  // Optional: to make the output more readable
        //        };

        //        foreach (var account in entityAll)
        //        {
        //            i++;
        //            msg = $"Processing account {i}: {JsonSerializer.Serialize(account)}.";
        //            _logger.LogError(msg);

        //            var teller = await _tellerRepository.GetTeller(account.TellerId);
        //            msg = $"Retrieved teller information for account {i}: {JsonSerializer.Serialize(teller)}.";
        //            _logger.LogError(msg);

        //            if (teller.IsPrimary)
        //            {

        //                msg = $"Teller {teller.Name} (ID: {teller.Id}) is a primary teller.";
        //                _logger.LogError(msg);
        //                var primaryTellerProvisioning = await _primaryTellerProvisioningHistoryRepository.FindAsync(account.OpenningOfDayReference);

        //                string jsonResult = JsonSerializer.Serialize(primaryTellerProvisioning, options);
        //                msg = $"Retrieved primary teller provisioning history: {jsonResult}.";
        //                _logger.LogError(msg);

        //                provisioningHistoryDto.Add(MapToTellerProvisioningHistory(primaryTellerProvisioning));
        //            }
        //            else
        //            {
        //                msg = $"Teller {teller.Name} (ID: {teller.Id}) is a sub teller.";
        //                _logger.LogError(msg);

        //                var subTellerProvisioningHistory = await _subTellerProvisioningHistoryRepository.FindBy(x => x.TellerId == account.TellerId && x.ReferenceId == account.OpenningOfDayReference).FirstOrDefaultAsync();
        //                msg = $"Retrieved sub teller provisioning history: {JsonSerializer.Serialize(subTellerProvisioningHistory, options)}.";
        //                _logger.LogError(msg);

        //                provisioningHistoryDto.Add(MapToTellerProvisioningHistory(subTellerProvisioningHistory));
        //            }
        //        }

        //        msg = "Successfully completed provisioning history retrieval and mapping.";
        //        _logger.LogError(msg);

        //        // Log success and return the result
        //        msg = "Audit trail logged successfully.";
        //        _logger.LogError(msg);

        //        var provisioningHistory = MapToTellerProvisioningHistory(provisioningHistoryDto);
        //        string jsonResulttx = JsonSerializer.Serialize(provisioningHistory, options);
        //        msg = $"Final provisioning history result: {jsonResulttx}.";
        //        _logger.LogError(msg);

        //        return ServiceResponse<TillOpenAndCloseOfDayDto>.ReturnResultWith200(provisioningHistory);
        //    }

        //    // Return 404 if the account was not found
        //    msg = "Account not found. Returning 404 response.";
        //    _logger.LogError(msg);
        //    return AccountNotFound();
        //}

        /// <summary>
        /// Handles the process of closing the day for the primary teller.
        /// Ensures that all sub-tellers are closed before proceeding.
        /// </summary>
        /// <param name="request">The request containing query parameters.</param>
        /// <returns>A service response with the closing balance details.</returns>
        private async Task<ServiceResponse<TillOpenAndCloseOfDayDto>> HandlePrimaryTellerCloseOfDay(GetTellerAccountBalanceQuery request)
        {
            _logger.LogInformation("Starting HandlePrimaryTellerCloseOfDay process.");
            await BaseUtilities.LogAndAuditAsync("Initiating primary teller closing process.", request, HttpStatusCodeEnum.OK, LogAction.TellerCloseDay, LogLevelInfo.Information);

            // Step 1: Retrieve the active primary teller for the current date
            var dailyTeller = await _dailyTellerRepository.GetAnActivePrimaryTellerForTheDate();
            if (dailyTeller == null)
            {
                string errorMessage = "No active primary teller found for today. Please verify teller assignments.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.TellerCloseDay, LogLevelInfo.Error);
                return ServiceResponse<TillOpenAndCloseOfDayDto>.Return404(errorMessage);
            }
            _logger.LogInformation("Successfully retrieved daily primary teller: {TellerId}", dailyTeller.TellerId);

            // Step 2: Retrieve the account associated with the primary teller
            var entity = await _accountRepository.FindBy(x => x.TellerId == dailyTeller.TellerId).FirstOrDefaultAsync();
            if (entity == null)
            {
                string errorMessage = "Teller account not found. Ensure the primary teller has an assigned account.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.TellerCloseDay, LogLevelInfo.Error);
                return ServiceResponse<TillOpenAndCloseOfDayDto>.Return404(errorMessage);
            }
            _logger.LogInformation("Retrieved Teller Account: {AccountId}", entity.Id);

            // Step 3: Fetch all accounts linked to the opening of the day reference
            var entityAll = await _accountRepository.FindBy(x => x.OpenningOfDayReference == entity.OpenningOfDayReference && x.IsTellerAccount && x.BranchId == _userInfoToken.BranchID).ToListAsync();
            _logger.LogInformation("Found {Count} accounts linked to the opening of the day reference.", entityAll.Count);

            // Step 4: Check for open sub-tellers before closing the primary teller
            var openAccounts = entityAll.Where(x => x.OpenningOfDayStatus.Contains(CloseOfDayStatus.OOD.ToString())).ToList();
            if (openAccounts.Any(x => x.Id != entity.Id))
            {
                var openTills = string.Join(", ", openAccounts.Select(x => $"{x.AccountName} (Opened: {x.OpenningOfDayDate:yyyy-MM-dd})"));
                string warningMessage = $"The following sub-teller accounts are still open: {openTills}. Please close all sub-tellers before proceeding.";
                _logger.LogWarning(warningMessage);
                await BaseUtilities.LogAndAuditAsync(warningMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.TellerCloseDay, LogLevelInfo.Warning);
                return ServiceResponse<TillOpenAndCloseOfDayDto>.Return403(warningMessage);
            }
            _logger.LogInformation("All sub-teller accounts are closed. Proceeding to finalize primary teller closure.");

            var provisioningHistoryDto = new List<TillOpenAndCloseOfDayDto>();

            // Step 5: Retrieve teller provisioning history
            foreach (var account in entityAll)
            {
                var teller = await _tellerRepository.GetTeller(account.TellerId);
                if (teller.IsPrimary)
                {
                    var primaryTellerProvisioning = await _primaryTellerProvisioningHistoryRepository.FindAsync(account.OpenningOfDayReference);
                    provisioningHistoryDto.Add(MapToTellerProvisioningHistory(primaryTellerProvisioning));
                }
                else
                {
                    var subTellerProvisioningHistory = await _subTellerProvisioningHistoryRepository.FindBy(x => x.TellerId == account.TellerId && x.ReferenceId == account.OpenningOfDayReference).FirstOrDefaultAsync();
                    provisioningHistoryDto.Add(MapToTellerProvisioningHistory(subTellerProvisioningHistory));
                }
            }
            _logger.LogInformation("Successfully retrieved provisioning history for all tellers.");

            // Step 6: Log success and return the result
            string successMessage = "Primary teller closing process completed successfully.";
            await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.TellerCloseDay, LogLevelInfo.Information);
            _logger.LogInformation(successMessage);

            return ServiceResponse<TillOpenAndCloseOfDayDto>.ReturnResultWith200(MapToTellerProvisioningHistory(provisioningHistoryDto));
        }


        /// <summary>
        /// Handles retrieving a specific teller account.
        /// </summary>
        /// <param name="request">The query containing the request parameters.</param>
        /// <returns>A service response containing the account information.</returns>
        private async Task<ServiceResponse<TillOpenAndCloseOfDayDto>> HandleSpecificTellerAccount(GetTellerAccountBalanceQuery request)
        {
            // Retrieve the specific account based on TellerId
            var entity = await _accountRepository
                .AllIncluding(/*Include related entities*/)
                .FirstOrDefaultAsync(x => x.TellerId == request.TellerId);

            if (entity != null)
            {
                // Map the account to DTO and log success
                var provisioningHistory = _mapper.Map<TillOpenAndCloseOfDayDto>(entity);
                return ServiceResponse<TillOpenAndCloseOfDayDto>.ReturnResultWith200(provisioningHistory);
            }

            // Return 404 if the account was not found
            return AccountNotFound();
        }

        /// <summary>
        /// Handles the process of opening the operational day for the primary teller.
        /// Ensures there are no open tills before proceeding, retrieves teller account details,
        /// calculates the opening balance, and returns the appropriate response.
        /// </summary>
        /// <param name="request">The request containing teller and branch details.</param>
        /// <returns>A service response containing the opening balance information.</returns>

        private async Task<ServiceResponse<TillOpenAndCloseOfDayDto>> HandlePrimaryTellerOpenDay(GetTellerAccountBalanceQuery request)
        {
            // Retrieve the current accounting date for the branch
            var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);
            string msg = null;

            // Retrieve the active primary teller for the day
            var dailyTeller = await _dailyTellerRepository.GetAnActivePrimaryTellerForTheDate();
            if (dailyTeller == null)
            {
                msg = "No active primary teller found for today.";
                _logger.LogError(msg);
                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.NotFound, LogAction.TillOpen, LogLevelInfo.Warning);
                return ServiceResponse<TillOpenAndCloseOfDayDto>.Return404(msg);
            }

            // Retrieve the teller's account details
            var entity = await _accountRepository.FindBy(x => x.TellerId == dailyTeller.TellerId).FirstOrDefaultAsync();
            if (entity == null)
            {
                msg = $"No account found for the primary teller {dailyTeller.Teller.Name}.";
                _logger.LogError(msg);
                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.NotFound, LogAction.TillOpen, LogLevelInfo.Warning);
                return AccountNotFound();
            }

            // Retrieve all accounts linked to the same opening of day reference
            var entityAll = await _accountRepository
                .FindBy(x => x.OpenningOfDayReference == entity.OpenningOfDayReference && x.IsTellerAccount && x.BranchId == entity.BranchId)
                .ToListAsync();

            // Check if any till is still open for the same operation day
            var openAccounts = entityAll.Where(x => x.OpenningOfDayStatus == CloseOfDayStatus.OOD.ToString()).ToList();
            if (openAccounts.Any())
            {
                if (openAccounts.Any(x => x.TellerId == entity.TellerId))
                {
                    // Prevent opening a new day if the same teller still has open operations
                    msg = $"The primary till [{dailyTeller.Teller.Name}] (User: [{dailyTeller.UserName}]) cannot open a new operational day because operations for accounting day [{entity.OpenningOfDayDate}] are still active.";
                }
                else
                {
                    // Prevent opening a new day if any other tills are still open
                    msg = $"The primary till [{dailyTeller.Teller.Name}] (User: [{dailyTeller.UserName}]) cannot open a new operational day because there are still open tills for accounting day [{entity.OpenningOfDayDate}]. Please close all prior tills before proceeding.";
                }
                _logger.LogError(msg);
                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.Forbidden, LogAction.TillOpen, LogLevelInfo.Warning);
                return ServiceResponse<TillOpenAndCloseOfDayDto>.Return403(msg);
            }

            // Compute the opening balance for the primary till
            if (entity.OpenningOfDayReference == null)
            {
                entity.Balance = 0;
            }
            else
            {
                entity.Balance = entityAll.Sum(x => x.Balance);
            }

            // Retrieve or create the provisioning history for opening the day
            var openOfTill = await _primaryTellerProvisioningHistoryRepository.FindAsync(entity.OpenningOfDayReference);
            if (openOfTill == null)
            {
                openOfTill = new PrimaryTellerProvisioningHistory();
            }

            // Assign relevant values to the provisioning history
            openOfTill.Teller = dailyTeller.Teller;
            openOfTill.CashAtHand = entity.Balance;
            openOfTill.OpenedDate = accountingDate;

            // Map the data to DTO and return success response
            var provisioningHistory = MapToTellerProvisioningHistory(openOfTill);
            msg = $"Primary till [{dailyTeller.Teller.Name}] successfully opened for the day with balance: {BaseUtilities.FormatCurrency(entity.Balance)}.";
            _logger.LogInformation(msg);
            await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.TillOpen, LogLevelInfo.Information);

            return ServiceResponse<TillOpenAndCloseOfDayDto>.ReturnResultWith200(provisioningHistory);
        }

        /// <summary>
        /// Handles operations for a sub-teller.
        /// </summary>
        /// <param name="request">The query containing the request parameters.</param>
        /// <returns>A service response containing the sub-teller's operations.</returns>
        private async Task<ServiceResponse<TillOpenAndCloseOfDayDto>> HandleSubTellerOperations(GetTellerAccountBalanceQuery request)
        {
            // Retrieve the active primary teller for the date
            var dailyTeller = await _dailyTellerRepository.GetAnActiveSubTellerForTheDate();

            var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);
            var tellerAccount = await _accountRepository.RetrieveTellerAccount(dailyTeller.Teller);
            // Retrieve the account based on TellerId



            //// Retrieve all accounts linked to the opening of day reference
            //var entityAll = await _accountRepository
            //    .FindBy(x => x.OpenningOfDayReference == entity.OpenningOfDayReference && x.IsTellerAccount && x.BranchId == entity.BranchId && x.TellerId==dailyTeller.TellerId)
            //    .ToListAsync();

            // Calculate the total balance and map the provisioning history
            //entity.Balance = entityAll.Sum(x => x.Balance);
            var openOfTill = await _subTellerProvisioningHistoryRepository.FindBy(x => x.ReferenceId== tellerAccount.OpenningOfDayReference && x.TellerId == dailyTeller.TellerId && x.BranchId==dailyTeller.BranchId).FirstOrDefaultAsync();
            var tellerOperations = await _tellerOperationRepository.GetTellerLastOperations(dailyTeller.TellerId, accountingDate);
            if (openOfTill == null)
            {
                openOfTill = new SubTellerProvioningHistory();

            }
            else
            {
                decimal clossedOperationAmount = 0;
                decimal cashAtHand = 0;
                if (tellerOperations.Any())
                {
                    clossedOperationAmount = tellerOperations.LastOrDefault().Balance;
                    cashAtHand = openOfTill.CashAtHand;
                    openOfTill.CashAtHand = clossedOperationAmount;
                }
            }
            openOfTill.Teller = dailyTeller.Teller;
            openOfTill.OpenedDate = accountingDate;
            //openOfTill.CashAtHand = entity.Balance;
            var provisioningHistory = MapToTellerProvisioningHistory(openOfTill);

            return ServiceResponse<TillOpenAndCloseOfDayDto>.ReturnResultWith200(provisioningHistory);

            // Return 404 if the account was not found
        }

        /// <summary>
        /// Maps a teller provisioning history to the DTO.
        /// </summary>
        /// <param name="openOfTill">The provisioning history to map.</param>
        /// <returns>A DTO containing the mapped provisioning history.</returns>
        private TillOpenAndCloseOfDayDto MapToTellerProvisioningHistory(SubTellerProvioningHistory openOfTill)
        {
            return _mapper.Map<TillOpenAndCloseOfDayDto>(openOfTill);
        }
        /// <summary>
        /// Maps a teller provisioning history to the DTO.
        /// </summary>
        /// <param name="openOfTill">The provisioning history to map.</param>
        /// <returns>A DTO containing the mapped provisioning history.</returns>
        private TillOpenAndCloseOfDayDto MapToTellerProvisioningHistory(PrimaryTellerProvisioningHistory openOfTill)
        {
            return _mapper.Map<TillOpenAndCloseOfDayDto>(openOfTill);
        }
        private TillOpenAndCloseOfDayDto MapToTellerProvisioningHistory(List<TillOpenAndCloseOfDayDto> tillOpenAndCloseOfs)
        {
            if (tillOpenAndCloseOfs == null || !tillOpenAndCloseOfs.Any())
            {
                return null; // Handle accordingly if the list is empty or null
            }

            var result = new TillOpenAndCloseOfDayDto
            {
                Id = Guid.NewGuid().ToString(),
                TellerId = tillOpenAndCloseOfs.FirstOrDefault()?.TellerId,
                UserIdInChargeOfThisTeller = tillOpenAndCloseOfs.FirstOrDefault()?.UserIdInChargeOfThisTeller,
                ProvisionedBy = tillOpenAndCloseOfs.FirstOrDefault()?.ProvisionedBy,
                IsCashReplenished = tillOpenAndCloseOfs.Any(t => t.IsCashReplenished),
                ReplenishedAmount = tillOpenAndCloseOfs.Sum(t => t.ReplenishedAmount),
                OpenedDate = tillOpenAndCloseOfs.Min(t => t.OpenedDate),
                ClossedDate = tillOpenAndCloseOfs.Max(t => t.ClossedDate),
                OpenOfDayAmount = tillOpenAndCloseOfs.Sum(t => t.OpenOfDayAmount),
                ReferenceId = tillOpenAndCloseOfs.FirstOrDefault()?.ReferenceId,
                CloseOfReferenceId = tillOpenAndCloseOfs.FirstOrDefault()?.CloseOfReferenceId,
                IsRequestedForCashReplenishment = tillOpenAndCloseOfs.Any(t => t.IsRequestedForCashReplenishment),
                CashAtHand = tillOpenAndCloseOfs.Sum(t => t.CashAtHand),
                EndOfDayAmount = tillOpenAndCloseOfs.Sum(t => t.EndOfDayAmount),
                AccountBalance = tillOpenAndCloseOfs.Sum(t => t.AccountBalance),
                LastOPerationAmount = tillOpenAndCloseOfs.Sum(t => t.LastOPerationAmount),
                LastOperationType = tillOpenAndCloseOfs.FirstOrDefault()?.LastOperationType,
                PreviouseBalance = tillOpenAndCloseOfs.Sum(t => t.PreviouseBalance),
                LastUserID = tillOpenAndCloseOfs.FirstOrDefault()?.LastUserID,
                SubTellerComment = tillOpenAndCloseOfs.FirstOrDefault()?.SubTellerComment,
                Note = tillOpenAndCloseOfs.FirstOrDefault()?.Note,
                BankId = tillOpenAndCloseOfs.FirstOrDefault()?.BankId,
                BranchId = tillOpenAndCloseOfs.FirstOrDefault()?.BranchId,
                ClossedStatus = tillOpenAndCloseOfs.FirstOrDefault()?.ClossedStatus,
                PrimaryTellerComment = tillOpenAndCloseOfs.FirstOrDefault()?.PrimaryTellerComment,
                PrimaryTellerConfirmationStatus = tillOpenAndCloseOfs.FirstOrDefault()?.PrimaryTellerConfirmationStatus,
                DailyTellerId = tillOpenAndCloseOfs.FirstOrDefault()?.DailyTellerId,
                Teller = tillOpenAndCloseOfs.FirstOrDefault()?.Teller,
                InitialPrinting = tillOpenAndCloseOfs.FirstOrDefault()?.InitialPrinting,

                // Summing up Opening Notes and Coins Counts
                OpeningNote10000 = tillOpenAndCloseOfs.Sum(t => t.OpeningNote10000),
                OpeningNote5000 = tillOpenAndCloseOfs.Sum(t => t.OpeningNote5000),
                OpeningNote2000 = tillOpenAndCloseOfs.Sum(t => t.OpeningNote2000),
                OpeningNote1000 = tillOpenAndCloseOfs.Sum(t => t.OpeningNote1000),
                OpeningNote500 = tillOpenAndCloseOfs.Sum(t => t.OpeningNote500),
                OpeningCoin500 = tillOpenAndCloseOfs.Sum(t => t.OpeningCoin500),
                OpeningCoin100 = tillOpenAndCloseOfs.Sum(t => t.OpeningCoin100),
                OpeningCoin50 = tillOpenAndCloseOfs.Sum(t => t.OpeningCoin50),
                OpeningCoin25 = tillOpenAndCloseOfs.Sum(t => t.OpeningCoin25),
                OpeningCoin10 = tillOpenAndCloseOfs.Sum(t => t.OpeningCoin10),
                OpeningCoin5 = tillOpenAndCloseOfs.Sum(t => t.OpeningCoin5),
                OpeningCoin1 = tillOpenAndCloseOfs.Sum(t => t.OpeningCoin1),

                // Summing up Closing Notes and Coins Counts
                ClosingNote10000 = tillOpenAndCloseOfs.Sum(t => t.ClosingNote10000),
                ClosingNote5000 = tillOpenAndCloseOfs.Sum(t => t.ClosingNote5000),
                ClosingNote2000 = tillOpenAndCloseOfs.Sum(t => t.ClosingNote2000),
                ClosingNote1000 = tillOpenAndCloseOfs.Sum(t => t.ClosingNote1000),
                ClosingNote500 = tillOpenAndCloseOfs.Sum(t => t.ClosingNote500),
                ClosingCoin500 = tillOpenAndCloseOfs.Sum(t => t.ClosingCoin500),
                ClosingCoin100 = tillOpenAndCloseOfs.Sum(t => t.ClosingCoin100),
                ClosingCoin50 = tillOpenAndCloseOfs.Sum(t => t.ClosingCoin50),
                ClosingCoin25 = tillOpenAndCloseOfs.Sum(t => t.ClosingCoin25),
                ClosingCoin10 = tillOpenAndCloseOfs.Sum(t => t.ClosingCoin10),
                ClosingCoin5 = tillOpenAndCloseOfs.Sum(t => t.ClosingCoin5),
                ClosingCoin1 = tillOpenAndCloseOfs.Sum(t => t.ClosingCoin1),

                // Other Fields
                InitialOpening = tillOpenAndCloseOfs.FirstOrDefault()?.InitialOpening,
                TellerComment = tillOpenAndCloseOfs.FirstOrDefault()?.TellerComment,
                IsPrimary = tillOpenAndCloseOfs.FirstOrDefault()?.IsPrimary,
                BranchCode = tillOpenAndCloseOfs.FirstOrDefault()?.BranchCode,
                TellerName = tillOpenAndCloseOfs.FirstOrDefault()?.TellerName,
                TellerType = tillOpenAndCloseOfs.FirstOrDefault()?.TellerType,

                // Calculated Properties (Will be calculated automatically)
                HasError = tillOpenAndCloseOfs.Any(t => t.HasError),
                ErrorMessage = string.Join(" | ", tillOpenAndCloseOfs.Select(t => t.ErrorMessage).Where(e => !string.IsNullOrEmpty(e))),
            };


            // Validate the closing amount
            bool isClosingAmountCorrect = result.IsClosingAmountValid();
            if (isClosingAmountCorrect)
            {
                Console.WriteLine("The closing amount matches the cash at hand.");
            }
            else
            {
                Console.WriteLine("There is a discrepancy between the closing amount and the cash at hand.");
            }

            return result;
        }

       


     
        /// <summary>
        /// Returns a 404 service response indicating the account was not found.
        /// </summary>
        /// <returns>A service response with a 404 status code.</returns>
        private ServiceResponse<TillOpenAndCloseOfDayDto> AccountNotFound()
        {
            string msg = "Account was not found.";
            _logger.LogError(msg);
            return ServiceResponse<TillOpenAndCloseOfDayDto>.Return404(msg);
        }
    }



    ///// <summary>
    ///// Handles the request to retrieve a specific Account based on its unique identifier.
    ///// </summary>
    //public class GetTellerAccountBalanceQueryHandler : IRequestHandler<GetTellerAccountBalanceQuery, ServiceResponse<TillOpenAndCloseOfDayDto>>
    //{
    //    private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
    //    private readonly IMapper _mapper; // AutoMapper for object mapping.
    //    private readonly ILogger<GetAccountQueryHandler> _logger; // Logger for logging handler actions and errors.
    //    private readonly UserInfoToken _userInfoToken;
    //    private readonly IDailyTellerRepository _dailyTellerRepository; // Repository for accessing Account data.
    //    private readonly ITellerOperationRepository _tellerOperationRepository; // Repository for accessing Account data.
    //    private readonly ISubTellerProvioningHistoryRepository _subTellerProvioningHistoryRepository; // Repository for accessing Account data.
    //    private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository; // Repository for accessing Account data.

    //    /// <summary>
    //    /// Constructor for initializing the GetTransactionQueryHandler.
    //    /// </summary>
    //    /// <param name="AccountRepository">Repository for Account data access.</param>
    //    /// <param name="mapper">AutoMapper for object mapping.</param>
    //    /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
    //    /// <param name="logger">Logger for logging handler actions and errors.</param>
    //    public GetTellerAccountBalanceQueryHandler(
    //        IAccountRepository AccountRepository,
    //        UserInfoToken UserInfoToken,
    //        IMapper mapper,
    //        ILogger<GetAccountQueryHandler> logger,
    //        IDailyTellerRepository dailyTellerRepository = null,
    //        ITellerOperationRepository tellerOperationRepository = null,
    //        ISubTellerProvioningHistoryRepository subTellerProvioningHistoryRepository = null,
    //        IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository = null)
    //    {
    //        _AccountRepository = AccountRepository;
    //        _userInfoToken = UserInfoToken;
    //        _mapper = mapper;
    //        _logger = logger;
    //        _dailyTellerRepository = dailyTellerRepository;
    //        _tellerOperationRepository = tellerOperationRepository;
    //        _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
    //        _primaryTellerProvisioningHistoryRepository = primaryTellerProvisioningHistoryRepository;
    //    }

    //    /// <summary>
    //    /// Handles the GetAccountQuery to retrieve a specific Account.
    //    /// </summary>
    //    /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
    //    /// <param name="cancellationToken">A cancellation token.</param>
    //    public async Task<ServiceResponse<TillOpenAndCloseOfDayDto>> Handle(GetTellerAccountBalanceQuery request, CancellationToken cancellationToken)
    //    {
    //        string errorMessage = null;

    //        try
    //        {
    //            // Check if it's for the primary teller's close of day
    //            if (request.IsCloseOfDayPrimaryTeller)
    //            {
    //                var dailyTeller = await _dailyTellerRepository.GetAnActivePrimaryTellerForTheDate();
    //                var entity = await _AccountRepository.FindBy(x => x.TellerId == dailyTeller.TellerId).FirstOrDefaultAsync();

    //                if (entity != null)
    //                {
    //                    var entityAll = await _AccountRepository.FindBy(x => x.OpenningOfDayReference == entity.OpenningOfDayReference && x.IsTellerAccount && x.BranchId == _userInfoToken.BranchID).ToListAsync();
    //                    var openAccounts = entityAll.Where(x => x.OpenningOfDayStatus != CloseOfDayStatus.CLOSSED.ToString()).ToList();

    //                    if (openAccounts.Any())
    //                    {
    //                        // Check if there are any records with OpenningOfDayStatus not equal to CLOSED
    //                        if (openAccounts.Any(x => x.Id != entity.Id))
    //                        {
    //                            // If open accounts found, return an error message
    //                            string msg = $"Some till(s) are still opened. Get them closed before you can close your day.";
    //                            _logger.LogError(msg);
    //                            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, msg, LogLevelInfo.Information.ToString(), 403, _userInfoToken.Token);
    //                            return ServiceResponse<TillOpenAndCloseOfDayDto>.Return403(msg);
    //                        }
    //                    }

    //                    // Sum up the balances of all accounts and assign it to the entity's balance
    //                    entity.Balance = entityAll.Sum(x => x.Balance);
    //                    var openOfTill = await _primaryTellerProvisioningHistoryRepository.FindAsync(entity.OpenningOfDayReference);
    //                    openOfTill.Teller = dailyTeller.Teller;
    //                    openOfTill.CashAtHand = entity.Balance;
    //                    var provioningHistory = MapToTellerProvisioningHistory(openOfTill);

    //                    // Map the Account entity to TellerProvioningHistory and return it with a success response
    //                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Customer account returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
    //                    return ServiceResponse<TillOpenAndCloseOfDayDto>.ReturnResultWith200(provioningHistory);
    //                }
    //            }
    //            else
    //            {
    //                // Check if the request has a value
    //                if (request.HasValue)
    //                {
    //                    // Retrieve the Account entity with the specified ID from the repository
    //                    var entity = await _AccountRepository.AllIncluding(/*Include related entities*/).FirstOrDefaultAsync(x => x.TellerId == request.TellerId);

    //                    if (entity != null)
    //                    {
    //                        // Map the Account entity to TellerProvioningHistory and return it with a success response
    //                        var TellerProvioningHistory = _mapper.Map<TillOpenAndCloseOfDayDto>(entity);
    //                        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Customer account returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
    //                        return ServiceResponse<TillOpenAndCloseOfDayDto>.ReturnResultWith200(TellerProvioningHistory);
    //                    }
    //                }
    //                else
    //                {
    //                    if (request.IsPrimary)
    //                    {
    //                        // Determine if it's a primary or sub teller
    //                        var dailyTeller = await _dailyTellerRepository.GetAnActivePrimaryTellerForTheDate();
    //                        var entity = await _AccountRepository.FindBy(x => x.TellerId == dailyTeller.TellerId).FirstOrDefaultAsync();
    //                        var openOfTill = await _primaryTellerProvisioningHistoryRepository.FindAsync(entity.OpenningOfDayReference);
    //                        openOfTill.Teller = dailyTeller.Teller;

    //                        if (entity != null)
    //                        {

    //                            //var entityAll = await _AccountRepository.FindBy(x => x.OpenningOfDayReference == entity.OpenningOfDayReference && x.OpenningOfDayStatus == CloseOfDayStatus.CLOSSED.ToString() && x.IsTellerAccount && x.BranchId == _userInfoToken.BranchID).ToListAsync();
    //                            //entity.Balance = entityAll.Sum(x => x.Balance);
    //                            var entityAll = await _AccountRepository.FindBy(x => x.OpenningOfDayReference == entity.OpenningOfDayReference && x.IsTellerAccount &&x.BranchId== entity.BranchId).ToListAsync();
    //                            var openAccounts = entityAll.Where(x => x.OpenningOfDayStatus != CloseOfDayStatus.CLOSSED.ToString()).ToList();

    //                            if (openAccounts.Any())
    //                            {
    //                                // Check if there are any records with OpenningOfDayStatus not equal to CLOSED
    //                                string msg = $"Primary teller can't open the day while some Till(s) are still opened for previouse days. Get them closed before you can open a new operation day.";
    //                                _logger.LogError(msg);
    //                                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, msg, LogLevelInfo.Information.ToString(), 403, _userInfoToken.Token);
    //                                return ServiceResponse<TillOpenAndCloseOfDayDto>.Return403(msg);

    //                            }
    //                            entity.Balance = entityAll.Sum(x => x.Balance);
    //                            var provioningHistory = MapToTellerProvisioningHistory(openOfTill);
    //                            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Customer account returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
    //                            return ServiceResponse<TillOpenAndCloseOfDayDto>.ReturnResultWith200(provioningHistory);
    //                        }
    //                    }
    //                    else
    //                    {


    //                        // Determine if it's a primary or sub teller
    //                        var dailyTeller = await _dailyTellerRepository.GetAnActiveSubTellerForTheDate();
    //                        var tellOps = await _tellerOperationRepository.GetTellerLastOperations(dailyTeller.TellerId);

    //                        if (tellOps.Count()>0)
    //                        {
    //                            var entity = await _AccountRepository.FindBy(x => x.TellerId == dailyTeller.TellerId).FirstOrDefaultAsync();
    //                            var openOfTill = await _subTellerProvioningHistoryRepository.FindAsync(entity.OpenningOfDayReference);
    //                            openOfTill.Teller = dailyTeller.Teller;
    //                            var provioningHistory = MapToTellerProvisioningHistory(openOfTill);
    //                            if (entity != null)
    //                            {
    //                                if (tellOps.LastOrDefault().Balance== entity.PreviousBalance)
    //                                {

    //                                }
    //                                else
    //                                {
    //                                    if (entity.Balance>0 && tellOps.LastOrDefault().Balance<0)
    //                                    {

    //                                    }
    //                                    else
    //                                    {
    //                                        if (openOfTill.IsCashReplenished)
    //                                        {
    //                                            var newBalance = openOfTill.ReplenishedAmount + tellOps.LastOrDefault().Balance;

    //                                            if (newBalance== entity.Balance)
    //                                            {
    //                                                entity.Balance = newBalance;
    //                                            }
    //                                        }
    //                                        else
    //                                        {
    //                                            entity.Balance = tellOps.LastOrDefault().Balance;

    //                                        }
    //                                        if (openOfTill.CashAtHand==0)
    //                                        {
    //                                            openOfTill.CashAtHand = entity.Balance;
    //                                        }
    //                                        if (openOfTill.CashAtHand< entity.Balance)

    //                                        {
    //                                            openOfTill.CashAtHand = entity.Balance;
    //                                        }
    //                                        provioningHistory.CashAtHand = openOfTill.CashAtHand;
    //                                    }
    //                                    //entity.Balance = tellOps.LastOrDefault().Balance;

    //                                }
    //                                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Customer account returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
    //                                return ServiceResponse<TillOpenAndCloseOfDayDto>.ReturnResultWith200(provioningHistory);
    //                            }

    //                        }
    //                        else
    //                        {
    //                            var entity = await _AccountRepository.FindBy(x => x.TellerId == dailyTeller.TellerId).FirstOrDefaultAsync();
    //                            if (entity != null)
    //                            {
    //                                // Map the Account entity to TellerProvioningHistory and return it with a success response
    //                                var TellerProvioningHistory = _mapper.Map<TillOpenAndCloseOfDayDto>(entity);
    //                                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Customer account returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
    //                                return ServiceResponse<TillOpenAndCloseOfDayDto>.ReturnResultWith200(TellerProvioningHistory);
    //                            }

    //                        }

    //                    }
    //                }
    //            }

    //            // If the Account entity was not found, log the error and return a 404 Not Found response
    //            _logger.LogError("Account not found.");
    //            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Account not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
    //            return ServiceResponse<TillOpenAndCloseOfDayDto>.Return404();
    //        }
    //        catch (Exception e)
    //        {
    //            // Log the error and return a 500 Internal Server Error response with the error message
    //            errorMessage = $"Error occurred while getting Account: {e.Message}";
    //            _logger.LogError(errorMessage);
    //            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
    //            return ServiceResponse<TillOpenAndCloseOfDayDto>.Return500(e);
    //        }
    //    }
    //    public TillOpenAndCloseOfDayDto MapToTellerProvisioningHistory(PrimaryTellerProvisioningHistory primary)
    //    {
    //        if (primary == null)
    //            throw new ArgumentNullException(nameof(primary));

    //        return new TillOpenAndCloseOfDayDto
    //        {
    //            Id = primary.Id,
    //            TellerId = primary.TellerId,
    //            UserIdInChargeOfThisTeller = primary.UserIdInChargeOfThisTeller,
    //            ProvisionedBy = primary.ProvisionedBy,
    //            IsCashReplenished = primary.IsCashReplenishment,
    //            ReplenishedAmount = primary.CashReplenishmentAmount,
    //            OpenedDate = primary.OpenedDate,
    //            ClossedDate = primary.ClossedDate,
    //            OpenOfDayAmount = primary.OpenOfDayAmount,
    //            ReferenceId = primary.ReferenceId,
    //            CloseOfReferenceId = primary.CloseOfDayReferenceId,
    //            IsRequestedForCashReplenishment = false, // Assuming this doesn't exist in PrimaryTellerProvisioningHistory
    //            CashAtHand = primary.CashAtHand,
    //            EndOfDayAmount = primary.EndOfDayAmount,
    //            AccountBalance = primary.AccountBalance,
    //            LastOPerationAmount = primary.LastOPerationAmount,
    //            LastOperationType = primary.LastOperationType,
    //            PreviouseBalance = primary.PreviouseBalance,
    //            LastUserID = primary.LastUserID,
    //            SubTellerComment = null, // Assuming this doesn't exist in PrimaryTellerProvisioningHistory
    //            Note = null, // Assuming this doesn't exist in PrimaryTellerProvisioningHistory
    //            BankId = primary.BankId,
    //            BranchId = primary.BranchId,
    //            ClossedStatus = primary.ClossedStatus,
    //            PrimaryTellerComment = primary.PrimaryTellerComment,
    //            PrimaryTellerConfirmationStatus = null, // Assuming this doesn't exist in PrimaryTellerProvisioningHistory
    //            DailyTellerId = primary.DailyTellerId,
    //            Teller = primary.Teller,

    //            // Copying Opening Denominations
    //            OpeningNote10000 = primary.OpeningNote10000,
    //            OpeningNote5000 = primary.OpeningNote5000,
    //            OpeningNote2000 = primary.OpeningNote2000,
    //            OpeningNote1000 = primary.OpeningNote1000,
    //            OpeningNote500 = primary.OpeningNote500,
    //            OpeningCoin500 = primary.OpeningCoin500,
    //            OpeningCoin100 = primary.OpeningCoin100,
    //            OpeningCoin50 = primary.OpeningCoin50,
    //            OpeningCoin25 = primary.OpeningCoin25,
    //            OpeningCoin10 = primary.OpeningCoin10,
    //            OpeningCoin5 = primary.OpeningCoin5,
    //            OpeningCoin1 = primary.OpeningCoin1,

    //            // Copying Closing Denominations
    //            ClosingNote10000 = primary.ClosingNote10000,
    //            ClosingNote5000 = primary.ClosingNote5000,
    //            ClosingNote2000 = primary.ClosingNote2000,
    //            ClosingNote1000 = primary.ClosingNote1000,
    //            ClosingNote500 = primary.ClosingNote500,
    //            ClosingCoin500 = primary.ClosingCoin500,
    //            ClosingCoin100 = primary.ClosingCoin100,
    //            ClosingCoin50 = primary.ClosingCoin50,
    //            ClosingCoin25 = primary.ClosingCoin25,
    //            ClosingCoin10 = primary.ClosingCoin10,
    //            ClosingCoin5 = primary.ClosingCoin5,
    //            ClosingCoin1 = primary.ClosingCoin1,

    //            // Calculated properties will be automatically calculated based on the above properties
    //            HasError = false, // Assuming default value for mapping
    //            ErrorMessage = null // Assuming default value for mapping
    //        };
    //    }

    //    public TillOpenAndCloseOfDayDto MapToTellerProvisioningHistory(SubTellerProvioningHistory subTeller)
    //    {
    //        if (subTeller == null)
    //            throw new ArgumentNullException(nameof(subTeller));

    //        return new TillOpenAndCloseOfDayDto
    //        {
    //            Id = subTeller.Id,
    //            TellerId = subTeller.TellerId,
    //            UserIdInChargeOfThisTeller = subTeller.UserIdInChargeOfThisTeller,
    //            ProvisionedBy = subTeller.ProvisionedBy,
    //            IsCashReplenished = subTeller.IsCashReplenished,
    //            ReplenishedAmount = subTeller.ReplenishedAmount,
    //            OpenedDate = subTeller.OpenedDate,
    //            ClossedDate = subTeller.ClossedDate,
    //            OpenOfDayAmount = subTeller.OpenOfDayAmount,
    //            ReferenceId = subTeller.ReferenceId,
    //            CloseOfReferenceId = subTeller.CloseOfReferenceId,
    //            IsRequestedForCashReplenishment = false, // Assuming this doesn't exist in PrimaryTellerProvisioningHistory
    //            CashAtHand = subTeller.CashAtHand,
    //            EndOfDayAmount = subTeller.EndOfDayAmount,
    //            AccountBalance = subTeller.AccountBalance,
    //            LastOPerationAmount = subTeller.LastOPerationAmount,
    //            LastOperationType = subTeller.LastOperationType,
    //            PreviouseBalance = subTeller.PreviouseBalance,
    //            LastUserID = subTeller.LastUserID,
    //            SubTellerComment = null, // Assuming this doesn't exist in PrimaryTellerProvisioningHistory
    //            Note = null, // Assuming this doesn't exist in PrimaryTellerProvisioningHistory
    //            BankId = subTeller.BankId,
    //            BranchId = subTeller.BranchId,
    //            ClossedStatus = subTeller.ClossedStatus,
    //            PrimaryTellerComment = subTeller.PrimaryTellerComment,
    //            PrimaryTellerConfirmationStatus = null, // Assuming this doesn't exist in PrimaryTellerProvisioningHistory
    //            DailyTellerId = subTeller.DailyTellerId,
    //            Teller = subTeller.Teller,

    //            // Copying Opening Denominations
    //            OpeningNote10000 = subTeller.OpeningNote10000,
    //            OpeningNote5000 = subTeller.OpeningNote5000,
    //            OpeningNote2000 = subTeller.OpeningNote2000,
    //            OpeningNote1000 = subTeller.OpeningNote1000,
    //            OpeningNote500 = subTeller.OpeningNote500,
    //            OpeningCoin500 = subTeller.OpeningCoin500,
    //            OpeningCoin100 = subTeller.OpeningCoin100,
    //            OpeningCoin50 = subTeller.OpeningCoin50,
    //            OpeningCoin25 = subTeller.OpeningCoin25,
    //            OpeningCoin10 = subTeller.OpeningCoin10,
    //            OpeningCoin5 = subTeller.OpeningCoin5,
    //            OpeningCoin1 = subTeller.OpeningCoin1,

    //            // Copying Closing Denominations
    //            ClosingNote10000 = subTeller.ClosingNote10000,
    //            ClosingNote5000 = subTeller.ClosingNote5000,
    //            ClosingNote2000 = subTeller.ClosingNote2000,
    //            ClosingNote1000 = subTeller.ClosingNote1000,
    //            ClosingNote500 = subTeller.ClosingNote500,
    //            ClosingCoin500 = subTeller.ClosingCoin500,
    //            ClosingCoin100 = subTeller.ClosingCoin100,
    //            ClosingCoin50 = subTeller.ClosingCoin50,
    //            ClosingCoin25 = subTeller.ClosingCoin25,
    //            ClosingCoin10 = subTeller.ClosingCoin10,
    //            ClosingCoin5 = subTeller.ClosingCoin5,
    //            ClosingCoin1 = subTeller.ClosingCoin1,

    //            // Calculated properties will be automatically calculated based on the above properties
    //            HasError = false, // Assuming default value for mapping
    //            ErrorMessage = null // Assuming default value for mapping
    //        };
    //    }

    //}

}
