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
using CBS.TransactionManagement.MediatR.UtilityServices;
using System.Runtime.InteropServices;

namespace CBS.TransactionManagement.otherCashIn.Handlers
{
    /// <summary>
    /// Handles the command to add a new OtherTransaction.
    /// </summary>
    public class AddNoneCashMobileMoneyCommandHandler : IRequestHandler<AddNoneCashMobileMoneyCommand, ServiceResponse<OtherTransactionDto>>
    {
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.

        private readonly IOtherTransactionRepository _OtherTransactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddNoneCashMobileMoneyCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ITellerRepository _tellerRepository; // Repository for accessing teller data.
        private readonly IConfigRepository _configRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly IDailyTransactionCodeGenerator _dailyTransactionCodeGenerator;
        private readonly IAPIUtilityServicesRepository _aPIUtilityServicesRepository;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddNoneCashMobileMoneyCommandHandler.
        /// </summary>
        /// <param name="OtherTransactionRepository">Repository for OtherTransaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddNoneCashMobileMoneyCommandHandler(
            IOtherTransactionRepository OtherTransactionRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddNoneCashMobileMoneyCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            IAccountRepository accountRepository = null,
            ITellerRepository tellerRepository = null,
            IConfigRepository configRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            ITransactionRepository transactionRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null,
            IDailyTransactionCodeGenerator dailyTransactionCodeGenerator = null,
            IAPIUtilityServicesRepository aPIUtilityServicesRepository = null)
        {
            _OtherTransactionRepository = OtherTransactionRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _accountRepository = accountRepository;
            _tellerRepository = tellerRepository;
            _configRepository = configRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _transactionRepository = transactionRepository;
            _accountingDayRepository = accountingDayRepository;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
            _dailyTransactionCodeGenerator=dailyTransactionCodeGenerator;
            _aPIUtilityServicesRepository=aPIUtilityServicesRepository;
        }



