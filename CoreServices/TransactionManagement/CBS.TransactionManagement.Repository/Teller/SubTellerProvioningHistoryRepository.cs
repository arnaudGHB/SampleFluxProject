using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.ChangeManagement;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Repository
{
    public class SubTellerProvioningHistoryRepository : GenericRepository<SubTellerProvioningHistory, TransactionContext>, ISubTellerProvisioningHistoryRepository
    {
        // The constructor initializes the repository with a UnitOfWork and optionally a UserInfoToken.
        private readonly UserInfoToken _userInfoToken;
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly ILogger<SubTellerProvioningHistoryRepository> _logger; // Logger for logging handler actions and errors.
        private readonly ITellerRepository _tellerRepository;
        private readonly ICashChangeHistoryRepository _cashChangeHistoryRepository;

        // Constructor for the SubTellerProvioningHistoryRepository class.
        public SubTellerProvioningHistoryRepository(IUnitOfWork<TransactionContext> unitOfWork, UserInfoToken userInfoToken = null, IDailyTellerRepository dailyTellerRepository = null, ILogger<SubTellerProvioningHistoryRepository> logger = null, ITellerRepository tellerRepository = null, ICashChangeHistoryRepository cashChangeHistoryRepository = null) : base(unitOfWork)
        {
            _userInfoToken = userInfoToken;
            _dailyTellerRepository = dailyTellerRepository;
            _logger=logger;
            _tellerRepository=tellerRepository;
            _cashChangeHistoryRepository=cashChangeHistoryRepository;
        }

        // Checks if the accounting day for a specific teller is still open.
        public async Task<bool> CheckIfDayIsStillOpened(string tellerid, DateTime accountingDay)
        {
            // Finds a record for the given teller ID where the created date is today and the status is "OOD" (Open of Day).
            var tellerProvision = await FindBy(t => t.TellerId == tellerid && t.OpenedDate.Value.Date == accountingDay.Date && t.ClossedStatus == CloseOfDayStatus.OOD.ToString()).FirstOrDefaultAsync();

            // If no record is found, throw an exception indicating that the till is not opened.
            if (tellerProvision == null)
            {
                var errorMessage = "Your Till is not opened. Open the day before performing cash operations.";
                throw new InvalidOperationException(errorMessage);
            }

            // If the status is not "OOD", throw an exception indicating that the till is not properly opened.
            if (tellerProvision.ClossedStatus != CloseOfDayStatus.OOD.ToString())
            {
                var errorMessage = "Your Till is not opened.";
                throw new InvalidOperationException(errorMessage);
            }

            // Return true indicating the day is still open.
            return true;
        }

        // Opens a new accounting day for the teller.
        public SubTellerProvioningHistory OpenDay(CurrencyNotesRequest request, SubTellerProvioningHistory _tellerHistory)
        {
            // Set the opening denominations based on the request.
            _tellerHistory.OpeningNote10000 = request.Note10000;
            _tellerHistory.OpeningNote5000 = request.Note5000;
            _tellerHistory.OpeningNote2000 = request.Note2000;
            _tellerHistory.OpeningNote1000 = request.Note1000;
            _tellerHistory.OpeningNote500 = request.Note500;
            _tellerHistory.OpeningCoin500 = request.Coin500;
            _tellerHistory.OpeningCoin100 = request.Coin100;
            _tellerHistory.OpeningCoin50 = request.Coin50;
            _tellerHistory.OpeningCoin25 = request.Coin25;
            _tellerHistory.OpeningCoin10 = request.Coin10;
            _tellerHistory.OpeningCoin5 = request.Coin5;
            _tellerHistory.OpeningCoin1 = request.Coin1;
            _tellerHistory.OpenOfDayAmount = _tellerHistory.TotalOpeningAmount;
            _tellerHistory.ClosingNote10000 = request.Note10000;
            _tellerHistory.ClosingNote5000 = request.Note5000;
            _tellerHistory.ClosingNote2000 = request.Note2000;
            _tellerHistory.ClosingNote1000 = request.Note1000;
            _tellerHistory.ClosingNote500 = request.Note500;
            _tellerHistory.ClosingCoin500 = request.Coin500;
            _tellerHistory.ClosingCoin100 = request.Coin100;
            _tellerHistory.ClosingCoin50 = request.Coin50;
            _tellerHistory.ClosingCoin25 = request.Coin25;
            _tellerHistory.ClosingCoin10 = request.Coin10;
            _tellerHistory.ClosingCoin5 = request.Coin5;
            _tellerHistory.ClosingCoin1 = request.Coin1;
            _tellerHistory.LastOperationType = "Open of day";
            _tellerHistory.LastOPerationAmount = _tellerHistory.TotalOpeningAmount;
            Add(_tellerHistory);
            return _tellerHistory;
        }

        // Closes the accounting day for the teller.
        public SubTellerProvioningHistory CloseDay(CurrencyNotesRequest request, SubTellerProvioningHistory _tellerHistory)
        {
            // Set the closing denominations based on the request.
            _tellerHistory.ClosingNote10000 = request.Note10000;
            _tellerHistory.ClosingNote5000 = request.Note5000;
            _tellerHistory.ClosingNote2000 = request.Note2000;
            _tellerHistory.ClosingNote1000 = request.Note1000;
            _tellerHistory.ClosingNote500 = request.Note500;
            _tellerHistory.ClosingCoin500 = request.Coin500;
            _tellerHistory.ClosingCoin100 = request.Coin100;
            _tellerHistory.ClosingCoin50 = request.Coin50;
            _tellerHistory.ClosingCoin25 = request.Coin25;
            _tellerHistory.ClosingCoin10 = request.Coin10;
            _tellerHistory.ClosingCoin5 = request.Coin5;
            _tellerHistory.ClosingCoin1 = request.Coin1;
            _tellerHistory.EndOfDayAmount = _tellerHistory.TotalClosingAmount;
            _tellerHistory.LastOperationType = "Close of day";
            _tellerHistory.LastOPerationAmount = _tellerHistory.TotalClosingAmount;
            UpdateInCasecade(_tellerHistory);
            return _tellerHistory;

        }
        public async Task<SubTellerProvioningHistory> GetLastUpdatedRecordForSubTellerProvisioningHistory(string tellerId)
        {
            // Query the repository for the most recent record based on the OpenedDate or similar timestamp
            var latestRecord = await FindBy(x => x.TellerId == tellerId).OrderByDescending(x => x.OpenedDate).FirstOrDefaultAsync();
            return latestRecord;
        }

        // Handles cash-in transactions for the teller.
        public SubTellerProvioningHistory CashInByDinomination(decimal amount, CurrencyNotesRequest request, string tellerId, DateTime accountingDate, string OpenOfDayReference)
        {
            // Find the teller history for the given tellerId and accountingDate.
            var _tellerHistory = FindBy(x => x.TellerId == tellerId && x.OpenedDate == accountingDate && x.ReferenceId==OpenOfDayReference).FirstOrDefault();

            // Check if _tellerHistory is null and throw an exception if no record is found.
            if (_tellerHistory == null)
            {
                throw new InvalidOperationException($"No teller history found for Teller ID: {tellerId} on {accountingDate.ToShortDateString()}.");
            }
            if (request == null)
            {
                // Generate new denominations based on the amount without charges.
                request = CurrencyNotesMapper.CalculateCurrencyNotes(amount);
            }

            // Update the denominations and increase the cash at hand.
            _tellerHistory = UpdateDenominations(request, isCashIn: true, _tellerHistory);
            _tellerHistory.CashAtHand += amount;
            _tellerHistory.LastOPerationAmount = amount;
            _tellerHistory.LastOperationType = "CashIn";
            _tellerHistory.NumberOfCashIn += 1;
            _tellerHistory.VolumeOfCashIn += amount;
            _tellerHistory.LastUserID = _tellerHistory.UserIdInChargeOfThisTeller;
            // Update the teller history with the new cash-in details.
            UpdateInCasecade(_tellerHistory);
            return _tellerHistory;
        }


        public SubTellerProvioningHistory CashInByDinominationReplenisment(decimal amount, CurrencyNotesRequest notesRequest, string tellerId, DateTime accountingDate, string OpenOfDayReference)
        {
            // Find the teller history for the given tellerId and accountingDate.
            var _tellerHistory = FindBy(x => x.TellerId == tellerId && x.OpenedDate == accountingDate && x.ReferenceId== OpenOfDayReference).FirstOrDefault();

            // Check if _tellerHistory is null and throw an exception if no record is found.
            if (_tellerHistory == null)
            {
                throw new InvalidOperationException($"No teller history found for Teller ID: {tellerId} on {accountingDate.ToShortDateString()}.");
            }
            if (notesRequest==null)
            {
                // Generate new denominations based on the amount without charges.
                notesRequest = CurrencyNotesMapper.CalculateCurrencyNotes(amount);
            }

            // Update the denominations and increase the cash at hand.
            _tellerHistory = UpdateDenominations(notesRequest, isCashIn: true, _tellerHistory);
            _tellerHistory.CashAtHand += amount;
            _tellerHistory.LastOPerationAmount = amount;
            _tellerHistory.LastOperationType = "Cash Replenishment";
            _tellerHistory.LastUserID = _tellerHistory.UserIdInChargeOfThisTeller;
            _tellerHistory.IsRequestedForCashReplenishment = true;
            _tellerHistory.ReplenishedAmount = amount;
            _tellerHistory.IsCashReplenished = true;
            UpdateInCasecade(_tellerHistory);
            return _tellerHistory;
        }

        public SubTellerProvioningHistory CashInAndOutByDenomination(
            decimal amountToCashOut,
            decimal charges,
            CurrencyNotesRequest notesRequest,
            string tellerId,
            DateTime accountingDate,
            string openOfDayReference)
        {
            // Find the teller history for the given tellerId and accountingDate.
            var _tellerHistory = FindBy(x => x.TellerId == tellerId && x.OpenedDate == accountingDate && x.ReferenceId == openOfDayReference).FirstOrDefault();

            // Check if _tellerHistory is null and throw an exception if no record is found.
            if (_tellerHistory == null)
            {
                throw new InvalidOperationException($"No teller history found for Teller ID: {tellerId} on {accountingDate.ToShortDateString()}.");
            }

            // Calculate the net change (charges as cash in, amountToCashOut as cash out)
            decimal netChange = charges - amountToCashOut;

            // Generate new denominations for the net cash change if none are provided
            if (notesRequest == null)
            {
                notesRequest = CurrencyNotesMapper.CalculateCurrencyNotes(Math.Abs(netChange));
            }

            // Update the denominations based on the net change
            _tellerHistory = UpdateDenominations(notesRequest, netChange > 0, _tellerHistory);

            // Update the teller history with the net effect
            _tellerHistory.CashAtHand += netChange; // Adjust the cash at hand based on net change
            _tellerHistory.LastOPerationAmount = Math.Abs(netChange); // Record the absolute value of the operation amount
            _tellerHistory.LastOperationType = netChange > 0 ? "Cash Replenishment (In)" : "Cash Dispensed (Out)";
            _tellerHistory.LastUserID = _tellerHistory.UserIdInChargeOfThisTeller;
            _tellerHistory.IsRequestedForCashReplenishment = netChange > 0;
            _tellerHistory.ReplenishedAmount = netChange > 0 ? netChange : 0;
            _tellerHistory.IsCashReplenished = netChange > 0;


            // Log the operation details
            //_logger.LogInformation($"Cash operation performed: Charges (Cash In): {charges}, Cash Out: {amountToCashOut}, Net Change: {netChange}");

            // Update the teller history in the database
            UpdateInCasecade(_tellerHistory);

            return _tellerHistory;
        }

        /// <summary>
        /// Manages cash change operations for the primary teller.
        /// </summary>
        /// <param name="changeManagement">An object containing details of the cash change operation, including denominations, reference, and reason.</param>
        /// <returns>A `CashChangeHistoryDto` object representing the completed change operation.</returns>
        public async Task<CashChangeHistory> ManageCashChangeAsync(CashChangeDataCarrier changeManagement)
        {
            try
            {
                // Step 1: Find the teller history record for the provided tellerId and accountingDate.
                var subTellerProvisioning = FindBy(x =>
                    x.TellerId == changeManagement.tellerId &&
                    x.OpenedDate == changeManagement.accountingDate &&
                    x.ReferenceId == changeManagement.OpenningOfDayReference).FirstOrDefault();

                // Step 2: Validate if a teller history exists for the provided parameters.
                if (subTellerProvisioning == null)
                {
                    throw new InvalidOperationException(
                        $"No teller history found for Teller ID: {changeManagement.tellerId} on {changeManagement.accountingDate.ToShortDateString()}.");
                }
                bool isReceivingCash = false;
                // Step 3: Extract current balances from the sub-teller's provisioning.
                var currentBalances = ExtractCurrentBalances(subTellerProvisioning);

                // Step 4: Calculate the total amounts for given and received denominations.
                decimal totalAmountGiven = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsGiven);
                decimal totalAmountReceived = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsReceived);

                // Step 5: Check sufficiency of received denominations (partial acceptance).
                var (currentBalancesReceived, insufficientDenominationsReceived) = CheckDenominationSufficiency(changeManagement.denominationsReceived, subTellerProvisioning);
                if (insufficientDenominationsReceived.Count > 0)
                {
                    decimal shortfall = insufficientDenominationsReceived
     .Sum(kvp => GetDenominationValue(kvp.Key) *
                ((int)(changeManagement.denominationsReceived.GetType().GetProperty(kvp.Key)?.GetValue(changeManagement.denominationsReceived) ?? 0) - kvp.Value));
                    isReceivingCash=true;

                    totalAmountReceived -= shortfall; // Adjust total received for shortfall.
                }

                // Step 6: Check sufficiency of funds for the denominations to be given.
                var (currentBalancesGiven, insufficientDenominationsGiven) = CheckDenominationSufficiency(changeManagement.denominationsGiven, subTellerProvisioning);
                if (insufficientDenominationsGiven.Count > 0)
                {
                    string errorMessage = $"Insufficient denominations to complete the cash given operation: {string.Join(", ", insufficientDenominationsGiven.Select(kvp => $"{kvp.Key}"))}.";
                    _logger.LogWarning(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 7: Check total balance sufficiency.
                decimal totalCashierBalance = CurrencyNotesMapper.CalculateTotalAmount(ConvertToCurrencyNotesRequest(currentBalances));
                if (totalCashierBalance < totalAmountGiven)
                {
                    string errorMessage = $"Insufficient total funds for cash exchange. Available: {BaseUtilities.FormatCurrency(totalCashierBalance)}, Requested: {BaseUtilities.FormatCurrency(totalAmountGiven)}.";
                    _logger.LogWarning(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 8: Optimize denominations for the given request.
                var optimizedDenominationsGiven = OptimizeDenominationsForRequest(changeManagement.denominationsGiven, currentBalancesGiven);
                if (optimizedDenominationsGiven == null)
                {
                    string errorMessage = $"Unable to fulfill the cash exchange request due to insufficient substitute denominations.";
                    _logger.LogWarning(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 9: Update the denominations.
                if (isReceivingCash)
                {
                    UpdateDenominations(changeManagement.denominationsReceived, true, subTellerProvisioning); // Deduct received denominations.
                    UpdateDenominations(optimizedDenominationsGiven, false, subTellerProvisioning); // Add optimized denominations for given.

                }
                else
                {
                    UpdateDenominations(changeManagement.denominationsReceived, true, subTellerProvisioning); // Deduct the received denominations.
                    UpdateDenominations(changeManagement.denominationsGiven, false, subTellerProvisioning);    // Add the given denominations.

                }
                // Step 10: Log the successful operation in the cash change history.
                var cashChangeHistory = new CashChangeHistory
                {
                    Reference = changeManagement.reference,
                    ServiceOperationType = "Cash Change Operation",
                    ChangeReason = changeManagement.changeReason,
                    AmountGiven = totalAmountGiven,
                    AmountReceive = totalAmountReceived,
                    BranchId = _userInfoToken.BranchID,
                    BranchCode = _userInfoToken.BranchCode,
                    BranchName = _userInfoToken.BranchName,
                    ChangedBy = _userInfoToken.FullName,
                    SubTellerId = subTellerProvisioning.TellerId,
                    SystemName = changeManagement.SystemName
                };
                _cashChangeHistoryRepository.CreateChangeHistory(changeManagement, cashChangeHistory);

                // Step 11: Persist all changes to the database.
                Update(subTellerProvisioning);
                await _uow.SaveAsync();

                // Step 12: Log and audit the successful operation.
                string successMessage = $"Cash change operation successfully processed. Given: {BaseUtilities.FormatCurrency(totalAmountGiven)}, Received: {BaseUtilities.FormatCurrency(totalAmountReceived)}. Done by {_userInfoToken.FullName}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, cashChangeHistory, HttpStatusCodeEnum.OK, LogAction.CashChangeOperationPrimaryTill, LogLevelInfo.Information);

                // Step 13: Return the completed change history.
                return cashChangeHistory;
            }
            catch (Exception ex)
            {
                // Step 14: Log and handle unexpected errors.
                string errorMessage = $"An error occurred during the cash change operation: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, changeManagement.denominationsGiven, HttpStatusCodeEnum.InternalServerError, LogAction.CashChangeOperationPrimaryTill, LogLevelInfo.Error);
                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Extracts current balances of all denominations from the sub-teller provisioning object.
        /// </summary>
        /// <param name="subTellerProvioning">The sub-teller provisioning object.</param>
        /// <returns>A dictionary with denomination names as keys and their respective balances as values.</returns>
        private Dictionary<string, decimal> ExtractCurrentBalances(SubTellerProvioningHistory subTellerProvioning)
        {
            return new Dictionary<string, decimal>
    {
        { "Note10000", subTellerProvioning.ClosingNote10000 },
        { "Note5000", subTellerProvioning.ClosingNote5000 },
        { "Note2000", subTellerProvioning.ClosingNote2000 },
        { "Note1000", subTellerProvioning.ClosingNote1000 },
        { "Note500", subTellerProvioning.ClosingNote500 },
        { "Coin500", subTellerProvioning.ClosingCoin500 },
        { "Coin100", subTellerProvioning.ClosingCoin100 },
        { "Coin50", subTellerProvioning.ClosingCoin50 },
        { "Coin25", subTellerProvioning.ClosingCoin25 },
        { "Coin10", subTellerProvioning.ClosingCoin10 },
        { "Coin5", subTellerProvioning.ClosingCoin5 },
        { "Coin1", subTellerProvioning.ClosingCoin1 }
    };
        }
        /// <summary>
        /// Converts a dictionary of denomination balances into a CurrencyNotesRequest object.
        /// </summary>
        /// <param name="balances">A dictionary with denomination names as keys and their respective balances as values.</param>
        /// <returns>A CurrencyNotesRequest object populated with the balances.</returns>
        private CurrencyNotesRequest ConvertToCurrencyNotesRequest(Dictionary<string, decimal> balances)
        {
            return new CurrencyNotesRequest
            {
                Note10000 = (int)(balances.TryGetValue("Note10000", out var value10000) ? value10000 : 0),
                Note5000 = (int)(balances.TryGetValue("Note5000", out var value5000) ? value5000 : 0),
                Note2000 = (int)(balances.TryGetValue("Note2000", out var value2000) ? value2000 : 0),
                Note1000 = (int)(balances.TryGetValue("Note1000", out var value1000) ? value1000 : 0),
                Note500 = (int)(balances.TryGetValue("Note500", out var value500) ? value500 : 0),
                Coin500 = (int)(balances.TryGetValue("Coin500", out var valueCoin500) ? valueCoin500 : 0),
                Coin100 = (int)(balances.TryGetValue("Coin100", out var valueCoin100) ? valueCoin100 : 0),
                Coin50 = (int)(balances.TryGetValue("Coin50", out var valueCoin50) ? valueCoin50 : 0),
                Coin25 = (int)(balances.TryGetValue("Coin25", out var valueCoin25) ? valueCoin25 : 0),
                Coin10 = (int)(balances.TryGetValue("Coin10", out var valueCoin10) ? valueCoin10 : 0),
                Coin5 = (int)(balances.TryGetValue("Coin5", out var valueCoin5) ? valueCoin5 : 0),
                Coin1 = (int)(balances.TryGetValue("Coin1", out var valueCoin1) ? valueCoin1 : 0)
            };
        }

        /// <summary>
        /// Attempts to fulfill the requested denominations using available cashier balances, substituting where necessary.
        /// </summary>
        /// <param name="requestedDenominations">The denominations requested by the customer.</param>
        /// <param name="currentBalances">The cashier's current balances.</param>
        /// <returns>An optimized set of denominations if possible, otherwise null.</returns>
        private CurrencyNotesRequest OptimizeDenominationsForRequest(CurrencyNotesRequest requestedDenominations, Dictionary<string, decimal> currentBalances)
        {
            var optimizedDenominations = new CurrencyNotesRequest();
            decimal totalRequestedAmount = CurrencyNotesMapper.CalculateTotalAmount(requestedDenominations);

            foreach (var property in typeof(CurrencyNotesRequest).GetProperties().OrderByDescending(p => GetDenominationValue(p.Name)))
            {
                string denominationName = property.Name;
                int requestedCount = (int)(property.GetValue(requestedDenominations) ?? 0);
                decimal denominationValue = GetDenominationValue(denominationName);

                if (currentBalances.TryGetValue(denominationName, out decimal availableCount) && availableCount > 0)
                {
                    // Use as many of the requested denomination as available.
                    int toUse = Math.Min(requestedCount, (int)availableCount);
                    property.SetValue(optimizedDenominations, toUse);

                    totalRequestedAmount -= toUse * denominationValue;
                    currentBalances[denominationName] -= toUse;
                }
            }

            // If the remaining amount is zero, return the optimized denominations.
            return totalRequestedAmount == 0 ? optimizedDenominations : null;
        }

        /// <summary>
        /// Returns the monetary value of a denomination based on its name.
        /// </summary>
        private decimal GetDenominationValue(string denominationName)
        {
            return denominationName switch
            {
                "Note10000" => 10000,
                "Note5000" => 5000,
                "Note2000" => 2000,
                "Note1000" => 1000,
                "Note500" => 500,
                "Coin500" => 500,
                "Coin100" => 100,
                "Coin50" => 50,
                "Coin25" => 25,
                "Coin10" => 10,
                "Coin5" => 5,
                "Coin1" => 1,
                _ => 0
            };
        }

        /// <summary>
        /// Validates the denominations to ensure they are non-negative and not all zeros.
        /// </summary>
        /// <param name="denominations">The denominations to validate.</param>
        /// <returns>True if valid; otherwise, false.</returns>
        private bool IsValidDenominationRequest(CurrencyNotesRequest denominations)
        {
            // Check for negative values.
            foreach (var property in denominations.GetType().GetProperties())
            {
                var value = (int?)property.GetValue(denominations) ?? 0;
                if (value < 0)
                    return false;
            }

            // Check if all denominations are zero.
            bool allZero = denominations.GetType().GetProperties()
                .All(property => ((int?)property.GetValue(denominations) ?? 0) == 0);

            return !allZero;
        }


        // Handles cash-out transactions for the teller.
        private (Dictionary<string, decimal> CurrentBalances, Dictionary<string, decimal> InsufficientDenominations) CheckDenominationSufficiency(CurrencyNotesRequest request, SubTellerProvioningHistory _tellerHistory)
        {
            var currentBalances = new Dictionary<string, decimal>
                            {
                                { "Note10000", _tellerHistory.ClosingNote10000 },
                                { "Note5000", _tellerHistory.ClosingNote5000 },
                                { "Note2000", _tellerHistory.ClosingNote2000 },
                                { "Note1000", _tellerHistory.ClosingNote1000 },
                                { "Note500", _tellerHistory.ClosingNote500 },
                                { "Coin500", _tellerHistory.ClosingCoin500 },
                                { "Coin100", _tellerHistory.ClosingCoin100 },
                                { "Coin50", _tellerHistory.ClosingCoin50 },
                                { "Coin25", _tellerHistory.ClosingCoin25 },
                                { "Coin10", _tellerHistory.ClosingCoin10 },
                                { "Coin5", _tellerHistory.ClosingCoin5 },
                                { "Coin1", _tellerHistory.ClosingCoin1 }
                            };

            var insufficientDenominations = new Dictionary<string, decimal>();

            // Compare each denomination in the request against the available denominations in the teller history.
            if (_tellerHistory.ClosingNote10000 < request.Note10000)
                insufficientDenominations.Add("Note10000", _tellerHistory.ClosingNote10000);
            if (_tellerHistory.ClosingNote5000 < request.Note5000)
                insufficientDenominations.Add("Note5000", _tellerHistory.ClosingNote5000);
            if (_tellerHistory.ClosingNote2000 < request.Note2000)
                insufficientDenominations.Add("Note2000", _tellerHistory.ClosingNote2000);
            if (_tellerHistory.ClosingNote1000 < request.Note1000)
                insufficientDenominations.Add("Note1000", _tellerHistory.ClosingNote1000);
            if (_tellerHistory.ClosingNote500 < request.Note500)
                insufficientDenominations.Add("Note500", _tellerHistory.ClosingNote500);
            if (_tellerHistory.ClosingCoin500 < request.Coin500)
                insufficientDenominations.Add("Coin500", _tellerHistory.ClosingCoin500);
            if (_tellerHistory.ClosingCoin100 < request.Coin100)
                insufficientDenominations.Add("Coin100", _tellerHistory.ClosingCoin100);
            if (_tellerHistory.ClosingCoin50 < request.Coin50)
                insufficientDenominations.Add("Coin50", _tellerHistory.ClosingCoin50);
            if (_tellerHistory.ClosingCoin25 < request.Coin25)
                insufficientDenominations.Add("Coin25", _tellerHistory.ClosingCoin25);
            if (_tellerHistory.ClosingCoin10 < request.Coin10)
                insufficientDenominations.Add("Coin10", _tellerHistory.ClosingCoin10);
            if (_tellerHistory.ClosingCoin5 < request.Coin5)
                insufficientDenominations.Add("Coin5", _tellerHistory.ClosingCoin5);
            if (_tellerHistory.ClosingCoin1 < request.Coin1)
                insufficientDenominations.Add("Coin1", _tellerHistory.ClosingCoin1);

            return (currentBalances, insufficientDenominations);
        }



        public SubTellerProvioningHistory CashOutByDenomination(decimal amount, CurrencyNotesRequest request, string tellerId, DateTime accountingDate, decimal customerCharges, bool IsChargeInclusive, string OpenOfDayReference)
        {
            // Fetch the teller's provisioning history for the given tellerId and accountingDate.
            var tellerHistory = FindBy(x => x.TellerId == tellerId && x.OpenedDate == accountingDate && x.ReferenceId==OpenOfDayReference).FirstOrDefault();

            // If no teller history is found, throw an exception.
            if (tellerHistory == null)
            {
                throw new InvalidOperationException($"No teller history found for Teller ID: {tellerId} on {accountingDate.ToShortDateString()}.");
            }


            // Handle scenario where charge is inclusive (customer charge is deducted from their account).
            if (IsChargeInclusive && customerCharges > 0)
            {
                // Calculate the amount without charges since charges will be deducted from the member's account.
                decimal amountWithoutCharges = amount - customerCharges;

                // Generate new denominations based on the amount without charges.
                request = CurrencyNotesMapper.CalculateCurrencyNotes(amountWithoutCharges);

                // Check if sufficient denominations are available.
                var (currentBalances, insufficientDenominations) = CheckDenominationSufficiency(request, tellerHistory);

                // If sufficient denominations are available, proceed with cash-out.
                if (insufficientDenominations.Count == 0)
                {
                    // Update denominations by deducting cash from the teller.
                    tellerHistory = UpdateDenominations(request, isCashIn: false, tellerHistory);

                    // Deduct the amount without charges from CashAtHand.
                    tellerHistory.CashAtHand -= amountWithoutCharges;
                    tellerHistory.LastOPerationAmount = amountWithoutCharges;
                    tellerHistory.LastOperationType = "CashOut";
                    tellerHistory.LastUserID = tellerHistory.UserIdInChargeOfThisTeller;
                    tellerHistory.NumberOfCashOut += 1;
                    tellerHistory.VolumeOfCashOut += amountWithoutCharges;

                    // Update the teller's history after cash-out.
                    UpdateInCasecade(tellerHistory);
                    return tellerHistory;
                }
                else
                {
                    // Throw exception if there are insufficient denominations.
                    var insufficientDenomsMessage = string.Join(", ",
                        insufficientDenominations.Select(kvp =>
                            $"[{kvp.Key} Available: {currentBalances[kvp.Key]}, Requested: {request.GetType().GetProperty(kvp.Key)?.GetValue(request)}]"));

                    throw new InvalidOperationException($"Insufficient denominations for CashOut operation: {insufficientDenomsMessage}.");
                }
            }
            // Handle scenario where no customer charges are involved (cash-out only) Or The Charge is paid to to the Cashier.
            else
            {
                // No charges, use the original request to proceed.
                decimal amountWithoutCharges = amount;

                // Check if sufficient denominations are available.
                var (currentBalances, insufficientDenominations) = CheckDenominationSufficiency(request, tellerHistory);

                // If sufficient denominations are available, proceed with cash-out.
                if (insufficientDenominations.Count == 0)
                {
                    // Update denominations by deducting cash from the teller.
                    tellerHistory = UpdateDenominations(request, isCashIn: false, tellerHistory);

                    // Deduct the full amount from CashAtHand.
                    tellerHistory.CashAtHand -= amountWithoutCharges;
                    tellerHistory.LastOPerationAmount = amountWithoutCharges;
                    tellerHistory.LastOperationType = "CashOut";
                    tellerHistory.LastUserID = tellerHistory.UserIdInChargeOfThisTeller;
                    tellerHistory.NumberOfCashOut += 1;
                    tellerHistory.VolumeOfCashOut += amountWithoutCharges;

                    // Update the teller's history after cash-out.
                    UpdateInCasecade(tellerHistory);
                    return tellerHistory;
                }
                else
                {
                    // Throw exception if there are insufficient denominations.
                    var insufficientDenomsMessage = string.Join(", ",
                        insufficientDenominations.Select(kvp =>
                            $"[{kvp.Key} Available: {currentBalances[kvp.Key]}, Requested: {request.GetType().GetProperty(kvp.Key)?.GetValue(request)}]"));

                    throw new InvalidOperationException($"Insufficient denominations for CashOut operation: {insufficientDenomsMessage}.");
                }
            }
        }
        public SubTellerProvioningHistory CashOutByDinomination(decimal amount, CurrencyNotesRequest request, SubTellerProvioningHistory _tellerHistory)
        {





            // Check if sufficient denominations are available before proceeding.
            var (currentBalances, insufficientDenominations) = CheckDenominationSufficiency(request, _tellerHistory);

            if (insufficientDenominations.Count == 0)
            {
                // If there are no insufficient denominations, proceed with the cash-out operation.

                // Update the denominations in the till by decreasing the amounts based on the request.
                _tellerHistory = UpdateDenominations(request, isCashIn: false, _tellerHistory);

                // Deduct the cash-out amount from CashAtHand.
                _tellerHistory.CashAtHand -= amount;
                _tellerHistory.LastOPerationAmount = amount;
                _tellerHistory.LastOperationType = EventCode.Subteller_Cash_To_PrimaryTeller.ToString();
                _tellerHistory.LastUserID = _tellerHistory.UserIdInChargeOfThisTeller;
                _tellerHistory.NumberOfCashOut += 1;
                _tellerHistory.VolumeOfCashOut += amount;
                UpdateInCasecade(_tellerHistory);
                return _tellerHistory;
            }
            else
            {
                // If there are insufficient denominations, throw an exception with details.
                var insufficientDenomsMessage = string.Join(", ",
                    insufficientDenominations.Select(kvp =>
                        $"[{kvp.Key} Available: {currentBalances[kvp.Key]}, Requested: {request.GetType().GetProperty(kvp.Key)?.GetValue(request)}]"));
                throw new InvalidOperationException($"Insufficient denominations for CashOut operation: {insufficientDenomsMessage}.");
            }
        }

        public SubTellerProvioningHistory CashOutByDinomination(decimal amount, CurrencyNotesRequest request, string tellerId, DateTime accountingDate, string OpenOfDayReference)
        {



            // Fetch the teller's provisioning history for the given tellerId and accountingDate.
            var _tellerHistory = FindBy(x => x.TellerId == tellerId && x.OpenedDate == accountingDate && x.ReferenceId==OpenOfDayReference).FirstOrDefault();

            // Check if _tellerHistory is null and throw an exception if no record is found.
            if (_tellerHistory == null)
            {
                throw new InvalidOperationException($"No teller history found for Teller ID: {tellerId} on {accountingDate.ToShortDateString()}.");
            }

            // Check if sufficient denominations are available before proceeding.
            var (currentBalances, insufficientDenominations) = CheckDenominationSufficiency(request, _tellerHistory);

            if (insufficientDenominations.Count == 0)
            {
                // If there are no insufficient denominations, proceed with the cash-out operation.

                // Update the denominations in the till by decreasing the amounts based on the request.
                _tellerHistory = UpdateDenominations(request, isCashIn: false, _tellerHistory);

                // Deduct the cash-out amount from CashAtHand.
                _tellerHistory.CashAtHand -= amount;
                _tellerHistory.LastOPerationAmount = amount;
                _tellerHistory.LastOperationType = "CashOut";
                _tellerHistory.LastUserID = _tellerHistory.UserIdInChargeOfThisTeller;
                _tellerHistory.NumberOfCashOut += 1;
                _tellerHistory.VolumeOfCashOut += amount;
                UpdateInCasecade(_tellerHistory);
                return _tellerHistory;
            }
            else
            {
                // If there are insufficient denominations, throw an exception with details.
                var insufficientDenomsMessage = string.Join(", ",
                    insufficientDenominations.Select(kvp =>
                        $"[{kvp.Key} Available: {currentBalances[kvp.Key]}, Requested: {request.GetType().GetProperty(kvp.Key)?.GetValue(request)}]"));
                throw new InvalidOperationException($"Insufficient denominations for CashOut operation: {insufficientDenomsMessage}.");
            }
        }


        // Updates the denominations in the teller history based on the transaction type (cash-in or cash-out).
        private SubTellerProvioningHistory UpdateDenominations(CurrencyNotesRequest request, bool isCashIn, SubTellerProvioningHistory _tellerHistory)
        {
            // Adjust each denomination based on whether it's a cash-in or cash-out transaction.
            _tellerHistory.ClosingNote10000 += (isCashIn ? request.Note10000 : -request.Note10000);
            _tellerHistory.ClosingNote5000 += (isCashIn ? request.Note5000 : -request.Note5000);
            _tellerHistory.ClosingNote2000 += (isCashIn ? request.Note2000 : -request.Note2000);
            _tellerHistory.ClosingNote1000 += (isCashIn ? request.Note1000 : -request.Note1000);
            _tellerHistory.ClosingNote500 += (isCashIn ? request.Note500 : -request.Note500);
            _tellerHistory.ClosingCoin500 += (isCashIn ? request.Coin500 : -request.Coin500);
            _tellerHistory.ClosingCoin100 += (isCashIn ? request.Coin100 : -request.Coin100);
            _tellerHistory.ClosingCoin50 += (isCashIn ? request.Coin50 : -request.Coin50);
            _tellerHistory.ClosingCoin25 += (isCashIn ? request.Coin25 : -request.Coin25);
            _tellerHistory.ClosingCoin10 += (isCashIn ? request.Coin10 : -request.Coin10);
            _tellerHistory.ClosingCoin5 += (isCashIn ? request.Coin5 : -request.Coin5);
            _tellerHistory.ClosingCoin1 += (isCashIn ? request.Coin1 : -request.Coin1);

            // Return the updated teller history.
            return _tellerHistory;
        }
        /// <summary>
        /// Retrieves all tills by branch ID.
        /// </summary>
        /// <param name="branchId">The ID of the branch to retrieve tills for.</param>
        /// <returns>A list of TillStatusReportDto objects representing the tills.</returns>
        public List<TillStatusReportDto> GetAllTillsByBranchId(string branchId, DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            // Fetch all SubTellerProvisioningHistory records for the specified branch ID.
            var tellerHistories = FindBy(x => x.BranchId == branchId && x.OpenedDate.Value.Date >= dateTimeFrom && x.OpenedDate.Value <= dateTimeTo);

            // Prepare a list to hold the report DTOs.
            var reports = new List<TillStatusReportDto>();

            // Iterate through the fetched teller histories.
            foreach (var tellerHistory in tellerHistories)
            {
                // Convert the SubTellerProvisioningHistory to TillStatusReportDto.
                var report = GetTellerReport(tellerHistory);
                // Add the report to the list.
                reports.Add(report);
            }

            // Return the list of reports.
            return reports;
        }

        // Existing GetTellerReport method
        private TillStatusReportDto GetTellerReport(SubTellerProvioningHistory _tellerHistory)
        {
            return new TillStatusReportDto
            {
                TillId = _tellerHistory.TellerId,
                TillName = _tellerHistory.Teller.Name,
                TillUserName = _tellerHistory.UserIdInChargeOfThisTeller,
                OpenDate = _tellerHistory.OpenedDate,
                Satus = _tellerHistory.ClossedStatus,
                CloseDate = _tellerHistory.ClossedDate,
                CashAtHand = _tellerHistory.CashAtHand,
                LastOperationAmount = _tellerHistory.LastOPerationAmount,
                LastOperationType = _tellerHistory.LastOperationType,
                OpeningDenominations = new CurrencyNotesRequest
                {
                    Note10000 = _tellerHistory.OpeningNote10000,
                    Note5000 = _tellerHistory.OpeningNote5000,
                    Note2000 = _tellerHistory.OpeningNote2000,
                    Note1000 = _tellerHistory.OpeningNote1000,
                    Note500 = _tellerHistory.OpeningNote500,
                    Coin500 = _tellerHistory.OpeningCoin500,
                    Coin100 = _tellerHistory.OpeningCoin100,
                    Coin50 = _tellerHistory.OpeningCoin50,
                    Coin25 = _tellerHistory.OpeningCoin25,
                    Coin10 = _tellerHistory.OpeningCoin10,
                    Coin5 = _tellerHistory.OpeningCoin5,
                    Coin1 = _tellerHistory.OpeningCoin1
                }
            };
        }





    }

}
