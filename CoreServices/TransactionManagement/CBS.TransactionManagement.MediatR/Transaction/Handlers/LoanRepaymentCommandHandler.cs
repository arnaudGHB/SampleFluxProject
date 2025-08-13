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
using CBS.TransactionManagement.Helper.Helper;
using Microsoft.IdentityModel.Tokens;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.MediatR;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.LoanRepayment.Command;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transaction.
    /// </summary>
    public class LoanRepaymentCommandHandler : IRequestHandler<LoanRepaymentCommand, ServiceResponse<TransactionDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly ICurrencyNotesRepository _CurrencyNotesRepository;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }

        private readonly IConfigRepository _ConfigRepository;
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<LoanRepaymentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IAccountingDayRepository _accountingDayRepository;
        /// <summary>
        /// Constructor for initializing the LoanRepaymentCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public LoanRepaymentCommandHandler(
            IAccountRepository AccountRepository,
            ICurrencyNotesRepository CurrencyNotesRepository,
IConfigRepository ConfigRepository,
            UserInfoToken UserInfoToken,
            ITransactionRepository TransactionRepository,
            IMapper mapper,
            ILogger<LoanRepaymentCommandHandler> logger,

            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            IAccountingDayRepository accountingDayRepository = null)
        {
            _ConfigRepository = ConfigRepository;
            _CurrencyNotesRepository = CurrencyNotesRepository;
            _userInfoToken = UserInfoToken;
            _AccountRepository = AccountRepository;
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _tellerProvisioningAccountRepository = tellerProvisioningAccountRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _tellerRepository = tellerRepository;
            _mediator = mediator;
            _accountingDayRepository = accountingDayRepository;
        }

        /// <summary>
        /// Handles the LoanRepaymentCommand to add a new Transaction.
        /// </summary>
        /// <param name="request">The LoanRepaymentCommand containing Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<TransactionDto>> Handle(LoanRepaymentCommand request, CancellationToken cancellationToken)
        {
            bool isInterBanchOperation = false;

            try
            {
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);
                // Check if the user account serves as a teller today
                var tellerProvision = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();

                // Retrieve teller information
                var teller = await _tellerRepository
                    .FindBy(t => t.Id == tellerProvision.TellerId && !t.IsPrimary)
                    .FirstOrDefaultAsync();

                if (teller == null)
                {
                    var errorMessage = $"Teller {tellerProvision.TellerId} does not exist.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return404(errorMessage);
                }

                // Retrieve sub teller account
                var tellerAccount = await _AccountRepository.FindBy(t => t.TellerId == tellerProvision.TellerId)
                    .FirstOrDefaultAsync();
                if (tellerAccount == null)
                {
                    var errorMessage = $"Teller {teller.Name} does not have an account";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<TransactionDto>.Return404(errorMessage);
                }

                // Check if year and day are open
                var configNames = new List<string> { "IsDayOpen", "IsYearOpen" };
                var configs = await _ConfigRepository.FindBy(a => configNames.Contains(a.Name)).ToListAsync();

                foreach (var config in configs)
                {
                    if (config.Name.Equals("IsDayOpen") && !config.Value.Equals("True"))
                    {
                        var errorMessage = $"Day is not open";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<TransactionDto>.Return403(errorMessage);
                    }

                    if (config.Name.Equals("IsYearOpen") && !config.Value.Equals("True"))
                    {
                        var errorMessage = $"Year is not open";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<TransactionDto>.Return403(errorMessage);
                    }
                }

                string Reference = BaseUtilities.GenerateInsuranceUniqueNumber(20, $"LR-{_userInfoToken.BranchCode}-");
                var notes = request.CurrencyNotes;
                var currencyNotesList = CurrencyNotesMapper.ComputeCurrencyNote(notes, Reference);
                _CurrencyNotesRepository.AddRange(currencyNotesList);


                // Validate deposit limits
                var customerAccount = await _AccountRepository
                    .FindBy(a => a.AccountNumber == request.AccountNumber)
                    .Include(a => a.Product)
                    .ThenInclude(p => p.CashDepositParameters)
                    .FirstOrDefaultAsync();
                // Verify the integrity of the account balance
                string accountBalance = customerAccount.Balance.ToString();
                if (!BalanceEncryption.VerifyBalanceIntegrity(accountBalance, customerAccount.EncryptedBalance, customerAccount.AccountNumber))
                {
                    var errorMessage = $"Error occurred while initiating loan repayment, Account balance {BaseUtilities.FormatCurrency(customerAccount.Balance)} has been tampered with. Please contact your system administrator";
                    _logger.LogError(errorMessage);
                    //return ServiceResponse<TransactionDto>.Return403(errorMessage);
                }
                var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = customerAccount.CustomerId };
                var serviceResponse = await _mediator.Send(customerCommandQuery);
                if (serviceResponse.StatusCode == 200)
                {
                    var customer = serviceResponse.Data;
                    if (customer.MembershipApprovalStatus.ToLower() == AccountStatus.approved.ToString().ToLower())
                    {
                        var branchCommandQuery = new GetBranchByIdCommand { BranchId = customerAccount.BranchId };
                        var brachResponse = await _mediator.Send(branchCommandQuery);
                        if (brachResponse.StatusCode != 200)
                        {
                            return ServiceResponse<TransactionDto>.Return403(brachResponse.Message);

                        }
                        var branch = brachResponse.Data;
                        if (branch.id != _userInfoToken.BranchID)
                        {
                            isInterBanchOperation = true;
                        }

                        var depositLimits = customerAccount.Product.CashDepositParameters.ToList();
                        var depositLimit = depositLimits.FirstOrDefault(dl => dl.DepositType == request.DepositType);
                        if (depositLimit == null)
                        {
                            var errorMessage = $"Error occurred while initiating deposit with amount: {request.Amount} please set the {request.DepositType} limit before proceeding";
                            _logger.LogError(errorMessage);
                            return ServiceResponse<TransactionDto>.Return403(errorMessage);
                        }

                        if (DecimalComparer.IsLessThan(request.Amount, depositLimit.MinAmount) || DecimalComparer.IsGreaterThan(request.Amount, depositLimit.MaxAmount))
                        {
                            var errorMessage = $"Error occurred while initiating deposit with amount: {request.Amount}. Deposit amount must be between {depositLimit.MinAmount} and {depositLimit.MaxAmount}.";
                            _logger.LogError(errorMessage);
                            return ServiceResponse<TransactionDto>.Return403(errorMessage);
                        }

                        // Create transaction history for customer
                        var transactionEntity = await CreateTransactionHistory(request, customerAccount, teller, tellerAccount, depositLimit, customer, isInterBanchOperation, _userInfoToken.BranchID, branch?.id, Reference,accountingDate);
                        var transactionDto = CurrencyNotesMapper.MapTransactionToDto(transactionEntity, null, _userInfoToken);

                        //Calling Loan Refund API
                        var loanCommand = new AddLoanRepaymentCommandDetails { Amount = request.Amount, Comment = request.Note, LoanId = request.LoanId, Interest = request.Interest, PaymentChannel = request.PaymentChannel, Penalty = request.Penalty, Principal = request.Principal, Tax = request.Tax, PaymentMethod = request.PaymentMethod, TransactionCode = transactionDto.TransactionReference };
                        var loanResponse = await _mediator.Send(loanCommand);
                        if (loanResponse.StatusCode != 200)
                        {
                            return ServiceResponse<TransactionDto>.Return403(loanResponse.Message);
                        }
                        // Update Account entity properties with values from the request
                        await _uow.SaveAsync();
                        // Map the Transaction entity to TransactionDto and return it with a success response
                        transactionDto.ProductId = customerAccount.ProductId;
                        //Sending SMS.
                        var sMSPICallCommand = SendSMS(customerAccount, transactionEntity, customer, branch);
                        var resultsms = await _mediator.Send(sMSPICallCommand);
                        //Posting to Accounting
                        var apiRequest = MakeAccountingPosting(request, depositLimit, customerAccount, transactionEntity, branch, isInterBanchOperation);
                        var result = await _mediator.Send(apiRequest);
                        if (result.StatusCode == 200)
                        {
                            return ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto);
                        }
                        return ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto, result.Message);


                    }
                    return ServiceResponse<TransactionDto>.Return403($"Customer membership is not approved, Current Status: {customer.MembershipApprovalStatus}");
                }
                return ServiceResponse<TransactionDto>.Return403(serviceResponse.Message);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Transaction: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TransactionDto>.Return500(e);
            }
        }
        private SendSMSPICallCommand SendSMS(Account account, Transaction transaction, CustomerDto customer, BranchDto branch)
        {
            string bankName = branch.name;
            // Encrypt the account number partially
            string encryptedAccountNumber = BaseUtilities.PartiallyEncryptAccountNumber(account.AccountNumber);
            // Construct the SMS message with the partially encrypted account number
            string msg = $"Dear {customer.FirstName} {customer.LastName}, This is to confirm that a deposit of {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} has been successfully credited to your {account.AccountType} account ({encryptedAccountNumber}). Transaction Reference: {transaction.TransactionReference}.\n Date and Time: {transaction.CreatedDate}.\nYour current balance is now {BaseUtilities.FormatCurrency(account.Balance)}.\nThank you for banking with us.\n{bankName}.";
            return new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.Phone
            };
        }

        private AddAccountingPostingCommand MakeAccountingPosting(LoanRepaymentCommand request, CashDepositParameter depositParameter, Account account, Transaction transaction, BranchDto branch, bool IsInterBranch)
        {
            // Create a new AddAccountingPostingCommand instance
            var addAccountingPostingCommand = new AddAccountingPostingCommand
            {
                AccountHolder = account.AccountName,
                OperationType = TransactionType.DEPOSIT.ToString(),
                AccountNumber = account.AccountNumber,
                ProductId = account.ProductId,
                ProductName = account.Product.Name,
                Naration = transaction.Note,
                TransactionReferenceId = transaction.TransactionReference,
                IsInterBranchTransaction = IsInterBranch,
                ExternalBranchCode = branch.branchCode,
                ExternalBranchId = branch.id,
                AmountCollection = new List<AmountCollection>()
            };

            // Add amount collections based on IsInterBranch flag
            if (IsInterBranch)
            {
                // For inter-branch transactions
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = request.Amount,
                    IsPrincipal = true,
                    EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = transaction.SourceBranchCommission,
                    IsPrincipal = false,
                    EventAttributeName = SharingWithPartner.SourceBrachCommission_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = transaction.DestinationBranchCommission,
                    IsPrincipal = false,
                    EventAttributeName = SharingWithPartner.DestinationBranchCommission_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });
            }
            else
            {
                // For regular branch transactions
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = request.Principal,
                    IsPrincipal = true,
                    EventAttributeName = OperationEventRubbriqueName.Loan_Principal_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = request.Tax,
                    IsPrincipal = true,
                    EventAttributeName = OperationEventRubbriqueName.Loan_VAT_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = request.Penalty,
                    IsPrincipal = true,
                    EventAttributeName = OperationEventRubbriqueName.Loan_Penalty_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = request.Interest,
                    IsPrincipal = true,
                    EventAttributeName = OperationEventRubbriqueName.Loan_Interest_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });

            }

            return addAccountingPostingCommand;
        }
        private async Task<Transaction> CreateTransactionHistory(LoanRepaymentCommand request, Account account, Teller teller, Account tellerAccount, CashDepositParameter depositLimit, CustomerDto customer, bool isInterbranch, string sourceBranchId, string destinationBranchId, string Reference, DateTime accountingDate)
        {
            var transactionEntity = _mapper.Map<Transaction>(request);

            // Calculate charges
            //decimal charges = XAFWallet.CalculateCustomerCharges(
            //    account.Product.CashDepositParameters.FirstOrDefault(dl => dl.DepositType == request.DepositType)?.DepositFeeRate ?? 0,
            //    0,0,
            //    request.Amount);
            decimal charges = 0;
            // Create transaction history for customer
            transactionEntity = CreateTransaction(request, account, teller.Id, depositLimit, charges, sourceBranchId, destinationBranchId, isInterbranch, Reference);

            // Add the new Transaction entity to the repository
            _TransactionRepository.Add(transactionEntity);

            // Update Teller Account balances
            UpdateTellerAccountBalances(request.Amount, transactionEntity.Fee, teller, tellerAccount, transactionEntity, charges != 0 ? Events.ChargeOfDeposit.ToString() : Events.Deposit.ToString(), isInterbranch, sourceBranchId, destinationBranchId, accountingDate);

            // Update customer account
            UpdateCustomerAccount(account, transactionEntity, customer);

            return transactionEntity;
        }

        private Transaction CreateTransaction(LoanRepaymentCommand request, Account account, string TellerId, CashDepositParameter deposit, decimal charges, string sourceBranchId, string destinationBranchId, bool isInterBranch, string Reference)
        {

            var transactionEntityEntryFee = new Transaction
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                Status = TransactionStatus.COMPLETED.ToString(),
                TransactionReference = Reference,
                TransactionType = deposit.DepositType,
                AccountNumber = account.AccountNumber,
                PreviousBalance = account.Balance,
                AccountId = account.Id,
                ProductId = account.ProductId,
                SenderAccountId = account.Id,
                ReceiverAccountId = account.Id,
                BankId = _userInfoToken.BankID,
                Operation = TransactionType.LR_DEPOSIT.ToString(),
                BranchId = _userInfoToken.BranchID,
                OriginalDepositAmount = request.Amount,
                Fee = charges,
                Tax = 0,
                Amount = request.Amount - charges,
                Note = string.IsNullOrEmpty(request.Note) ? $"{TransactionType.LR_DEPOSIT.ToString()} the sum of {BaseUtilities.FormatCurrency(request.Amount)} to {account.AccountNumber}" : request.Note,
                OperationType = OperationType.Credit.ToString(),
                FeeType = Events.ChargeOfDeposit.ToString(),
                TellerId = TellerId,
                DepositerNote = request.DepositerNote,
                DepositerTelephone = request.DepositerTelephone,
                DepositorIDNumber = request.DepositorIDNumber,
                DepositorIDExpiryDate = request.DepositorIDExpiryDate,
                DepositorIDIssueDate = request.DepositorIDIssueDate,
                DepositorIDNumberPlaceOfIssue = request.DepositorIDNumberPlaceOfIssue,
                IsDepositDoneByAccountOwner = request.IsDepositDoneByAccountOwner,
                DepositorName = request.DepositorName,
                Balance = account.Balance + (request.Amount - charges),
                Credit = request.Amount - charges,
                Debit = 0,
                DestinationBrachId = destinationBranchId,
                SourceBrachId = sourceBranchId,
                IsInterBrachOperation = isInterBranch,
                DestinationBranchCommission = isInterBranch ? XAFWallet.CalculateCommission(deposit.DestinationBranchOfficeShare, charges) : 0,
                SourceBranchCommission = isInterBranch ? XAFWallet.CalculateCommission(deposit.SourceBrachOfficeShare, charges) : charges
            };
            return transactionEntityEntryFee;
        }

        private void UpdateTellerAccountBalances(decimal depositAmount, decimal fee, Teller teller, Account tellerAccount, Transaction transaction, string eventType, bool isInterbranch, string sourceBranchId, string destinationBranchId, DateTime accountingDate)
        {
            if (isInterbranch)
            {
                // For inter-branch transactions
                var tellerOperationDeposit = CreateTellerOperation(depositAmount, OperationType.Debit, teller, tellerAccount, tellerAccount.Balance, transaction, eventType, isInterbranch, sourceBranchId, destinationBranchId, accountingDate);
                _tellerOperationRepository.Add(tellerOperationDeposit);

                if (fee > 0)
                {
                    // If there is a fee, create a teller operation for fee charges
                    var tellerOperationFeeCharges = CreateTellerOperation(transaction.SourceBranchCommission, OperationType.Credit, teller, tellerAccount, tellerAccount.Balance, transaction, Events.ChargeOfDeposit.ToString(), isInterbranch, sourceBranchId, destinationBranchId, accountingDate);
                    _tellerOperationRepository.Add(tellerOperationFeeCharges);
                }


            }
            else
            {
                // For regular branch transactions
                // Update Teller Account balances for deposit
                tellerAccount.PreviousBalance = tellerAccount.Balance;
                tellerAccount.Balance -= depositAmount;

                // Create Teller Operation for deposit
                var tellerOperationDeposit = CreateTellerOperation(depositAmount, OperationType.Debit, teller, tellerAccount, tellerAccount.Balance, transaction, eventType, isInterbranch, sourceBranchId, destinationBranchId, accountingDate);
                _tellerOperationRepository.Add(tellerOperationDeposit);

                // Credit Teller Account for the entry fee
                if (fee > 0)
                {
                    // Create Teller Operation for fee charges
                    var tellerOperationFeeCharges = CreateTellerOperation(fee, OperationType.Credit, teller, tellerAccount, tellerAccount.Balance, transaction, Events.ChargeOfDeposit.ToString(), isInterbranch, sourceBranchId, destinationBranchId, accountingDate);
                    _tellerOperationRepository.Add(tellerOperationFeeCharges);
                }

                // Update the teller account balance
                _AccountRepository.Update(tellerAccount);
            }
        }


        private TellerOperation CreateTellerOperation(decimal amount, OperationType operationType, Teller teller, Account tellerAccount, decimal currentBalance, Transaction transaction, string eventType, bool isInterBranch, string sourceBranchId, string destinationBranchId, DateTime accountingDate)
        {
            return new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = amount,
                Description = $"{transaction.TransactionType} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                OpenOfDayReference = tellerAccount.OpenningOfDayReference,
                OperationType = operationType.ToString(),
                BranchId = tellerAccount.BranchId,
                BankId = tellerAccount.BankId,
                CurrentBalance = currentBalance,
                Date = accountingDate,
                AccountingDate = accountingDate,
                CustomerId = transaction.CustomerId,
                MemberAccountNumber = transaction.AccountNumber,
                MemberName = "N/A",
                ReferenceId = transaction.TransactionReference,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = teller.Id,
                TransactionReference = transaction.TransactionReference,
                TransactionType = transaction.TransactionType,
                UserID = _userInfoToken.Id,
                EventName = eventType,
                DestinationBrachId = destinationBranchId,
                SourceBranchId = sourceBranchId,
                IsInterBranch = isInterBranch,
                DestinationBranchCommission = transaction.DestinationBranchCommission,
                SourceBranchCommission = transaction.SourceBranchCommission,

            };
        }

        private void UpdateCustomerAccount(Account account, Transaction transactionEntityEntryFee, CustomerDto customer)
        {
            // Check if the account status is 'In Progress'
            if (account.Status == AccountStatus.Inprogress.ToString())
            {
                // If in progress, update account details for activation
                account.Balance = transactionEntityEntryFee.Amount; // Set balance to the transaction amount
                account.PreviousBalance = 0; // Previous balance is 0
                account.Status = AccountStatus.Active.ToString(); // Update account status to 'Active'
                account.AccountName = $"{account.Product.Name}-{customer.FirstName} {customer.LastName}"; // Update account name
                account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance
            }
            else
            {
                // If not in progress, update account details for normal transaction
                account.PreviousBalance = account.Balance; // Set previous balance to current balance
                account.Balance = transactionEntityEntryFee.Balance; // Set balance to the balance brought forward from the transaction
                account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance
            }

            // Update the account in the repository
            _AccountRepository.Update(account);
        }






    }
}