        public async Task<ServiceResponse<OtherTransactionDto>> Handle(AddNoneCashMobileMoneyCommand request, CancellationToken cancellationToken)
        {
            string transactionReference = string.Empty;
            try
            {
                // Step 1: Retrieve the current accounting date for the branch.
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Step 2: Validate and prepare initial data for the request.
                await ValidateAndPrepareData(request);

                // Set the operation type to the booking direction from the request.
                request.OperationType = OperationType.Deposit.ToString();

                // Retrieve non-member mobile money collection account using the provided member reference.
                var noneMemberMobileMoneyCollectionAccount = await _accountRepository.RetrieveNoneMemberMobileMoneyAccountByMemberReference(request.MemberReference);

                // Retrieve the main teller responsible for non-cash operations in the branch.
                var teller = await _tellerRepository.GetTellerByOperationType(OperationType.NoneCash.ToString(), _userInfoToken.BranchID);

                // Retrieve the main teller's account details.
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Declare a Teller object to store mobile money teller details.
                var mobileMoneyTeller = new Teller();

                // Retrieve the receiver’s account details using the provided account number.
                var receivingMemberaccount = await _accountRepository.GetAccountByAccountNumber(request.ReceiverAccountNumber);

                // Prepare a list of customer IDs to fetch multiple customer details.
                List<string> customersLists = new List<string> { request.MemberReference, receivingMemberaccount.CustomerId };

                // Fetch details of multiple customers based on the provided customer IDs.
                var customers = await _aPIUtilityServicesRepository.GetMultipleMembers(customersLists, null, false);

                // Retrieve sender's (initiator's) customer details.
                var customer = customers.Where(x => x.CustomerId == request.MemberReference).FirstOrDefault();

                // Retrieve branch details based on the sender’s branch ID.
                var branch = await _aPIUtilityServicesRepository.GetBranch(customer.BranchId);

                // Retrieve the receiver's customer details.
                var customerReceiving = customers.Where(x => x.CustomerId == receivingMemberaccount.CustomerId).FirstOrDefault();

                // Find all mobile money tellers available in the branch for the specified source type.
                var momoTellersInBranch = await _tellerRepository.FindBy(x => x.BranchId == teller.BranchId && x.TellerType == request.SourceType).ToListAsync();

                if (momoTellersInBranch.Count == 1)
                {
                    // If only one mobile money teller exists, assign it to the operation.
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
                    // If multiple tellers exist, retrieve the one matching the provided teller code.
                    mobileMoneyTeller = await _tellerRepository.FindBy(x => x.Code == request.TellerCode && x.BranchId == teller.BranchId).FirstOrDefaultAsync();

                    if (mobileMoneyTeller == null)
                    {
                        throw new InvalidOperationException(
                            $"Multiple tellers exist in branch '{branch.name}' (Code: {branch.branchCode}), but no teller with code '{request.TellerCode}' was found. " +
                            "Please ensure the provided Teller Code is correct."
                        );
                    }

                    // Validate that the mobile money reference matches the non-member collection account.
                    if (mobileMoneyTeller.MapMobileMoneyToNoneMemberMobileMoneyReference != noneMemberMobileMoneyCollectionAccount.AccountNumber)
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
                    // No tellers found in the branch for the specified source type.
                    throw new InvalidOperationException(
                        $"No {request.SourceType} tellers are configured for branch '{branch.name}' (Code: {branch.branchCode}). " +
                        "Please ensure the branch has at least one teller configured for this operation."
                    );
                }

                // Retrieve the account details for the selected mobile money teller.
                var mobileMoneyTellerAccount = await RetrieveMobileMoneyTellerAccount(request, mobileMoneyTeller.Code, mobileMoneyTeller.Id);

                // Step 4: Control inter-branch operations for mobile money.
                if (customer.BranchId != teller.BranchId)
                {
                    var errorMessage = $"{_userInfoToken.FullName}, Note that interbranch operations are not permitted with mobile money operations. Branch '{branch.name}' (Code: {branch.branchCode}). Please contact the administrator for more assistance.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.MTNorOrangeMoneyOperation, LogLevelInfo.Warning);
                    return ServiceResponse<OtherTransactionDto>.Return403(errorMessage);
                }

                // Step 5: Validate that the non-member account type matches the source type.
                if (noneMemberMobileMoneyCollectionAccount.AccountType != request.SourceType)
                {
                    var errorMessage = $"{_userInfoToken.FullName}, You have entered the wrong non-member reference for {request.SourceType} operation. Branch '{branch.name}' (Code: {branch.branchCode}). Please enter the correct non-member reference for the selected operator service {request.SourceType}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.MTNorOrangeMoneyOperation, LogLevelInfo.Warning);
                    return ServiceResponse<OtherTransactionDto>.Return403(errorMessage);
                }

                // Update non-member mobile money account balances with teller account values.
                noneMemberMobileMoneyCollectionAccount.Balance = mobileMoneyTellerAccount.Balance;
                noneMemberMobileMoneyCollectionAccount.PreviousBalance = mobileMoneyTellerAccount.PreviousBalance;
                noneMemberMobileMoneyCollectionAccount.EncryptedBalance = mobileMoneyTellerAccount.EncryptedBalance;

                // Step 6: Generate a unique transaction reference based on the operation.
                var operationPrefix = GenerateTransactionReference(request);

                // Determine the transaction type based on the operation prefix.
                var transactionType = operationPrefix.ToString().Contains("OMC") ? TransactionType.OrangeMoney : TransactionType.MobileMoney;

                // Reserve a transaction code using the determined operation type.
                transactionReference = await _dailyTransactionCodeGenerator.ReserveTransactionCode(
                    _userInfoToken.BranchCode, operationPrefix, transactionType, isInterBranch: false
                );

                // Step 7: Prepare the transaction entity for processing.
                var transactionEntity = PrepareTransactionEntity(request, teller, transactionReference, mobileMoneyTeller.OperationEventCode, mobileMoneyTellerAccount.AccountNumber, accountingDate,customerReceiving);

                // Step 8: Process the transaction, updating relevant accounts.
                await ProcessTransaction(request, tellerAccount, mobileMoneyTellerAccount, transactionEntity, noneMemberMobileMoneyCollectionAccount, accountingDate, receivingMemberaccount);

                // Step 9: Save the transaction data, updating the involved accounts.
                var transactionDto = await SaveTransactionData(transactionEntity, mobileMoneyTellerAccount, noneMemberMobileMoneyCollectionAccount,receivingMemberaccount,teller, request.Charges);
                await _dailyTransactionCodeGenerator.MarkTransactionAsSuccessful(transactionReference);
                // Step 10: Send an SMS notification and post the accounting event.
                await SendSMS(customerReceiving.Phone, request.Amount, transactionReference, branch, 0, request.SourceType, request.OperationType, request.CustomerName);
                var result = await PostAccountingEvent(request.Amount, transactionReference, accountingDate, request.OperationType, mobileMoneyTeller);
                //
                // Step 11: Log transaction completion and return the response.
                string msg = result == "OK"
                    ? $"Mobile money transaction of {BaseUtilities.FormatCurrency(request.Amount)} completed successfully."
                    : $"Mobile money transaction of {BaseUtilities.FormatCurrency(request.Amount)} completed successfully, but encountered an error while posting the accounting event.";

                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.MTNorOrangeMoneyOperation, LogLevelInfo.Information);

                transactionDto.Teller = teller;
                return ServiceResponse<OtherTransactionDto>.ReturnResultWith200(transactionDto, msg);
            }
            catch (Exception e)
            {
                await _dailyTransactionCodeGenerator.RevertTransactionCode(transactionReference);
                var errorMessage = $"An error occurred while processing the mobile money transaction: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.MTNorOrangeMoneyOperation, LogLevelInfo.Error);
                return ServiceResponse<OtherTransactionDto>.Return500(errorMessage);
            }
        }


        // Step 1: Validate and prepare initial data
        private async Task ValidateAndPrepareData(AddNoneCashMobileMoneyCommand request)
        {
            await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString()); // Check if system configuration is set
            request.TelephoneNumber = BaseUtilities.ProcessTelephoneNumber(request.TelephoneNumber); // Validate and format telephone number
        }



        // Step 2: Retrieve mobile money teller account
        private async Task<Account> RetrieveMobileMoneyTellerAccount(AddNoneCashMobileMoneyCommand request, string tellerCode, string tellerid)
        {
            var account = request.OperationType == OperationType.Deposit.ToString()
                ? await _accountRepository.RetrieveMobileMoneyTellerAccountAndCheckBalance(_userInfoToken.BranchID, tellerid, request.Amount)
                : await _accountRepository.RetrieveMobileMoneyTellerAccountByTellerCodeAndBranchId(_userInfoToken.BranchID, tellerCode);
            return account;
        }



        // Step 3: Generate and prepare transaction entity
        private OperationPrefix GenerateTransactionReference(AddNoneCashMobileMoneyCommand request)
        {
            return request.SourceType == AccountType.MobileMoneyMTN.ToString()
                ? (request.OperationType == OperationType.Deposit.ToString() ? OperationPrefix.MMC_In : OperationPrefix.MMC_Out)
                : (request.OperationType == OperationType.Deposit.ToString() ? OperationPrefix.OMC_In : OperationPrefix.OMC_Out);
        }



        private OtherTransaction PrepareTransactionEntity(AddNoneCashMobileMoneyCommand request, Teller teller, string reference, string eventCode, string accountNumber, DateTime accountingDay,CustomerDto customerReceiving)
        {
            var entity = _mapper.Map<OtherTransaction>(request);
            entity.CreatedDate = BaseUtilities.UtcNowToDoualaTime();
            entity.Id = BaseUtilities.GenerateUniqueNumber();
            entity.TransactionReference = reference;
            entity.TellerId = teller.Id;
            entity.MemberName = request.CustomerName;
            entity.BranchId = teller.BranchId;
            entity.BankId = teller.BankId;
            entity.CNI = customerReceiving.IDNumber;
            entity.AccountNumber = accountNumber;
            entity.Direction = "Credit";
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
        private async Task ProcessTransaction(AddNoneCashMobileMoneyCommand request, Account tellerAccount, Account mobileMoneyTellerAccount, OtherTransaction transactionEntity, Account noneMemberAccountNumber, DateTime AccountingDate, Account recevingMemberAccount)
        {
            string membersName = $"[{request.SourceType} {request.CustomerName} {request.TelephoneNumber}]";

            transactionEntity.Direction = OperationType.Credit.ToString();
            transactionEntity.Credit = request.Amount;
            transactionEntity.Debit = 0;
            transactionEntity.Amount = request.Amount;
            _accountRepository.CreditAccount(mobileMoneyTellerAccount, request.Amount);
            _accountRepository.CreditAccount(noneMemberAccountNumber, request.Amount);
            _accountRepository.CreditAccount(recevingMemberAccount, request.Amount);

            CreateTellerOperation(request.Amount, OperationType.Credit.ToString(), tellerAccount, transactionEntity.TransactionReference, TransactionType.CASH_IN.ToString(), request.SourceType, membersName, request.MemberReference, noneMemberAccountNumber.AccountNumber, AccountingDate);

            CreateMobileMoneyTellerOperation(request.Amount, OperationType.Credit.ToString(), mobileMoneyTellerAccount, transactionEntity.TransactionReference, TransactionType.CASH_IN.ToString(), request.SourceType, membersName, request.MemberReference, noneMemberAccountNumber.AccountNumber, AccountingDate, tellerAccount.OpenningOfDayReference);
           
        }

        // Step 5: Save transaction and related data
        private async Task<OtherTransactionDto> SaveTransactionData(OtherTransaction transactionEntity, Account mobileMoneyTellerAccount, Account noneMemberAccountNumber, Account RecevingMemberAccount, Teller NoneTeller, decimal charges)
        {
            //var transaction = CurrencyNotesMapper.MapTransaction(transactionEntity, _userInfoToken, mobileMoneyTellerAccount);
            var otherTransactionDto = CurrencyNotesMapper.MapOtherTransaction(transactionEntity);
            var transaction = CurrencyNotesMapper.MapTransactionMobileMoney(transactionEntity, _userInfoToken, noneMemberAccountNumber, otherTransactionDto);
            transaction.Account = null;
            transaction.Teller = null;
           

            var transaction1=CreateTransactionForReceivingMember(transactionEntity.Amount, RecevingMemberAccount, NoneTeller, charges, transactionEntity.BranchId, transactionEntity.BranchId,false, transactionEntity.TransactionReference, transactionEntity.DateOfOperation, mobileMoneyTellerAccount);

            _transactionRepository.Add(transaction);
            _transactionRepository.Add(transaction1);
            _OtherTransactionRepository.Add(transactionEntity);
            await _uow.SaveAsync();

            return otherTransactionDto;
        }
        private Transaction CreateTransactionForReceivingMember(decimal Amount, Account account, Teller teller, decimal charges, string sourceBranchId, string destinationBranchId, bool isInterBranch, string Reference, DateTime accountingDate, Account MobileMoneyOoRangeAccount)
        {
            decimal balance = account.Balance;
            decimal originalAmount = Amount;
            string N_A = "N/A";

            var transactionEntityEntryFee = new Transaction
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                Status = TransactionStatus.COMPLETED.ToString(),
                TransactionReference = Reference,
                ExternalReference = N_A,
                IsExternalOperation = false,
                ExternalApplicationName = N_A,
                SourceType = OperationSourceType.Web_Portal.ToString(),
                Currency = Currency.XAF.ToString(),
                TransactionType = OperationType.Credit.ToString(), // This is a credit transaction for the receiving member
                AccountNumber = account.AccountNumber,
                PreviousBalance = account.Balance,
                AccountId = account.Id,
                CustomerId = account.CustomerId,
                ProductId = account.ProductId,
                SenderAccountId = account.Id,
                ReceiverAccountId = account.Id,
                BankId = teller.BankId,
                Operation = TransactionType.WITHDRAWAL_MOBILEMONEY.ToString(),
                BranchId = teller.BranchId,
                OriginalDepositAmount = originalAmount,
                Fee = charges,
                Tax = 0,
                Amount = -(Amount),

                // Updated Note: Includes the source type (MTN Money / Orange Money)
                Note = $"Statement: A deposit of {BaseUtilities.FormatCurrency(originalAmount)} has been credited to your {account.AccountType} account from {MobileMoneyOoRangeAccount.AccountType}. Transaction Reference: {Reference}.",

                OperationType = OperationType.Debit.ToString(),
                FeeType = Events.None.ToString(),
                TellerId = teller.Id,
                DepositerNote = N_A,
                DepositerTelephone = N_A,
                DepositorIDNumber = N_A,
                DepositorIDExpiryDate = N_A,
                DepositorIDIssueDate = N_A,
                DepositorIDNumberPlaceOfIssue = N_A,
                IsDepositDoneByAccountOwner = true,
                DepositorName = N_A,
                Balance = balance,
                Credit = 0,
                Debit = -(Amount),
                DestinationBrachId = destinationBranchId,
                OperationCharge = 0,
                AccountingDate = accountingDate,
                ReceiptTitle = $"{account.AccountType} Cashin Receipt, Reference: " + Reference,
                WithrawalFormCharge = Amount,
                SourceBrachId = sourceBranchId,
                IsInterBrachOperation = isInterBranch,
                DestinationBranchCommission = 0,
                SourceBranchCommission = charges
            };

            return transactionEntityEntryFee;
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
                TellerSources = Teller.TellerType==AccountType.MobileMoneyMTN.ToString() ? TellerSources.Members_Account.ToString() : TellerSources.Virtual_Teller_Orange.ToString(),
                Amount = amount,
                OperationType = operationType,
                TransactionDate = accountingDate,
                TransactionReference = ReferenceId,
            };
            return moneyOperationCommand;
        }
        // Method to post accounting event
        private async Task<string> PostAccountingEvent(decimal amount, string transactionReferenceId, DateTime accountingDate, string operationType, Teller teller)
        {
            var apiRequest = MakeAccountingPostingEvent(amount, transactionReferenceId, accountingDate, operationType, teller);
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
                IsCashOperation = false,
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
        string eventType, string MemberName, string memberReference, string accountNumber, DateTime AccountingDate, string openningOfDayReference)
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
                MemberName = MemberName,
                IsCashOperation=false,
                EventName = eventType,
                DestinationBrachId = tellerAccount.BranchId,
                SourceBranchId = tellerAccount.BranchId,
                IsInterBranch = false,
                DestinationBranchCommission = 0,
                SourceBranchCommission = 0,
            };
            _tellerOperationRepository.Add(tellerOperation);
        }


        
       
    }

}
