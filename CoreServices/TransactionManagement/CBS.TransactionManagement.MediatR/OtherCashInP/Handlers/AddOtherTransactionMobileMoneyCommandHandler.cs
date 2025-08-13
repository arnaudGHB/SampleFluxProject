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
using DocumentFormat.OpenXml.Wordprocessing;
using System.Security.Principal;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.Repository.MongoDBManager.SerialNumberGenerator;

namespace CBS.TransactionManagement.otherCashIn.Handlers
{
    /// <summary>
    /// Handles the command to add a new OtherTransaction.
    /// </summary>
    public class AddOtherTransactionMobileMoneyCommandHandler : IRequestHandler<AddOtherTransactionMobileMoneyCommand, ServiceResponse<OtherTransactionDto>>
    {
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.

        private readonly IOtherTransactionRepository _OtherTransactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWithdrawalServices _withdrawalServices;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddOtherTransactionMobileMoneyCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository; // Repository for accessing teller data.
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository; // Repository for accessing teller provisioning account data.
        private readonly ITellerRepository _tellerRepository; // Repository for accessing teller data.
        private readonly IConfigRepository _configRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        public IMediator _mediator { get; set; }
        private readonly IDailyTransactionCodeGenerator _dailyTransactionCodeGenerator;
        /// <summary>
        /// Constructor for initializing the AddOtherTransactionMobileMoneyCommandHandler.
        /// </summary>
        /// <param name="OtherTransactionRepository">Repository for OtherTransaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddOtherTransactionMobileMoneyCommandHandler(
            IOtherTransactionRepository OtherTransactionRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddOtherTransactionMobileMoneyCommandHandler> logger,
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
            IDailyTransactionCodeGenerator dailyTransactionCodeGenerator = null)
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
            _dailyTransactionCodeGenerator=dailyTransactionCodeGenerator;
        }



        public async Task<ServiceResponse<OtherTransactionDto>> Handle(AddOtherTransactionMobileMoneyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the current accounting date for the branch.
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Step 2: Validate and prepare initial data for the request.
                await ValidateAndPrepareData(request);

                // Set the operation type to the booking direction from the request.
                request.OperationType = request.BookingDirection;
                var noneMemberAccountNumber = await _accountRepository.RetrieveNoneMemberMobileMoneyAccountByMemberReference(request.MemberReference); // Retrieve non-member account by member reference.

                // Step 3: Retrieve required information for the transaction.
                var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate(); // Retrieve an active sub-teller for the day.
                var teller = await RetrieveAndValidateTeller(dailyTeller, request, accountingDate); // Validate and get the main teller details.
                var customer = await GetCustomer(request.MemberReference); // Get customer details using the member reference.
                var branch = await GetBranch(customer.BranchId); // Retrieve branch information based on the customer's branch ID.
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller); // Get the main teller's account details.
                var mobileMoneyTeller = new Teller();

                // Check if there are multiple tellers in the branch
                var momoTellersInBranch = await _tellerRepository
                    .FindBy(x => x.BranchId == teller.BranchId && x.TellerType == request.SourceType).ToListAsync();

                if (momoTellersInBranch.Count == 1)
                {
                    // Only one teller in the branch, use teller.Id to retrieve the specific teller
                    mobileMoneyTeller = momoTellersInBranch.FirstOrDefault();
                    if (mobileMoneyTeller == null)
                    {
                        throw new InvalidOperationException(
                            $"The teller with ID '{teller.Id}' in branch '{branch.name}' (Code: {branch.branchCode}) could not be found. " +
                            "Please ensure the teller is configured correctly for this operation."
                        );
                    }

                }
                else if (momoTellersInBranch.Count > 1)
                {
                    // Multiple tellers exist, retrieve using TellerCode from the request
                    mobileMoneyTeller = await _tellerRepository.FindBy(x => x.Code == request.TellerCode && x.BranchId == teller.BranchId).FirstOrDefaultAsync();

                    if (mobileMoneyTeller == null)
                    {
                        throw new InvalidOperationException(
                            $"Multiple tellers exist in branch '{branch.name}' (Code: {branch.branchCode}), but no teller with code '{request.TellerCode}' was found. " +
                            "Please ensure the provided Teller Code is correct."
                        );
                    }

                    if (mobileMoneyTeller.MapMobileMoneyToNoneMemberMobileMoneyReference != noneMemberAccountNumber.AccountNumber)
                    {
                        throw new InvalidOperationException(
                            $"The provided {request.SourceType} reference '{request.MemberReference}' does not match the registered {mobileMoneyTeller.TellerType} account '{mobileMoneyTeller.MapMobileMoneyToNoneMemberMobileMoneyReference}' " +
                            $"linked to the teller with code '{request.TellerCode}' in branch '{branch.name}' (Branch Code: {branch.branchCode}). " +
                            "This indicates a mismatch between the account reference and the mobile money account registered for the specified teller. " +
                            "Please verify the teller code, account reference, and ensure the details align correctly with the mobile money registration before retrying."
                        );
                    }

                }
                else
                {
                    // No tellers found in the branch
                    throw new InvalidOperationException(
                        $"No {request.SourceType} tellers are configured for branch '{branch.name}' (Code: {branch.branchCode}). " +
                        "Please ensure the branch has at least one teller configured for this operation."
                    );
                }

                var mobileMoneyTellerAccount = await RetrieveMobileMoneyTellerAccount(request, mobileMoneyTeller.Code, mobileMoneyTeller.Id); // Retrieve the mobile money teller account details.

                // Step 4: Control inter-branch operations for mobile money.
                if (customer.BranchId != teller.BranchId)
                {
                    var errorMessage = $"{_userInfoToken.FullName}, Note that interbranch operations are not permitted with mobile money operations. Branch '{branch.name}' (Code: {branch.branchCode}). Please contact the administrator for more assistance.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.MTNorOrangeMoneyOperation, LogLevelInfo.Warning);
                    return ServiceResponse<OtherTransactionDto>.Return403(errorMessage);
                }

                // Step 5: Validate that the non-member account type matches the source type specified in the request.
                if (noneMemberAccountNumber.AccountType != request.SourceType)
                {
                    var errorMessage = $"{_userInfoToken.FullName}, You have entered the wrong non-member reference for {request.SourceType} operation. Branch '{branch.name}' (Code: {branch.branchCode}). Please enter the correct non-member reference for the selected operator service {request.SourceType}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.MTNorOrangeMoneyOperation, LogLevelInfo.Warning);
                    return ServiceResponse<OtherTransactionDto>.Return403(errorMessage);
                }

                noneMemberAccountNumber.Balance = mobileMoneyTellerAccount.Balance;
                noneMemberAccountNumber.PreviousBalance = mobileMoneyTellerAccount.PreviousBalance;
                noneMemberAccountNumber.EncryptedBalance = mobileMoneyTellerAccount.EncryptedBalance;
                // Step 6: Check for sufficient funds based on the transaction type.
                if (request.OperationType.ToLower() == TransactionType.WITHDRAWAL.ToString().ToLower())
                {
                    // Check if the teller account has enough balance for withdrawal.
                    if (tellerAccount.Balance - request.Amount < 0)
                    {
                        // Log and audit if insufficient funds, then return a forbidden response.
                        var errorMessage = $"{teller.Name}, You have insufficient funds to perform this operation. Your current balance is {BaseUtilities.FormatCurrency(tellerAccount.Balance)}. Make a cash requisition to proceed.";
                        await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.MTNorOrangeMoneyOperation, LogLevelInfo.Warning);
                        return ServiceResponse<OtherTransactionDto>.Return403(errorMessage);
                    }
                }

                if (request.OperationType.ToLower() == TransactionType.DEPOSIT.ToString().ToLower())
                {
                    // Check if the mobile money teller account has enough balance for deposit.
                    if (mobileMoneyTellerAccount.Balance - request.Amount < 0)
                    {
                        // Log and audit if insufficient funds, then return a forbidden response.
                        var errorMessage = $"{mobileMoneyTeller.Name}, You have insufficient funds to perform this operation. Your current balance is {BaseUtilities.FormatCurrency(mobileMoneyTellerAccount.Balance)}. Make a cash requisition to proceed.";
                        await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.MTNorOrangeMoneyOperation, LogLevelInfo.Warning);
                        return ServiceResponse<OtherTransactionDto>.Return403(errorMessage);
                    }
                }

                // Step 7: Generate a transaction reference and prepare the transaction entity for processing.
                string transactionReference = await _dailyTransactionCodeGenerator.ReserveTransactionCode(
                    _userInfoToken.BranchCode, OperationPrefix.Remittance, TransactionType.TRANSFER, isInterBranch: false);
                var transactionEntity = PrepareTransactionEntity(request, teller, transactionReference, mobileMoneyTeller.OperationEventCode, mobileMoneyTellerAccount.AccountNumber, accountingDate); // Prepare the transaction details.

                // Step 8: Process the transaction, updating relevant accounts and records.
                await ProcessTransaction(request, tellerAccount, mobileMoneyTellerAccount, transactionEntity, dailyTeller, noneMemberAccountNumber, accountingDate); // Perform the transaction.

                // Step 10: Save the transaction data, updating both mobile money teller account and non-member account.
                var transactionDto = await SaveTransactionData(transactionEntity, mobileMoneyTellerAccount, noneMemberAccountNumber);

                // Step 11: Send an SMS notification to the customer and post the accounting event.
                await SendSMS(request.TelephoneNumber, request.Amount, transactionReference, branch, 0, request.SourceType, request.OperationType, request.CustomerName); // Notify the customer.
                var result = await PostAccountingEvent(request.Amount, transactionReference, accountingDate, request.OperationType, mobileMoneyTeller); // Post accounting event.

                // Step 12: Log and audit the completion of the transaction and return the response.
                string msg = result == "OK"
                     ? $"Mobile money transaction of {BaseUtilities.FormatCurrency(request.Amount)} completed successfully."
                     : $"Mobile money transaction of {BaseUtilities.FormatCurrency(request.Amount)} completed successfully, but encountered an error while posting the accounting event.";

                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.MTNorOrangeMoneyOperation, LogLevelInfo.Information); // Log success message.

                transactionDto.Teller = teller; // Assign the teller details to the response DTO.

                return ServiceResponse<OtherTransactionDto>.ReturnResultWith200(transactionDto, msg); // Return success response.
            }
            catch (Exception e)
            {
                var errorMessage = $"An error occurred while processing the mobile money transaction: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.MTNorOrangeMoneyOperation, LogLevelInfo.Error);
                return ServiceResponse<OtherTransactionDto>.Return500(errorMessage);
            }
        }

        // Step 1: Validate and prepare initial data
        private async Task ValidateAndPrepareData(AddOtherTransactionMobileMoneyCommand request)
        {
            await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString()); // Check if system configuration is set
            request.TelephoneNumber = BaseUtilities.ProcessTelephoneNumber(request.TelephoneNumber); // Validate and format telephone number
        }

        // Step 2: Retrieve and validate teller information
        private async Task<Teller> RetrieveAndValidateTeller(DailyTeller dailyTeller, AddOtherTransactionMobileMoneyCommand request, DateTime accountingDate)
        {
            await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate); // Check if the accounting day is still open
            var teller = await _tellerRepository.RetrieveTeller(dailyTeller); // Retrieve teller information
            await _tellerRepository.CheckTellerOperationalRights(teller, request.OperationType, request.IsCashOperation); // Check teller operational rights
            return teller;
        }

        // Step 2: Retrieve mobile money teller account
        private async Task<Account> RetrieveMobileMoneyTellerAccount(AddOtherTransactionMobileMoneyCommand request,string tellerCode,string tellerid)
        {
            var account = request.OperationType == OperationType.Deposit.ToString()
                ? await _accountRepository.RetrieveMobileMoneyTellerAccountAndCheckBalance(_userInfoToken.BranchID, tellerid, request.Amount)
                : await _accountRepository.RetrieveMobileMoneyTellerAccountByTellerCodeAndBranchId(_userInfoToken.BranchID, tellerCode);
            return account;
        }

        

       

        private OtherTransaction PrepareTransactionEntity(AddOtherTransactionMobileMoneyCommand request, Teller teller, string reference, string eventCode, string accountNumber, DateTime accountingDay)
        {
            var entity = _mapper.Map<OtherTransaction>(request);
            entity.CreatedDate = BaseUtilities.UtcNowToDoualaTime();
            entity.Id = BaseUtilities.GenerateUniqueNumber();
            entity.TransactionReference = reference;
            entity.TellerId = teller.Id;
            entity.MemberName = request.CustomerName;
            entity.BranchId = teller.BranchId;
            entity.BankId = teller.BankId;
            entity.CNI = request.CNI;
            entity.AccountNumber = accountNumber;
            entity.Direction = request.BookingDirection == "Withdrawal" ? "Credit" : "Debit";
            entity.CustomerId = request.MemberReference;
            entity.TransactionType = "MobileMoney";
            entity.EnventName = request.OperationType;
            entity.EventCode = eventCode == null ? "N/A" : eventCode;
            entity.TelephoneNumber = request.TelephoneNumber;
            entity.AmountInWord = BaseUtilities.ConvertToWords(request.Amount);
            entity.ReceiptTitle = $"{request.SourceType} {request.OperationType.ToUpper()} CASH RECEIPT: Reference: " + reference;
            entity.DateOfOperation = accountingDay;
            entity.Narration = $"{request.SourceType}, {request.OperationType}. Amount: {BaseUtilities.FormatCurrency(request.Amount)}";
            entity.Description = $"{entity.Narration}, {entity.Description}";
            entity.Teller = null;
            return entity;
        }

        // Step 4: Process transaction
        private async Task ProcessTransaction(AddOtherTransactionMobileMoneyCommand request, Account tellerAccount, Account mobileMoneyTellerAccount, OtherTransaction transactionEntity, DailyTeller dailyTeller, Account noneMemberAccountNumber, DateTime AccountingDate)
        {
            string membersName = $"[{request.SourceType} {request.CustomerName} {request.TelephoneNumber}]";

            if (request.OperationType == OperationType.Deposit.ToString())
            {
                transactionEntity.Direction = OperationType.Credit.ToString();
                transactionEntity.Credit = request.Amount;
                transactionEntity.Debit = 0;
                transactionEntity.Amount = request.Amount;
                _accountRepository.DebitAccount(mobileMoneyTellerAccount, request.Amount);
                _accountRepository.DebitAccount(noneMemberAccountNumber, request.Amount);

                _accountRepository.CreditAccount(tellerAccount, request.Amount);
               var subTellerProvioning= _subTellerProvioningHistoryRepository.CashInByDinomination(request.Amount, request.CurrencyNotesRequest, tellerAccount.TellerId, AccountingDate,tellerAccount.OpenningOfDayReference);
                CreateTellerOperation(request.Amount, OperationType.Credit.ToString(), tellerAccount, transactionEntity.TransactionReference, TransactionType.CASH_IN.ToString(), request.SourceType, membersName, request.MemberReference, noneMemberAccountNumber.AccountNumber, AccountingDate);
                CreateMobileMoneyTellerOperation(request.Amount, OperationType.Debit.ToString(), mobileMoneyTellerAccount, transactionEntity.TransactionReference, TransactionType.CASH_IN.ToString(), request.SourceType, membersName, request.MemberReference, noneMemberAccountNumber.AccountNumber, AccountingDate, tellerAccount.OpenningOfDayReference);
                var TellerBranch = await GetBranch(tellerAccount.BranchId); // Get the teller branch information.
                if (request.SourceType == AccountType.MobileMoneyMTN.ToString())
                {
                    var cashOperation = new CashOperation(tellerAccount.BranchId, request.Amount, 0, TellerBranch.name, TellerBranch.branchCode, CashOperationType.MobileMoneyCashIn, LogAction.MTNorOrangeMoneyOperation, subTellerProvioning);
                    await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation); // Update the dashboard for MTN deposit.
                }
                else
                {
                    var cashOperation = new CashOperation(tellerAccount.BranchId, request.Amount, 0, TellerBranch.name, TellerBranch.branchCode, CashOperationType.OrangeMoneyCashIn, LogAction.MTNorOrangeMoneyOperation, subTellerProvioning);
                    await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation); // Update the dashboard for Orange Money deposit.
                }
            }
            else
            {
                transactionEntity.Direction = OperationType.Debit.ToString();
                transactionEntity.Credit = 0;
                transactionEntity.Debit = request.Amount;
                transactionEntity.Amount = -request.Amount;
                _accountRepository.CreditAccount(mobileMoneyTellerAccount, request.Amount);
                _accountRepository.CreditAccount(noneMemberAccountNumber, request.Amount);
                _accountRepository.DebitAccount(tellerAccount, request.Amount);
                var subTellerProvioning = _subTellerProvioningHistoryRepository.CashOutByDinomination(request.Amount, request.CurrencyNotesRequest, tellerAccount.TellerId, AccountingDate,tellerAccount.OpenningOfDayReference);
                CreateTellerOperation(request.Amount, OperationType.Debit.ToString(), tellerAccount, transactionEntity.TransactionReference, TransactionType.CASH_WITHDRAWAL.ToString(), request.SourceType, membersName, request.MemberReference, noneMemberAccountNumber.AccountNumber, AccountingDate);
                CreateMobileMoneyTellerOperation(request.Amount, OperationType.Credit.ToString(), mobileMoneyTellerAccount, transactionEntity.TransactionReference, TransactionType.CASH_WITHDRAWAL.ToString(), request.SourceType, membersName, request.MemberReference, noneMemberAccountNumber.AccountNumber, AccountingDate,tellerAccount.OpenningOfDayReference);
                var TellerBranch = await GetBranch(tellerAccount.BranchId); // Get the teller branch information.
                if (request.SourceType == AccountType.MobileMoneyMTN.ToString())
                {
                    var cashOperation = new CashOperation(tellerAccount.BranchId, request.Amount, 0, TellerBranch.name, TellerBranch.branchCode, CashOperationType.MobileMoneyCashOut, LogAction.MTNorOrangeMoneyOperation, subTellerProvioning);
                    await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation); // Update the dashboard for MTN withdrawal.
                }
                else
                {
                    var cashOperation = new CashOperation(tellerAccount.BranchId, request.Amount, 0, TellerBranch.name, TellerBranch.branchCode, CashOperationType.OrangeMoneyCashOut, LogAction.MTNorOrangeMoneyOperation, subTellerProvioning);
                    await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation); // Update the dashboard for Orange Money withdrawal.

                }
            }

        }

        // Step 5: Save transaction and related data
        private async Task<OtherTransactionDto> SaveTransactionData(OtherTransaction transactionEntity, Account mobileMoneyTellerAccount, Account noneMemberAccountNumber)
        {
            //var transaction = CurrencyNotesMapper.MapTransaction(transactionEntity, _userInfoToken, mobileMoneyTellerAccount);
            var otherTransactionDto = CurrencyNotesMapper.MapOtherTransaction(transactionEntity);
            var transaction = CurrencyNotesMapper.MapTransactionMobileMoney(transactionEntity, _userInfoToken, noneMemberAccountNumber, otherTransactionDto);
            transaction.Account = null;
            transaction.Teller = null;
            _transactionRepository.Add(transaction);
            _OtherTransactionRepository.Add(transactionEntity);
            await _uow.SaveAsync();

            return otherTransactionDto;
        }

        // Method to send SMS notification
        private async Task SendSMS(string telephoneNumber, decimal amount, string reference, BranchDto branch, decimal charge, string source, string operationType, string name)
        {
            // Determine operation type
            string operation = operationType == "Withdrawal" ? "withdrawn" : "received";
            string direction = null;
            if (operationType == "Withdrawal")
            {
                direction = "from";
            }
            else
            {
                direction = "to";
            }
            // Extract branch name or use a default value if branch is null
            string branchName = branch?.name ?? "";

            // Construct SMS message
            string msg = $"{name} ({telephoneNumber}), from {branchName.ToUpper()}, you have {operation} {BaseUtilities.FormatCurrency(amount)} {direction} your {source} account.\n" +
                         $"Transaction Reference: {reference}." +
                         $"\nCharge: {BaseUtilities.FormatCurrency(charge)}." +
                         $"\nDate and Time: {BaseUtilities.UtcNowToDoualaTime()}." +
                         $"\nThank you for banking with us.";
            // Create command to send SMS
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = telephoneNumber
            };

            // Send command to _mediator
            await _mediator.Send(sMSPICallCommand);
        }


        // Method to create accounting posting event
        private MobileMoneyOperationCommand MakeAccountingPostingEvent(decimal amount, string ReferenceId, DateTime accountingDate, string operationType, Teller Teller)
        {
            var moneyOperationCommand = new MobileMoneyOperationCommand
            {
                TellerSources = Teller.TellerType==AccountType.MobileMoneyMTN.ToString()? TellerSources.Virtual_Teller_MTN.ToString(): TellerSources.Virtual_Teller_Orange.ToString(),
                Amount = amount,
                OperationType = operationType,
                TransactionDate = accountingDate,
                TransactionReference = ReferenceId,
            };
            return moneyOperationCommand;
        }
        // Method to post accounting event
        private async Task<string> PostAccountingEvent(decimal amount, string transactionReferenceId, DateTime accountingDate,string operationType,Teller teller)
        {
            var apiRequest = MakeAccountingPostingEvent(amount, transactionReferenceId, accountingDate,operationType,teller);
            var result = await _mediator.Send(apiRequest);
            return result.StatusCode != 200 ? result.Message : "OK";
        }

        private void CreateTellerOperation(decimal amount, string operationType, Account tellerAccount,
            string TransactionReference, string TransactionType,
            string eventType, string MemberName, string memberReference, string accountNumber, DateTime AccountingDate)
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
                CurrentBalance = tellerAccount.Balance,
                Date = AccountingDate,
                AccountingDate = AccountingDate,
                OpenOfDayReference = tellerAccount.OpenningOfDayReference,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = tellerAccount.TellerId,
                TransactionReference = TransactionReference,
                TransactionType = TransactionType,
                IsCashOperation = true,
                UserID = _userInfoToken.FullName,
                Description = $"{TransactionType} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode),
                CustomerId = memberReference,
                MemberAccountNumber = accountNumber,
                MemberName = MemberName, 
                EventName = eventType,
                DestinationBrachId = tellerAccount.BranchId,
                SourceBranchId = tellerAccount.BranchId,
                IsInterBranch = false,
                DestinationBranchCommission = 0,
                SourceBranchCommission = 0,
            };
            _tellerOperationRepository.Add(tellerOperation);
        }

        private void CreateMobileMoneyTellerOperation(decimal amount, string operationType, Account tellerAccount,
    string TransactionReference, string TransactionType,
    string eventType, string MemberName, string memberReference, string accountNumber, DateTime AccountingDate,string openningOfDayReference)
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
                CurrentBalance = tellerAccount.Balance,
                Date = AccountingDate,
                AccountingDate = AccountingDate,
                OpenOfDayReference = openningOfDayReference,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = tellerAccount.TellerId,
                TransactionReference = TransactionReference,
                TransactionType = TransactionType,
                UserID = _userInfoToken.FullName,
                Description = $"{TransactionType} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode),
                CustomerId = memberReference,
                MemberAccountNumber = accountNumber,
                MemberName = MemberName, IsCashOperation=true,
                EventName = eventType,
                DestinationBrachId = tellerAccount.BranchId,
                SourceBranchId = tellerAccount.BranchId,
                IsInterBranch = false,
                DestinationBranchCommission = 0,
                SourceBranchCommission = 0,
            };
            _tellerOperationRepository.Add(tellerOperation);
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

            if (!customerResponse.Data.Active)
            {
                var errorMessage = $"Member's account services are in active. Please contact the member service for more details or for re-activation.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            var customer = customerResponse.Data; // Get customer data from response.
            return customer; // Return customer data.
        }
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

    }

}
