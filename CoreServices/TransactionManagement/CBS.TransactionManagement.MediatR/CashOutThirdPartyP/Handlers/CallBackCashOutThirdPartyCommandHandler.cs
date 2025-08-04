using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository.CashOutThirdPartyP;
using CBS.TransactionManagement.CashOutThirdPartyP.Commands;
using CBS.TransactionManagement.Data.Dto.CashOutThirdPartyP;
using CBS.TransactionManagement.Data.Dto.User;
using CBS.TransactionManagement.MediatR.User.Command;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data.Entity.CashOutThirdPartyP;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Repository.AccountServices;

namespace CBS.TransactionManagement.CashOutThirdPartyP.Handlers
{

    public class CallBackCashOutThirdPartyCommandHandler : IRequestHandler<CallBackCashOutThirdPartyCommand, ServiceResponse<TransactionThirdPartyDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICashOutThirdPartyRepository _cashOutThirdPartyRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IWithdrawalServices _withdrawalServices;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork<TransactionContext> _unitOfWork;
        private readonly ILogger<CallBackCashOutThirdPartyCommandHandler> _logger;
        private readonly IConfigRepository _configRepository;
        public CallBackCashOutThirdPartyCommandHandler(
            IAccountRepository accountRepository,
            ITellerRepository tellerRepository,
            IMediator mediator,
            IUnitOfWork<TransactionContext> unitOfWork,
            ILogger<CallBackCashOutThirdPartyCommandHandler> logger,
            UserInfoToken userInfoToken = null,
            IConfigRepository configRepository = null,
            ICashOutThirdPartyRepository cashOutThirdPartyRepository = null,
            IMapper mapper = null,
            IWithdrawalServices withdrawalServices = null)
        {
            // Dependency injection
            _accountRepository = accountRepository;
            _tellerRepository = tellerRepository;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _configRepository = configRepository;
            _cashOutThirdPartyRepository = cashOutThirdPartyRepository;
            _mapper = mapper;
            _withdrawalServices = withdrawalServices;
        }

        public async Task<ServiceResponse<TransactionThirdPartyDto>> Handle(CallBackCashOutThirdPartyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountingDate = BaseUtilities.UtcNowToDoualaTime();
                
                // Retrieve teller information
                var cashOut = await GetCashoutInformation(request.TransactionReference);

                // Verify OTP
                var otpDto = await VerifyOTP(request);

                bool isInterBranchOperation = false; // Flag to indicate if the operation is inter-branch.

                // Check if system configuration is set.
                var config = await _configRepository.GetConfigAsync(OperationSourceType.TTP.ToString());


                // Get customer account information
                var customerAccount = await _accountRepository.GetAccount(cashOut.AccountNumber, OperationType.Withdrawal.ToString());

                // Retrieve customer information
                var customer = await GetCustomerInfo(customerAccount.CustomerId);

                // Retrieve branch information
                var branch = await GetBranchInfo(customer.BranchId);

                // Generate transaction reference based on branch type
                string reference = cashOut.TransactionReference;

                // Retrieve teller information
                var teller = cashOut.Teller;

                decimal amount = cashOut.Amount; // Calculate total amount.
                decimal customer_charges = 0; // Calculate total charges.
                                              // Generate transaction reference based on branch type
                                              // Retrieve sub teller account
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller); // Retrieve teller's account.

                var bulkOp = new BulkOperation
                {
                    CustomerId = cashOut.CustomerId,
                    AccountNumber = cashOut.AccountNumber,
                    Amount = cashOut.Amount,
                    Total = cashOut.Amount,
                    currencyNotes = CurrencyNotesMapper.CalculateCurrencyNotes(cashOut.Amount),
                    OperationType = TransactionType.WITHDRAWAL.ToString()
                };
                // Create a list and add the BulkOperations object to it
                var BulkOperations = new List<BulkOperation> { bulkOp };

                // Retrieve currency notes
                var currencyNote = await RetrieveCurrencyNotes(reference, bulkOp.currencyNotes);
                // Check if customer branch matches teller branch
                if (customer.BranchId == teller.BranchId)
                {
                    isInterBranchOperation = true;
                }

                // Map bulk operations to bulk deposit list and process transactions
                var bulkWithdrawals = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(BulkOperations);
                var transactions = await ProcessBulkTransactions(bulkWithdrawals, teller, tellerAccount, false, customer, currencyNote.Data.ToList(), branch, reference, config, cashOut,accountingDate);
                cashOut.Status = Status.Completed.ToString();
                cashOut.DateOfConfirmation = BaseUtilities.UtcNowToDoualaTime();
                // Add the CashOutThirdParty entity to the repository
                _cashOutThirdPartyRepository.Update(cashOut);

                // Save changes to the database
                await _unitOfWork.SaveAsync();

                // Send SMS notification to customer
                await SendSMS(transactions.Select(t => t.AccountNumber).ToList(), amount, reference, customer, branch, customer_charges, transactions); // Send SMS notification.
                // Prepare transaction details for response
                var transactionDto = PrepareTransactionDetails(transactions, amount, customer_charges); // Prepare transaction details.

