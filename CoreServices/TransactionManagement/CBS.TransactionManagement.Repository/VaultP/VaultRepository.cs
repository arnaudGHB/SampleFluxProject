using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.CashVaultP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository.VaultOperationP;
using CBS.TransactionManagement.Data.Entity.VaultOperationP;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.ChangeManagement;
using Microsoft.EntityFrameworkCore;

namespace CBS.TransactionManagement.Repository.VaultP
{

    /// <summary>
    /// Repository for managing vault-related operations, including cash transactions and denomination updates.
    /// </summary>
    public class VaultRepository : GenericRepository<Vault, TransactionContext>, IVaultRepository
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<VaultRepository> _logger; // Logger for logging handler actions and errors.
        private readonly IVaultOperationRepository _vaultOperationRepository;
        private readonly ICashChangeHistoryRepository _cashChangeHistoryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">Unit of Work for managing database transactions.</param>
        /// <param name="userInfoToken">User information for auditing and contextual purposes.</param>
        public VaultRepository(IUnitOfWork<TransactionContext> unitOfWork, UserInfoToken userInfoToken, ILogger<VaultRepository> logger, IVaultOperationRepository vaultOperationRepository, ICashChangeHistoryRepository cashChangeHistoryRepository) : base(unitOfWork)
        {
            _userInfoToken = userInfoToken;
            _logger=logger;
            _vaultOperationRepository=vaultOperationRepository;
            _cashChangeHistoryRepository=cashChangeHistoryRepository;
        }

