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
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryFiles;
using CBS.TransactionManagement.Repository.FileUploadP;
using CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Commands;
using CBS.TransactionManagement.MediatR.UtilityServices;
using CBS.TransactionManagement.Common.Repository.Uow;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using DocumentFormat.OpenXml.Features;
using CBS.TransactionManagement.Data.Entity.MongoDBObjects;
using DocumentFormat.OpenXml.Bibliography;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.MediatR.Accounting.Command;
using CBS.TransactionManagement.Repository.OldLoanConfiguration;
using LoanRefundCollection = CBS.TransactionManagement.Data.Entity.MongoDBObjects.LoanRefundCollection;
using LoanRefundCollectionAlpha = CBS.TransactionManagement.Data.Entity.MongoDBObjects.LoanRefundCollectionAlpha;
using MongoDB.Driver;
using System.Transactions;
using Transaction = CBS.TransactionManagement.Data.Transaction;
using CBS.TransactionManagement.Commands;

namespace CBS.TransactionManagement.MediatR.SalaryManagement.SalaryUploadedModelP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transfer Transaction.
    /// </summary>
    public class ExecuteSalaryCommandHandler : IRequestHandler<ExecuteSalaryCommand, ServiceResponse<SalaryProcessingDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly ILogger<ExecuteSalaryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly IConfigRepository _ConfigRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly ISalaryExecutedRepository _salaryExtractRepository;
        private readonly IFileUploadRepository _fileUploadRepository;
        private readonly IPaymentReceiptRepository _paymentReceiptRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private const decimal VatRate = 19.25m;
        UserInfoToken _userInfoToken;
        private readonly ISavingProductFeeRepository _savingProductFeeRepository;
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository;
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        private readonly IOldLoanAccountingMapingRepository _oldLoanAccountingMapingRepository;
        private readonly ISavingProductRepository _savingProductRepository;

        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the ExecuteSalaryCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public ExecuteSalaryCommandHandler(
            IAccountRepository AccountRepository,
            ITransactionRepository TransactionRepository,
            ILogger<ExecuteSalaryCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            ITellerOperationRepository tellerOperationRepository = null,
            ITellerRepository tellerRepository = null,
            IConfigRepository configRepository = null,
            IMediator mediator = null,
            ITransferRepository transferRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            ISalaryExecutedRepository salaryExtractRepository = null,
            IFileUploadRepository fileUploadRepository = null,
            IPaymentReceiptRepository paymentReceiptRepository = null,
            IMapper mapper = null,
            ISavingProductFeeRepository savingProductFeeRepository = null,
            IAPIUtilityServicesRepository utilityServicesRepository = null,
            IMongoUnitOfWork mongoUnitOfWork = null,
            IOldLoanAccountingMapingRepository oldLoanAccountingMapingRepository = null,
            ISavingProductRepository savingProductRepository = null)
        {
            _accountRepository = AccountRepository;
            _TransactionRepository = TransactionRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken=new UserInfoToken();
            _tellerOperationRepository = tellerOperationRepository;
            _tellerRepository = tellerRepository;
            _ConfigRepository = configRepository;
            _mediator = mediator;
            _transferRepository = transferRepository;
            _accountingDayRepository = accountingDayRepository;
            _salaryExtractRepository = salaryExtractRepository;
            _fileUploadRepository = fileUploadRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _mapper = mapper;
            _savingProductFeeRepository = savingProductFeeRepository;
            _utilityServicesRepository=utilityServicesRepository;
            _mongoUnitOfWork=mongoUnitOfWork;
            _oldLoanAccountingMapingRepository=oldLoanAccountingMapingRepository;
            _savingProductRepository=savingProductRepository;
        }

        public async Task<ServiceResponse<SalaryProcessingDto>> Handle(ExecuteSalaryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _userInfoToken = request.UserInfoToken;

                // Step 1: Retrieve the current accounting day for the branch
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);
                _logger.LogInformation($"[INFO] Salary processing started for FileUploadId: {request.FileUploadId}");

                // Step 2: Retrieve and validate file upload
                var fileUpload = await _fileUploadRepository.FindAsync(request.FileUploadId);
                if (fileUpload == null)
                    return await LogAndReturnError($"[ERROR] File ID {request.FileUploadId} not found.", request, HttpStatusCodeEnum.NotFound);

                if (fileUpload.SalaryProcessingStatus == Status.Completed.ToString())
                    return await LogAndReturnError($"[WARNING] File '{fileUpload.FileName}' already processed.", request, HttpStatusCodeEnum.Forbidden, LogLevelInfo.Warning);

                // Step 3: Generate transaction reference
                string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, AccountType.Salary.ToString(), false);

                // Step 4: Retrieve salary extracts
                var salaryExtracts = await _salaryExtractRepository.FindBy(x => x.FileUploadId == request.FileUploadId && !x.IsDeleted).ToListAsync();
                if (!salaryExtracts.Any())
                    return await LogAndReturnError($"[ERROR] No salary extracts found for File ID '{request.FileUploadId}'.", request, HttpStatusCodeEnum.NotFound);

                // Step 5: Retrieve the Non-Cash Teller and validate account
                var nonCashTeller = await _tellerRepository.GetTellerByOperationType("NoneCash", _userInfoToken.BranchID);
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(nonCashTeller);

                // Step 6: Retrieve the source salary account
                var sourceSalaryAccount = await _accountRepository.FindBy(x => x.CustomerId == request.MemberReferenceNumber &&
                                                                              x.AccountType == AccountType.Salary.ToString() &&
                                                                              x.BranchId == _userInfoToken.BranchID)
                                                                  .Include(x => x.Product)
                                                                  .ThenInclude(x => x.TransferParameters)
                                                                  .FirstOrDefaultAsync();

                if (sourceSalaryAccount == null)
                    return await LogAndReturnError($"[ERROR] No salary account found for Member Reference '{request.MemberReferenceNumber}'.", request, HttpStatusCodeEnum.NotFound);

                // Step 7: Validate balance
                decimal totalSalaryToProcess = salaryExtracts.Sum(x => x.NetSalary);
                if (sourceSalaryAccount.Balance < totalSalaryToProcess)
                    return await LogAndReturnError($"[ERROR] Insufficient funds in salary account '{sourceSalaryAccount.AccountNumber}'.", request, HttpStatusCodeEnum.BadRequest);

                // Step 8: Validate transfer parameters
                var transferParameter = sourceSalaryAccount.Product.TransferParameters.FirstOrDefault(x => x.TransferType == TransferType.Local.ToString());
                if (transferParameter == null)
                    return await LogAndReturnError($"[ERROR] Missing transfer parameters for product '{sourceSalaryAccount.Product.Name}'.", request, HttpStatusCodeEnum.InternalServerError);

                // Step 9: Debit the source salary account
                _logger.LogInformation($"[INFO] Debiting account '{sourceSalaryAccount.AccountNumber}'.");
                _accountRepository.DebitAccount(sourceSalaryAccount, totalSalaryToProcess);

                var transaction = CreateSenderTransactionEntity(totalSalaryToProcess, sourceSalaryAccount, 0, nonCashTeller, reference, accountingDate, TransactionType.SALARY_TRANSFER);
                CreateTellerOperation(totalSalaryToProcess, OperationType.Debit, nonCashTeller, tellerAccount, transaction.TransactionReference, TransactionType.SALARY_TRANSFER, accountingDate, sourceSalaryAccount);

                // Step 10: Process salary payments
                foreach (var salaryExtract in salaryExtracts)
                {
                    reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, AccountType.Salary.ToString(), false);
                    var targetAccount = await _accountRepository.GetMemberAccount(salaryExtract.MemberReference, AccountType.Salary.ToString());

                    var (processedTransactions, destinationAccount, sourceMemberSalaryAccount) = await ProcessAccountTransactionNetSalary(
                        salaryExtract, salaryExtract.NetSalary, AccountType.Salary.ToString(), transaction, nonCashTeller, accountingDate, sourceSalaryAccount, tellerAccount, reference, transferParameter, targetAccount);

                    salaryExtract.Status = true;
                    salaryExtract.ExecutionDate = BaseUtilities.UtcNowToDoualaTime();
                    _salaryExtractRepository.Update(salaryExtract);

                    await ProcessPaymentReceipt(processedTransactions, salaryExtract);
                    await AddSalaryStandingOrderAccounting(salaryExtract, reference, targetAccount, destinationAccount, processedTransactions, sourceSalaryAccount.CustomerName);
                }

                // Step 11: Mark salary processing as complete
                fileUpload.SalaryProcessingStatus = Status.Completed.ToString();
                _fileUploadRepository.Update(fileUpload);

                await _uow.SaveAsync(_userInfoToken); // Save everything atomically

                // Step 12: Call external services
                _utilityServicesRepository.SalaryStandingOrderLoanRepayment(request.FileUploadId, _userInfoToken);
                _utilityServicesRepository.SalaryAccountToAccountTransfer(request.FileUploadId, accountingDate, _userInfoToken);

                // Commit transaction
                //scope.Complete();

                _logger.LogInformation($"[SUCCESS] Salary processing completed for file '{fileUpload.FileName}'.");
                return ServiceResponse<SalaryProcessingDto>.ReturnResultWith200(new SalaryProcessingDto { NetSalryUploaded = totalSalaryToProcess });
            }
            catch (Exception ex)
            {
                // Rollback MongoDB changes if needed
                await RollBackMongoData(request.FileUploadId);
                string errorMessage = $"[ERROR] Unexpected error during salary execution: {ex.Message}.";
                return await LogAndReturnError(errorMessage, request, HttpStatusCodeEnum.InternalServerError);
            }

        }


        

        public async Task<bool> RollBackMongoData(string bulkOperationCode)
        {
            try
            {
                string startMessage = $"[INFO] Initiating rollback for BulkOperationCode: '{bulkOperationCode}'.";
                _logger.LogInformation(startMessage);
                await BaseUtilities.LogAndAuditAsync(startMessage, bulkOperationCode, HttpStatusCodeEnum.OK, LogAction.FailedSalaryExecutionMogoRollback, LogLevelInfo.Information, _userInfoToken.FullName, _userInfoToken.Token);

                var repoAccountingBulk = _mongoUnitOfWork.GetRepository<AccountToAccountTransfer>();
                var repoLoanBulk = _mongoUnitOfWork.GetRepository<LoanRepaymentMongo>();

                // Define MongoDB filters
                var filterAccounting = Builders<AccountToAccountTransfer>.Filter.Eq(x => x.BulkExecutionCode, bulkOperationCode);
                var filterLoan = Builders<LoanRepaymentMongo>.Filter.Eq(x => x.SalaryCode, bulkOperationCode);

                // Retrieve matching documents
                var bulksAccounting = await repoAccountingBulk.FindByAsync(filterAccounting);
                var bulksLoan = await repoLoanBulk.FindByAsync(filterLoan);

                if (!bulksAccounting.Any() && !bulksLoan.Any())
                {
                    string noDataMessage = $"[WARNING] No MongoDB records found for rollback with BulkOperationCode: '{bulkOperationCode}'.";
                    _logger.LogWarning(noDataMessage);
                    await BaseUtilities.LogAndAuditAsync(noDataMessage, bulkOperationCode, HttpStatusCodeEnum.NotFound, LogAction.FailedSalaryExecutionMogoRollback, LogLevelInfo.Warning, _userInfoToken.FullName, _userInfoToken.Token);
                    return false;
                }

                // Get all IDs for batch deletion
                var accountingIds = bulksAccounting.Select(x => (object)x.Id).ToList();
                var loanIds = bulksLoan.Select(x => (object)x.Id).ToList();

                // Perform batch deletion with verification
                if (accountingIds.Any())
                {
                    await repoAccountingBulk.DeleteManyAsync(accountingIds);
                    string accountingRollbackMessage = $"[SUCCESS] Rolled back {accountingIds.Count} accounting records for BulkOperationCode: '{bulkOperationCode}'.";
                    _logger.LogInformation(accountingRollbackMessage);
                    await BaseUtilities.LogAndAuditAsync(accountingRollbackMessage, bulkOperationCode, HttpStatusCodeEnum.OK, LogAction.FailedSalaryExecutionMogoRollback, LogLevelInfo.Information, _userInfoToken.FullName, _userInfoToken.Token);
                }

                if (loanIds.Any())
                {
                    await repoLoanBulk.DeleteManyAsync(loanIds);
                    string loanRollbackMessage = $"[SUCCESS] Rolled back {loanIds.Count} loan records for BulkOperationCode: '{bulkOperationCode}'.";
                    _logger.LogInformation(loanRollbackMessage);
                    await BaseUtilities.LogAndAuditAsync(loanRollbackMessage, bulkOperationCode, HttpStatusCodeEnum.OK, LogAction.FailedSalaryExecutionMogoRollback, LogLevelInfo.Information, _userInfoToken.FullName, _userInfoToken.Token);
                }

                string finalSuccessMessage = $"[SUCCESS] MongoDB rollback completed successfully for BulkOperationCode: '{bulkOperationCode}'.";
                _logger.LogInformation(finalSuccessMessage);
                await BaseUtilities.LogAndAuditAsync(finalSuccessMessage, bulkOperationCode, HttpStatusCodeEnum.OK, LogAction.FailedSalaryExecutionMogoRollback, LogLevelInfo.Information, _userInfoToken.FullName, _userInfoToken.Token);

                return true;
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERROR] Rollback failed for BulkOperationCode '{bulkOperationCode}': {ex.Message}.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, bulkOperationCode, HttpStatusCodeEnum.InternalServerError, LogAction.FailedSalaryExecutionMogoRollback, LogLevelInfo.Error, _userInfoToken.FullName, _userInfoToken.Token);
                return false;
            }
        }


        /// <summary>
        /// Logs an error message, audits the event, and returns an appropriate error response.
        /// </summary>
        private async Task<ServiceResponse<SalaryProcessingDto>> LogAndReturnError(string message, ExecuteSalaryCommand request, HttpStatusCodeEnum statusCode, LogLevelInfo logLevel = LogLevelInfo.Error)
        {
            _logger.Log(logLevel == LogLevelInfo.Error ? LogLevel.Error : LogLevel.Warning, message);
            await BaseUtilities.LogAndAuditAsync(message, request, statusCode, LogAction.SalaryProcessing, logLevel, _userInfoToken.FullName, _userInfoToken.Token);
            return statusCode switch
            {
                HttpStatusCodeEnum.NotFound => ServiceResponse<SalaryProcessingDto>.Return404(message),
                HttpStatusCodeEnum.BadRequest => ServiceResponse<SalaryProcessingDto>.Return400(message),
                HttpStatusCodeEnum.Forbidden => ServiceResponse<SalaryProcessingDto>.Return403(message),
                _ => ServiceResponse<SalaryProcessingDto>.Return500(message)
            };
        }

        /// <summary>
        /// Processes an account transaction for salary execution, including loan repayments, savings, and deposits.
        /// </summary>
        /// <param name="salaryExtract">The salary extract containing transaction details.</param>
        /// <param name="amount">The amount to process.</param>
        /// <param name="accountType">The type of account being credited or debited.</param>
        /// <param name="teller">The teller processing the transaction.</param>
        /// <param name="accountingDate">The accounting date of the transaction.</param>
        /// <param name="sendingAccount">The source account for the transaction.</param>
        /// <param name="TellerAccount">The teller's operational account.</param>
        /// <param name="reference">The unique reference for the transaction.</param>
        /// <returns>A tuple containing the processed transaction and the updated target account.</returns>
        private async Task<(Transaction, Account,Account)> ProcessAccountTransaction(
            SalaryExtract salaryExtract,
            decimal amount,
            string accountType,
            Teller teller,
            DateTime accountingDate,
            Account sendingAccount = null,
            Account TellerAccount = null,
            string reference = null)
        {
            try
            {
                // Step 1: Validate transaction amount before proceeding
                if (amount <= 0)
                {
                    string noAmountMessage = $"[INFO] No amount to process for {accountType} account of Member: [Customer ID: {salaryExtract.MemberReference}]. Skipping transaction.";
                    _logger.LogInformation(noAmountMessage);
                    await BaseUtilities.LogAndAuditAsync(noAmountMessage, salaryExtract, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information, _userInfoToken.FullName, _userInfoToken.Token);
                    return (new Transaction(), null,null);
                }

                Transaction processedTransaction = new Transaction();
                Account updatedAccount = null;
                // Step 2: Process Loan Repayment separately
                if (accountType == SalaryExecution.Loan.ToString())
                {
                    sendingAccount= _accountRepository.DebitAccountBalanceReturned(sendingAccount, salaryExtract.TotalLoanRepayment, "Loan Repayment Standing Order");
                    var loanAccount = await _accountRepository.FindBy(a => a.CustomerId == salaryExtract.MemberReference && a.AccountType == accountType && a.IsDeleted == false).FirstOrDefaultAsync();
         


                    if (loanAccount == null)
                    {
                        string loanAccountError = $"[ERROR] Missing Salary or Loan account for Member: [Customer ID: {salaryExtract.MemberReference}]. Skipping loan repayment.";
                        _logger.LogError(loanAccountError);
                        await BaseUtilities.LogAndAuditAsync(loanAccountError, salaryExtract, HttpStatusCodeEnum.NotFound, LogAction.SalaryProcessing, LogLevelInfo.Error, _userInfoToken.FullName, _userInfoToken.Token);
                        var addLoanAccountCommand = new AddLoanAccountCommand
                        {
                            BranchId = _userInfoToken.BranchID,
                            BankId = _userInfoToken.BankID,
                            CustomerId = "n/a",
                            FileUploadId = "n/a",
                            IsBGS = false,
                            IsForSalaryTreatement = true,
                            UserInfoToken = _userInfoToken
                        };
                        
                        loanAccount = await CreateAccountEntity(addLoanAccountCommand);
                        if (loanAccount==null)
                        {
                            return (new Transaction(), null, null);

                        }
                    }
                    var transaction = CreateSenderTransactionEntityForSalary(amount, sendingAccount, 0, teller, reference, accountingDate, TransactionType.SALARY_TRANSFER, salaryExtract.StandingOrderStatement, loanAccount);

                    // Process Loan Repayment
                    processedTransaction = await ProcessLoanRepayment(salaryExtract, sendingAccount, loanAccount, amount, accountingDate, teller, reference);
                    CreateTransfer(amount, sendingAccount, reference, accountingDate, loanAccount, teller);

                    string loanSuccessMessage = $"[SUCCESS] Loan account '{loanAccount.AccountNumber}' credited with {BaseUtilities.FormatCurrency(amount)} for Member: [Customer ID: {salaryExtract.MemberReference}].";
                    _logger.LogInformation(loanSuccessMessage);
                    await BaseUtilities.LogAndAuditAsync(loanSuccessMessage, salaryExtract, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information, _userInfoToken.FullName, _userInfoToken.Token);

                    updatedAccount = loanAccount;
                }
                else
                {
                    // Step 3: Process non-loan transactions (Savings, Deposits, etc.)
                    sendingAccount= _accountRepository.DebitAccountBalanceReturned(sendingAccount, amount, salaryExtract.StandingOrderStatement);
                    var targetAccount = await _accountRepository.GetMemberAccount(salaryExtract.MemberReference, accountType);
                    var transaction = CreateSenderTransactionEntityForSalary(amount, sendingAccount, 0, teller, reference, accountingDate, TransactionType.SALARY_TRANSFER, salaryExtract.StandingOrderStatement, targetAccount);
                   

                    if (sendingAccount == null || targetAccount == null)
                    {
                        string accountErrorMessage = $"[ERROR] Missing Salary or Target account ({accountType}) for Member: [Customer ID: {salaryExtract.MemberReference}]. Skipping transfer.";
                        _logger.LogError(accountErrorMessage);
                        await BaseUtilities.LogAndAuditAsync(accountErrorMessage, salaryExtract, HttpStatusCodeEnum.NotFound, LogAction.SalaryProcessing, LogLevelInfo.Error, _userInfoToken.FullName, _userInfoToken.Token);
                        return (new Transaction(), null,null);
                    }

                    // Step 4: Credit the target account
                    _accountRepository.CreditAccount(targetAccount, amount, "Standing Order From Salary Payment.");

                    processedTransaction = CreateReceiverTransactionEntity(targetAccount, OperationType.Credit.ToString(), reference, amount, TransactionType.SALARY_TRANSFER, accountingDate, teller, sendingAccount);

                    // Step 6: Record Transfer & Teller Operations
                    CreateTransfer(amount, sendingAccount, reference, accountingDate, targetAccount, teller);
                    CreateTellerOperation(amount, OperationType.Credit, teller, TellerAccount, reference, TransactionType.SALARY_TRANSFER, accountingDate, targetAccount);

                    string generalSuccessMessage = $"[SUCCESS] {accountType} account '{targetAccount.AccountNumber}' credited with {BaseUtilities.FormatCurrency(amount)} for Member: [Customer ID: {salaryExtract.MemberReference}].";
                    _logger.LogInformation(generalSuccessMessage);
                    await BaseUtilities.LogAndAuditAsync(generalSuccessMessage, salaryExtract, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information, _userInfoToken.FullName, _userInfoToken.Token);

                    updatedAccount = targetAccount;
                }

                // Step 7: Return the processed transaction and updated account
                return (processedTransaction, updatedAccount, sendingAccount);
            }
            catch (Exception ex)
            {
                // Step 8: Handle unexpected errors and log them for auditing
                string errorMessage = $"[ERROR] Failed to process {accountType} transaction for Member: [Customer ID: {salaryExtract.MemberReference}]. Error: {ex.Message}.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, salaryExtract, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryProcessing, LogLevelInfo.Error, _userInfoToken.FullName, _userInfoToken.Token);
                return (new Transaction(), null,null);
            }
        }
        private async Task<Account> CreateAccountEntity(AddLoanAccountCommand request)
        {
           var product= await _savingProductRepository.FindBy(x => x.AccountType.ToLower() == "loan").FirstOrDefaultAsync();
            var accountEntity = _mapper.Map<Account>(request);
            accountEntity.Id = BaseUtilities.GenerateUniqueNumber();
            accountEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            accountEntity.ModifiedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            accountEntity.AccountNumber = $"{product.AccountNuber}{request.CustomerId}";
            accountEntity.AccountType = product.AccountType;
            accountEntity.AccountName = $"{product.AccountType}";
            accountEntity.BranchCode = request.IsBGS ? request.UserInfoToken.BranchCode : _userInfoToken.BranchCode;
            accountEntity.CustomerName = request.CustomerId;
            accountEntity.BranchId = request.IsBGS ? request.UserInfoToken.BranchID : _userInfoToken.BranchID;
            accountEntity.ProductId = product.Id;
            _accountRepository.Add(accountEntity);
            return accountEntity;
        }
        /// <summary>
        /// Processes the net salary transaction, handling salary deposits and deductions.
        /// </summary>
        /// <param name="salaryExtract">The salary extract containing salary breakdown.</param>
        /// <param name="amount">The net salary amount to process.</param>
        /// <param name="accountType">The type of account being credited.</param>
        /// <param name="senderTransaction">The sender's transaction details.</param>
        /// <param name="teller">The teller processing the transaction.</param>
        /// <param name="accountingDate">The accounting date for the transaction.</param>
        /// <param name="sendingAccount">The source account for the salary transaction.</param>
        /// <param name="TellerAccount">The teller's account for logging operations.</param>
        /// <param name="reference">The transaction reference ID.</param>
        /// <param name="transferParameter">Transfer parameters for the transaction.</param>
        /// <param name="targetAccount">The recipient's salary account.</param>
        /// <returns>A tuple containing a list of transactions processed and the updated target account.</returns>
        private async Task<(List<TransactionDto>, Account, Account)> ProcessAccountTransactionNetSalary(
            SalaryExtract salaryExtract,
            decimal amount,
            string accountType,
            Transaction senderTransaction,
            Teller teller,
            DateTime accountingDate,
            Account sendingAccount = null,
            Account TellerAccount = null,
            string reference = null,
            TransferParameter transferParameter = null,
            Account targetAccount = null)
        {
            try
            {
                // Step 1: Validate amount before processing
                if (amount <= 0)
                {
                    string warningMessage = $"[WARNING] No salary amount to process for Member [Customer ID: {salaryExtract.MemberReference}]. Skipping transaction.";
                    _logger.LogWarning(warningMessage);
                    await BaseUtilities.LogAndAuditAsync(warningMessage, salaryExtract, HttpStatusCodeEnum.BadRequest, LogAction.SalaryProcessing, LogLevelInfo.Warning, _userInfoToken.FullName, _userInfoToken.Token);
                    return (new List<TransactionDto>(), targetAccount, targetAccount);
                }

                var transactions = new List<TransactionDto>();

                // Step 2: Define salary deductions
                var deductions = new Dictionary<string, decimal>
                {
                    { SalaryExecution.Saving.ToString(), salaryExtract.Saving },
                    { SalaryExecution.Deposit.ToString(), salaryExtract.Deposit },
                    { SalaryExecution.MemberShare.ToString(), salaryExtract.Shares },
                    { SalaryExecution.PreferenceShare.ToString(), salaryExtract.PreferenceShares },
                    { SalaryExecution.Loan.ToString(), salaryExtract.TotalLoanRepayment }
                };
                decimal GroseSalary = salaryExtract.NetSalary-salaryExtract.Charges;
                // Step 3: Process salary deposit transaction
                targetAccount= _accountRepository.CreditAccountBalanceReturned(targetAccount, GroseSalary, "Salary Payment");

                var salaryTransaction = CreateReceiverTransactionEntity(
                    targetAccount,
                    OperationType.Credit.ToString(),
                    reference,
                    salaryExtract.NetSalary,
                    TransactionType.SALARY_TRANSFER,
                    accountingDate,
                    teller,
                    sendingAccount,
                    salaryExtract.Charges,
                    transferParameter);

                transactions.Add(_mapper.Map<TransactionDto>(salaryTransaction));

                // Step 4: Create Transfer entry
                CreateTransfer(amount, sendingAccount, reference, accountingDate, targetAccount, teller, salaryExtract.Charges);

                // Step 5: Create Teller Operation entry For GRoss salary
                CreateTellerOperation(
                    salaryExtract.NetSalary,
                    OperationType.Credit,
                    teller,
                    TellerAccount,
                    reference,
                    TransactionType.SALARY_TRANSFER,
                    accountingDate,
                    targetAccount);
                // Step 5: Create Teller Operation entry For Charges
                CreateTellerOperation(
                    salaryExtract.Charges,
                    OperationType.Debit,
                    teller,
                    TellerAccount,
                    reference,
                    TransactionType.SALARY_TRANSFER,
                    accountingDate,
                    targetAccount);

                string successMessage = $"[SUCCESS] Salary deposit of {BaseUtilities.FormatCurrency(salaryExtract.Salary)} processed successfully for Member [Customer ID: {salaryExtract.MemberReference}].";
                _logger.LogInformation(successMessage);
                //await BaseUtilities.LogAndAuditAsync(successMessage, salaryExtract, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information);

                Account updatedAccount = new Account();

                // Step 6: Process deduction transactions (Loan, Savings, Deposits, etc.)
                foreach (var deduction in deductions.Where(d => d.Value > 0))
                {
                    updatedAccount = new Account();
                    string deductionMessage = $"[INFO] Processing {deduction.Key} deduction of {BaseUtilities.FormatCurrency(deduction.Value)} for Member [Customer ID: {salaryExtract.MemberReference}].";
                    _logger.LogInformation(deductionMessage);
                    //await BaseUtilities.LogAndAuditAsync(deductionMessage, salaryExtract, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information);

                    var (processedTransaction, destinationAccount,sourceSalaryAccount) = await ProcessAccountTransaction(
                        salaryExtract,
                        deduction.Value,
                        deduction.Key,
                        teller,
                        accountingDate,
                        targetAccount,
                        TellerAccount,
                        reference);

                    transactions.Add(_mapper.Map<TransactionDto>(processedTransaction));
                    updatedAccount = destinationAccount;
                }

                string finalMessage = $"[SUCCESS] Net salary processing completed for Member [Customer ID: {salaryExtract.MemberReference}]. Total Transactions: {transactions.Count}.";
                _logger.LogInformation(finalMessage);
                //await BaseUtilities.LogAndAuditAsync(finalMessage, salaryExtract, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information);

                // Return the list of processed transactions and the updated target account
                return (transactions, updatedAccount, targetAccount);
            }
            catch (Exception ex)
            {
                // Step 7: Log and audit errors
                string errorMessage = $"[ERROR] Salary processing failed for Member [Customer ID: {salaryExtract.MemberReference}]. Error: {ex.Message}.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, salaryExtract, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryProcessing, LogLevelInfo.Error, _userInfoToken.FullName, _userInfoToken.Token);
                throw;
            }
        }


        public async Task CreateBatchLoanRepaymentData(SalaryExtract salaryExtract, string reference)
        {
            // Map SalaryExtract to LoanRepaymentMongo
            LoanRepaymentMongo loanRepayment = new LoanRepaymentMongo
            {
                Id = BaseUtilities.GenerateUniqueNumber(10), // Generate a unique ID for MongoDB
                SalaryCode = salaryExtract.FileUploadId, // Use provided salary code
                BranchId = salaryExtract.BranchId,
                TransactionReference = reference, // Generate a short unique transaction reference
                Status = Status.Pending.ToString(), // Default status as pending
                FileUploadId = salaryExtract.FileUploadId,
                ExecutionDate = salaryExtract.ExecutionDate,
                RefundDate = BaseUtilities.UtcNowToDoualaTime(), // Assuming refund date is now
                Description = $"Standing Order Loan repayment for {salaryExtract.MemberName}",
                LoanId = salaryExtract.LoanId ?? "N/A",
                Error = string.Empty, // No error initially
                PrincipalAmount = salaryExtract.LoanCapital, // Map from SalaryExtract
                Interest = salaryExtract.LoanInterest,
                ChargeAmount = salaryExtract.Charges,
                TaxAmount = salaryExtract.VAT,
                TotalRepaymentAmount = salaryExtract.TotalLoanRepayment, // This includes all deductions
                PaymentMethod = "Salary Deduction",
                PaymentChannel = "Standing Order",
                BranchCode=salaryExtract.BranchCode,
                MemberReference=salaryExtract.MemberReference,
                BranchName=salaryExtract.BranchName
            };

            // Get the MongoDB repository for LoanRepayment
            var loanRepaymentMongoRepository = _mongoUnitOfWork.GetRepository<LoanRepaymentMongo>();
            // Add the Loan Repayment entity to MongoDB
            await loanRepaymentMongoRepository.InsertAsync(loanRepayment);
        }

        /// <summary>
        /// Processes salary standing order transactions by handling transfers to loan repayments, savings, and other member accounts.
        /// Ensures accurate account mappings, transaction logging, and audit tracking.
        /// </summary>
        public async Task AddSalaryStandingOrderAccounting(
            SalaryExtract salaryExtract,
            string reference,
            Account sourceAccount,
            Account destinationAccount,
            List<TransactionDto> transactions,
            string mainSourceName)
        {
            try
            {
                // Step 1: Validate input parameters
                if (salaryExtract == null)
                {
                    string errorMessage = "[ERROR] Salary extract is null. Cannot process standing order. Bulk recording for accounting";
                    await LogAndThrow(errorMessage, salaryExtract, HttpStatusCodeEnum.BadRequest);
                }

                if (sourceAccount == null || destinationAccount == null)
                {
                    string errorMessage = "[ERROR] Source or destination account is missing. Standing order cannot proceed. Bulk recording for accounting";
                    await LogAndThrow(errorMessage, salaryExtract, HttpStatusCodeEnum.BadRequest);
                }

                if (transactions == null || transactions.Count == 0)
                {
                    string errorMessage = "[ERROR] No transactions provided for standing order processing. Bulk recording for accounting";
                    await LogAndThrow(errorMessage, salaryExtract, HttpStatusCodeEnum.BadRequest);
                }

                // Step 2: Initialize AccountToAccountTransfer entity
                var accountToAccountTransfer = new AccountToAccountTransfer
                {
                    Id = BaseUtilities.GenerateUniqueNumber(10),
                    TransactionReferenceId = reference,
                    BulkExecutionCode=salaryExtract.FileUploadId,
                    MemberReference = salaryExtract.MemberReference,
                    Status = "Pending",
                    TotalAmount = transactions.Sum(x => Math.Abs(x.Amount)),
                    Charges = salaryExtract.Charges,
                    Narration = $"Standing Order Execution: Debit {mainSourceName} Member Account and Credit {destinationAccount.CustomerName} via Account-to-Account Transfer. " +
                                $"Includes deductions for loans, savings, and fees.",
                    BranchId = salaryExtract.BranchId,
                    BranchCode = salaryExtract.BranchCode,
                    BranchName = salaryExtract.BranchName,
                    TransactionType = "Salary_Standing_Order",
                    UserFullName = _userInfoToken.FullName,
                    ErrorMessage = "n/a",
                    UserInfoToken = _userInfoToken,
                    EndTime = DateTime.MinValue,
                    ExternalBranchcode = destinationAccount.BranchCode,
                    ExternalBranchName = "n/a",
                    MakeAccountPostingCommands = new List<MakeAccountPostingCommand>(),
                    LoanRefundCollectionAlpha = new LoanRefundCollectionAlpha(),
                    LoanRefundCollections = new List<LoanRefundCollection>(),
                    MemberName = sourceAccount.CustomerName,
                    StartTime = BaseUtilities.UtcNowToDoualaTime(),
                };

                // Step 3: Process each transaction and create corresponding posting commands
                foreach (var transaction in transactions)
                {
                    decimal amountToPost = Math.Abs(transaction.Amount);
                    string transactionType = transaction.AccountType;
                    string productType = "SavingProduct";
                    var loanRefundCollections = new List<LoanRefundCollection>();
                    var amountCollections = new List<AmountCollection>();
                    var loanRefundCollectionAlpha = new LoanRefundCollectionAlpha();
                    bool isLoanTransaction = transaction.Operation.Contains("LOAN");

                    if (isLoanTransaction)
                    {


                        if (salaryExtract.IsOnldLoan)
                        {
                            productType = "OldLoan";
                            var oldLoanConfig = await _oldLoanAccountingMapingRepository
                                .All
                                .FirstOrDefaultAsync(x => x.LoanTypeName.ToLower() == salaryExtract.LoanType.ToLower());

                            if (oldLoanConfig == null)
                            {
                                string errorMessage = $"[ERROR] Loan mapping configuration missing for '{salaryExtract.LoanType}'. Skipping loan repayment. Bulk recording for accounting";
                                await LogAndThrow(errorMessage, salaryExtract, HttpStatusCodeEnum.NotFound);
                            }

                            loanRefundCollectionAlpha = new LoanRefundCollectionAlpha
                            {
                                InterestAccountNumber = oldLoanConfig.ChartOfAccountIdForInterest ?? "N/A",
                                VatAccountNumber = oldLoanConfig.ChartOfAccountIdForVAT ?? "N/A",
                                AmountAccountNumber = oldLoanConfig.ChartOfAccountIdForCapital ?? "N/A",
                                AmountInterest = salaryExtract.LoanInterest,
                                AmountVAT = salaryExtract.VAT,
                                AmountCapital = salaryExtract.LoanCapital,
                                InterestNaration = $"Migrated Loan Repayment | Type: Interest | Amount: {BaseUtilities.FormatCurrency(salaryExtract.LoanInterest)} | REF: {reference} | Member: {salaryExtract.MemberName}",
                                VatNaration = $"Migrated Loan Repayment | Type: VAT | Amount: {BaseUtilities.FormatCurrency(salaryExtract.VAT)} | REF: {reference} | Member: {salaryExtract.MemberName}",
                            };
                        }
                        else
                        {
                            productType = "NewLoan";
                            // Process **New Loan Repayments**
                            var loanEvents = new Dictionary<string, decimal>
                            {
                                { OperationEventRubbriqueNameForLoan.Loan_Principal_Account.ToString(), salaryExtract.LoanCapital },
                                { OperationEventRubbriqueNameForLoan.Loan_Interest_Recieved_Account.ToString(), salaryExtract.LoanInterest },
                                { OperationEventRubbriqueNameForLoan.Loan_VAT_Account.ToString(), salaryExtract.VAT }
                            };

                            foreach (var entry in loanEvents)
                            {
                                if (entry.Value>0)
                                {
                                    loanRefundCollections.Add(new LoanRefundCollection
                                    {
                                        Amount = entry.Value,
                                        EventAttributeName = entry.Key,
                                        Naration = $"Loan Repayment | Type: {entry.Key.Split('_').Last()} | Amount: {BaseUtilities.FormatCurrency(entry.Value)} | REF: {reference} | Member: {salaryExtract.MemberName}"
                                    });
                                }

                            }
                        }
                        accountToAccountTransfer.LoanRefundCollectionAlpha=loanRefundCollectionAlpha;
                        accountToAccountTransfer.LoanRefundCollections=loanRefundCollections;
                    }

                    // Construct transaction narration
                    string transactionNarration = transaction.Fee > 0
                        ? $"[Fee Deduction] Salary payroll Fee for {transactionType} | Amount: {BaseUtilities.FormatCurrency(transaction.Fee)} | REF: {reference}"
                        : $"[Transfer] Standing Order Processing for {transactionType} | Amount: {BaseUtilities.FormatCurrency(amountToPost)} | REF: {reference}";


                    // Process **New Loan Repayments**
                    var tranfsreCollections = new Dictionary<string, decimal>();

                    // If the transaction is Salary type, only record the Fee
                    if (transaction.AccountType == AccountType.Salary.ToString() && transaction.Fee > 0)
                    {
                        tranfsreCollections.Add(OperationEventRubbriqueName.Transfer_Fee_Account.ToString(), Math.Abs(transaction.Fee));
                    }
                    else
                    {
                        // For other account types, record both Principal and Fee
                        tranfsreCollections.Add(OperationEventRubbriqueName.Principal_Saving_Account.ToString(), Math.Abs(transaction.Amount));
                        tranfsreCollections.Add(OperationEventRubbriqueName.Transfer_Fee_Account.ToString(), Math.Abs(transaction.Fee));
                    }

                    foreach (var collection in tranfsreCollections)
                    {
                        if (collection.Value > 0)
                        {
                            amountCollections.Add(new AmountCollection
                            {
                                Amount = collection.Value,
                                EventAttributeName = collection.Key,
                                HasPaidCommissionByCash = false,
                                IsInterBankOperationCommission = false,
                                IsInterBankOperationPrincipalCommission = false,
                                Naration=transactionNarration,
                                IsPrincipal=collection.Key==OperationEventRubbriqueName.Principal_Saving_Account.ToString() ? true : false,
                            });
                        }
                    }




                    // Create Account Posting Command
                    var postingCommand = new MakeAccountPostingCommand
                    {
                        AmountCollection = isLoanTransaction ? null : amountCollections,
                        LoanRefundCollectionAlpha = isLoanTransaction ? loanRefundCollectionAlpha : null,
                        LoanRefundCollections = isLoanTransaction ? loanRefundCollections : null,
                        ProductType = productType,
                        TransactionReferenceId=reference,
                        AmountEventCollections = new List<AmountEventCollection>(),
                        ExternalBranchCode = destinationAccount.BranchCode,
                        ExternalBranchId = destinationAccount.BranchId,
                        FromAccountNumber = sourceAccount.AccountNumber,
                        FromProductId = sourceAccount.ProductId,
                        FromProductName = "Salary",
                        IsInterBranchTransaction = false,
                        MemberReference = sourceAccount.CustomerId,
                        Naration = transactionNarration,
                        OperationType = transaction.Operation,
                        ToAccountNumber = destinationAccount.AccountNumber,
                        ToProductId = isLoanTransaction ? salaryExtract.LoanProductId : destinationAccount.ProductId,
                        ToProductName = destinationAccount.Product?.Name==null ? "n/a" : destinationAccount.Product?.Name,
                        TransactionDate = transaction.AccountingDate,
                    };

                    accountToAccountTransfer.MakeAccountPostingCommands.Add(postingCommand);
                }

                // Step 4: Insert into MongoDB
                await _mongoUnitOfWork.GetRepository<AccountToAccountTransfer>().InsertAsync(accountToAccountTransfer);

                string successMessage = $"[SUCCESS] Salary Standing Order successfully processed for Member '{salaryExtract.MemberName}' with Reference '{reference}'. Bulk recording for accounting";
                _logger.LogInformation(successMessage);
                //await BaseUtilities.LogAndAuditAsync(successMessage, salaryExtract, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information);
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERROR] Salary Standing Order Processing Failed. Bulk recording for accounting: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, salaryExtract, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryProcessing, LogLevelInfo.Error, _userInfoToken.FullName, _userInfoToken.Token);
                throw;
            }
        }

        /// <summary>
        /// Helper method to log and audit an error before throwing an exception.
        /// </summary>
        private async Task LogAndThrow(string errorMessage, SalaryExtract salaryExtract, HttpStatusCodeEnum httpStatusCode)
        {
            _logger.LogError(errorMessage);
            await BaseUtilities.LogAndAuditAsync(errorMessage, salaryExtract, httpStatusCode, LogAction.SalaryProcessing, LogLevelInfo.Error, _userInfoToken.FullName, _userInfoToken.Token);
            throw new InvalidOperationException(errorMessage);
        }
        private Transfer CreateTransfer(decimal Amount, Account senderAccount, string reference, DateTime accountingDate, Account receiverAccount, Teller teller, decimal charges = 0)
        {
            var transfer = new Transfer();
            transfer.DateOfInitiation = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            transfer.Id = BaseUtilities.GenerateUniqueNumber();
            transfer.TransactionRef = reference;
            transfer.TransactionType = TransactionType.SALARY_TRANSFER.ToString();
            transfer.AccountId = senderAccount.Id;
            transfer.SourceAccountNumber = senderAccount.AccountNumber;
            transfer.DestinationAccountNumber = receiverAccount.AccountNumber;
            transfer.SourceAccountType = senderAccount.AccountType;
            transfer.DestinationAccountType = receiverAccount.AccountType;
            transfer.Status = Status.Pending.ToString();
            transfer.InitiatorComment = $"Order to {TransactionType.TRANSFER.ToString()} the sum of {BaseUtilities.FormatCurrency(Amount)} from my account number {senderAccount.AccountNumber} to [Salary earners].";
            transfer.BranchId = senderAccount.BranchId;
            transfer.Charges = charges;
            transfer.Tax = 0;
            transfer.AccountingDate = accountingDate;
            transfer.Amount = Amount;
            transfer.InitiatedByUSerName = _userInfoToken.FullName;
            transfer.TellerId = teller.Id;
            transfer.SourceType = OperationSourceType.BackOffice_Operation.ToString();
            transfer.IsInterBranchOperation = false;
            transfer.SourceBrachId = senderAccount.BranchId;
            transfer.DestinationBrachId = receiverAccount.BranchId;
            transfer.DestinationCommision = 0;
            transfer.SourceCommision = 0;
            _transferRepository.Add(transfer);
            return transfer;
        }

        /// <summary>
        /// Processes loan repayment by debiting the sender's account and crediting the receiving account.
        /// </summary>
        /// <param name="salaryExtract">The salary extract containing transaction details.</param>
        /// <param name="senderAccount">The account from which the repayment is debited.</param>
        /// <param name="receivingAccount">The loan account where the repayment is credited.</param>
        /// <param name="amount">The repayment amount.</param>
        /// <param name="accountingDate">The date of the transaction.</param>
        /// <param name="teller">The teller processing the transaction.</param>
        /// <param name="reference">The unique reference for the transaction.</param>
        /// <returns>Returns the created loan repayment transaction.</returns>
        private async Task<Transaction> ProcessLoanRepayment(
            SalaryExtract salaryExtract,
            Account senderAccount,
            Account receivingAccount,
            decimal amount,
            DateTime accountingDate,
            Teller teller,
            string reference)
        {
            try
            {
                // Step 1: Validate amount before proceeding
                if (amount <= 0)
                {
                    string noAmountMessage = $"[INFO] No loan repayment to process for Member: [Customer ID: {salaryExtract.MemberReference}]. Skipping transaction.";
                    _logger.LogInformation(noAmountMessage);
                    await BaseUtilities.LogAndAuditAsync(noAmountMessage, salaryExtract, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information, _userInfoToken.FullName, _userInfoToken.Token);
                    return null;
                }

                // Step 2: Validate sender and receiver accounts
                if (senderAccount == null || receivingAccount == null)
                {
                    string accountErrorMessage = $"[ERROR] Missing sender or loan account for Member: [Customer ID: {salaryExtract.MemberReference}]. Skipping loan repayment.";
                    _logger.LogError(accountErrorMessage);
                    await BaseUtilities.LogAndAuditAsync(accountErrorMessage, salaryExtract, HttpStatusCodeEnum.NotFound, LogAction.SalaryProcessing, LogLevelInfo.Error, _userInfoToken.FullName, _userInfoToken.Token);
                    return null;
                }

                // Step 3: Create and log loan repayment transaction
                var loanRepaymentTransaction = CreateReceiverTransactionEntity(
                    receivingAccount,
                    OperationType.Debit.ToString(),
                    reference,
                    amount,
                    TransactionType.CASHIN_LOAN_REPAYMENT_STANDING_ORDER,
                    accountingDate,
                    teller,
                    senderAccount);

                string transactionMessage = $"[SUCCESS] Loan repayment transaction created. Member: [Customer ID: {salaryExtract.MemberReference}], Amount: {BaseUtilities.FormatCurrency(amount)}, Reference: {reference}.";
                _logger.LogInformation(transactionMessage);
                //await BaseUtilities.LogAndAuditAsync(transactionMessage, salaryExtract, HttpStatusCodeEnum.OK, LogAction.SalaryProcessing, LogLevelInfo.Information);

                // Step 4: Add the salary standing order for further processing
                await CreateBatchLoanRepaymentData(salaryExtract, reference);

                // Step 5: Return the created transaction
                return loanRepaymentTransaction;
            }
            catch (Exception ex)
            {
                // Step 6: Handle unexpected errors and log them for auditing
                string errorMessage = $"[ERROR] Failed to process loan repayment for Member: [Customer ID: {salaryExtract.MemberReference}]. Error: {ex.Message}.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, salaryExtract, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryProcessing, LogLevelInfo.Error, _userInfoToken.FullName, _userInfoToken.Token);
                return null;
            }
        }





        /// <summary>
        /// Processes the payment receipt by aggregating transaction details and initiating payment processing.
        /// </summary>
        /// <param name="transactions">List of transactions associated with the salary extract.</param>
        /// <param name="salaryExtract">Salary extract containing relevant salary details.</param>
        private async Task ProcessPaymentReceipt(List<TransactionDto> transactions, SalaryExtract salaryExtract)
        {
            try
            {
                // Step 1: Validate Input Parameters
                if (transactions == null || transactions.Count == 0)
                {
                    string errorMessage = "[ERROR] No transactions provided to process payment receipt.";
                    _logger.LogError(errorMessage);
                    throw new ArgumentException(errorMessage, nameof(transactions));
                }

                if (salaryExtract == null)
                {
                    string errorMessage = "[ERROR] Salary extract cannot be null.";
                    _logger.LogError(errorMessage);
                    throw new ArgumentNullException(nameof(salaryExtract), errorMessage);
                }

                // Step 2: Initialize Lists for Payment Receipts and Account Deposits
                var accountDeposits = new List<AccountDeposit>();
                var paymentReceipts = new List<PaymentDetailObject>();

                // Step 3: Iterate Over Transactions and Populate Receipt & Deposit Lists
                foreach (var transaction in transactions)
                {
                    if (transaction == null)
                    {
                        string transactionError = "[ERROR] Encountered a null transaction in the list.";
                        _logger.LogError(transactionError);
                        throw new InvalidOperationException(transactionError);
                    }

                    // Determine if transaction is a loan repayment or general payment
                    bool isLoanRepayment = transaction.Operation == TransactionType.CASHIN_LOAN_REPAYMENT_STANDING_ORDER.ToString();
                    decimal interest = isLoanRepayment ? 0 : salaryExtract.LoanInterest;
                    decimal loanCapital = isLoanRepayment ? 0 : salaryExtract.LoanCapital;
                    decimal vat = isLoanRepayment ? 0 : salaryExtract.VAT;

                    // Create Payment Detail Object
                    var paymentDetail = new PaymentDetailObject
                    {
                        AccountNumber = transaction.AccountNumber,
                        Fee = transaction.Fee,
                        Amount = Math.Abs(transaction.OriginalDepositAmount),
                        Interest = interest,
                        LoanCapital = loanCapital,
                        SericeOrEventName = transaction.Account?.AccountName ?? "Unknown Service",
                        VAT = vat,
                        Balance = 0
                    };
                    paymentReceipts.Add(paymentDetail);

                    // Create Account Deposit Object
                    var accountDeposit = new AccountDeposit
                    {
                        AccountName = transaction.Account?.AccountName ?? "Unknown Account",
                        Amount = Math.Abs(transaction.OriginalDepositAmount),
                        Charge = transaction.Fee
                    };
                    accountDeposits.Add(accountDeposit);
                }

                // Step 4: Construct the Payment Processing Request
                var paymentProcessingRequest = new PaymentProcessingRequest
                {
                    AccountingDay = transactions.First().AccountingDate,
                    Amount = transactions.Sum(t => t.Amount),
                    MemberName = salaryExtract.MemberName,
                    NotesRequest = new CurrencyNotesRequest(),
                    OperationType = OperationType.NoneCash.ToString(),
                    OperationTypeGrouping = TransactionType.SALARY_TRANSFER.ToString(),
                    PortalUsed = OperationSourceType.Web_Portal.ToString(),
                    PaymentDetails = paymentReceipts,
                    ServiceType = TransactionType.SALARY_TRANSFER.ToString(),
                    SourceOfRequest = OperationSourceType.BackOffice_Operation.ToString(),
                    TotalAmount = salaryExtract.NetSalary,
                    TotalCharges = transactions.Sum(x => Math.Abs(x.Fee)),
                    Transactions = transactions
                };

                // Step 5: Log the Payment Processing Request Before Execution
                string logMessage = $"[INFO] Processing payment receipt for Member: {salaryExtract.MemberName}, Total Amount: {salaryExtract.NetSalary:C}, Total Transactions: {transactions.Count}.";
                _logger.LogInformation(logMessage);
                // Step 6: Process the Payment
                _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessingRequest);


                //await BaseUtilities.LogAndAuditAsync(logMessage, salaryExtract, HttpStatusCodeEnum.OK, LogAction.SalaryPaymentProcessing, LogLevelInfo.Information);
            }
            catch (Exception ex)
            {
                // Step 8: Handle Errors and Log Failure
                string errorMessage = $"[ERROR] Failed to process payment receipt for Member: {salaryExtract?.MemberName ?? "Unknown"}, Error: {ex.Message}.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, salaryExtract, HttpStatusCodeEnum.InternalServerError, LogAction.SalaryPaymentProcessing, LogLevelInfo.Error, _userInfoToken.FullName, _userInfoToken.Token);
                throw;
            }
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






        // Helper method to create sender transaction entity
        private Transaction CreateSenderTransactionEntity(decimal Amount, Account senderAccount, decimal Charges, Teller noneCashTeller, string Reference, DateTime accountingDate, TransactionType transactionType)
        {
            var senderTransactionEntity = new Transaction();
            senderTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            senderTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            senderTransactionEntity.TransactionReference = Reference;
            senderTransactionEntity.TransactionType = OperationType.Debit.ToString();
            senderTransactionEntity.Operation = transactionType.ToString();
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
            senderTransactionEntity.SourceBrachId = senderAccount.BranchId;
            senderTransactionEntity.IsInterBrachOperation = false;
            senderTransactionEntity.DestinationBrachId = senderAccount.BranchId;
            senderTransactionEntity.DestinationBranchCommission =0;
            senderTransactionEntity.SourceBranchCommission = Charges;
            senderTransactionEntity.Note ??= $"Account number {senderAccount.AccountNumber} made a transfer of {BaseUtilities.FormatCurrency(Amount + Charges)} to her salaries earners. Amount: {BaseUtilities.FormatCurrency(Amount)}, Reference: {Reference}";
            _TransactionRepository.Add(senderTransactionEntity);
            return senderTransactionEntity;
        }

        // Helper method to create sender transaction entity
        private Transaction CreateSenderTransactionEntityForSalary(decimal Amount, Account senderAccount, decimal Charges, Teller noneCashTeller, string Reference, DateTime accountingDate, TransactionType transactionType, string standingOrderType,Account receiveraccount)
        {
            var senderTransactionEntity = new Transaction();
            senderTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            senderTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            senderTransactionEntity.TransactionReference = Reference;
            senderTransactionEntity.TransactionType = OperationType.Debit.ToString();
            senderTransactionEntity.Operation = transactionType.ToString();
            senderTransactionEntity.PreviousBalance = senderAccount.PreviousBalance;
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
            senderTransactionEntity.SourceBrachId = senderAccount.BranchId;
            senderTransactionEntity.IsInterBrachOperation = false;
            senderTransactionEntity.DestinationBrachId = senderAccount.BranchId;
            senderTransactionEntity.DestinationBranchCommission =0;
            senderTransactionEntity.SourceBranchCommission = Charges;
            senderTransactionEntity.Note ??= $"[{transactionType}], Transferred {BaseUtilities.FormatCurrency(Amount + Charges)} from this account type {senderAccount.AccountType} to {receiveraccount.AccountType} account to fulfill standing order obligations. [{standingOrderType}]. These obligations include loan repayment, savings deposits, and other scheduled financial commitments. The transferred amount is {BaseUtilities.FormatCurrency(Amount)}, and the transaction reference is: {Reference}.";
            _TransactionRepository.Add(senderTransactionEntity);
            return senderTransactionEntity;
        }


        // Helper method to create receiver transaction entity
        private Transaction CreateReceiverTransactionEntity(Account receiverAccount, string debitOrCredit, string reference, decimal Amount, TransactionType transactionType, DateTime accountingDate, Teller teller, Account senderAccount, decimal charges = 0, TransferParameter transferParameter = null)
        {


            decimal OriginalDepositAmount = Amount;
            Amount-=charges;

            decimal sourceCommision = transferParameter!=null ? XAFWallet.CalculateCommission(transferParameter.SourceBrachOfficeShare, charges) : 0;
            decimal destinationCommision = transferParameter!=null ? XAFWallet.CalculateCommission(transferParameter.DestinationBranchOfficeShare, charges) : 0;

            var receiverTransactionEntity = new Transaction();
            // Convert UTC to local time and set it in the entity
            receiverTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            receiverTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            receiverTransactionEntity.TransactionReference = reference;
            receiverTransactionEntity.TransactionType = debitOrCredit;
            receiverTransactionEntity.Operation = transactionType.ToString();
            receiverTransactionEntity.PreviousBalance = receiverAccount.PreviousBalance;
            receiverTransactionEntity.Balance = receiverAccount.Balance;
            receiverTransactionEntity.AccountId = receiverAccount.Id;
            receiverTransactionEntity.SenderAccountId = senderAccount.Id;
            receiverTransactionEntity.AccountNumber = receiverAccount.AccountNumber;
            receiverTransactionEntity.ReceiverAccountId = receiverAccount.Id;
            receiverTransactionEntity.ProductId = receiverAccount.ProductId;
            receiverTransactionEntity.Status = "COMPLETED";
            receiverTransactionEntity.Note ??= $"Note: [{transactionType}], Account number {receiverAccount.AccountNumber} recieves a transfer of {BaseUtilities.FormatCurrency(OriginalDepositAmount)} from account number {senderAccount.AccountNumber}, Reference: {reference}. Charge: {charges:N1}";
            receiverTransactionEntity.BankId = receiverAccount.BankId;
            receiverTransactionEntity.BranchId = receiverAccount.BranchId;
            receiverTransactionEntity.OriginalDepositAmount = OriginalDepositAmount;
            receiverTransactionEntity.Fee = charges;
            receiverTransactionEntity.Tax = 0;
            receiverTransactionEntity.AccountType=receiverAccount.AccountType;
            receiverTransactionEntity.AccountingDate = accountingDate;
            receiverTransactionEntity.Amount = Amount;
            receiverTransactionEntity.CustomerId = receiverAccount.CustomerId;
            receiverTransactionEntity.OperationType = OperationType.Credit.ToString();
            receiverTransactionEntity.FeeType = Events.None.ToString();
            receiverTransactionEntity.TellerId = teller.Id;
            receiverTransactionEntity.Debit = debitOrCredit == "Debit" ? OriginalDepositAmount : 0;
            receiverTransactionEntity.Credit = debitOrCredit == "Credit" ? OriginalDepositAmount : 0; ;
            receiverTransactionEntity.SourceType =OperationSourceType.BackOffice_Operation.ToString();
            receiverTransactionEntity.IsInterBrachOperation = false;
            receiverTransactionEntity.SourceBrachId = teller.BranchId;
            receiverTransactionEntity.DestinationBrachId = teller.BranchId;
            receiverTransactionEntity.DestinationBranchCommission = destinationCommision;
            receiverTransactionEntity.SourceBranchCommission = sourceCommision;
            receiverTransactionEntity.ReceiptTitle = $"{transactionType} Receipt Reference: " + reference;
            _TransactionRepository.Add(receiverTransactionEntity);
            return receiverTransactionEntity;
        }


        private void CreateTellerOperation(decimal amount, OperationType operationType, Teller teller, Account tellerAccount,
             string TransactionReference, TransactionType transactionType, DateTime accountingDate, Account receivingAccount)
        {
            var tellerOperation = new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = amount,
                OperationType = operationType.ToString(),
                BranchId = tellerAccount.BranchId,
                BankId = tellerAccount.BankId,
                OpenOfDayReference = "n/a",
                CurrentBalance = tellerAccount.Balance,
                Date = accountingDate,
                AccountingDate = accountingDate,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = teller.Id,
                TransactionReference = TransactionReference,
                TransactionType = transactionType.ToString(),
                UserID = _userInfoToken.Id,
                Description = $"{transactionType.ToString()} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode),
                CustomerId = receivingAccount.CustomerId,
                MemberAccountNumber = receivingAccount.AccountNumber,
                EventName = transactionType.ToString(),
                MemberName = receivingAccount.CustomerName,
                DestinationBrachId = teller.BranchId,
                SourceBranchId = teller.BranchId,
                IsInterBranch = false,
                DestinationBranchCommission = 0,
                SourceBranchCommission = 0,
                IsCashOperation = false,

            };
            _tellerOperationRepository.Add(tellerOperation);
        }

    }
}