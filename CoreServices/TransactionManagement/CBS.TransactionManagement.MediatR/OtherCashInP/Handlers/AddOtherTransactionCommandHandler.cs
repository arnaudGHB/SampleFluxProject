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
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Repository.OtherCashIn;
using CBS.TransactionManagement.otherCashIn.Commands;
using CBS.TransactionManagement.Data.Dto.OtherCashInP;
using CBS.TransactionManagement.Repository.AccountServices;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data.Entity.OtherCashInP;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using AutoMapper.Internal;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using Irony.Parsing;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.otherCashIn.Handlers
{
    /// <summary>
    /// Handles the command to add a new OtherTransaction.
    /// </summary>
    public class AddOtherTransactionCommandHandler : IRequestHandler<AddOtherTransactionCommand, ServiceResponse<OtherTransactionDto>>
    {
        private readonly IOtherTransactionRepository _OtherTransactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly IWithdrawalServices _withdrawalServices;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddOtherTransactionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository; // Repository for accessing teller data.
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository; // Repository for accessing teller provisioning account data.
        private readonly ITellerRepository _tellerRepository; // Repository for accessing teller data.
        private readonly IConfigRepository _configRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for accessing account data.

        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddOtherTransactionCommandHandler.
        /// </summary>
        /// <param name="OtherTransactionRepository">Repository for OtherTransaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddOtherTransactionCommandHandler(
            IOtherTransactionRepository OtherTransactionRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddOtherTransactionCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            IAccountRepository accountRepository = null,
            IWithdrawalServices withdrawalServices = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerRepository tellerRepository = null,
            IConfigRepository configRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            ITransactionRepository transactionRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null,
            IAPIUtilityServicesRepository utilityServicesRepository = null)
        {
            _OtherTransactionRepository = OtherTransactionRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _accountRepository = accountRepository;
            _withdrawalServices = withdrawalServices;
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _tellerProvisioningAccountRepository = tellerProvisioningAccountRepository;
            _tellerRepository = tellerRepository;
            _configRepository = configRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _transactionRepository = transactionRepository;
            _accountingDayRepository = accountingDayRepository;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
            _utilityServicesRepository=utilityServicesRepository;
        }


        public async Task<ServiceResponse<OtherTransactionDto>> Handle(AddOtherTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);
                // Using Split method

                var transactionDto = new TransactionDto();
                // Check if system configuration is set.
                var config = await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString());
                // Check if the user account serves as a teller today.
                var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();

                // Check if the accounting day is still open.
                await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate);

                // Retrieve teller information.
                var teller = await _tellerRepository.RetrieveTeller(dailyTeller);
                // Retrieve sub teller account.
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Generate transaction reference.
                string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, request.TransactionType, false);
                var account = new Account();
                var customer = new CustomerDto();
                // Map request to OtherTransaction entity
                var OtherTransactionEntity = _mapper.Map<OtherTransaction>(request);

                // Process income transactions
                if (request.TransactionType == OtherCashInType.Income.ToString())
                {
                    // Handle Member Account source type
                    if (request.SourceType == OtherCashInSourceType.Member_Account.ToString())
                    {
                        account = await _accountRepository.GetAccount(request.AccountNumber, TransactionType.WITHDRAWAL.ToString().ToLower());

                        var bulkOp = new BulkOperation
                        {
                            CustomerId = request.CustomerId,
                            AccountNumber = request.AccountNumber,
                            Amount = request.Amount,
                            Total = request.Amount,
                            currencyNotes = request.CurrencyNotesRequest,
                            OperationType = TransactionType.WITHDRAWAL.ToString()
                        };
                        // Generate transaction reference for withdrawal
                        reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.OtherCashIn_Income_Withdrawal.ToString(), false);

                        // Create a list and add the BulkOperations object to it
                        var BulkOperations = new List<BulkOperation> { bulkOp };

                        // Retrieve customer information.
                        customer = await _utilityServicesRepository.GetCustomer(request.CustomerId);

                        // Retrieve branch information.
                        var branch = await _utilityServicesRepository.GetBranch(customer.BranchId);
                        OtherTransactionEntity.MemberName = customer.FirstName + " " + customer.LastName;

                        // Check if customer branch matches teller branch
                        if (customer.BranchId != teller.BranchId)
                        {
                            var errorMessage = $"Operation failed. Other cash-in or payment services do not support inter-branch operations. Please advise the member to perform a withdrawal before making a payment.";
                            _logger.LogError(errorMessage); // Log error message.
                            await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.OtherCashIn, LogLevelInfo.Warning);

                            throw new InvalidOperationException(errorMessage); // Throw exception.
                        }

                        // Retrieve currency notes
                        var currencyNote = await RetrieveCurrencyNotes(reference, bulkOp.currencyNotes);

                        // Map bulk operations to bulk deposit list and process transactions
                        var bulkWithdrawals = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(BulkOperations);
                        var trasactions = await ProcessBulkTransactions(bulkWithdrawals, teller, tellerAccount, false, customer, currencyNote.Data.ToList(), branch, reference, config, accountingDate);
                    }
                    else
                    {
                        // Generate transaction reference for other income type
                        reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.OtherCashIn_Income.ToString(), false);
                    }
                }
                else
                {
                    OtherTransactionEntity.Description = request.Naration;
                    //// Generate transaction reference for expense type
                    //reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.OtherCashIn_Expense.ToString(), false);
                }

                OtherTransactionEntity.CreatedDate = BaseUtilities.UtcNowToDoualaTime();
                OtherTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
                OtherTransactionEntity.TransactionReference = reference;
                OtherTransactionEntity.TellerId = teller.Id;
                OtherTransactionEntity.MemberName = request.Name;
                OtherTransactionEntity.BranchId = teller.BranchId;
                OtherTransactionEntity.BankId = teller.BankId;
                OtherTransactionEntity.AmountInWord = BaseUtilities.ConvertToWords(request.Amount);
                OtherTransactionEntity.ReceiptTitle = $"CASH RECIEPT on {request.EnventName}: Reference: " + reference;
                OtherTransactionEntity.DateOfOperation = accountingDate;
                OtherTransactionEntity.Narration = $"{request.TransactionType}, {request.EnventName}. Amount: {BaseUtilities.FormatCurrency(request.Amount)}";
                OtherTransactionEntity.Description = $"{OtherTransactionEntity.Narration}, {OtherTransactionEntity.Description}";
                bool isCashOperation = false;
                // Determine the direction based on TransactionType
                if (request.TransactionType == OtherCashInType.Income.ToString())
                {
                    OtherTransactionEntity.Direction = OperationType.Credit.ToString();
                    OtherTransactionEntity.Credit = request.Amount;
                    OtherTransactionEntity.Debit = 0;
                    OtherTransactionEntity.Amount = request.Amount;
                    // Update Teller Account balances for Income
                    if (request.SourceType == OtherCashInSourceType.Cash_Collected.ToString())
                    {
                        isCashOperation = true;
                        var currencyNote = await RetrieveCurrencyNotes(reference, request.CurrencyNotesRequest);
                        var subTellerProvioning = _subTellerProvioningHistoryRepository.CashInByDinomination(request.Amount, request.CurrencyNotesRequest, teller.Id, accountingDate, tellerAccount.OpenningOfDayReference);

                        _accountRepository.CreditAccount(tellerAccount, request.Amount);
                        CreateTellerOperation(request.Amount, OperationType.Credit, dailyTeller, tellerAccount, reference, TransactionType.OTHER_CASH_IN.ToString(), request.EnventName, accountingDate, request.Name, request.AccountNumber, request.CustomerId, isCashOperation);
                        var TellerBranch = await GetBranch(teller.BranchId);
                        var cashOperation = new CashOperation(teller.BranchId, request.Amount, 0, TellerBranch.name, TellerBranch.branchCode, CashOperationType.OtherCashIn, LogAction.OtherCashIn, subTellerProvioning);
                        await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

                    }
                    else
                    {
                        isCashOperation = false;
                        CreateTellerOperation(request.Amount, OperationType.Credit, dailyTeller, tellerAccount, reference, TransactionType.OTHER_CASH_IN.ToString(), request.EnventName, accountingDate, request.Name, request.AccountNumber, request.CustomerId, isCashOperation);

                    }
                }
                else
                {
                    isCashOperation = true;
                    OtherTransactionEntity.Direction = OperationType.Debit.ToString();
                    OtherTransactionEntity.Credit = 0;
                    OtherTransactionEntity.Debit = request.Amount;
                    OtherTransactionEntity.Amount = -(request.Amount);
                    var currencyNote = await RetrieveCurrencyNotes(reference, request.CurrencyNotesRequest);
                    var subTellerProvioning = _subTellerProvioningHistoryRepository.CashOutByDenomination(request.Amount, request.CurrencyNotesRequest, teller.Id, accountingDate, 0, false, tellerAccount.OpenningOfDayReference);
                    // Update Teller Account balances for expense
                    _accountRepository.DebitAccount(tellerAccount, request.Amount);
                    CreateTellerOperation(request.Amount, OperationType.Debit, dailyTeller, tellerAccount, reference, TransactionType.OTHER_CASH_PAYMENT.ToString(), request.EnventName, accountingDate, request.Name, request.AccountNumber, request.CustomerId, isCashOperation);
                    var TellerBranch = await GetBranch(teller.BranchId);
                    var cashOperation = new CashOperation(teller.BranchId, request.Amount, 0, TellerBranch.name, TellerBranch.branchCode, CashOperationType.OtherCashOut, LogAction.OtherCashOut, subTellerProvioning);
                    //await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);


                }
                // Retrieve currency notes
                //var currencyNotes = await RetrieveCurrencyNotes(reference, request.CurrencyNotesRequest);

                if (request.SourceType != OtherCashInSourceType.Member_Account.ToString() && request.TransactionType == OtherCashInType.Income.ToString())
                {
                    //var trans = CurrencyNotesMapper.MapTransaction(OtherTransactionEntity, _userInfoToken, account);
                    //trans.Account = null;
                    //trans.Teller = null;
                    //_transactionRepository.Add(trans);
                }
                var transaction = CurrencyNotesMapper.MapOtherTransaction(OtherTransactionEntity);
                transaction.Teller = teller;

                // Add the new OtherTransaction entity to the repository
                _OtherTransactionRepository.Add(OtherTransactionEntity);
                await _uow.SaveAsync();
                string msg = null;
                // Call accounting to process the transaction
                if (request.SourceType == OtherCashInSourceType.Cash_Collected.ToString())
                {
                    var result = await PostAccounting(request.Amount, reference, request, accountingDate,customer);
                    if (result)
                    {
                        // Map the entity to DTO
                        msg = $"Other cash payment of {request.Amount} completed successfully.";
                        // Log and audit the operation
                        await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.OtherCashIn, LogLevelInfo.Information);

                        // Return the response
                        return ServiceResponse<OtherTransactionDto>.ReturnResultWith200(transaction, msg);
                    }
                }
                else
                {

                    if (request.TransactionType == OtherCashInType.Expense.ToString())
                    {
                        var result = await PostAccounting(request.Amount, reference, request, accountingDate, customer);
                        if (result)
                        {
                            // Map the entity to DTO
                            msg = $"Other cash payment of {request.Amount} completed successfully.";
                            // Log and audit the operation
                            await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.OtherCashIn, LogLevelInfo.Information);

                            // Return the response
                            return ServiceResponse<OtherTransactionDto>.ReturnResultWith200(transaction, msg);
                        }
                    }
                    else
                    {
                        // Call Accounting to debit cash and credit class 7
                        var result = await MakeAccountingPostingEvent(request.Amount, request.EventCode, reference, accountingDate, OtherTransactionEntity.TransactionType, isCashOperation, account.ProductId);
                        if (result)
                        {
                            // Map the entity to DTO
                            msg = $"Other cash payment of {request.Amount} completed successfully.";
                            // Log and audit the operation
                            await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.OtherCashIn, LogLevelInfo.Information);

                            // Return the response
                            return ServiceResponse<OtherTransactionDto>.ReturnResultWith200(transaction, msg);
                        }
                    }


                }
                // Map the entity to DTO
                msg = $"Other cash payment of {request.Amount} completed successfully. with error posting event.";
                // Log and audit the operation
                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.OtherCashIn, LogLevelInfo.Information);

                // Return the response
                return ServiceResponse<OtherTransactionDto>.ReturnResultWith200(transaction, msg);

            }
            catch (Exception e)
            {
                // Handle and log the error
                var errorMessage = $"Error occurred while saving income: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.OtherCashIn, LogLevelInfo.Error);
                return ServiceResponse<OtherTransactionDto>.Return500(errorMessage);
            }
        }
        private AddEventMemberAccountPostingCommand MakeOtherCashIn(decimal amount, string eventCode, string transactionReferenceId, string eventName, Account account, DateTime accountingDay)
        {
            // Create a new AutoPostingEventCommand instance
            var addAccountingPostingCommand = new AddEventMemberAccountPostingCommand
            {
                Source = TellerSources.Members_Account.ToString(),
                AmountEventCollections = new List<AmountEventCollection>(),
                TransactionReferenceId = transactionReferenceId,
                ProductId = account.ProductId,
            };

            // Add a new AmountEventCollection to the AmountEventCollections list
            addAccountingPostingCommand.AmountEventCollections.Add(new AmountEventCollection
            {
                Amount = amount,
                EventCode = eventCode,
                Naration = $"Payment of {eventName} from member's {account.AccountType}. [Amount: {BaseUtilities.FormatCurrency(amount)}]", // Set the narration
            });

            return addAccountingPostingCommand;
        }
        /// <summary>
        /// Prepares an accounting posting event command for recording a transaction in the accounting system.
        /// </summary>
        /// <param name="amount">The amount involved in the transaction.</param>
        /// <param name="transactionReferenceId">The unique transaction reference ID.</param>
        /// <param name="command">The command containing event details like event code and name.</param>
        /// <param name="accountingDay">The date of the accounting event.</param>
        /// <param name="MemberReference">The reference ID of the member associated with the transaction.</param>
        /// <param name="MemberName">The name of the member associated with the transaction.</param>
        /// <returns>Returns a fully populated <see cref="AutoPostingEventCommand"/> object.</returns>
        private AutoPostingEventCommand MakeAccountingPostingEvent(
            decimal amount,
            string transactionReferenceId,
            AddOtherTransactionCommand command,
            DateTime accountingDay,
            string MemberReference,
            string MemberName)
        {
            // Step 1: Create an AutoPostingEventCommand instance and initialize its properties
            var addAccountingPostingCommand = new AutoPostingEventCommand
            {
                Source = TellerSources.Physical_Teller.ToString(), // Indicate the source of the transaction
                AmountEventCollections = new List<AmountEventCollection>(), // Initialize the collection of event amounts
                TransactionReferenceId = transactionReferenceId, // Set the unique transaction reference
                TransactionDate = accountingDay, // Set the accounting date
                MemberReference = MemberReference // Associate the transaction with the member reference
            };

            // Step 2: Construct a detailed narration for the transaction
            string detailedNarration =
                $"{command.EnventName}: " +
                $"[Amount: {BaseUtilities.FormatCurrency(amount)}], " +
                $"Member Details: [Name: {MemberName}, ID: {MemberReference}], " +
                $"Transaction Reference: {transactionReferenceId}, " +
                $"Date: {accountingDay:yyyy-MM-dd HH:mm:ss}.";

            // Step 3: Add the transaction details to the AmountEventCollections list
            addAccountingPostingCommand.AmountEventCollections.Add(new AmountEventCollection
            {
                Amount = amount, // Set the transaction amount
                EventCode = command.EventCode, // Set the event code from the command
                Naration = detailedNarration // Use the detailed narration
            });

            // Step 4: Return the prepared AutoPostingEventCommand object
            return addAccountingPostingCommand;
        }

        private async Task<bool> PostAccounting(decimal Amount, string TransactionRefenceId, AddOtherTransactionCommand command, DateTime accountingDay, CustomerDto customer)
        {
            var apiRequest = MakeAccountingPostingEvent(Amount, TransactionRefenceId, command, accountingDay, customer.CustomerId, customer.Name); // Create accounting posting request.
            apiRequest.TransactionDate = accountingDay;
            var result = await _mediator.Send(apiRequest); // Send request to _mediator.
            if (result.StatusCode != 200)
            {
                return false; // Append error message to response messages.
            }
            return true; // Return accounting response messages.
        }
        // Updates the balance of the teller account
        private void UpdateTellerAccountBalance(Account tellerAccount, decimal amount, OperationType operationType)
        {
            tellerAccount.PreviousBalance = tellerAccount.Balance;
            tellerAccount.Balance += operationType == OperationType.Credit ? amount : -amount;
            _accountRepository.Update(tellerAccount);
        }
        //TransactionID = transaction.Id,
        //        TransactionRef = transaction.TransactionReference,
        //        TransactionType = transaction.TransactionType,
        private void CreateTellerOperation(decimal amount, OperationType operationType, DailyTeller teller, Account tellerAccount,
            string TransactionReference, string TransactionType,
            string eventType, DateTime accountingDate, string name, string accountnumber, string memberReference, bool isCashOperation)
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
                OpenOfDayReference = tellerAccount.OpenningOfDayReference,
                CurrentBalance = tellerAccount.Balance,
                Date = accountingDate,
                AccountingDate = accountingDate,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = teller.TellerId,
                TransactionReference = TransactionReference,
                TransactionType = TransactionType,
                UserID = teller.UserName,
                Description = $"{TransactionType} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode),
                CustomerId = memberReference,
                MemberAccountNumber = accountnumber,
                EventName = eventType,
                MemberName = name,
                DestinationBrachId = teller.BranchId,
                SourceBranchId = teller.BranchId,
                IsInterBranch = false,
                DestinationBranchCommission = 0,
                SourceBranchCommission = 0,
                IsCashOperation = isCashOperation,

            };
            _tellerOperationRepository.Add(tellerOperation);
        }

        // Method to retrieve branch information
        private async Task<BranchDto> GetBranch(string branchid)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = branchid }; // Create command to get branch.
            var branchResponse = await _mediator.Send(branchCommandQuery); // Send command to _mediator.

            // Check if branch information retrieval was successful
            if (branchResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed to retrieve branch information: {branchResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return branchResponse.Data; // Return branch data.
        }
        // Method to retrieve currency notes
        private async Task<ServiceResponse<List<CurrencyNotesDto>>> RetrieveCurrencyNotes(string reference, CurrencyNotesRequest currencyNotesRequest)
        {
            var addCurrencyNotesCommand = new AddCurrencyNotesCommand { CurrencyNote = currencyNotesRequest, Reference = reference }; // Create command to add currency notes.
            var currencyNoteResponse = await _mediator.Send(addCurrencyNotesCommand); // Send command to _mediator.

            if (currencyNoteResponse.StatusCode != 200)
            {
                return ServiceResponse<List<CurrencyNotesDto>>.Return403(""); // Return error response if currency notes retrieval fails.
            }
            return currencyNoteResponse; // Return currency notes data.
        }
        // Method to process bulk transactions
        private async Task<List<TransactionDto>> ProcessBulkTransactions(List<BulkDeposit> requests, Teller teller, Account tellerAccount, bool isInterBranchOperation, CustomerDto customer, List<CurrencyNotesDto> currencyNotes, BranchDto branch, string reference, Config config, DateTime accountingDate)
        {
            var transactions = new List<TransactionDto>(); // Initialize list to store transactions.

            foreach (var request in requests)
            {
                var customerAccount = await _accountRepository.GetAccount(request.AccountNumber, TransactionType.WITHDRAWAL.ToString()); // Get customer account information.
                var customCharge = request.Fee; // Get custom charge for the transaction.
                // Set transaction properties
                request.Customer = customer;
                request.currencyNotes = currencyNotes;
                request.Branch = branch;
                request.IsExternalOperation = false;
                request.ExternalApplicationName = "N/A";
                request.ExternalReference = "N/A";
                request.SourceType = "Local";

                // Deposit amount into account
                var transaction = await _withdrawalServices.Withdrawal(teller, tellerAccount, request, isInterBranchOperation, _userInfoToken.BranchID, customer.BranchId, customCharge, reference, customerAccount, config, true, accountingDate, false, null);

                transactions.Add(transaction); // Add transaction to list.
            }

            return transactions; // Return list of transactions.
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


        private async Task LogAndAuditError(AddOtherTransactionCommand request, string errorMessage, int statusCode)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

        private async Task LogAndAuditInfo(AddOtherTransactionCommand request, string message, int statusCode)
        {
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, message, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }
        private async Task<bool> MakeAccountingPostingEvent(decimal amount, string eventCode, string transactionReferenceId, DateTime accountingDay, string typeOfOperation, bool isCashOperation, string productId)
        {
            var addEventPostingCommand = new AddEventMemberAccountPostingCommand
            {
                Source = isCashOperation ? TellerSources.Physical_Teller.ToString() : TellerSources.Members_Account.ToString(),
                TransactionReferenceId = transactionReferenceId,
                TransactionDate = accountingDay,
                ProductId= productId,
                AmountEventCollections = new List<AmountEventCollection>()


            };
            addEventPostingCommand.AmountEventCollections.Add(new AmountEventCollection
            {
                Amount = amount,
                EventCode = eventCode,
                Naration = $"Other {typeOfOperation}. Amount: {amount}",
            });
            var result = await _mediator.Send(addEventPostingCommand); // Send request to _mediator.
            if (result.StatusCode != 200)
            {
                return false; // Append error message to response messages.
            }
            return true; // Return accounting response messages.
        }
    }

}
