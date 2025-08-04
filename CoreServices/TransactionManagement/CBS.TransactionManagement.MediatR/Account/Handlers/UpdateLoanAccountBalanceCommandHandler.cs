using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace CBS.AccountManagement.Handlers
{
    /// <summary>
    /// Handles the command to update an Account based on UpdateAccountCommand.
    /// </summary>
    public class UpdateAccountCommandHandler : IRequestHandler<UpdateLoanAccountBalanceCommand, ServiceResponse<bool>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly ILogger<UpdateAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ITellerRepository _tellerRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMediator _mediator;
        private readonly ISavingProductRepository _savingProductFeeRepository;
        /// <summary>
        /// Constructor for initializing the UpdateAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAccountCommandHandler(
            IAccountRepository AccountRepository,
            ILogger<UpdateAccountCommandHandler> logger,
            UserInfoToken userInfoToken,
            IUnitOfWork<TransactionContext> uow = null,
            ITellerRepository tellerRepository = null,
            ITransactionRepository transactionRepository = null,
            IMediator mediator = null,
            ISavingProductRepository savingProductFeeRepository = null)
        {
            _AccountRepository = AccountRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _tellerRepository = tellerRepository;
            _transactionRepository = transactionRepository;
            _mediator = mediator;
            _savingProductFeeRepository = savingProductFeeRepository;
        }

        /// <summary>
        /// Handles the UpdateAccountCommand to update an Account.
        /// </summary>
        /// <param name="request">The UpdateAccountCommand containing updated Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(UpdateLoanAccountBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the Account entity to be updated from the repository
                var loanAccount = await _AccountRepository.FindBy(a => a.CustomerId == request.CustomerId && a.AccountType.ToLower() == "loan")
                          .FirstOrDefaultAsync();
                if (loanAccount == null)
                {
                    var sp = await _savingProductFeeRepository.FindBy(x => x.AccountType.ToLower() == "loan").FirstOrDefaultAsync();
                    var acccount = new AddAccountCommand
                    {
                        BankId = _userInfoToken.BankID,
                        CustomerId = request.CustomerId,
                        BranchId = _userInfoToken.BranchID,
                        CustomerName = "N/A",
                        IsRemoveAccount = false,
                        ProductId = sp.Id,
                    };
                    var serviceResponse = await _mediator.Send(acccount);
                    if (serviceResponse.StatusCode==200)
                    {
                        loanAccount = await _AccountRepository.FindAsync(serviceResponse.Data.Id);
                        _logger.LogInformation($"New loan account created for customer {request.CustomerId}");
                    }
                    
                }
                // Generate transaction reference based on branch type
                string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.Loan_Accrual_Interest.ToString(), false);

                // Retrieve the teller entity based on the code "000"
                var teller = await _tellerRepository.FindBy(x => x.Code == "000").FirstOrDefaultAsync();

                // Step 2: Check if the Account entity exists
                if (loanAccount != null)
                {
                    // Step 3: Update Account entity properties with values from the request
                    loanAccount.PreviousBalance = loanAccount.Balance; // Save the current balance as the previous balance
                    loanAccount.Balance = request.Balance; // Update the balance to the new balance provided in the request
                    loanAccount.EncryptedBalance = BalanceEncryption.Encrypt(loanAccount.Balance.ToString(), loanAccount.AccountNumber); // Encrypt the updated balance
                    loanAccount.LastOperation = TransactionType.Loan_Accrual_Interest.ToString();
                    loanAccount.LastInterestCalculatedDate = BaseUtilities.UtcNowToDoualaTime();
                    loanAccount.DateOfLastOperation = BaseUtilities.UtcNowToDoualaTime();
                    loanAccount.LastOperationAmount = request.Interest;

                    // Step 4: Use the repository to update the existing Account entity
                    _AccountRepository.Update(loanAccount);

                    // Create a new transaction record
                    var trans = CreateTransaction(request.Interest, loanAccount, teller, reference, request.LoanId, request.ExternalReference);
                    _transactionRepository.Add(trans);

                    // Step 5: Save changes using Unit of Work
                    await _uow.SaveAsync();

                    // Step 6: Log success message
                    string msg = $"Loan account balance for customer {request.CustomerId} updated successfully. Newly added Accrual Interest: {request.Interest}, New balance {request.Balance}. Existing balance: {loanAccount.PreviousBalance}";
                    var logTask = Task.Run(() => _logger.LogInformation(msg));

                    // Step 7: Audit log the update action
                    var auditTask = APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    // Step 8: Await parallel tasks
                    await Task.WhenAll(logTask, auditTask);

                    // Step 9: Prepare the response and return a successful response with 200 Status code
                    var response = ServiceResponse<bool>.ReturnResultWith200(true, msg);
                    return response;
                }
                else
                {
                    // Step 10: If the Account entity was not found, log an error and return a 404 Not Found response with an error message
                    string errorMessage = $"Loan account was not found to be updated.";
                    var logTask = Task.Run(() => _logger.LogError(errorMessage));

                    // Step 11: Audit log the failed update attempt
                    var auditTask = APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    // Step 12: Await parallel tasks
                    await Task.WhenAll(logTask, auditTask);

                    return ServiceResponse<bool>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Step 13: Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Account: {e.Message}";
                var logTask = Task.Run(() => _logger.LogError(errorMessage));

                // Step 14: Audit log the error
                var auditTask = APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Step 15: Await parallel tasks
                await Task.WhenAll(logTask, auditTask);

                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        private Transaction CreateTransaction(decimal Amount, Account account, Teller teller, string Reference, string loanId, string externalReference)
        {

            // Calculate balance and credit based on original amount and fees, if IsOfflineCharge is true
            decimal balance = account.Balance;
            decimal credit = 0;
            decimal originalAmount = Amount;
            string N_A = "N/A";
            // Create the transaction entity
            var transactionEntityEntryFee = new Transaction
            {
                Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique transaction ID
                CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow), // Set transaction creation date
                Status = TransactionStatus.COMPLETED.ToString(), // Set transaction status to completed
                TransactionReference = Reference, // Set transaction reference
                ExternalReference = externalReference, // Set external reference
                IsExternalOperation = true, // Set external operation flag
                ExternalApplicationName = "LoanMicroService", // Set external application name
                SourceType = OperationSourceType.Job.ToString(), // Set source type
                Currency = Currency.XAF.ToString(), // Set currency
                TransactionType = TransactionType.Loan_Accrual_Interest.ToString(), // Set transaction type (deposit)
                AccountNumber = account.AccountNumber, // Set account number
                PreviousBalance = account.Balance, // Set previous balance
                AccountId = account.Id, // Set account ID
                CustomerId = account.CustomerId, // Set customer ID
                ProductId = account.ProductId, // Set product ID
                SenderAccountId = account.Id, // Set sender account ID
                ReceiverAccountId = account.Id, // Set receiver account ID
                BankId = account.BankId, // Set bank ID
                Operation = TransactionType.Loan_Accrual_Interest.ToString(), // Set operation type (deposit)
                BranchId = account.BranchId, // Set branch ID
                OriginalDepositAmount = originalAmount, // Set original deposit amount including fees
                Fee = 0, // Set fee (charges)
                Tax = 0, // Set tax (assuming tax is not applicable)
                Amount = Amount, // Set amount (excluding fees)
                Note = $"Statement: Account Number {account.AccountNumber} & Account Name: {account.AccountName} was debited with daily calculated Interest & VAT: {BaseUtilities.FormatCurrency(Amount)} on loan previouse balance: {BaseUtilities.FormatCurrency(account.PreviousBalance)}. New Balancë: {BaseUtilities.FormatCurrency(account.Balance)}, Reference: {Reference}", // Set transaction note
                OperationType = OperationType.Debit.ToString(), // Set operation type (credit)
                FeeType = Events.None.ToString(), // Set fee type
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
                Credit = 0, // Set credit amount (including fees if IsOfflineCharge is true, otherwise excluding fees)
                Debit = Amount, // Set debit amount (assuming no debit)
                DestinationBrachId = account.BranchId,
                OperationCharge = 0,
                ReceiptTitle = "Loan Daily Calculated Interest Receipt, Reference: " + Reference,
                WithrawalFormCharge = Amount,  // Set destination branch ID
                SourceBrachId = account.BranchId, // Set source branch ID
                IsInterBrachOperation = false, // Set flag indicating if inter-branch operation
                DestinationBranchCommission = 0, // Calculate destination branch commission
                SourceBranchCommission = 0, // Calculate source branch commission
            };

            return transactionEntityEntryFee; // Return the transaction entity
        }

    }
}
