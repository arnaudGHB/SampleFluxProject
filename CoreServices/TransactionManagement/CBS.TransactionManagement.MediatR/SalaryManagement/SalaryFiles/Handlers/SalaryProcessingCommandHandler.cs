using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data.Entity.TransferLimits;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.LoanRepayment.Command;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Repository.Receipts.Payments;
using AutoMapper;
using CBS.TransactionManagement.Data.LoanQueries;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles;
using CBS.TransactionManagement.Repository.FileUploadP;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Commands;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Commands;
using CBS.TransactionManagement.Repository.VaultP;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryAnalysisResultP.Commands;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryFiles.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transfer Transaction.
    /// </summary>
    public class SalaryProcessingCommandHandler : IRequestHandler<SalaryProcessingCommand, ServiceResponse<SalaryProcessingDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly ILogger<SalaryProcessingCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfigRepository _ConfigRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly ISalaryProcessingRepository _salaryExtractRepository;
        private readonly IFileUploadRepository _fileUploadRepository;
        private readonly IPaymentReceiptRepository _paymentReceiptRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private const decimal VatRate = 19.25m;
        private readonly ISavingProductFeeRepository _savingProductFeeRepository;

        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the SalaryProcessingCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public SalaryProcessingCommandHandler(
            IAccountRepository AccountRepository,
            ITransactionRepository TransactionRepository,
            ILogger<SalaryProcessingCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            ITellerOperationRepository tellerOperationRepository = null,
            ITellerRepository tellerRepository = null,
            UserInfoToken userInfoToken = null,
            IConfigRepository configRepository = null,
            IMediator mediator = null,
            ITransferRepository transferRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            ISalaryProcessingRepository salaryExtractRepository = null,
            IFileUploadRepository fileUploadRepository = null,
            IPaymentReceiptRepository paymentReceiptRepository = null,
            IMapper mapper = null,
            ISavingProductFeeRepository savingProductFeeRepository = null)
        {
            _accountRepository = AccountRepository;
            _TransactionRepository = TransactionRepository;
            _logger = logger;
            _uow = uow;
            _tellerOperationRepository = tellerOperationRepository;
            _tellerRepository = tellerRepository;
            _userInfoToken = userInfoToken;
            _ConfigRepository = configRepository;
            _mediator = mediator;
            _transferRepository = transferRepository;
            _accountingDayRepository = accountingDayRepository;
            _salaryExtractRepository = salaryExtractRepository;
            _fileUploadRepository = fileUploadRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _mapper = mapper;
            _savingProductFeeRepository = savingProductFeeRepository;
        }

        /// <summary>
        /// Handles the SalaryAnalysisCommand to add a new Transfer Transaction.
        /// </summary>
        /// <param name="request">The SalaryAnalysisCommand containing Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>


        public async Task<ServiceResponse<SalaryProcessingDto>> Handle(SalaryProcessingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Get the current accounting date for the branch
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Step 2: Retrieve the uploaded salary file by its ID and check if salary processing has already been completed
                var fileUpload = await _fileUploadRepository.FindAsync(request.FileUploadId);
                if (fileUpload.SalaryProcessingStatus == Status.Completed.ToString())
                {
                    // Log the audit trail if the salary has already been processed and return a 403 Forbidden response
                    await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Create.ToString(), request, "Attempt to process salary file that has already been processed.", LogLevelInfo.Information.ToString(), 403, _userInfoToken.Token);
                    return ServiceResponse<SalaryProcessingDto>.Return403("The salary has already been processed.");
                }

                // Step 3: Retrieve the NoneCashTeller to handle non-cash transactions
                var noneCashTeller = await _tellerRepository.GetTellerByOperationType("NoneCashTeller");

                // Step 4: Find the source account for salary disbursement by its account number and include product details
                var sourceAccount = await _accountRepository
                    .FindBy(x => x.AccountNumber == request.SourceAccountNumber)
                    .Include(x => x.Product)
                    .ThenInclude(x => x.TransferParameters)
                    .FirstOrDefaultAsync();

                // If the source account is not found, log and return a 404 Not Found response
                if (sourceAccount == null)
                {
                    await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Create.ToString(), request, "Source account not found for salary processing.", LogLevelInfo.Warning.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<SalaryProcessingDto>.Return404("Source account not found.");
                }

                // Step 5: Retrieve all non-deleted salary extracts for the file
                var salaryExtracts = await _salaryExtractRepository.FindBy(x => x.FileUploadId == request.FileUploadId && !x.IsDeleted).ToListAsync();

                // Step 6: Calculate the total salary to be processed from the salary extracts
                decimal totalSalaryToProcess = salaryExtracts.Sum(x => x.NetSalary);

                // Step 7: Retrieve the transfer parameters for the source account's product
                var transferParameter = sourceAccount.Product.TransferParameters.FirstOrDefault(x => x.TransferType == TransferType.Local.ToString());
                if (transferParameter == null)
                {
                    var errorMessage = $"Transfer parameters are not configured for the product {sourceAccount.Product.Name}. Contact the administrator to configure this.";
                    await LogErrorAndAudit(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 403, _userInfoToken.Token);
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 8: Retrieve customer information for the sending (source) account
                var sendingCustomerInformation = await GetCustomer(sourceAccount.CustomerId);

                // Step 9: Calculate transfer charges based on the total salary and transfer parameters
                //var transferCharges = await CalculateTransferCharges(totalSalaryToProcess, transferParameter, TransferType.Local.ToString());

                // Step 10: Generate a transaction reference using the CurrencyNotesMapper (this ensures each transaction has a unique identifier for auditing)
                string Reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, OperationType.Transfer.ToString(), false);

                // Step 11: Ensure the sender has enough funds to cover the salary processing and transfer charges
                if (sourceAccount.Balance<totalSalaryToProcess)
                {
                    var errorMessage = $"{sendingCustomerInformation.FirstName} {sendingCustomerInformation.LastName}, insufficient balance in the source account for salary processing.";
                    await LogErrorAndAudit(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 403, _userInfoToken.Token);
                    throw new InvalidOperationException(errorMessage);
                }

                // Step 12: Calculate the total amount to transfer and update the source account balance
                decimal previousBalance = sourceAccount.Balance;
                _accountRepository.DebitAccount(sourceAccount, totalSalaryToProcess);

                // Step 13: Create a transaction record for the debited source account
                var transaction = await CreateSenderTransactionEntity(totalSalaryToProcess, sourceAccount, 0, _userInfoToken.BranchID, _userInfoToken.BranchID, false, noneCashTeller, Reference, accountingDate);

                // Step 14: Create a transfer record for the salary disbursement
                var transfer = await CreateTransfer(totalSalaryToProcess, transferParameter, sourceAccount, transaction, accountingDate);

                // Log the successful debit of the source account
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Create.ToString(), request, $"Source account {sourceAccount.AccountNumber} successfully debited for salary processing.", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 15: Initialize a DTO to store the results of the salary processing operation
                var salaryProcessingDto = new SalaryProcessingDto
                {
                    TotalDeposit = 0,
                    TotalSaving = 0,
                    TotalLoanCapital = 0,
                    TotalInterest = 0,
                    TotalVat = 0,
                    TotalShares = 0,
                    NumberOfDeposit = 0,
                    NumberOfSaving = 0,
                    NumberOfShare = 0,
                    NumberOfLoanRepayment = 0,
                    NetSalryUploaded = totalSalaryToProcess
                };

                // Step 16: Process each salary extract by crediting the corresponding member's accounts
                foreach (var salaryExtract in salaryExtracts)
                {
                    decimal remainingSalary = salaryExtract.NetSalary;

                    // Credit member's savings account and update the DTO
                    remainingSalary = await ProcessAccountTransaction(salaryExtract, remainingSalary, AccountType.Saving.ToString(), salaryProcessingDto, accountingDate, sourceAccount, transaction);

                    // Credit member's deposit account and update the DTO
                    remainingSalary = await ProcessAccountTransaction(salaryExtract, remainingSalary, AccountType.Deposit.ToString(), salaryProcessingDto, accountingDate, sourceAccount, transaction);

                    // Credit member's share account and update the DTO
                    remainingSalary = await ProcessAccountTransaction(salaryExtract, remainingSalary, AccountType.MemberShare.ToString(), salaryProcessingDto, accountingDate, sourceAccount, transaction);

                    // Repay any outstanding loans and update the DTO
                    remainingSalary = await ProcessLoanRepayment(salaryExtract, remainingSalary, salaryProcessingDto, accountingDate, sourceAccount, transaction);

                    // Log the successful processing of the member's salary extract
                    await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Create.ToString(), request, $"Processed salary for member {salaryExtract.MembersName} with reference {Reference}.", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // If there is any remaining salary, additional logic to handle it can be added here
                    if (remainingSalary != 0)
                    {
                        // (Optional) handle remaining salary
                    }

                    // Mark the salary extract as processed and set its accounting date
                    salaryExtract.Status = true;
                    salaryExtract.AccountingDate = accountingDate;
                    _salaryExtractRepository.Update(salaryExtract);
                }

                // Step 17: Update the file status to 'Completed' and save all changes to the database
                fileUpload.SalaryProcessingStatus = Status.Completed.ToString();
                _fileUploadRepository.Update(fileUpload);
                await _uow.SaveAsync();

                // Log the successful completion of the salary processing operation
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Create.ToString(), request, "Salary processing completed successfully.", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Step 18: Return the result of the salary processing operation
                return ServiceResponse<SalaryProcessingDto>.ReturnResultWith200(salaryProcessingDto);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the salary processing operation
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Create.ToString(), request, "Error during salary processing: " + ex.Message, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<SalaryProcessingDto>.Return500("An error occurred during salary processing: " + ex.Message);
            }
        }
        /// <summary>
        /// Checks if the sender's account has enough funds for a transfer based on the provided amount, charges, and member type.
        /// </summary>
        /// <param name="account">The sender's account to check funds for.</param>
        /// <param name="amount">The transfer amount.</param>
        /// <param name="charges">The transfer charges.</param>
        /// <param name="memberType">The type of member (physical or moral person).</param>
        /// <returns>True if the sender's account has enough funds for the transfer; otherwise, false.</returns>
        private bool CheckIfSenderHasEnoughFundForTransfer(Account account, decimal amount, decimal charges, string memberType)
        {
            // Check if the member type is a physical person
            if (memberType == MemberType.Physical_Person.ToString())
            {
                // Calculate the minimum required balance for a physical person account
                decimal minimumBalance = account.Product.MinimumAccountBalancePhysicalPerson;
                // Check if the account has enough funds considering the transfer amount, charges, and blocked amount
                return account.Balance - (amount + charges + account.BlockedAmount) >= minimumBalance;
            }
            else
            {
                // Calculate the minimum required balance for a moral person account
                decimal minimumBalance = account.Product.MinimumAccountBalanceMoralPerson;
                // Check if the account has enough funds considering the transfer amount, charges, and blocked amount
                return account.Balance - (amount + charges + account.BlockedAmount) >= minimumBalance;
            }
        }
        private async Task<Transfer> CreateTransfer(decimal Amount, TransferParameter transferLimit, Account senderAccount, Transaction senderTransaction, DateTime accountingDate)
        {
            var transfer = new Transfer();
            transfer.DateOfInitiation = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            transfer.Id = BaseUtilities.GenerateUniqueNumber();
            transfer.TransactionRef = senderTransaction.TransactionReference;
            transfer.TransactionType = transferLimit.TransferType;
            transfer.AccountId = "S";
            transfer.SourceAccountNumber = senderAccount.AccountNumber;
            transfer.DestinationAccountNumber = "S";
            transfer.SourceAccountType = senderAccount.AccountType;
            transfer.DestinationAccountType = "S";
            transfer.Status = Status.Pending.ToString();
            transfer.InitiatorComment = $"Order to {TransactionType.TRANSFER.ToString()} the sum of {BaseUtilities.FormatCurrency(Amount)} from my account number {senderAccount.AccountNumber} to [Salary earners].";
            transfer.BranchId = "S";
            transfer.Charges = senderTransaction.Fee;
            transfer.Tax = senderTransaction.Tax;
            transfer.AccountingDate = accountingDate;
            transfer.Amount = Amount;
            transfer.InitiatedByUSerName = _userInfoToken.FullName;
            transfer.TellerId = senderTransaction.TellerId;
            transfer.SourceType = senderTransaction.SourceType;
            transfer.IsInterBranchOperation = senderTransaction.IsInterBrachOperation;
            transfer.SourceBrachId = senderTransaction.BranchId;
            transfer.DestinationBrachId = senderTransaction.DestinationBrachId;
            transfer.DestinationCommision = senderTransaction.DestinationBranchCommission;
            transfer.SourceCommision = senderTransaction.SourceBranchCommission;
            _transferRepository.Add(transfer);
            return transfer;
        }
        // This method processes individual account transactions such as savings, deposit, and shares for each salary extract.
        private async Task<decimal> ProcessAccountTransaction(SalaryExtract salaryExtract, decimal remainingSalary, string accountType, SalaryProcessingDto dto, DateTime accountingDate, Account sourceAccount, Transaction senderTransaction)
        {
            // Step 1: Find the account by customer ID (member reference) and account type (e.g., Saving, Deposit, Shares)
            var account = await _accountRepository.FindBy(x => x.CustomerId == salaryExtract.MemberReference && x.AccountType == accountType).FirstOrDefaultAsync();
            // Step 2: If no account is found or there is no remaining salary, exit the process and return the remaining salary
            if (account == null || remainingSalary <= 0) return remainingSalary;

            // Step 3: Determine the amount to process for the specified account type.
            // Use the minimum of the allocated salary portion (e.g., Saving, Deposit, Shares) and the remaining salary.
            decimal amountToProcess = accountType switch
            {
                "Saving" => Math.Min(salaryExtract.Saving, remainingSalary),   // Process savings portion
                "Deposit" => Math.Min(salaryExtract.Deposit, remainingSalary), // Process deposit portion
                "Shares" => Math.Min(salaryExtract.Shares, remainingSalary),   // Process shares portion
                _ => 0  // Default case: if none of the account types match, the amount to process is 0
            };

            // Step 4: Credit the account with the calculated amount using the repository method
            // The repository's CreditAccount method abstracts the logic for updating the account balance.
            _accountRepository.CreditAccount(account, amountToProcess);

            // Step 5: Reduce the remaining salary by the amount processed
            remainingSalary -= amountToProcess;

            // Step 6: Generate a transaction reference using the CurrencyNotesMapper (this ensures each transaction has a unique identifier for auditing)
            string Reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, OperationType.Deposit.ToString(), false);

            // Step 7: Create a receiver transaction entity that records the salary credit
            // This will log the transaction for the account and source account
            var transaction = await CreateReceiverTransactionEntity(account, "Credit", $"Salary - {accountType}", sourceAccount.Id, sourceAccount.AccountNumber, senderTransaction, Reference, amountToProcess);

            // Step 8: Create a transaction DTO list and map the transaction entity to a DTO object using AutoMapper
            var transactions = new List<TransactionDto>();
            transactions.Add(_mapper.Map<TransactionDto>(transaction));

            // Step 9: (Optional) If more transaction handling or operations need to be added, they can go here.

            // Step 10: Update the SalaryProcessingDto counters based on the account type and the amount processed
            UpdateDtoCounters(dto, accountType, amountToProcess);

            // Step 11: Process the payment receipt by passing the transaction details and salary extract
            await ProcessPaymentReceipt(transactions, amountToProcess, accountingDate, accountType, salaryExtract, 0, 0);

            // Step 12: Return the remaining salary after processing the account transaction, allowing for further processing if necessary
            return remainingSalary;
        }

        // Extracted function for loan repayment
        private async Task<decimal> ProcessLoanRepayment(SalaryExtract salaryExtract, decimal remainingSalary, SalaryProcessingDto dto, DateTime accountingDate, Account sourceAccount, Transaction senderTransaction)
        {
            // Step 1: Find the loan account for the member (if no loan account or no remaining salary, exit early)
            var loanAccount = await _accountRepository.FindBy(x => x.CustomerId == salaryExtract.MemberReference && x.AccountType == AccountType.Loan.ToString()).FirstOrDefaultAsync();
            if (loanAccount == null || remainingSalary <= 0) return remainingSalary;

            // Step 2: Calculate the total loan repayment amount (principal + interest)
            string Reference = string.Empty;
            decimal totalLoanRepayment = salaryExtract.LoanPrincipal + salaryExtract.LoanInterest;
            decimal amountToRepay = Math.Min(totalLoanRepayment, remainingSalary);  // Repay only up to the remaining salary

            if (amountToRepay > 0)
            {
                // Step 3: Split the amount to repay between principal and interest
                decimal principalRepayment = salaryExtract.LoanPrincipal;
                decimal interestRepayment = salaryExtract.LoanInterest;  // Remaining part goes to interest

                // Update the loan account with the interest repayment
                loanAccount.LastInterestPosted += interestRepayment;

                // Reduce the remaining salary by the total amount being repaid
                remainingSalary -= amountToRepay;

                // Step 4: Log the transaction (Debit the loan account)
                var transaction = await CreateReceiverTransactionEntity(loanAccount, "Debit", $"Salary-Loan Repayment", sourceAccount.Id, sourceAccount.AccountNumber, senderTransaction, Reference, salaryExtract.);

                // Step 5: Map transaction to DTO and log it
                var transactions = new List<TransactionDto>();
                transactions.Add(_mapper.Map<TransactionDto>(transaction));

                // Step 6: Update the SalaryProcessingDto counters with the repayment information
                dto.TotalLoanCapital += principalRepayment;  // Add to total capital repaid
                dto.TotalInterest += interestRepayment;      // Add to total interest repaid
                dto.NumberOfLoanRepayment++;                 // Increment the loan repayment count

                // Step 7: Retrieve loan information and calculate VAT (if applicable)
                var loan = await GetLoanAsync(salaryExtract.MemberReference);
                decimal VAT = CalculateVAT(salaryExtract.LoanInterest, loan.LoanAmount);

                // Make the loan repayment (record the VAT and refund if applicable)
                var refund = await MakeLoanRepayment(amountToRepay, salaryExtract, Reference, "Salary-Loan repayment", VAT);

                // Step 8: Process the payment receipt for the loan repayment
                await ProcessPaymentReceipt(transactions, amountToRepay, accountingDate, TransactionType.CASHIN_LOAN_REPAYMENT.ToString(), salaryExtract, VAT, refund.Balance);
            }

            // Step 9: Return the remaining salary after the loan repayment processing
            return remainingSalary;
        }

        // Update counters in DTO based on account type
        private void UpdateDtoCounters(SalaryProcessingDto dto, string accountType, decimal amount)
        {
            switch (accountType)
            {
                case "Saving":
                    // Increment the total saving amount and the number of saving operations
                    dto.TotalSaving += amount;
                    dto.NumberOfSaving++;
                    break;

                case "Deposit":
                    // Increment the total deposit amount and the number of deposit operations
                    dto.TotalDeposit += amount;
                    dto.NumberOfDeposit++;
                    break;

                case "Shares":
                    // Increment the total shares amount and the number of share transactions
                    dto.TotalShares += amount;
                    dto.NumberOfShare++;
                    break;

                // Optional: Consider adding a default case to handle unexpected account types
                default:
                    // Log or handle unexpected account types if necessary
                    break;
            }
        }

        // Calculate VAT based on loan amount
        private decimal CalculateVAT(decimal loanInterest, decimal loanAmount)
        {
            // If the loan amount is greater than or equal to 2,000,000, apply a 19.25% VAT on the loan interest.
            // Otherwise, no VAT is charged.
            return loanAmount >= 2000000 ? Math.Round(VatRate * loanInterest / 100, 2) : 0;
        }


        // Handle payment receipt processing
        private async Task ProcessPaymentReceipt(List<TransactionDto> transactions, decimal amount, DateTime accountingDate, string serviceType, SalaryExtract salaryExtract, decimal VAT, decimal LoanBalance)
        {
            var accountDeposits = new List<AccountDeposit>();
            var paymentReciepts = new List<PaymentDetailObject>();

            // If it's not a loan repayment, handle the generic account processing.
            if (serviceType != TransactionType.CASHIN_LOAN_REPAYMENT.ToString())
            {
                foreach (var transaction in transactions)
                {
                    // Add payment details for each transaction
                    paymentReciepts.Add(new PaymentDetailObject
                    {
                        AccountNumber = transaction.AccountNumber,
                        Fee = transaction.Fee,
                        Amount = transaction.Amount,
                        Interest = 0,
                        LoanCapital = 0,
                        SericeOrEventName = transaction.Account.AccountName,
                        VAT = 0,
                        Balance = 0
                    });
                    accountDeposits.Add(new AccountDeposit
                    {
                        AccountName = transaction.Account.AccountName,
                        Amount = transaction.Amount,
                        Charge = transaction.Fee
                    });
                }

                // Create the payment processing request for savings, deposits, or shares
                var paymentProcessingRequest = new PaymentProcessingRequest
                {
                    AccountingDay = accountingDate,
                    Amount = amount,
                    MemberName = salaryExtract.MembersName,
                    NotesRequest = new CurrencyNotesRequest(),
                    OperationType = OperationType.NoneCash.ToString(),
                    OperationTypeGrouping = TransactionType.CASH_IN.ToString(),
                    PortalUsed = OperationSourceType.Web_Portal.ToString(),
                    PaymentDetails = paymentReciepts,
                    ServiceType = TransactionType.CASH_IN.ToString(),
                    SourceOfRequest = OperationSourceType.BackOffice_Operation.ToString(),
                    TotalAmount = amount,
                    TotalCharges = transactions.Sum(x => x.Fee),
                    Transactions = transactions
                };

                // Process the payment
                _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessingRequest);
            }
            else // Handle loan repayment transactions
            {
                foreach (var transaction in transactions)
                {
                    // Add loan repayment details for each transaction
                    paymentReciepts.Add(new PaymentDetailObject
                    {
                        AccountNumber = transaction.AccountNumber,
                        Fee = transaction.Fee,
                        Amount = transaction.Amount,
                        Interest = salaryExtract.LoanInterest,
                        LoanCapital = salaryExtract.LoanPrincipal,
                        SericeOrEventName = transaction.Account.AccountName,
                        VAT = VAT,
                        Balance = LoanBalance
                    });
                    accountDeposits.Add(new AccountDeposit
                    {
                        AccountName = transaction.Account.AccountName,
                        Amount = transaction.Amount,
                        Charge = transaction.Fee
                    });
                }

                // Create the payment processing request for loan repayments
                var paymentProcessingRequest = new PaymentProcessingRequest
                {
                    AccountingDay = accountingDate,
                    Amount = amount,
                    MemberName = salaryExtract.MembersName,
                    NotesRequest = new CurrencyNotesRequest(),
                    OperationType = OperationType.NoneCash.ToString(),
                    OperationTypeGrouping = TransactionType.CASH_IN.ToString(),
                    PortalUsed = OperationSourceType.Web_Portal.ToString(),
                    PaymentDetails = paymentReciepts,
                    ServiceType = TransactionType.CASHIN_LOAN_REPAYMENT.ToString(),
                    SourceOfRequest = OperationSourceType.BackOffice_Operation.ToString(),
                    TotalAmount = amount,
                    TotalCharges = transactions.Sum(x => x.Fee),
                    Transactions = transactions
                };

                // Process the payment
                _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessingRequest);
            }
        }
        private async Task<Loan> GetLoanAsync(string customerId)
        {
            // Prepare the command for retrieving loan information
            var loanQueryCommand = new GetCustomerLoan { CustomerId = customerId };

            // Send the command via the _mediator
            var loanQueryResponse = await _mediator.Send(loanQueryCommand);

            // Check for null or error in the response
            if (loanQueryResponse == null || !loanQueryResponse.Success)
            {
                // Log an error and throw an exception with details about the failure
                var errorMessage = loanQueryResponse == null
                    ? "Loan API Failed: No response from the server."
                    : $"Loan API Failed: {loanQueryResponse.Message}";

                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage); // Throw exception with meaningful error message
            }

            // If no loans exist for the customer
            if (loanQueryResponse.Data == null || !loanQueryResponse.Data.Any())
            {
                var noLoanMessage = $"No loans found for customer ID: {customerId}";
                _logger.LogWarning(noLoanMessage);
                throw new InvalidOperationException(noLoanMessage);
            }

            // Retrieve the oldest loan based on the LoanDate
            var oldestLoan = loanQueryResponse.Data.OrderBy(loan => loan.LoanDate).FirstOrDefault();

            // Return the oldest loan
            return oldestLoan;
        }

        // Method to retrieve customer information
        private async Task<CustomerDto> GetCustomer(string customerId)
        {
            var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = customerId }; // Create command to get customer.
            var customerResponse = await _mediator.Send(customerCommandQuery); // Send command to _mediator.

            // Check if customer information retrieval was successful
            if (customerResponse.StatusCode != 200)
            {
                var errorMessage = "Failed getting member's information";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            var customer = customerResponse.Data; // Get customer data from response.


            return customer; // Return customer data.
        }
        private async Task<RefundDto> MakeLoanRepayment(decimal amount, SalaryExtract salaryExtract, string transactionCode, string note, decimal VAT)
        {
            // Prepare the command for adding a loan repayment
            var repaymentCommand = new AddLoanRepaymentCommandDetails
            {
                Amount = amount,
                Comment = string.IsNullOrEmpty(note) ? "Loan Repayment" : note,
                LoanId = salaryExtract.LoanId,
                Interest = salaryExtract.LoanInterest,
                PaymentChannel = "Web_Portal",
                Penalty = 0,
                Principal = salaryExtract.LoanPrincipal,
                Tax = VAT,
                PaymentMethod = "Salary",
                TransactionCode = transactionCode
            };

            // Send the command via the _mediator
            var repaymentResponse = await _mediator.Send(repaymentCommand);
            // Check for null or error in the response
            if (repaymentResponse == null || !repaymentResponse.Success)
            {
                // Log the error and throw an exception with details
                var errorMessage = repaymentResponse == null
                    ? "Loan API Failed: No response from the server."
                    : $"Loan API Failed: {repaymentResponse.Message}";

                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            // Return the refund data if the operation was successful
            return repaymentResponse.Data;
        }
        /// <summary>
        /// Calculates the transfer charges for a given amount based on the transfer parameters and fee configuration.
        /// </summary>
        /// <param name="amount">The amount for which the transfer charges need to be calculated.</param>
        /// <param name="transferParameter">The transfer parameters containing the product and fee information.</param>
        /// <param name="transferType">The type of transfer (e.g., local transfer or inter-brnach transfer).</param>
        /// <returns>A TransferCharges object containing the calculated service charge and total charges.</returns>
        private async Task<TransferCharges> CalculateTransferCharges(decimal amount, TransferParameter transferParameter, string transferType)
        {
            // Initialize TransferCharges object to store calculated charges
            TransferCharges transferCharges = new TransferCharges();
            // Retrieve transfer fees based on the transfer parameters
            var fees = await _savingProductFeeRepository.FindBy(x => x.SavingProductId == transferParameter.ProductId && x.FeePolicyType == transferType && x.FeeType.ToLower() == "transfer" && !x.IsDeleted).Include(x => x.Fee).Include(x => x.Fee.FeePolicies).ToListAsync();
            // Check if transfer fees are configured for the product
            if (!fees.Any())
            {
                var errorMessage = $"Transfer charges are not configured for {transferParameter.Product.Name}. Contact your administrator to configure charges on {transferParameter.Product.Name}.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Extract fee types from the retrieved fees
            var rate = fees.FirstOrDefault(x => x.Fee.FeeType.ToLower() == "rate");
            var range = fees.FirstOrDefault(x => x.Fee.FeeType.ToLower() == "range");
            // Calculate service charge based on withdrawal fee rate
            decimal percentageChargedAmount = rate?.Fee?.FeePolicies?.FirstOrDefault()?.Value ?? 0;

            if (rate != null && rate.Fee != null && rate.Fee.FeePolicies != null)
            {
                percentageChargedAmount = XAFWallet.ConvertPercentageToCharge(rate.Fee.FeePolicies.FirstOrDefault().Value, amount);
            }
            else
            {
                // Handle the case where rate is null or any of its properties are null
                // For example:
                percentageChargedAmount = 0; // Assign a default value or handle accordingly
            }
            if (percentageChargedAmount > 0)
            {
                // Apply percentage-based charge if applicable
                transferCharges.ServiceCharge = percentageChargedAmount;
            }
            else
            {
                // Calculate range-based charge if applicable
                decimal rangeChargeCalculated = CurrencyNotesMapper.GetFeePolicyRange(range.Fee.FeePolicies.ToList(), amount).Charge;
                transferCharges.ServiceCharge = rangeChargeCalculated;
            }
            // Calculate total charges including additional fees
            var all_charges = XAFWallet.CalculateCustomerWithdrawalCharges(transferCharges.ServiceCharge, 0);
            transferCharges.TotalCharges = all_charges;
            return transferCharges;
        }

        private async Task SendSenderSMS(Account senderAccount, decimal sourceAmount, string senderName, string senderTransRef, decimal senderFee, string senderPhone)
        {
            // Prepare sender SMS message
            string senderMsg = $"Hello {senderName}, your salary transfer of {BaseUtilities.FormatCurrency(sourceAmount)} was successful. " +
                               $"Ref: {senderTransRef}. Date: {BaseUtilities.UtcNowToDoualaTime():dd/MM/yyyy hh:mm tt}. " +
                               $"Fee: {BaseUtilities.FormatCurrency(senderFee)}. New Balance: {BaseUtilities.FormatCurrency(senderAccount.Balance)}. " +
                               $"Thank you for banking with us.";

            // Create and send SMS command for the sender
            var senderSmsCommand = new SendSMSPICallCommand
            {
                messageBody = senderMsg,
                recipient = senderPhone
            };
            await _mediator.Send(senderSmsCommand);
        }

        private async Task SendReceiverSMS(Account receiverAccount, Account senderAccount, decimal amountToReceive, string receiverName, string senderName, string receiverTransRef, string receiverPhone)
        {
            // Prepare receiver SMS message
            string receiverMsg = $"Hello {receiverName}, you have received a salary alert of {BaseUtilities.FormatCurrency(amountToReceive)} in your account " +
                                 $"{BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)} from {senderName}. " +
                                 $"Ref: {receiverTransRef}. Date: {BaseUtilities.UtcNowToDoualaTime():dd/MM/yyyy hh:mm tt}. " +
                                 $"New Balance: {BaseUtilities.FormatCurrency(receiverAccount.Balance)}. Thank you for banking with us.";

            // Create and send SMS command for the receiver
            var receiverSmsCommand = new SendSMSPICallCommand
            {
                messageBody = receiverMsg,
                recipient = receiverPhone
            };
            await _mediator.Send(receiverSmsCommand);
        }




        private decimal TransferCharges(TransferParameter transferLimit, decimal amount)
        {
            return XAFWallet.CalculateCustomerCharges(
                transferLimit?.TransferFeeRate ?? 0,
                0, 0,
                amount);
        }

        private decimal TotalAmountToTransferWithCharges(TransferParameter transferLimit, decimal amount)
        {
            return amount + TransferCharges(transferLimit, amount);
        }

        // Helper method to create sender transaction entity
        private async Task<Transaction> CreateSenderTransactionEntity(decimal Amount, Account senderAccount, decimal Charges, string sourceBranchId, string destinationBranchId, bool isInternalTransfer, Teller noneCashTeller, string Reference, DateTime accountingDate)
        {
            var senderTransactionEntity = new Transaction();
            senderTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            senderTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            senderTransactionEntity.TransactionReference = Reference;
            senderTransactionEntity.TransactionType = OperationType.Debit.ToString();
            senderTransactionEntity.Operation = TransactionType.TRANSFER.ToString();
            senderTransactionEntity.PreviousBalance = senderAccount.Balance;
            senderTransactionEntity.Balance = senderAccount.Balance;
            senderTransactionEntity.AccountId = senderAccount.Id;
            senderTransactionEntity.AccountNumber = senderAccount.AccountNumber;
            senderTransactionEntity.SenderAccountId = senderAccount.Id;
            senderTransactionEntity.ReceiverAccountId = "N/A";
            senderTransactionEntity.Status = "COMPLETED";
            senderTransactionEntity.ProductId = senderAccount.ProductId;
            senderTransactionEntity.BankId = senderAccount.BankId;
            senderTransactionEntity.BranchId = senderAccount.BranchId;
            senderTransactionEntity.OriginalDepositAmount = Amount;
            senderTransactionEntity.Fee = Charges;
            senderTransactionEntity.Tax = 0;
            senderTransactionEntity.AccountingDate = accountingDate;
            senderTransactionEntity.Credit = 0;
            senderTransactionEntity.Debit = Amount + Charges;
            senderTransactionEntity.Amount = -(Amount + Charges);
            senderTransactionEntity.CustomerId = senderAccount.CustomerId;
            senderTransactionEntity.ReceiptTitle = "Cash Transfer Receipt Reference: " + Reference;
            senderTransactionEntity.OperationType = OperationType.Debit.ToString();
            senderTransactionEntity.FeeType = Events.ChargeOfTransfer.ToString();
            senderTransactionEntity.TellerId = noneCashTeller.Id;
            senderTransactionEntity.SourceType = "BackOffice";
            senderTransactionEntity.SourceBrachId = sourceBranchId;
            senderTransactionEntity.IsInterBrachOperation = isInternalTransfer;
            senderTransactionEntity.DestinationBrachId = destinationBranchId;
            senderTransactionEntity.DestinationBranchCommission =0;
            senderTransactionEntity.SourceBranchCommission = Charges;
            senderTransactionEntity.Note ??= $"Account number {senderAccount.AccountNumber} made a transfer of {BaseUtilities.FormatCurrency(Amount + Charges)} to her salaries earners. Amount: {BaseUtilities.FormatCurrency(Amount)}, Reference: {Reference}";
            _TransactionRepository.Add(senderTransactionEntity);
            return senderTransactionEntity;
        }


        // Helper method to create receiver transaction entity
        private async Task<Transaction> CreateReceiverTransactionEntity(Account receiverAccount, string transactionType, string Note, string senderAccountId, string senderAccountNumber, Transaction senderTransaction, string Reference, decimal Amount)
        {
            var receiverTransactionEntity = new Transaction();
            // Convert UTC to local time and set it in the entity
            receiverTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            receiverTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            receiverTransactionEntity.TransactionReference = senderTransaction.TransactionReference;
            receiverTransactionEntity.TransactionType = transactionType;
            receiverTransactionEntity.Operation = TransactionType.TRANSFER.ToString();
            receiverTransactionEntity.PreviousBalance = receiverAccount.PreviousBalance;
            receiverTransactionEntity.Balance = receiverAccount.Balance;
            receiverTransactionEntity.AccountId = receiverAccount.Id;
            receiverTransactionEntity.SenderAccountId = senderAccountId;
            receiverTransactionEntity.AccountNumber = receiverAccount.AccountNumber;
            receiverTransactionEntity.ReceiverAccountId = receiverAccount.Id;
            receiverTransactionEntity.ProductId = receiverAccount.ProductId;
            receiverTransactionEntity.Status = "COMPLETED";
            receiverTransactionEntity.Note ??= $"Note: [{Note}], Account number {receiverAccount.AccountNumber} recieves a transfer of {BaseUtilities.FormatCurrency(Amount)} from account number {senderAccountNumber}, Reference: {Reference}";
            receiverTransactionEntity.BankId = receiverAccount.BankId;
            receiverTransactionEntity.BranchId = receiverAccount.BranchId;
            receiverTransactionEntity.OriginalDepositAmount = Amount;
            receiverTransactionEntity.Fee = 0;
            receiverTransactionEntity.Tax = 0;
            receiverTransactionEntity.AccountingDate = senderTransaction.AccountingDate;
            receiverTransactionEntity.Amount = Amount;
            receiverTransactionEntity.CustomerId = receiverAccount.CustomerId;
            receiverTransactionEntity.OperationType = OperationType.Credit.ToString();
            receiverTransactionEntity.FeeType = Events.None.ToString();
            receiverTransactionEntity.TellerId = senderTransaction.TellerId;
            receiverTransactionEntity.Debit = transactionType == "Debit" ? Amount : 0;
            receiverTransactionEntity.Credit = transactionType == "Credit" ? Amount : 0; ;
            receiverTransactionEntity.SourceType = senderTransaction.SourceType;
            receiverTransactionEntity.IsInterBrachOperation = senderTransaction.IsInterBrachOperation;
            receiverTransactionEntity.SourceBrachId = senderTransaction.BranchId;
            receiverTransactionEntity.DestinationBrachId = senderTransaction.DestinationBrachId;
            receiverTransactionEntity.DestinationBranchCommission = senderTransaction.DestinationBranchCommission;
            receiverTransactionEntity.SourceBranchCommission = senderTransaction.SourceBranchCommission;
            receiverTransactionEntity.ReceiptTitle = senderTransaction.ReceiptTitle;
            _TransactionRepository.Add(receiverTransactionEntity);
            return receiverTransactionEntity;
        }


        // Helper method to log error and audit
        private async Task LogErrorAndAudit(string userEmail, string logAction, SalaryProcessingCommand request, string errorMessage, string logLevel, int statusCode, string userToken)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(userEmail, logAction, request, errorMessage, logLevel, statusCode, userToken);
        }

    }
}