                var transactionDtox = CurrencyNotesMapper.MapToTransactionThirdPartyDto(transactionDto, customer.Phone, branch, teller);

                // Post accounting entries for transactions
                var accountingResponseMessages = await PostAccounting(transactions, branch, isInterBranchOperation, customer); // Post accounting entries.
                // Prepare response with transaction details and accounting messages
                if (accountingResponseMessages == null)
                {
                    accountingResponseMessages = "Operation completed with success.";
                }
                return ServiceResponse<TransactionThirdPartyDto>.ReturnResultWith200(transactionDtox, accountingResponseMessages); // Return success response.
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while performing transaction: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TransactionThirdPartyDto>.Return500(e);
            }
        }
        // Method to post accounting entries
        private async Task<string> PostAccounting(List<TransactionDto> transactions, BranchDto branch, bool isInterBranchOperation, CustomerDto customer)
        {
            string accountingResponseMessages = null; // Initialize variable to store accounting response messages.

            foreach (var transaction in transactions)
            {
                var apiRequest = MakeAccountingPosting(transaction.Amount, transaction.Account, transaction, branch, isInterBranchOperation, customer.LegalForm); // Create accounting posting request.
                var result = await _mediator.Send(apiRequest); // Send request to _mediator.

                if (result.StatusCode != 200)
                {
                    accountingResponseMessages += $"{result.Message}, "; // Append error message to response messages.
                }
            }

            return accountingResponseMessages; // Return accounting response messages.
        }
        // Method to create an accounting posting command
        private AddAccountingPostingCommand MakeAccountingPosting(decimal Amount, Account account, TransactionDto transaction, BranchDto branch, bool IsInterBranch, string legalForm)
        {
            // Create a new AddAccountingPostingCommand instance
            var addAccountingPostingCommand = new AddAccountingPostingCommand
            {
                AccountHolder = account.AccountName,
                OperationType = TransactionType.WITHDRAWAL.ToString(),
                AccountNumber = account.AccountNumber,
                ProductId = account.ProductId,
                ProductName = account.Product.Name,
                Naration = transaction.Note,
                TransactionReferenceId = transaction.TransactionReference,
                IsInterBranchTransaction = IsInterBranch,
                ExternalBranchCode = branch.branchCode,
                ExternalBranchId = branch.id, Source= TellerSources.Virtual_Teller_GAV.ToString(),
                AmountCollection = new List<AmountCollection>(),
                AmountEventCollections = new List<AmountEventCollection>()
            };

            // Add amount collections based on IsInterBranch flag
            if (IsInterBranch)
            {
                // For inter-branch transactions
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = Amount,
                    IsPrincipal = true,
                    EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = IsInterBranch
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = transaction.SourceBranchCommission,
                    IsPrincipal = false,
                    EventAttributeName = SharingWithPartner.SourceBrachCommission_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = IsInterBranch
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = transaction.DestinationBranchCommission,
                    IsPrincipal = false,
                    EventAttributeName = SharingWithPartner.DestinationBranchCommission_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = IsInterBranch
                });
                // Add amount event collections for inter- branch transactions
                //addAccountingPostingCommand.AmountEventCollections.Add(new AmountEventCollection
                //{
                //    Amount = transaction.WithrawalFormCharge, // Set the building contribution amount
                //    EventCode = legalForm == LegalForms.Physical_Person.ToString() ? account.Product.EventCodePhysicalPersonWithdrawalFormFee : account.Product.EventCodeMoralPersonWithdrawalFormFee, // Set the event code
                //    Naration = $"Payment of withdrawal form fee. [Saving Withdrawal Form Fee: {BaseUtilities.FormatCurrency(transaction.WithrawalFormCharge)}]", // Set the narration
                //});
            }
            else
            {
                // For regular branch transactions
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = Amount,
                    IsPrincipal = true,
                    EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = transaction.OperationCharge,
                    IsPrincipal = false,
                    EventAttributeName = OperationEventRubbriqueName.CashOut_Commission_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });
                // Add amount event collections for regular branch transactions
                //addAccountingPostingCommand.AmountEventCollections.Add(new AmountEventCollection
                //{
                //    Amount = transaction.WithrawalFormCharge, // Set the building contribution amount
                //    EventCode = legalForm == LegalForms.Physical_Person.ToString() ? account.Product.EventCodePhysicalPersonWithdrawalFormFee : account.Product.EventCodeMoralPersonWithdrawalFormFee, // Set the event code
                //    Naration = $"Payment of withdrawal form fee. [Saving Withdrawal Form Fee: {BaseUtilities.FormatCurrency(transaction.WithrawalFormCharge)}]", // Set the narration
                //});
            }

            return addAccountingPostingCommand;
        }
        // Method to prepare transaction details for response
        private TransactionDto PrepareTransactionDetails(List<TransactionDto> transactions, decimal amount, decimal charge)
        {
            var transaction = transactions.FirstOrDefault(); // Get first transaction.
            // Set transaction details
            transaction.AccountNumber = CurrencyNotesMapper.ExtractAccountNumbersFromTransaction(transactions);
            transaction.OriginalDepositAmount = amount;
            transaction.Account.AccountType = CurrencyNotesMapper.ExtractAccountTypesAndAmountFromTransaction(transactions);
            transaction.AmountInWord = BaseUtilities.ConvertToWords(amount);
            transaction.Fee = charge;
            return transaction; // Return list containing modified transaction.
        }

        // Method to send SMS notification
        private async Task SendSMS(List<string> accountNUmbers, decimal amount, string reference, CustomerDto customer, BranchDto branch, decimal charge, List<TransactionDto> transactions)
        {
            // Partially encrypt account numbers
            charge = transactions.Sum(x => x.Fee);
            string encryptedAccountNumber = null;
            foreach (var account in accountNUmbers)
            {
                encryptedAccountNumber += $"{BaseUtilities.PartiallyEncryptAccountNumber(account)}, ";
            }

            // Construct SMS message
            string msg = $"{customer.FirstName} {customer.LastName}, a cash-out of {BaseUtilities.FormatCurrency(amount)} to your account(s) {CurrencyNotesMapper.ExtractAccountTypesAndAmountFromTransaction(transactions)} was successful.\nTransaction Reference: {reference}.\nCharge: {BaseUtilities.FormatCurrency(charge)}.\nDate and Time: {BaseUtilities.UtcToDoualaTime(DateTime.Now)}.\nThank you for banking with us.\n{branch.name}.";
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.Phone
            }; // Create command to send SMS.

            await _mediator.Send(sMSPICallCommand); // Send command to _mediator.
        }

        // Method to process bulk transactions
        private async Task<List<TransactionDto>> ProcessBulkTransactions(List<BulkDeposit> requests, Teller teller, Account tellerAccount, bool isInterBranchOperation, CustomerDto customer, List<CurrencyNotesDto> currencyNotes, BranchDto branch, string reference, Config config, CashOutThirdParty cashOut, DateTime accountingDate)
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
                request.IsExternalOperation = true;
                request.ExternalApplicationName = teller.Name;
                request.ExternalReference = cashOut.ExternalTransactionReference;
                request.SourceType = cashOut.SourceType;
                // Deposit amount into account
                var transaction = await _withdrawalServices.Withdrawal(teller, tellerAccount, request, isInterBranchOperation, _userInfoToken.BranchID, customer.BranchId, customCharge, reference, customerAccount, config, false,accountingDate,false,null);

                transactions.Add(transaction); // Add transaction to list.
            }

            return transactions; // Return list of transactions.
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


        // Retrieves customer information based on the provided customer ID
        private async Task<CustomerDto> GetCustomerInfo(string customerId)
        {
            var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = customerId };
            var customerResponse = await _mediator.Send(customerCommandQuery);
            // Check if retrieving customer information was successful
            if (customerResponse.StatusCode != 200)
                throw new InvalidOperationException("Failed getting member's information");
            // Check if the customer's membership is approved
            if (customerResponse.Data.MembershipApprovalStatus.ToLower() != AccountStatus.approved.ToString().ToLower())
                throw new InvalidOperationException($"Customer membership is not approved. Current Status: {customerResponse.Data.MembershipApprovalStatus}");
            return customerResponse.Data;
        }


        // Verify OPT
        private async Task<OTPDto> VerifyOTP(CallBackCashOutThirdPartyCommand request)
        {
            var verifyOTPCommand = new VerifyOTPCommand { UserId = request.TransactionReference, OtpCode= request.OTP.ToString() };
            var verifyOTPResponse = await _mediator.Send(verifyOTPCommand);
            // Check if retrieving customer information was successful
            if (verifyOTPResponse.StatusCode != 200)
            {
                var errorMessage = $"Failed verifying otp, {verifyOTPResponse.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Update.ToString(), request.TransactionReference, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                throw new InvalidOperationException(errorMessage);
            }
            return verifyOTPResponse.Data;
        }

        // Retrieves branch information based on the provided branch ID
        private async Task<BranchDto> GetBranchInfo(string branchId)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = branchId };
            var branchResponse = await _mediator.Send(branchCommandQuery);
            // Check if retrieving branch information was successful
            if (branchResponse.StatusCode != 200)
                throw new InvalidOperationException("Failed getting branch information.");
            return branchResponse.Data;
        }

        // Retrieves cashout information based on the provided application code and branch
        private async Task<CashOutThirdParty> GetCashoutInformation(string transactionReference)
        {
            var cashOutThirdParty = await _cashOutThirdPartyRepository.FindBy(t => t.TransactionReference == transactionReference).Include(x => x.Teller).FirstOrDefaultAsync();
            // Check if the reference exists
            if (cashOutThirdParty == null)
            {
                var errorMessage = $"Transaction with id {transactionReference} was not found";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Update.ToString(), transactionReference, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                throw new InvalidOperationException(errorMessage);
            }
            if (cashOutThirdParty.Status!=Status.Pending.ToString())
            {
                var errorMessage = $"Transaction with id {transactionReference} is already in {cashOutThirdParty.Status} state.";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Update.ToString(), transactionReference, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                throw new InvalidOperationException(errorMessage);
            }
            return cashOutThirdParty;
        }

    }

}
