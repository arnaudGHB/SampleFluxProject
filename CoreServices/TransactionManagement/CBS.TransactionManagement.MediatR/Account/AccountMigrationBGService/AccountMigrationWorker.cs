using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Helper.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.AccountMigrationBGService
{
    public class AccountMigrationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly AccountMigrationQueue _queue;
        private readonly ILogger<AccountMigrationWorker> _logger;

        public AccountMigrationWorker(IServiceScopeFactory serviceScopeFactory, AccountMigrationQueue queue, ILogger<AccountMigrationWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _queue = queue;
            _logger = logger;
        }

        /// <summary>
        /// Continuously executes account migration requests from the queue until cancellation is requested.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // ✅ Dequeue the next migration request
                    var request = await _queue.DequeueAsync();
                    if (request != null)
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<TransactionContext>();

                        // ✅ Retrieve the Saving Product
                        var product = await dbContext.SavingProducts.FindAsync(request.ProductId);

                        // ✅ Retrieve the Teller associated with the branch
                        var teller = await dbContext.Tellers.FirstOrDefaultAsync(t => t.BranchId == request.BranchId);

                        if (product != null && teller != null)
                        {
                            string processingMessage = $"Processing account migration for accounts: {request.Accounts.Count}, Branch: {request.UserInfoToken.BranchName}. Initiated by {request.UserInfoToken.FullName}";
                            await BaseUtilities.LogAndAuditAsync(processingMessage, request, HttpStatusCodeEnum.OK, LogAction.AccountBalanceMigrationProcessing, LogLevelInfo.Information, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.CorrelationId);

                            // ✅ Process the account migration
                            await ProcessAccountMigration(request, product, teller);

                            string successMessage = $"Account migration process completed successfully for {request.UserInfoToken.BranchName}. Initiated by {request.UserInfoToken.FullName}.";
                            await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.AccountBalanceMigrationProcessing, LogLevelInfo.Information, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.CorrelationId);
                        }
                        else
                        {
                            string errorMessage = $"Failed to process migration request: Product or Teller not found. For {request.UserInfoToken.BranchName}. Initiated by {request.UserInfoToken.FullName}.";
                            await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.AccountBalanceMigrationProcessing, LogLevelInfo.Warning, request.UserInfoToken.FullName, request.UserInfoToken.Token, request.CorrelationId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string exceptionMessage = $"Unexpected error occurred in ExecuteAsync: {ex.Message}";
                    await BaseUtilities.LogAndAuditAsync(exceptionMessage, "AccountMigration", HttpStatusCodeEnum.InternalServerError, LogAction.AccountBalanceMigrationProcessing, LogLevelInfo.Error, "System", "", "");
                    _logger.LogError(ex, exceptionMessage);
                }

                // ✅ Delay to prevent CPU overuse
                await Task.Delay(1000, stoppingToken);
            }
        }

        /// <summary>
        /// Processes the account migration by deleting old accounts and transactions, then inserting new ones in batches.
        /// Logs and audits every step of the process for transparency and debugging.
        /// </summary>
        /// <param name="request">The account migration request.</param>
        /// <param name="product">The saving product associated with the accounts.</param>
        /// <param name="teller">The teller processing the migration.</param>
        /// <returns>Returns an AccountMigrationCommand containing migration results.</returns>
        private async Task<AccountMigrationCommand> ProcessAccountMigration(AccountMigrationCommand request, SavingProduct product, Teller teller)
        {
            string N_A = "N/A";
            var migrationResult = new AccountMigrationCommand();
            int newAccountsTotal = 0;
            int existingAccountsTotal = 0;
            int batchSize = request.Accounts.Count > 1000 ? 20 : 10; // Determine batch size dynamically

            DateTime startTime = BaseUtilities.UtcNowToDoualaTime(); ; // Capture the start time of the migration process

            // Split accounts into batches for parallel processing
            var accountBatches = request.Accounts
                .Select((account, index) => new { account, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.account).ToList())
                .ToList();

            // Log the start of the migration process
            await BaseUtilities.LogAndAuditAsync(
                $"Account balances migration processing started at {startTime}. Processing {request.Accounts.Count} accounts in {accountBatches.Count} batches (Batch Size: {batchSize}). Ordinary Account: {product.Name}.",
                request,
                HttpStatusCodeEnum.OK,
                LogAction.AccountBalanceMigrationProcessing,
                LogLevelInfo.Information,
                request.UserInfoToken.FullName,
                request.UserInfoToken.Token,
                request.CorrelationId
            );

            // Process each batch of accounts in parallel
            await Parallel.ForEachAsync(accountBatches, new ParallelOptions { MaxDegreeOfParallelism = 6 }, async (batch, _) =>
            {
                using var scope = _serviceScopeFactory.CreateScope(); // Create a new service scope
                var scopedProvider = scope.ServiceProvider;
                await using var dbContext = scopedProvider.GetRequiredService<TransactionContext>(); // Retrieve database context

                List<Account> newAccounts = new List<Account>();
                List<Account> accountsToDelete = new List<Account>();
                List<Transaction> transactions = new List<Transaction>();

                using var transaction = await dbContext.Database.BeginTransactionAsync(); // Begin database transaction
                try
                {
                    // Generate list of customer references
                    var membersReferences = batch.Select(data => $"{request.BranchCode}{data.CustomerId}").ToList();

                    // Fetch and delete existing transactions to prevent foreign key conflicts
                    var transactionsToDelete = await dbContext.Transactions
                        .Where(t => t.Account.ProductId == request.ProductId && membersReferences.Contains(t.Account.CustomerId))
                        .ToListAsync();

                    if (transactionsToDelete.Any())
                    {
                        dbContext.Transactions.RemoveRange(transactionsToDelete);
                        await BaseUtilities.LogAndAuditAsync(
                            $"Deleted {transactionsToDelete.Count} transactions in batch of {product.Name}.",
                           membersReferences,
                            HttpStatusCodeEnum.OK,
                            LogAction.TransactionDeletionForMigration,
                            LogLevelInfo.Information,
                            request.UserInfoToken.FullName,
                            request.UserInfoToken.Token,
                            request.CorrelationId
                        );
                    }

                    // Fetch and delete existing accounts before inserting new ones
                    accountsToDelete = await dbContext.MemberAccounts
                        .Where(a => a.ProductId == request.ProductId && membersReferences.Contains(a.CustomerId))
                        .ToListAsync();

                    if (accountsToDelete.Any())
                    {
                        dbContext.MemberAccounts.RemoveRange(accountsToDelete);
                        await BaseUtilities.LogAndAuditAsync(
                            $"Deleted {accountsToDelete.Count} accounts in batch of {product.Name}.",
                            membersReferences,
                            HttpStatusCodeEnum.OK,
                            LogAction.TransactionDeletionForMigration,
                            LogLevelInfo.Information,
                            request.UserInfoToken.FullName,
                            request.UserInfoToken.Token,
                            request.CorrelationId
                        );
                    }

                    // Commit deletions before inserting new records
                    if (accountsToDelete.Any() || transactionsToDelete.Any())
                    {
                        await dbContext.SaveChangesAsync();
                    }

                    // Process new accounts
                    foreach (var data in batch)
                    {
                        string membersReference = $"{request.BranchCode}{data.CustomerId}";
                        string accountNumber = $"{product.AccountNuber}{membersReference}";

                        var accountEntity = new Account
                        {
                            Id = BaseUtilities.GenerateUniqueNumber(),
                            AccountNumber = accountNumber,
                            AccountType = product.AccountType,
                            AccountName = product.AccountType,
                            ProductId = request.ProductId,
                            BankId = request.BankId,
                            BranchId = request.BranchId,
                            CustomerId = membersReference,
                            Balance = data.OpeningBalance,
                            EncryptedBalance = BalanceEncryption.Encrypt(data.OpeningBalance.ToString(), accountNumber),
                            OpeningBalance = data.OpeningBalance,
                            DateOfOpeningBalance = BaseUtilities.UtcNowToDoualaTime(),
                            CreatedDate = BaseUtilities.UtcNowToDoualaTime(),
                            ModifiedDate = BaseUtilities.UtcNowToDoualaTime(),
                            Status = AccountStatus.Active.ToString(),
                            DateOfLastOperation = BaseUtilities.UtcNowToDoualaTime(),
                            LastOperation = $"Members_Account_Balance_Migration_For_{product.AccountType}",
                            InterestGenerated = 0,
                            LastOperationAmount = data.OpeningBalance,
                            PreviousBalance = 0,
                            ReasonOfBlocked = N_A,
                            LastInterestPosted = 0,
                            LastInterestCalculatedDate = DateTime.MinValue,
                            BlockedAmount = 0,
                            CustomerName = data.CustomerName,
                            BranchCode = data.BranchCode,
                            CreatedBy = request.UserInfoToken.FullName,
                            DeletedBy = N_A,
                            ModifiedBy = N_A,
                        };

                        newAccounts.Add(accountEntity);
                        transactions.Add(CreateTransaction(
                            data.OpeningBalance,
                            accountEntity,
                            teller,
                            BaseUtilities.GenerateInsuranceUniqueNumber(8, $"M-{request.UserInfoToken.BranchCode}-"),
                            request.UserInfoToken.FullName
                        ));
                    }

                    if (newAccounts.Any()) await dbContext.MemberAccounts.AddRangeAsync(newAccounts);
                    if (transactions.Any()) await dbContext.Transactions.AddRangeAsync(transactions);

                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    await BaseUtilities.LogAndAuditAsync(
                        $"Transaction failed for batch of {product.Name}. Error: {ex.Message}",
                        request,
                        HttpStatusCodeEnum.InternalServerError,
                        LogAction.AccountBalanceMigrationProcessing,
                        LogLevelInfo.Error,
                        request.UserInfoToken.FullName,
                        request.UserInfoToken.Token,
                        request.CorrelationId
                    );
                }

                Interlocked.Add(ref newAccountsTotal, newAccounts.Count);
                Interlocked.Add(ref existingAccountsTotal, accountsToDelete.Count);
            });

            DateTime endTime = BaseUtilities.UtcNowToDoualaTime();
            TimeSpan totalDuration = endTime - startTime;

            await BaseUtilities.LogAndAuditAsync(
                $"Migration completed. Start Time: {endTime}. End Time: {endTime}. Total Execution Time: {totalDuration}. Ordinary Account: {product.Name}. New Accounts: {newAccountsTotal}, Deleted Accounts: {existingAccountsTotal}.",
                request,
                HttpStatusCodeEnum.OK,
                LogAction.AccountBalanceMigrationCompletion,
                LogLevelInfo.Information,
                request.UserInfoToken.FullName,
                request.UserInfoToken.Token,
                request.CorrelationId
            );

            migrationResult.NewAccountsCount = newAccountsTotal;
            migrationResult.ExistingAccountsCount = existingAccountsTotal;
            return migrationResult;
        }
        private Transaction CreateTransaction(decimal Amount, Account account, Teller teller, string Reference, string fullUserName)
        {

            // Calculate balance and credit based on original amount and fees, if IsOfflineCharge is true
            decimal balance = account.Balance;
            decimal credit = Amount;
            decimal originalAmount = Amount;
            string N_A = "N/A";
            // Create the transaction entity
            var transactionEntityEntryFee = new Transaction
            {
                Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique transaction ID
                CreatedDate = BaseUtilities.UtcNowToDoualaTime(), // Set transaction creation date
                Status = TransactionStatus.COMPLETED.ToString(), // Set transaction status to completed
                TransactionReference = Reference, // Set transaction reference
                ExternalReference = N_A, // Set external reference
                IsExternalOperation = false, // Set external operation flag
                ExternalApplicationName = N_A, // Set external application name
                SourceType = OperationSourceType.Web_Portal.ToString(), // Set source type
                Currency = Currency.XAF.ToString(), // Set currency
                TransactionType = TransactionType.Migration.ToString(), // Set transaction type (deposit)
                AccountNumber = account.AccountNumber, // Set account number
                PreviousBalance = account.Balance, // Set previous balance
                AccountId = account.Id, // Set account ID
                CustomerId = account.CustomerId, // Set customer ID
                ProductId = account.ProductId, // Set product ID
                SenderAccountId = account.Id, // Set sender account ID
                ReceiverAccountId = account.Id, // Set receiver account ID
                BankId = teller.BankId, // Set bank ID
                Operation = TransactionType.DEPOSIT.ToString(), // Set operation type (deposit)
                BranchId = teller.BranchId, // Set branch ID
                OriginalDepositAmount = originalAmount, // Set original deposit amount including fees
                Fee = 0, // Set fee (charges)
                Tax = 0, // Set tax (assuming tax is not applicable)
                Amount = credit, // Set amount (excluding fees)
                Note = $"Statement: Opening Balance of {BaseUtilities.FormatCurrency(originalAmount)} was completed on account {account.AccountType}, Reference: {Reference}.", // Set transaction note
                OperationType = OperationType.Credit.ToString(), // Set operation type (credit)
                FeeType = Events.Charge_Of_Saving_Withdrawal_Form.ToString(), // Set fee type
                TellerId = teller.Id, // Set teller ID
                DepositerNote = N_A, // Set depositer note
                DepositerTelephone = N_A, // Set depositer telephone
                DepositorIDNumber = N_A, // Set depositor ID number
                DepositorIDExpiryDate = N_A, // Set depositor ID expiry date
                DepositorIDIssueDate = N_A, // Set depositor ID issue date
                DepositorIDNumberPlaceOfIssue = N_A, // Set depositor ID place of issue
                IsDepositDoneByAccountOwner = true, // Set flag indicating if deposit is done by account owner
                DepositorName = N_A, // Set depositor name
                Balance = balance, // Set balance after deposit (including original amount)
                Credit = credit, // Set credit amount (including fees if IsOfflineCharge is true, otherwise excluding fees)
                Debit = 0, // Set debit amount (assuming no debit)
                DestinationBrachId = teller.BranchId,
                OperationCharge = 0,
                ReceiptTitle = "Cash Receipt, Opening Balance. Reference: " + Reference,
                WithrawalFormCharge = Amount,  // Set destination branch ID
                SourceBrachId = teller.BranchId, // Set source branch ID
                IsInterBrachOperation = false, // Set flag indicating if inter-branch operation
                DestinationBranchCommission = 0, // Calculate destination branch commission
                SourceBranchCommission = 0, // Calculate source branch commission
                CreatedBy = fullUserName,
                CloseOfAccountCharge = 0,
                DeletedBy = N_A,
                DeletedDate = DateTime.MinValue,
                IsDeleted = false,
                ModifiedBy = N_A,
                ModifiedDate = DateTime.MaxValue,
                AccountType=account.AccountType,
                AccountingDate=BaseUtilities.UtcNowToDoualaTime(),
            };

            return transactionEntityEntryFee; // Return the transaction entity
        }

    }

}
