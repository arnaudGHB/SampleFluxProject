using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Data.Entity.CashVaultP;
using CBS.TransactionManagement.Data.Entity.ChangeManagement;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository
{

    public class PrimaryTellerProvisioningHistoryRepository : GenericRepository<PrimaryTellerProvisioningHistory, TransactionContext>, IPrimaryTellerProvisioningHistoryRepository
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly ICashChangeHistoryRepository _cashChangeHistoryRepository;

        public PrimaryTellerProvisioningHistoryRepository(IUnitOfWork<TransactionContext> unitOfWork, UserInfoToken userInfoToken, ICashChangeHistoryRepository cashChangeHistoryRepository) : base(unitOfWork)
        {
            _userInfoToken = userInfoToken;
            _cashChangeHistoryRepository=cashChangeHistoryRepository;
        }
        // Method to check if primary teller is opened for the day
        public async Task<PrimaryTellerProvisioningHistory> CheckIfPrimaryTellerIsOpened(DateTime accountingDate)
        {
            var provisioningHistory = await FindBy(t => t.BranchId == _userInfoToken.BranchID && t.OpenedDate.Value.Date == accountingDate.Date && t.ClossedStatus == CloseOfDayStatus.OOD.ToString()).FirstOrDefaultAsync();

            if (provisioningHistory == null)
                throw new InvalidOperationException($"Primary till of your branch is not open for the day.");
            return provisioningHistory;
        }
        // Opens a new accounting day for the teller.
        public PrimaryTellerProvisioningHistory OpenDay(CurrencyNotesRequest request, PrimaryTellerProvisioningHistory _tellerHistory)
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
            _tellerHistory.LastOPerationAmount = _tellerHistory.TotalOpeningAmount;
            _tellerHistory.LastOperationType = "Open of day";

            Add(_tellerHistory);
            return _tellerHistory;
        }

        // Closes the accounting day for the teller.
        public PrimaryTellerProvisioningHistory CloseDay(CurrencyNotesRequest request, PrimaryTellerProvisioningHistory _tellerHistory)
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
            _tellerHistory.LastOPerationAmount = _tellerHistory.TotalClosingAmount;
            _tellerHistory.LastOperationType = "Close of day";


            Update(_tellerHistory);
            return _tellerHistory;

        }

        // Handles cash-in transactions for the teller.
        public void CashInByDinomination(decimal amount, CurrencyNotesRequest request, string tellerId, DateTime accountingDate, string Reference)
        {
            // Find the teller history for the given tellerId and accountingDate.
            var _tellerHistory = FindBy(x => x.TellerId == tellerId && x.OpenedDate == accountingDate).FirstOrDefault();

            // Check if _tellerHistory is null and throw an exception if no record is found.
            if (_tellerHistory == null)
            {
                throw new InvalidOperationException($"No teller history found for Teller ID: {tellerId} on {accountingDate.ToShortDateString()}.");
            }

            // Update the denominations and increase the cash at hand.
            _tellerHistory = UpdateDenominations(request, isCashIn: true, _tellerHistory);
            _tellerHistory.CashAtHand += amount;
            _tellerHistory.LastOPerationAmount = amount;
            _tellerHistory.LastOperationType = "CashIn";
            _tellerHistory.IsCashReplenishment = true;
            _tellerHistory.ReplenishmentReferenceNumber = Reference;
            _tellerHistory.CashReplenishmentAmount = amount;
            _tellerHistory.LastUserID = _tellerHistory.UserIdInChargeOfThisTeller;

            // Update the teller history with the new cash-in details.
            Update(_tellerHistory);
        }


        // Handles cash-out transactions for the teller.
        public (Dictionary<string, decimal> CurrentBalances, Dictionary<string, decimal> InsufficientDenominations) CheckDenominationSufficiency(CurrencyNotesRequest request, PrimaryTellerProvisioningHistory _tellerHistory)
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
   
        public async Task<PrimaryTellerProvisioningHistory> GetLastUpdatedRecordForPrimaryProvisioningHistory(string tellerId)
        {
            // Query the repository for the most recent record based on the OpenedDate or similar timestamp
            var latestRecord = await FindBy(x => x.TellerId == tellerId).OrderByDescending(x => x.OpenedDate).FirstOrDefaultAsync();
            return latestRecord;
        }
        public void CashOutByDinomination(decimal amount, CurrencyNotesRequest request, PrimaryTellerProvisioningHistory _tellerHistory)
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

                // Update the teller history with details of the last operation.
                _tellerHistory.LastOPerationAmount = amount;
                _tellerHistory.LastOperationType = EventCode.Cash_To_Vault.ToString();
                _tellerHistory.LastUserID = _tellerHistory.UserIdInChargeOfThisTeller;

                // Save the updated teller history to the database.
                Update(_tellerHistory);
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
        /// <summary>
        /// Manages a cash change operation for the primary teller.
        /// </summary>
        /// <param name="changeManagement">The details of the cash change operation, including denominations, reason, and references.</param>
        /// <returns>A CashChangeHistoryDto object representing the operation details.</returns>
        public async Task<CashChangeHistory> ManageCashChangeAsync(CashChangeDataCarrier changeManagement)
        {
            try
            {
                // Step 1: Retrieve the primary teller's provisioning history for the current day.
                var primaryTellerHistory = FindBy(x =>
                  x.TellerId == changeManagement.tellerId &&
                  x.OpenedDate == changeManagement.accountingDate &&
                  x.ReferenceId == changeManagement.OpenningOfDayReference).FirstOrDefault();

                // Step 2: Validate that the primary teller is open.
                if (primaryTellerHistory == null)
                {
                    string errorMessage = "Primary teller is not open for the day.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, changeManagement, HttpStatusCodeEnum.NotFound, LogAction.TellerOperationChange, LogLevelInfo.Warning);
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 3: Check if sufficient denominations are available for the operation.
                var (currentBalancesReceived, insufficientDenominationsReceived) = CheckDenominationSufficiency(changeManagement.denominationsReceived, primaryTellerHistory);
                var (currentBalancesGiven, insufficientDenominationsGiven) = CheckDenominationSufficiency(changeManagement.denominationsGiven, primaryTellerHistory);

                // Step 3.1: Validate the sufficiency of denominations received.
                if (insufficientDenominationsReceived.Count > 0)
                {
                    var insufficientDenomsMessage = string.Join(", ",
                        insufficientDenominationsReceived.Select(kvp =>
                            $"[{kvp.Key} Available: {currentBalancesReceived[kvp.Key]}, Requested: {changeManagement.denominationsReceived.GetType().GetProperty(kvp.Key)?.GetValue(changeManagement.denominationsReceived)}]"));

                    string errorMessage = $"Insufficient denominations for the cash received operation: {insufficientDenomsMessage}.";
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 3.2: Validate the sufficiency of denominations given.
                if (insufficientDenominationsGiven.Count > 0)
                {
                    var insufficientDenomsMessage = string.Join(", ",
                        insufficientDenominationsGiven.Select(kvp =>
                            $"[{kvp.Key} Available: {currentBalancesGiven[kvp.Key]}, Needed: {changeManagement.denominationsGiven.GetType().GetProperty(kvp.Key)?.GetValue(changeManagement.denominationsGiven)}]"));

                    string errorMessage = $"Insufficient denominations to complete the cash given operation: {insufficientDenomsMessage}.";
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 3.3: Validate the total amount of denominations given matches the amount being exchanged.
                decimal totalAmountGiven = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsGiven);
                decimal totalAmountReceived = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsReceived);

                if (totalAmountGiven != totalAmountReceived)
                {
                    string errorMessage = $"The total value of denominations does not match. Amount Given: {BaseUtilities.FormatCurrency(totalAmountGiven)}, Amount Received: {BaseUtilities.FormatCurrency(totalAmountReceived)}.";
                    throw new InvalidOperationException(errorMessage);
                }


                // Step 5: Update the primary teller's denominations for the operation.
                UpdateDenominations(changeManagement.denominationsReceived, false, primaryTellerHistory); // Deduct the received denominations.
                UpdateDenominations(changeManagement.denominationsGiven, true, primaryTellerHistory);    // Add the given denominations.

                // Step 6: Create a CashChangeHistoryDto record to log the operation details.
                var cashChangeHistory = new CashChangeHistory
                {
                    Reference = changeManagement.reference, // Use the provided reference for tracking.
                    ChangeDate = DateTime.UtcNow, // Record the current date and time for the operation.
                    AmountGiven = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsGiven), // Calculate the total given amount.
                    AmountReceive = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsReceived), // Calculate the total received amount.
                    ServiceOperationType = "Cash Change From Primary Teller", // Specify the operation type.
                    BranchId = _userInfoToken.BranchID, // Record the branch ID from user context.
                    BranchCode = _userInfoToken.BranchCode, // Record the branch code from user context.
                    BranchName = _userInfoToken.BranchName, // Record the branch name from user context.
                    ChangedBy = _userInfoToken.FullName, // Record the name of the user performing the operation.
                    ChangeReason = changeManagement.changeReason, // Include the reason for the cash change.
                    SystemName = changeManagement.SystemName
                };

                // Step 7: Log the operation in the cash change history repository.
                _cashChangeHistoryRepository.CreateChangeHistory(changeManagement, cashChangeHistory);

                // Step 8: Save the updated state of the primary teller and the operation to the database.
                Update(primaryTellerHistory); // Persist changes to the primary teller's state.
                await _uow.SaveAsync(); // Save all changes to the database.

                // Step 9: Log and audit the success of the operation.
                string successMessage = $"Cash change operation completed successfully for primary teller. Reference: {changeManagement.reference}.";
                await BaseUtilities.LogAndAuditAsync(successMessage, cashChangeHistory, HttpStatusCodeEnum.OK, LogAction.TellerOperationChange, LogLevelInfo.Information);

                // Step 10: Return the completed CashChangeHistoryDto object as the result.
                return cashChangeHistory;
            }
            catch (Exception ex)
            {
                // Step 11: Handle unexpected exceptions by logging and auditing the error.
                string errorMessage = $"An error occurred during the cash change operation: {ex.Message}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, changeManagement, HttpStatusCodeEnum.InternalServerError, LogAction.TellerOperationChange, LogLevelInfo.Error);
                throw;
            }
        }

        public async Task<bool> CashOutByDinomination(decimal amount, CurrencyNotesRequest request, string tellerId, DateTime accountingDate,string Reference)
        {
            // Fetch the teller's provisioning history for the given tellerId and accountingDate.
            var _tellerHistory = await FindBy(X => X.TellerId == tellerId && X.OpenedDate.Value.Date == accountingDate.Date && X.ReferenceId== Reference).FirstOrDefaultAsync();

            // Check if _tellerHistory is null and throw an exception if no record is found.
            if (_tellerHistory == null)
            {
                throw new InvalidOperationException($"No teller history found: {tellerId} on {accountingDate.ToShortDateString()}.");
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

                // Update the teller history with details of the last operation.
                _tellerHistory.LastOPerationAmount = amount;
                _tellerHistory.LastOperationType = "CashOut";
                _tellerHistory.LastUserID = _tellerHistory.UserIdInChargeOfThisTeller;

                // Save the updated teller history to the database.
                Update(_tellerHistory);
            }
            else
            {
                // If there are insufficient denominations, throw an exception with details.
                var insufficientDenomsMessage = string.Join(", ",
                    insufficientDenominations.Select(kvp =>
                        $"[{kvp.Key} Available: {currentBalances[kvp.Key]}, Requested: {request.GetType().GetProperty(kvp.Key)?.GetValue(request)}]"));
                throw new InvalidOperationException($"Insufficient denominations for CashOut operation: {insufficientDenomsMessage}.");
            }
            return true;
        }
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

        //public void CashOutByDinomination(decimal amount, CurrencyNotesRequest request, string tellerId, DateTime accountingDate)
        //{
        //    // Fetch the teller's provisioning history for the given tellerId and accountingDate.
        //    var _tellerHistory = FindBy(x => x.TellerId == tellerId && x.OpenedDate == accountingDate).FirstOrDefault();

        //    // Check if _tellerHistory is null and throw an exception if no record is found.
        //    if (_tellerHistory == null)
        //    {
        //        throw new InvalidOperationException($"No teller history found for Teller ID: {tellerId} on {accountingDate.ToShortDateString()}.");
        //    }

        //    // Check if sufficient denominations are available before proceeding.
        //    var (currentBalances, insufficientDenominations) = CheckDenominationSufficiency(request, _tellerHistory);

        //    if (insufficientDenominations.Count == 0)
        //    {
        //        // If there are no insufficient denominations, proceed with the cash-out operation.

        //        // Update the denominations in the till by decreasing the amounts based on the request.
        //        _tellerHistory = UpdateDenominations(request, isCashIn: false, _tellerHistory);

        //        // Deduct the cash-out amount from CashAtHand.
        //        _tellerHistory.CashAtHand -= amount;
        //        _tellerHistory.LastOPerationAmount = amount;
        //        _tellerHistory.LastOperationType = "CashOut";
        //        _tellerHistory.LastUserID = _tellerHistory.UserIdInChargeOfThisTeller;
        //        _tellerHistory.NumberOfCashOut += 1;
        //        _tellerHistory.VolumeOfCashOut += amount;
        //        // Save the updated teller history to the database.
        //        Update(_tellerHistory);
        //    }
        //    else
        //    {
        //        // If there are insufficient denominations, throw an exception with details.
        //        var insufficientDenomsMessage = string.Join(", ",
        //            insufficientDenominations.Select(kvp =>
        //                $"[{kvp.Key} Available: {currentBalances[kvp.Key]}, Requested: {request.GetType().GetProperty(kvp.Key)?.GetValue(request)}]"));
        //        throw new InvalidOperationException($"Insufficient denominations for CashOut operation: {insufficientDenomsMessage}.");
        //    }
        //}

        // Updates the denominations in the teller history based on the transaction type (cash-in or cash-out).
        private PrimaryTellerProvisioningHistory UpdateDenominations(CurrencyNotesRequest request, bool isCashIn, PrimaryTellerProvisioningHistory _tellerHistory)
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
        private TillStatusReportDto GetTellerReport(PrimaryTellerProvisioningHistory _tellerHistory)
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