        /// <summary>
        /// Handles cash-in transactions by updating denominations and increasing the vault's balance.
        /// </summary>
        /// <param name="amount">The total amount to be added to the vault.</param>
        /// <param name="denominations">Details of denominations to update.</param>
        /// <param name="branchId">The ID of the branch whose vault is being updated.</param>
        /// <param name="reference">A unique reference for the cash-in transaction.</param>
        /// <param name="LastOperation">Description of the last operation performed on the vault.</param>
        /// <param name="cashInHand">The amount of cash physically in hand (optional).</param>
        /// <param name="cashinVault">The amount of cash added directly to the vault (optional).</param>
        public async void CashInByDenomination(decimal amount, CurrencyNotesRequest denominations, string branchId, string reference, string LastOperation, decimal cashInHand = 0, decimal cashinVault = 0, string InitializationNote = "N/A", bool isInternal = false)
        {
            try
            {
                // Step 1: Validate that the total amount matches the denominations provided.
                var (isValid, validationErrorMessage) = CurrencyNotesMapper.ValidateAmountAndDenominations(amount, denominations);
                if (!isValid)
                {
                    // Log and audit the validation failure, then throw an exception.
                    _logger.LogError(validationErrorMessage);
                    await BaseUtilities.LogAndAuditAsync(validationErrorMessage, denominations, HttpStatusCodeEnum.BadRequest, LogAction.DinominationValidationError, LogLevelInfo.Error);
                    throw new InvalidOperationException(validationErrorMessage);
                }

                // Step 2: Retrieve the vault associated with the specified branch ID.
                var vault = FindBy(v => v.BranchId == branchId).FirstOrDefault();
                if (vault == null)
                {
                    // Log and audit if no vault is found, then throw an exception.
                    string errorMessage = $"No vault found for Branch ID: {branchId}. Ensure the branch ID is correct.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, denominations, HttpStatusCodeEnum.NotFound, LogAction.VaultOperationCashin, LogLevelInfo.Error);
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 3: Update vault denominations and balance.
                UpdateDenominations(denominations, true, vault); // Add denominations to the vault.
                vault.CurrentBalance += amount;                 // Increase the vault's balance by the specified amount.
                vault.LastOperation = LastOperation;            // Record the description of the last operation.
                vault.LastOperationAmount = amount;             // Record the amount for the last operation.

                // Step 4: Log the success of the operation.
                string successMessage = $"Successfully completed cash-in operation on {LastOperation}. Amount: {BaseUtilities.FormatCurrency(amount)}. Operation Done By {_userInfoToken.FullName}.";
                _logger.LogInformation(successMessage);

                // Step 7: Record the operation in the vault operation history.
                var vaultOperation = new VaultOperation
                {
                    Amount = amount,
                    BranchId = branchId,
                    VaultId = vault.Id,
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    DoneBy = _userInfoToken.FullName,
                    OperationType = LastOperation,
                    Description = successMessage,
                    InitializationNote=InitializationNote,
                    Reference = reference,
                    CashInHand = cashInHand,
                    CashInVault = cashinVault,
                };
                _vaultOperationRepository.Add(vaultOperation);
                // Step 6: Save the updated vault state.
                Update(vault);
                // Step 8: Commit changes to the database.
                if (!isInternal)
                {
                    // Step 9: Commit the changes to the database.
                    await _uow.SaveAsync();
                }
                // Step 5: Audit the successful cash-in operation.
                await BaseUtilities.LogAndAuditAsync(successMessage, denominations, HttpStatusCodeEnum.OK, LogAction.VaultOperationCashin, LogLevelInfo.Information);

            }
            catch (Exception ex)
            {
                // Step 9: Handle unexpected exceptions by logging and auditing the error, then rethrow.
                string errorMessage = $"An error occurred during the cash-in operation: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, denominations, HttpStatusCodeEnum.InternalServerError, LogAction.VaultOperationCashin, LogLevelInfo.Error);
                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Verifies if the vault has sufficient funds for a cash-out operation.
        /// </summary>
        /// <param name="amount">The amount to be checked for cash-out.</param>
        /// <exception cref="InvalidOperationException">Thrown when the vault does not have sufficient funds.</exception>
        public void VerifySufficientFunds(decimal amount)
        {
            try
            {
                // Step 1: Retrieve the vault associated with the branch ID of the current user.
                // This ensures the check is performed on the appropriate vault corresponding to the branch.
                var vault = FindBy(v => v.BranchId == _userInfoToken.BranchID).FirstOrDefault();

                // Step 2: Validate if the vault exists for the branch.
                // If no vault is found, log an error and throw an exception to indicate a configuration issue.
                if (vault == null)
                {
                    string errorMessage = $"No vault found for Branch: {_userInfoToken.BranchName}. Please check the branch configuration.";
                    _logger.LogError(errorMessage); // Log the error for debugging or audit purposes.
                    throw new InvalidOperationException(errorMessage); // Terminate the operation with a meaningful exception.
                }

                // Step 3: Compare the current balance of the vault against the requested cash-out amount.
                // If the balance is insufficient, log a warning and throw an exception to prevent the operation.
                if (vault.CurrentBalance < amount)
                {
                    string errorMessage = $"Insufficient funds in the vault {vault.Name}. Current Balance: {BaseUtilities.FormatCurrency(vault.CurrentBalance)}, Requested: {BaseUtilities.FormatCurrency(amount)}.";
                    _logger.LogWarning(errorMessage); // Log the warning for operational visibility.
                    throw new InvalidOperationException(errorMessage); // Terminate the operation with a meaningful exception.
                }

                // Step 4: Log a success message indicating that sufficient funds are available.
                // This provides an audit trail for successful validations.
                _logger.LogInformation($"Sufficient funds verified for cash-out. Requested: {BaseUtilities.FormatCurrency(amount)}, Available: {BaseUtilities.FormatCurrency(vault.CurrentBalance)}.");
            }
            catch (Exception ex)
            {
                // Step 5: Catch any unexpected exceptions, log the details, and rethrow the exception.
                // This ensures that any issue is recorded and propagated to the calling context for further handling.
                _logger.LogError($"Error verifying sufficient funds: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the current balance of the vault for the specified branch.
        /// </summary>
        /// <param name="branchId">The ID of the branch whose vault balance is to be retrieved.</param>
        /// <returns>The current balance of the vault.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the vault is not found for the given branch ID.</exception>
        public decimal GetVaultBalance(string branchId)
        {
            try
            {
                // Attempt to retrieve the vault record associated with the specified branch ID.
                var vault = FindBy(v => v.BranchId == branchId).FirstOrDefault();

                // Check if the vault record was found.
                if (vault == null)
                {
                    // Construct an error message for when no vault is found for the given branch ID.
                    string errorMessage = $"No vault found for Branch ID: {branchId}. Ensure the branch ID is correct.";

                    // Log the error for auditing and debugging purposes.
                    _logger.LogError(errorMessage);

                    // Throw an exception to indicate that the operation failed due to a missing vault.
                    throw new InvalidOperationException(errorMessage);
                }

                // Log the successful retrieval of the vault balance, including the current balance.
                _logger.LogInformation($"Retrieved vault balance for Branch ID: {branchId}. Current Balance: {BaseUtilities.FormatCurrency(vault.CurrentBalance)}.");

                // Return the current balance of the retrieved vault.
                return vault.CurrentBalance;
            }
            catch (Exception ex)
            {
                // Log the details of any exception that occurs during the operation.
                _logger.LogError($"Error retrieving vault balance for Branch ID: {branchId}. {ex.Message}");

                // Re-throw the exception to ensure that the caller is informed of the failure.
                throw;
            }
        }


        /// <summary>
        /// Checks if sufficient denominations are available for a cash-out transaction.
        /// </summary>
        /// <param name="request">Denomination details to check.</param>
        /// <param name="vault">The vault to check against.</param>
        /// <returns>A tuple containing current balances and any insufficient denominations.</returns>
        private (Dictionary<string, decimal> CurrentBalances, Dictionary<string, decimal> InsufficientDenominations) CheckDenominationSufficiency(CurrencyNotesRequest request, Vault vault)
        {
            var currentBalances = new Dictionary<string, decimal>
    {
        { "Note10000", vault.ClosingNote10000 },
        { "Note5000", vault.ClosingNote5000 },
        { "Note2000", vault.ClosingNote2000 },
        { "Note1000", vault.ClosingNote1000 },
        { "Note500", vault.ClosingNote500 },
        { "Coin500", vault.ClosingCoin500 },
        { "Coin100", vault.ClosingCoin100 },
        { "Coin50", vault.ClosingCoin50 },
        { "Coin25", vault.ClosingCoin25 },
        { "Coin10", vault.ClosingCoin10 },
        { "Coin5", vault.ClosingCoin5 },
        { "Coin1", vault.ClosingCoin1 }
    };

            var insufficientDenominations = new Dictionary<string, decimal>();

            // Compare each denomination in the request against the available denominations in the teller history.
            if (vault.ClosingNote10000 < request.Note10000)
                insufficientDenominations.Add("Note10000", vault.ClosingNote10000);
            if (vault.ClosingNote5000 < request.Note5000)
                insufficientDenominations.Add("Note5000", vault.ClosingNote5000);
            if (vault.ClosingNote2000 < request.Note2000)
                insufficientDenominations.Add("Note2000", vault.ClosingNote2000);
            if (vault.ClosingNote1000 < request.Note1000)
                insufficientDenominations.Add("Note1000", vault.ClosingNote1000);
            if (vault.ClosingNote500 < request.Note500)
                insufficientDenominations.Add("Note500", vault.ClosingNote500);
            if (vault.ClosingCoin500 < request.Coin500)
                insufficientDenominations.Add("Coin500", vault.ClosingCoin500);
            if (vault.ClosingCoin100 < request.Coin100)
                insufficientDenominations.Add("Coin100", vault.ClosingCoin100);
            if (vault.ClosingCoin50 < request.Coin50)
                insufficientDenominations.Add("Coin50", vault.ClosingCoin50);
            if (vault.ClosingCoin25 < request.Coin25)
                insufficientDenominations.Add("Coin25", vault.ClosingCoin25);
            if (vault.ClosingCoin10 < request.Coin10)
                insufficientDenominations.Add("Coin10", vault.ClosingCoin10);
            if (vault.ClosingCoin5 < request.Coin5)
                insufficientDenominations.Add("Coin5", vault.ClosingCoin5);
            if (vault.ClosingCoin1 < request.Coin1)
                insufficientDenominations.Add("Coin1", vault.ClosingCoin1);

            return (currentBalances, insufficientDenominations);
        }

        /// <summary>
        /// Handles cash-out transactions by validating denominations, updating the vault's state, and reducing its balance.
        /// </summary>
        /// <param name="amount">The total amount to deduct from the vault.</param>
        /// <param name="denominations">Denomination details for the cash-out operation.</param>
        /// <param name="branchId">The ID of the branch whose vault is being updated.</param>
        /// <param name="reference">A unique reference for the cash-out transaction.</param>
        /// <param name="LastOperation">Description of the last operation performed on the vault.</param>
        /// <returns>A task representing the asynchronous operation, returning true if successful.</returns>
        public async Task<bool> CashOutByDenominationAsync(decimal amount, CurrencyNotesRequest denominations, string branchId, string reference, string LastOperation, bool isInternal = false)
        {
            try
            {
                // Step 1: Validate that the total amount matches the denominations provided.
                var (isValid, validationErrorMessage) = CurrencyNotesMapper.ValidateAmountAndDenominations(amount, denominations);
                if (!isValid)
                {
                    // Log and audit the validation failure, then throw an exception.
                    _logger.LogError(validationErrorMessage);
                    await BaseUtilities.LogAndAuditAsync(validationErrorMessage, denominations, HttpStatusCodeEnum.BadRequest, LogAction.DinominationValidationError, LogLevelInfo.Error);
                    throw new InvalidOperationException(validationErrorMessage);
                }

                // Step 2: Retrieve the vault associated with the specified branch ID.
                var vault = FindBy(v => v.BranchId == branchId).FirstOrDefault();
                if (vault == null)
                {
                    // Log and audit if no vault is found, then throw an exception.
                    string errorMessage = $"No vault found for Branch ID: {branchId}. Verify the branch ID and try again.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, denominations, HttpStatusCodeEnum.NotFound, LogAction.VaultOperationOut, LogLevelInfo.Error);
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 3: Check if the vault has sufficient denominations for the cash-out.
                var (currentBalance, insufficientDenominations) = CheckDenominationSufficiency(denominations, vault);

                if (insufficientDenominations.Count == 0)
                {
                    // Step 4: Update the vault's denominations by deducting the requested amounts.
                    UpdateDenominations(denominations, false, vault);

                    // Step 5: Update the vault's balance and metadata for the operation.
                    vault.CurrentBalance -= amount;
                    vault.LastOperation = LastOperation;
                    vault.LastOperationAmount = amount;


                    // Step 6: Log and audit the successful cash-out operation.
                    string successMessage = $"Successfully completed cash-out operation. Amount: {BaseUtilities.FormatCurrency(amount)}. Operation done by {_userInfoToken.FullName}.";


                    // Step 8: Record the operation in the vault operation history.
                    var vaultOperation = new VaultOperation
                    {
                        Amount = amount,
                        BranchId = branchId,
                        VaultId = vault.Id,
                        Id = BaseUtilities.GenerateUniqueNumber(),
                        DoneBy = _userInfoToken.FullName,
                        InitializationNote="N/A",
                        OperationType = TransactionType.WITHDRAWAL.ToString(),
                        Description = successMessage,
                        Reference = reference
                    };
                    _vaultOperationRepository.Add(vaultOperation);

                    // Step 7: Save the updated vault state.
                    Update(vault);
                    if (!isInternal)
                    {
                        // Step 9: Commit the changes to the database.
                        await _uow.SaveAsync();
                    }

                    await BaseUtilities.LogAndAuditAsync(successMessage, denominations, HttpStatusCodeEnum.OK, LogAction.VaultOperationOut, LogLevelInfo.Information);
                    _logger.LogInformation(successMessage);
                    // Step 10: Return true to indicate successful completion.
                    return true;
                }
                else
                {
                    // Step 11: Handle insufficient denominations.
                    var insufficientDenomsMessage = string.Join(", ",
                        insufficientDenominations.Select(kvp =>
                            $"[{kvp.Key} Available: {currentBalance[kvp.Key]}, Requested: {denominations.GetType().GetProperty(kvp.Key)?.GetValue(denominations)}]"));

                    string message = $"Insufficient denominations for cash-out from vault: {vault.Name}. Error: {insufficientDenomsMessage}.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, denominations, HttpStatusCodeEnum.Forbidden, LogAction.VaultOperationOut, LogLevelInfo.Warning);

                    // Throw an exception with the detailed error message.
                    throw new InvalidOperationException(message);
                }
            }
            catch (Exception ex)
            {
                // Step 12: Log and audit unexpected exceptions, then rethrow.
                string errorMessage = $"An error occurred during the cash-out operation: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, denominations, HttpStatusCodeEnum.InternalServerError, LogAction.VaultOperationOut, LogLevelInfo.Error);
                throw new InvalidOperationException(errorMessage);
            }
        }


        // Updates the denominations in the teller history based on the transaction type (cash-in or cash-out).
        private Vault UpdateDenominations(CurrencyNotesRequest request, bool isCashIn, Vault vault)
        {
            // Adjust each denomination based on whether it's a cash-in or cash-out transaction.
            vault.ClosingNote10000 += (isCashIn ? request.Note10000 : -request.Note10000);
            vault.ClosingNote5000 += (isCashIn ? request.Note5000 : -request.Note5000);
            vault.ClosingNote2000 += (isCashIn ? request.Note2000 : -request.Note2000);
            vault.ClosingNote1000 += (isCashIn ? request.Note1000 : -request.Note1000);
            vault.ClosingNote500 += (isCashIn ? request.Note500 : -request.Note500);
            vault.ClosingCoin500 += (isCashIn ? request.Coin500 : -request.Coin500);
            vault.ClosingCoin100 += (isCashIn ? request.Coin100 : -request.Coin100);
            vault.ClosingCoin50 += (isCashIn ? request.Coin50 : -request.Coin50);
            vault.ClosingCoin25 += (isCashIn ? request.Coin25 : -request.Coin25);
            vault.ClosingCoin10 += (isCashIn ? request.Coin10 : -request.Coin10);
            vault.ClosingCoin5 += (isCashIn ? request.Coin5 : -request.Coin5);
            vault.ClosingCoin1 += (isCashIn ? request.Coin1 : -request.Coin1);

            // Return the updated teller history.
            return vault;
        }

        /// <summary>
        /// Transfers cash between two vaults, validating denominations and updating vault balances.
        /// </summary>
        /// <param name="branchFromid">The source vault ID.</param>
        /// <param name="brachToId">The destination vault ID.</param>
        /// <param name="amount">The amount to transfer.</param>
        /// <param name="denominations">Details of denominations to transfer.</param>
        /// <param name="reference">A unique reference for the transfer operation.</param>
        /// <param name="LastOperation">Description of the last operation performed on the source vault.</param>
        public async void TransferCash(string branchFromid, string brachToId, decimal amount, CurrencyNotesRequest denominations, string reference, string LastOperation, bool isInternal = false)
        {
            try
            {
                // Step 1: Validate that the total amount matches the denominations provided.
                var (isValid, validationErrorMessage) = CurrencyNotesMapper.ValidateAmountAndDenominations(amount, denominations);
                if (!isValid)
                {
                    // Log and audit validation failure, then throw an exception.
                    _logger.LogError(validationErrorMessage);
                    await BaseUtilities.LogAndAuditAsync(validationErrorMessage, denominations, HttpStatusCodeEnum.BadRequest, LogAction.DinominationValidationError, LogLevelInfo.Error);
                    throw new InvalidOperationException(validationErrorMessage);
                }

                // Step 2: Retrieve the source and destination vaults by their branch IDs.
                var fromVault = await FindBy(x => x.BranchId == branchFromid && x.IsActive).FirstOrDefaultAsync();
                var toVault = await FindBy(x => x.BranchId == brachToId && x.IsActive).FirstOrDefaultAsync();

                // Step 3: Ensure both vaults exist.
                if (fromVault == null || toVault == null)
                {
                    string errorMessage = "Source or destination vault not found. Ensure both vaults exist.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, denominations, HttpStatusCodeEnum.NotFound, LogAction.VaultOperationTransferCash, LogLevelInfo.Error);
                    throw new ArgumentNullException(errorMessage);
                }

                // Step 4: Check if the source vault has sufficient denominations for the transfer.
                var (currentBalance, insufficientDenominations) = CheckDenominationSufficiency(denominations, fromVault);

                if (insufficientDenominations.Count == 0)
                {
                    // Step 5: Update denominations in both source and destination vaults.
                    UpdateDenominations(denominations, false, fromVault); // Deduct denominations from source vault.
                    UpdateDenominations(denominations, true, toVault); // Add denominations to destination vault.

                    // Step 6: Update balances and metadata for the source vault.
                    fromVault.CurrentBalance -= amount;
                    fromVault.LastOperation = LastOperation;
                    fromVault.LastOperationAmount = amount;

                    // Step 7: Update balances and metadata for the destination vault.
                    toVault.CurrentBalance += amount;
                    toVault.LastOperation = TransactionType.CASH_RECEPTION.ToString();
                    toVault.LastOperationAmount = amount;

                    // Step 8: Log and audit successful transfer.
                    string successMessage = $"Successfully transferred {BaseUtilities.FormatCurrency(amount)} from Vault {fromVault.Name} to Vault {toVault.Name}. Done by {_userInfoToken.FullName}.";




                    // Step 10: Record the transfer operation in the vault operation history.
                    var vaultOperation = new VaultOperation
                    {
                        Amount = amount,
                        BranchId = branchFromid,
                        VaultId = fromVault.Id,
                        InitializationNote="N/A",
                        Id = BaseUtilities.GenerateUniqueNumber(),
                        DoneBy = _userInfoToken.FullName,
                        OperationType = TransactionType.TRANSFER.ToString(),
                        Description = successMessage,
                        Reference = reference
                    };
                    _vaultOperationRepository.Add(vaultOperation);
                    // Step 9: Update both vaults in the repository.
                    Update(fromVault);
                    Update(toVault);
                    // Step 11: Save all changes to the database.
                    if (!isInternal)
                    {
                        // Step 9: Commit the changes to the database.
                        await _uow.SaveAsync();
                    }
                    _logger.LogInformation(successMessage);
                    await BaseUtilities.LogAndAuditAsync(successMessage, denominations, HttpStatusCodeEnum.OK, LogAction.VaultOperationTransferCash, LogLevelInfo.Information);
                }
                else
                {
                    // Step 12: Handle insufficient denominations.
                    var insufficientDenomsMessage = string.Join(", ",
                        insufficientDenominations.Select(kvp =>
                            $"[{kvp.Key} Available: {currentBalance[kvp.Key]}, Requested: {denominations.GetType().GetProperty(kvp.Key)?.GetValue(denominations)}]"));

                    string message = $"Insufficient denominations for transfer from source vault: {fromVault.Name}. Error: {insufficientDenomsMessage}.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, denominations, HttpStatusCodeEnum.Forbidden, LogAction.VaultOperationTransferCash, LogLevelInfo.Warning);

                    // Throw an exception with the detailed error message.
                    throw new InvalidOperationException(message);
                }
            }
            catch (Exception ex)
            {
                // Step 13: Handle unexpected exceptions.
                string errorMessage = $"An error occurred during vault cash transfer: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, denominations, HttpStatusCodeEnum.InternalServerError, LogAction.VaultOperationTransferCash, LogLevelInfo.Error);
                throw new InvalidOperationException(errorMessage);
            }
        }
        /// <summary>
        /// Extracts current balances of all denominations from the sub-teller provisioning object.
        /// </summary>
        /// <param name="subTellerProvioning">The sub-teller provisioning object.</param>
        /// <returns>A dictionary with denomination names as keys and their respective balances as values.</returns>
        private Dictionary<string, decimal> ExtractCurrentBalances(Vault subTellerProvioning)
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
        public async Task<CashChangeHistory> ManageCashChangeAsync(CashChangeDataCarrier changeManagement)
        {
            try
            {
                // Step 1: Validate input parameters for null values.
                if (changeManagement.denominationsGiven == null || changeManagement.denominationsReceived == null)
                {
                    string errorMessage = "DenominationsGiven and DenominationsReceived cannot be null.";
                    _logger.LogError(errorMessage);
                    throw new ArgumentNullException(errorMessage);
                }

                // Step 2: Retrieve the vault associated with the user's branch ID.
                var vault = await FindBy(v => v.BranchId == _userInfoToken.BranchID).FirstOrDefaultAsync();

                // Step 3: Validate the existence of the vault for the branch.
                if (vault == null)
                {
                    string errorMessage = $"No vault found for Branch name: {_userInfoToken.BranchName}. Ensure the branch ID is correct.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, changeManagement, HttpStatusCodeEnum.NotFound, LogAction.VaultOperationChange, LogLevelInfo.Error);
                    throw new InvalidOperationException(errorMessage);
                }
                bool isReceivingCash = false;
                // Step 3: Extract current balances from the sub-teller's provisioning.
                var currentBalances = ExtractCurrentBalances(vault);

                // Step 4: Calculate the total amounts for given and received denominations.
                decimal totalAmountGiven = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsGiven);
                decimal totalAmountReceived = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsReceived);

                // Step 5: Check sufficiency of received denominations (partial acceptance).
                var (currentBalancesReceived, insufficientDenominationsReceived) = CheckDenominationSufficiency(changeManagement.denominationsReceived, vault);
                if (insufficientDenominationsReceived.Count > 0)
                {
                    decimal shortfall = insufficientDenominationsReceived
     .Sum(kvp => GetDenominationValue(kvp.Key) *
                ((int)(changeManagement.denominationsReceived.GetType().GetProperty(kvp.Key)?.GetValue(changeManagement.denominationsReceived) ?? 0) - kvp.Value));
                    isReceivingCash=true;

                    totalAmountReceived -= shortfall; // Adjust total received for shortfall.
                }

                // Step 6: Check sufficiency of funds for the denominations to be given.
                var (currentBalancesGiven, insufficientDenominationsGiven) = CheckDenominationSufficiency(changeManagement.denominationsGiven, vault);
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
                    UpdateDenominations(changeManagement.denominationsReceived, true, vault); // Deduct received denominations.
                    UpdateDenominations(optimizedDenominationsGiven, false, vault); // Add optimized denominations for given.

                }
                else
                {
                    UpdateDenominations(changeManagement.denominationsReceived, true, vault); // Deduct the received denominations.
                    UpdateDenominations(changeManagement.denominationsGiven, false, vault);    // Add the given denominations.

                }
                // Step 10: Log the successful operation in the cash change history.
                var cashChangeHistory = new CashChangeHistory
                {
                    Id = Guid.NewGuid().ToString(), // Generate a unique ID for the history.
                    Reference = changeManagement.reference, // Store the provided reference for tracking.
                    ChangeDate = DateTime.UtcNow, // Set the date of the change operation.
                    AmountGiven = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsGiven), // Calculate the total given amount.
                    AmountReceive = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsReceived), // Calculate the total received amount.
                    ServiceOperationType = "Cash Changed From Vault.", // Specify the operation type.
                    BranchId = _userInfoToken.BranchID, // Assign the branch ID from the user info.
                    BranchCode = _userInfoToken.BranchCode, // Assign the branch code from the user info.
                    BranchName = _userInfoToken.BranchName, // Assign the branch name from the user info.
                    ChangedBy = _userInfoToken.FullName, // Record the name of the user performing the operation.
                    ChangeReason = changeManagement.changeReason, // Include the reason for the cash change operation.
                    SystemName = vault.Name // Include the reason for the cash change operation.
                };
                _cashChangeHistoryRepository.CreateChangeHistory(changeManagement, cashChangeHistory);

                // Step 11: Persist all changes to the database.
                Update(vault);
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
        /// Manages a cash change operation in the vault.
        /// </summary>
        /// <param name="changeManagement">An object containing details of the cash change operation, including denominations, reference, and reason.</param>
        /// <returns>A task representing the asynchronous operation, returning a CashChangeHistoryDto object if successful.</returns>
        //public async Task<CashChangeHistory> ManageCashChangeAsync(CashChangeDataCarrier changeManagement)
        //{
        //    try
        //    {
        //        // Step 1: Validate input parameters for null values.
        //        if (changeManagement.denominationsGiven == null || changeManagement.denominationsReceived == null)
        //        {
        //            string errorMessage = "DenominationsGiven and DenominationsReceived cannot be null.";
        //            _logger.LogError(errorMessage);
        //            throw new ArgumentNullException(errorMessage);
        //        }

        //        // Step 2: Retrieve the vault associated with the user's branch ID.
        //        var vault = await FindBy(v => v.BranchId == _userInfoToken.BranchID).FirstOrDefaultAsync();

        //        // Step 3: Validate the existence of the vault for the branch.
        //        if (vault == null)
        //        {
        //            string errorMessage = $"No vault found for Branch name: {_userInfoToken.BranchName}. Ensure the branch ID is correct.";
        //            _logger.LogError(errorMessage);
        //            await BaseUtilities.LogAndAuditAsync(errorMessage, changeManagement, HttpStatusCodeEnum.NotFound, LogAction.VaultOperationChange, LogLevelInfo.Error);
        //            throw new InvalidOperationException(errorMessage);
        //        }

        //        // Step 3: Check if sufficient denominations are available for the operation.
        //        var (currentBalancesReceived, insufficientDenominationsReceived) = CheckDenominationSufficiency(changeManagement.denominationsReceived, vault);
        //        var (currentBalancesGiven, insufficientDenominationsGiven) = CheckDenominationSufficiency(changeManagement.denominationsGiven, vault);

        //        // Step 3.1: Validate the sufficiency of denominations received.
        //        if (insufficientDenominationsReceived.Count > 0)
        //        {
        //            var insufficientDenomsMessage = string.Join(", ",
        //                insufficientDenominationsReceived.Select(kvp =>
        //                    $"[{kvp.Key} Available: {currentBalancesReceived[kvp.Key]}, Requested: {changeManagement.denominationsReceived.GetType().GetProperty(kvp.Key)?.GetValue(changeManagement.denominationsReceived)}]"));

        //            string errorMessage = $"Insufficient denominations for the cash received operation: {insufficientDenomsMessage}.";
        //            throw new InvalidOperationException(errorMessage);
        //        }

        //        // Step 3.2: Validate the sufficiency of denominations given.
        //        if (insufficientDenominationsGiven.Count > 0)
        //        {
        //            var insufficientDenomsMessage = string.Join(", ",
        //                insufficientDenominationsGiven.Select(kvp =>
        //                    $"[{kvp.Key} Available: {currentBalancesGiven[kvp.Key]}, Needed: {changeManagement.denominationsGiven.GetType().GetProperty(kvp.Key)?.GetValue(changeManagement.denominationsGiven)}]"));

        //            string errorMessage = $"Insufficient denominations to complete the cash given operation: {insufficientDenomsMessage}.";
        //            throw new InvalidOperationException(errorMessage);
        //        }

        //        // Step 3.3: Validate the total amount of denominations given matches the amount being exchanged.
        //        decimal totalAmountGiven = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsGiven);
        //        decimal totalAmountReceived = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsReceived);

        //        if (totalAmountGiven != totalAmountReceived)
        //        {
        //            string errorMessage = $"The total value of denominations does not match. Amount Given: {BaseUtilities.FormatCurrency(totalAmountGiven)}, Amount Received: {BaseUtilities.FormatCurrency(totalAmountReceived)}.";
        //            throw new InvalidOperationException(errorMessage);
        //        }

        //        // Step 6: Perform the cash change operation.
        //        UpdateDenominations(changeManagement.denominationsReceived, false, vault); // Deduct the received denominations from the vault.
        //        UpdateDenominations(changeManagement.denominationsGiven, true, vault);    // Add the given denominations to the vault.

        //        // Step 7: Create a CashChangeHistoryDto object to record the operation details.
        //        var cashChangeHistory = new CashChangeHistory
        //        {
        //            Id = Guid.NewGuid().ToString(), // Generate a unique ID for the history.
        //            Reference = changeManagement.reference, // Store the provided reference for tracking.
        //            ChangeDate = DateTime.UtcNow, // Set the date of the change operation.
        //            AmountGiven = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsGiven), // Calculate the total given amount.
        //            AmountReceive = CurrencyNotesMapper.CalculateTotalAmount(changeManagement.denominationsReceived), // Calculate the total received amount.
        //            ServiceOperationType = "Cash Changed From Vault.", // Specify the operation type.
        //            BranchId = _userInfoToken.BranchID, // Assign the branch ID from the user info.
        //            BranchCode = _userInfoToken.BranchCode, // Assign the branch code from the user info.
        //            BranchName = _userInfoToken.BranchName, // Assign the branch name from the user info.
        //            ChangedBy = _userInfoToken.FullName, // Record the name of the user performing the operation.
        //            ChangeReason = changeManagement.changeReason, // Include the reason for the cash change operation.
        //            SystemName = vault.Name // Include the reason for the cash change operation.
        //        };
        //        changeManagement.SystemName=vault.Name;
        //        // Step 8: Log the history of the change in the repository.
        //        _cashChangeHistoryRepository.CreateChangeHistory(changeManagement, cashChangeHistory);

        //        // Step 9: Update the vault state and persist changes to the database.
        //        Update(vault); // Apply changes to the vault's state.
        //        await _uow.SaveAsync(); // Save changes to the database.

        //        // Step 10: Log and audit the successful operation.
        //        string successMessage = $"Cash change operation in vault completed successfully for branch {_userInfoToken.BranchName}. Done by {_userInfoToken.FullName}.";
        //        _logger.LogInformation(successMessage);
        //        await BaseUtilities.LogAndAuditAsync(successMessage, cashChangeHistory, HttpStatusCodeEnum.OK, LogAction.VaultOperationChange, LogLevelInfo.Information);

        //        // Step 11: Return the cash change history object as the result.
        //        return cashChangeHistory;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Step 12: Log and audit unexpected exceptions.
        //        string errorMessage = $"An error occurred during the cash change operation: {ex.Message}.";
        //        _logger.LogError(errorMessage);
        //        await BaseUtilities.LogAndAuditAsync(errorMessage, changeManagement, HttpStatusCodeEnum.InternalServerError, LogAction.VaultOperationChange, LogLevelInfo.Error);
        //        throw;
        //    }
        //}

    }
}
