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

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transaction.
    /// </summary>
    public class InitialDepositCommandHandler : IRequestHandler<InitialDepositCommand, ServiceResponse<TransactionDto>>
    {
        private readonly ITransactionRepository _TransactionRepository;
        private readonly IConfigRepository _ConfigRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ITellerProvisioningAccountRepository _tellerProvisioningAccountRepository;
        public IMediator _mediator { get; set; }

        private readonly IDepositLimitRepository _DepositLimitRepository;
        private readonly ICurrencyNotesRepository _CurrencyNotesRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly ISavingProductRepository _SavingProductRepository;
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Transaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<InitialDepositCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddTransactionCommandHandler.
        /// </summary>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="SavingProductRepository">Repository for Transaction data access.</param>
        /// <param name="DepositLimitRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public InitialDepositCommandHandler(
            ITransactionRepository TransactionRepository,
            IConfigRepository ConfigRepository,
            ICurrencyNotesRepository CurrencyNotesRepository,
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IDepositLimitRepository DepositLimitRepository,
            ISavingProductRepository SavingProductRepository,
            IMapper mapper,
            ILogger<InitialDepositCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            ITellerRepository tellerRepository = null,
            ITellerProvisioningAccountRepository tellerProvisioningAccountRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            IMediator mediator = null)
        {
            _AccountRepository = AccountRepository;
            _CurrencyNotesRepository = CurrencyNotesRepository;
            _ConfigRepository = ConfigRepository;
            _TransactionRepository = TransactionRepository;
            _SavingProductRepository = SavingProductRepository;
            _userInfoToken = UserInfoToken;
            _DepositLimitRepository = DepositLimitRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _tellerRepository = tellerRepository;
            _tellerProvisioningAccountRepository = tellerProvisioningAccountRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddTransactionCommand to add a new Transaction.
        /// </summary>
        /// <param name="request">The AddTransactionCommand containing Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<TransactionDto>> Handle(InitialDepositCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //GetCustomerCommandQuery


                // Check if the user account serves as a teller today
                var tellerProvision = await _tellerProvisioningAccountRepository
                    .FindBy(t => t.UserIDInChargeOfTeller == _userInfoToken.Id && t.ActiveStatus && t.Date.Date == DateTime.Today.Date && !t.IsPrimaryTeller)
                    .FirstOrDefaultAsync();

                if (tellerProvision == null)
                {
                    var errorMessage = $"Your account does not serve the purpose of a teller today.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return403(errorMessage);
                }

                // Retrieve teller information
                var teller = await _tellerRepository
                    .FindBy(t => t.Id == tellerProvision.TellerID && !t.IsPrimary)
                    .FirstOrDefaultAsync();

                if (teller == null)
                {
                    var errorMessage = $"Teller {tellerProvision.TellerID} does not exist.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }

                // Retrieve sub teller account
                var tellerAccount = await _AccountRepository.FindBy(t => t.TellerId == tellerProvision.TellerID)
                    .FirstOrDefaultAsync();
                if (tellerAccount == null)
                {
                    var errorMessage = $"Teller {teller.Name} does not have an account";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<TransactionDto>.Return404();
                }

                // Check if teller has sufficient funds
                if (tellerAccount.Balance - request.Amount < 0)
                {
                    var errorMessage = $"Your teller account balance {BaseUtilities.FormatCurrency(tellerAccount.Balance)} does not have sufficient funds to proceed with this operation.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<TransactionDto>.Return404();
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
                        return ServiceResponse<TransactionDto>.Return404(errorMessage);
                    }

                    if (config.Name.Equals("IsYearOpen") && !config.Value.Equals("True"))
                    {
                        var errorMessage = $"Year is not open";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<TransactionDto>.Return404(errorMessage);
                    }
                }

                // Map currency notes from request
                var mappedDictionary = CurrencyNotesMapper.MapCurrencyNotesToDictionary(request.CurrencyNotes);

                // Check if mapped dictionary is empty
                if (mappedDictionary.IsNullOrEmpty())
                {
                    var errorMessage = $"Error occurred while initiating Transaction, no notes supplied for cash deposit";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }

                // Create XAFWallet and calculate total deposited amount
                var totalCashDeposited = XAFWallet.CollectAndCalculateTotalValue(mappedDictionary);

                // Check if requested amount matches collected notes
                if (request.Amount != totalCashDeposited)
                {
                    var errorMessage = $"Error occurred while initiating Transaction, collected notes don't correspond to entered amount";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }

                // Map currency notes to entity
                Dictionary<XAFDenomination, int> currencyNotes = CurrencyNotesMapper.MapCurrencyNotesToDictionary(request.CurrencyNotes);
                var currencyNotesEntity = XAFWallet.MapCurrencyNotesEntity(currencyNotes);
                _CurrencyNotesRepository.Add(currencyNotesEntity);

                // Validate deposit limits
                var account = await _AccountRepository
                    .FindBy(a => a.AccountNumber == request.AccountNumber)
                    .Include(a => a.Product)
                    .ThenInclude(p => p.CashDepositParameters)
                    .Include(a => a.Product.EntryFeeParameters)
                    .FirstOrDefaultAsync();
                var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = account.CustomerId };
                var serviceResponse = await _mediator.Send(customerCommandQuery);
                if (serviceResponse.StatusCode == 200)
                {
                    var customer = serviceResponse.Data;
                    if (customer.MembershipApprovalStatus.ToLower() == AccountStatus.APPROVED.ToString().ToLower())
                    {
                        var depositLimits = account.Product.CashDepositParameters.ToList();
                        var depositLimit = depositLimits.FirstOrDefault(dl => dl.DepositType == DepositType.CASH_INITIAL_DEPOSIT.ToString());
                        if (depositLimit == null)
                        {
                            var errorMessage = $"Error occurred while initiating deposit with amount: please Set the CASH deposit limit before proceeding";
                            _logger.LogError(errorMessage);
                            return ServiceResponse<TransactionDto>.Return400(errorMessage);
                        }

                        if (DecimalComparer.IsLessThan(request.Amount, depositLimit.MinAmount) || DecimalComparer.IsGreaterThan(request.Amount, depositLimit.MaxAmount))
                        {
                            var errorMessage = $"Error occurred while initiating deposit with amount: {request.Amount}. Initial deposit amount must be between {depositLimit.MinAmount} and {depositLimit.MaxAmount}.";
                            _logger.LogError(errorMessage);
                            return ServiceResponse<TransactionDto>.Return400(errorMessage);
                        }

                        // Create transaction history for customer
                        var transactionEntity = await CreateTransactionHistory(request, account, teller, tellerAccount, currencyNotesEntity.Id, customer);
                        var transactionDto = CurrencyNotesMapper.MapTransactionToDto(transactionEntity);
                        await _uow.SaveAsync();
                        //Sending SMS.
                        var sMSPICallCommand = SendSMS(account, transactionEntity, customer);
                        var resultsms = await _mediator.Send(sMSPICallCommand);
                        transactionEntity.ProductId = account.ProductId;
                        var apiRequest = MakeAccountingPosting(request, depositLimit, account, transactionEntity);
                        var result = await _mediator.Send(apiRequest);
                        if (result.StatusCode == 200)
                        {
                            return ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto);
                        }
                        return ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto, result.Description);
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
        private SendSMSPICallCommand SendSMS(Account account, Transaction transaction, CustomerDto customer)
        {
            string bankName = "BapCCUL";
            // Encrypt the account number partially
            string encryptedAccountNumber = BaseUtilities.PartiallyEncryptAccountNumber(account.AccountNumber);

            // Construct the SMS message with the partially encrypted account number
            string msg = $"Dear {customer.FirstName} {customer.LastName}, Congratulations! Your account ({encryptedAccountNumber}) has been successfully opened with {bankName}. An initial deposit of {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} has been credited to your account. Transaction Reference: {transaction.TransactionRef}. Date and Time: {transaction.CreatedDate}. Your current balance is now {BaseUtilities.FormatCurrency(account.Balance)}. Thank you for choosing {bankName} as your banking partner.";
            return new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.Phone
            };
        }
        private AddAccountingPostingCommand MakeAccountingPosting(InitialDepositCommand request, CashDepositParameter depositParameter, Account account, Transaction transaction)
        {
            return new AddAccountingPostingCommand
            {
                accountHolder = account.AccountName,
                operationType = TransactionType.DEPOSIT.ToString(),
                accountNumber = account.AccountNumber,
                productId = account.ProductId,
                productName = account.Product.Name,
                naration = "Initial Deposit/Account Activation",
                transactionReferenceId = transaction.TransactionRef,
                amountCollection = new List<AmountCollection>
        {
            new AmountCollection
            {
                 amount = request.Amount,
                isPrincipal = true,
               eventAttributeName =  OperationEventRubbriqueName.Pricipal_Saving_Account.ToString(),
            },new AmountCollection
            {
                 amount = transaction.Fee,
                isPrincipal = false,
                eventAttributeName = OperationEventRubbriqueName.Saving_Fee_Account.ToString(),
            }
        }
            };
        }

        private async Task<Transaction> CreateTransactionHistory(InitialDepositCommand request, Account account, Teller teller, Account tellerAccount, string currencyNoteId, CustomerDto customer)
        {
            var transactionEntity = new Transaction();

            // Calculate charges
            decimal charges = 0;

            if (account.Product.EntryFeeParameters != null && account.Product.EntryFeeParameters.Any())
            {
                charges = XAFWallet.CalculateCustomerCharges(
                    account.Product.EntryFeeParameters.FirstOrDefault()?.EntryFeeRate ?? 0,
                    account.Product.EntryFeeParameters.FirstOrDefault()?.EntryFeeFlat ?? 0,
                    request.Amount);
            }
            else
            {
                // If EntryFeeParameters is null or empty, create a new instance with default values
                EntryFeeParameter defaultEntryFeeParameters = new EntryFeeParameter
                {
                    EntryFeeRate = 0, // Default value for EntryFeeRate
                    EntryFeeFlat = 0   // Default value for EntryFeeFlat
                };

                // Perform the calculation using the default EntryFeeParameters
                charges = XAFWallet.CalculateCustomerCharges(
                    defaultEntryFeeParameters.EntryFeeRate,
                    defaultEntryFeeParameters.EntryFeeFlat,
                    request.Amount);
            }


            // Create transaction history for customer
            transactionEntity = CreateTransactionEntryFee(request, account, currencyNoteId, teller.Id, charges);

            // Add the new Transaction entity to the repository
            _TransactionRepository.Add(transactionEntity);

            // Update Teller Account balances
            UpdateTellerAccountBalances(request.Amount, charges, teller, tellerAccount, transactionEntity);

            // Update customer account
            UpdateCustomerAccount(account, transactionEntity, customer);

            return transactionEntity;
        }


        private Transaction CreateTransactionEntryFee(InitialDepositCommand request, Account account, string CurrencyNotesId, string tellerid, decimal charges)
        {
            var transactionEntityEntryFee = new Transaction
            {
                CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow),
                Id = BaseUtilities.GenerateUniqueNumber(),
                Status = TransactionStatus.COMPLETED.ToString(),
                TransactionRef = BaseUtilities.GenerateInsuranceUniqueNumber(17, "INIT"),
                TransactionType = TransactionType.INITIALDEPOSIT.ToString(),
                AccountNumber = account.AccountNumber,
                AccountId = account.Id,
                ProductId = account.ProductId,
                SenderAccountId = account.Id,
                ReceiverAccountId = account.Id, 
                BankId = request.BankId,
                BranchId = request.BranchId,
                OriginalDepositAmount = request.Amount,
                Fee = charges,
                Amount = request.Amount - charges,
                Note = "Initial Deposit/Account Activation",
                PreviousBalance = 0,
                CurrencyNotesId = CurrencyNotesId,
                OperationType = OperationType.Credit.ToString(),
                FeeType = Events.EntryFee.ToString(),
                TellerId = tellerid,
                BalanceBroughtForward = request.Amount - charges,
                Operation = TransactionType.DEPOSIT.ToString(),
                Tax = 0, Credit= request.Amount - charges, Debit=0
            };

            return transactionEntityEntryFee;

        }
        private void UpdateTellerAccountBalances(decimal depositAmount, decimal entryFee, Teller teller, Account tellerAccount, Transaction transaction)
        {
            // Debit Teller Account for the requested amount
            tellerAccount.PreviousBalance = tellerAccount.Balance;
            tellerAccount.Balance -= depositAmount;

            // Create Teller Operation for deposit
            var tellerOperationDeposit = CreateTellerOperation(depositAmount, OperationType.Debit, teller, tellerAccount, transaction, Events.InitialDeposit.ToString());
            _tellerOperationRepository.Add(tellerOperationDeposit);

            if (entryFee > 0)
            {
                // Credit Teller Account for the entry fee
                //tellerAccount.Balance += entryFee;

                // Create Teller Operation for entry fee charges
                var tellerOperationEntryFee = CreateTellerOperation(entryFee, OperationType.Credit, teller, tellerAccount, transaction, Events.EntryFee.ToString());
                _tellerOperationRepository.Add(tellerOperationEntryFee);


            }
            _AccountRepository.Update(tellerAccount);
        }

        //private void UpdateTellerAccountBalances(decimal depositAmount, decimal entryFee, Teller teller, Account tellerAccount,Transaction transaction)
        //{
        //    // Debit Teller Account for the requested amount

        //    tellerAccount.PreviousBalance = tellerAccount.Balance;
        //    tellerAccount.Balance -= depositAmount;
        //    var tellerOperationWithRequestedAmount = CreateTellerOperation(depositAmount, OperationType.Debit, teller, tellerAccount,transaction,Events.InitialDeposit.ToString());
        //    _tellerOperationRepository.Add(tellerOperationWithRequestedAmount);

        //    // Credit Teller Account for the entry fee
        //    tellerAccount.PreviousBalance = tellerAccount.Balance;
        //    tellerAccount.Balance += entryFee;
        //    var tellerOperationWithFeeCharges = CreateTellerOperation(entryFee, OperationType.Credit, teller, tellerAccount, transaction, Events.EntryFee.ToString());
        //    _tellerOperationRepository.Add(tellerOperationWithFeeCharges);
        //    _AccountRepository.Update(tellerAccount);
        //}

        private TellerOperation CreateTellerOperation(decimal amount, OperationType operationType, Teller teller, Account tellerAccount, Transaction transactionEntityEntryFee, string evenType)
        {
            return new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = amount,
                OperationType = operationType.ToString(),
                BranchId = tellerAccount.BranchId,
                BankId = tellerAccount.BankId,
                CurrentBalance = tellerAccount.Balance,
                Date = BaseUtilities.UtcToLocal(DateTime.UtcNow),
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = teller.Id,
                TransactionID = transactionEntityEntryFee.Id,
                TransactionRef = transactionEntityEntryFee.TransactionRef,
                TransactionType = transactionEntityEntryFee.TransactionType,
                UserID = _userInfoToken.Id,
                EventName = evenType
            };
        }

        private void UpdateCustomerAccount(Account account, Transaction transactionEntityEntryFee, CustomerDto customer)
        {
            account.Balance = transactionEntityEntryFee.Amount;
            account.PreviousBalance = 0;
            account.Status = AccountStatus.ACTIVE.ToString();
            account.AccountName = account.Product.Name + "-" + customer.FirstName + " " + customer.LastName;
            account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber);
            _AccountRepository.Update(account);
        }


    }

}
