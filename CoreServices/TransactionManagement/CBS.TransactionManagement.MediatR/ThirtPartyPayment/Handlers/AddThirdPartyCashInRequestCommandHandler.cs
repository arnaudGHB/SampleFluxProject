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
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Repository.AccountServices;
using CBS.TransactionManagement.MediatR.ThirtPartyPayment.Commands;

namespace CBS.TransactionManagement.MediatR.ThirtPartyPayment.Handlers
{
    public class AddThirdPartyCashInRequestCommandHandler : IRequestHandler<AddThirdPartyCashInRequestCommand, ServiceResponse<TransactionThirdPartyDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDepositServices _depositServices;
        private readonly ITellerRepository _tellerRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork<TransactionContext> _unitOfWork;
        private readonly ILogger<AddThirdPartyCashInRequestCommandHandler> _logger;
        private readonly IDailyTellerRepository _dailyTellerRepository;

        private readonly IConfigRepository _configRepository;
        public AddThirdPartyCashInRequestCommandHandler(
            IAccountRepository accountRepository,
            ITellerRepository tellerRepository,
            IMediator mediator,
            IUnitOfWork<TransactionContext> unitOfWork,
            ILogger<AddThirdPartyCashInRequestCommandHandler> logger,
            UserInfoToken userInfoToken = null,
            IDepositServices depositServices = null,
            IConfigRepository configRepository = null,
            IDailyTellerRepository tellerProvisioningAccountRepository = null)
        {
            // Dependency injection
            _accountRepository = accountRepository;
            _tellerRepository = tellerRepository;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _depositServices = depositServices;
            _configRepository = configRepository;
            _dailyTellerRepository = tellerProvisioningAccountRepository;
        }

        public async Task<ServiceResponse<TransactionThirdPartyDto>> Handle(AddThirdPartyCashInRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountingDate = BaseUtilities.UtcNowToDoualaTime();

                // Check if system configuration is set.
                var config = await GetConfigAsync();
                var customerAccount = await GetAccount(request.AccountNumber);
                // Retrieve customer information
                var customer = await GetCustomerInfo(customerAccount.CustomerId);
                customer.Name = $"{customer.FirstName} {customer.LastName}";
                // Retrieve branch information
                var branch = await GetBranchInfo(customer.BranchId);

                // Generate transaction reference based on branch type
                string Reference = GenerateTransactionReference(request.ApplicationCode, branch.branchCode);

                // Retrieve teller information
                var teller = await GetTellerInfo(request.ApplicationCode, branch);

                await _tellerRepository.CheckTellerOperationalRights(teller, OperationType.Deposit.ToString(), true);
                // Retrieve active teller account
                var dailyTeller = await _dailyTellerRepository.GetAnActiveSubTellerForTheDate3PP(teller.Id);

                // Retrieve sub teller account
                var tellerAccount = await GetTellerAccount(teller.Id);
                // Calculate currency notes
                var currencyNote = await CalculateCurrencyNotes(request.Amount, Reference);
                // Prepare bulk operation
                var bulkOperation = PrepareBulkOperation(request, customer, branch, currencyNote);

                // Perform deposit
                var depositResult = await PerformDeposit(bulkOperation, teller, tellerAccount, Reference, customerAccount, config, customer.Name,accountingDate);
                var transaction = CurrencyNotesMapper.MapToTransactionThirdPartyDto(depositResult, bulkOperation.Customer.Phone, bulkOperation.Branch, teller);
                // Save changes to the database
                await _unitOfWork.SaveAsync();

                // Send SMS notification
                await SendSMSNotification(transaction, Reference, customer, branch, customerAccount);

                // Post accounting entries
                var accountingResponse = await PostAccountingEntries(request.Amount, depositResult, branch);

                // Check if accounting entries posting was successful
                if (accountingResponse.StatusCode == 200)
                    return ServiceResponse<TransactionThirdPartyDto>.ReturnResultWith200(transaction, "Operation was completed with success.");

                // Return error response if accounting entries posting failed
                return ServiceResponse<TransactionThirdPartyDto>.ReturnResultWith200(transaction, "Error posting accounting entries.");
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Transaction: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TransactionThirdPartyDto>.Return500(e);
            }
        }

        // Method to retrieve system configuration
        private async Task<Config> GetConfigAsync()
        {
            var config = await _configRepository.All.FirstOrDefaultAsync();

            // Check if system config exist
            if (config == null)
            {
                var errorMessage = $"Sorry, System configuration not found.";
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            //// Verify if system year is opened.
            //if (!config.IsYearOpen)
            //{
            //    var errorMessage = $"Sorry, system year is not opened. Contact your system adminstrator.";
            //    throw new InvalidOperationException(errorMessage); // Throw exception.
            //}
            //// Verify if system day is opened.
            //if (!config.IsDayOpen)
            //{
            //    var errorMessage = $"Sorry, system day is not opened. Contact your system adminstrator.";
            //    throw new InvalidOperationException(errorMessage); // Throw exception.
            //}

            return config; // Return config.
        }


        // Retrieves account information based on the provided account number
        private async Task<Account> GetAccount(string AccountNumber)
        {
            var account = await _accountRepository
                       .FindBy(a => a.AccountNumber == AccountNumber)
                       .Include(a => a.Product)
                       .ThenInclude(p => p.CashDepositParameters)
                       .FirstOrDefaultAsync();
            // Check if the account type is a loan account
            if (account.AccountType == AccountType.Loan.ToString())
            {
                var errorMessage = $"Sorry, you can't deposit into a loan account directly.";
                throw new InvalidOperationException(errorMessage);
            }
            // Verify the integrity of the account balance
            string accountBalance = account.Balance.ToString();
            if (!BalanceEncryption.VerifyBalanceIntegrity(accountBalance, account.EncryptedBalance, account.AccountNumber))
            {
                var errorMessage = $"Error occurred while initiating deposit, Account balance {BaseUtilities.FormatCurrency(account.Balance)} has been tampered with. Please contact your system administrator";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            // Check if the account type is a gav account
            if (account.AccountType != AccountType.Gav.ToString())
            {
                var errorMessage = $"Sorry you have not subcribed for the gav service.";
                throw new InvalidOperationException(errorMessage);
            }
            return account;
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

        // Generates a transaction reference based on the provided application code and branch code
        private string GenerateTransactionReference(string applicationCode, string branchCode)
        {
            return BaseUtilities.GenerateInsuranceUniqueNumber(8, $"{applicationCode}-{branchCode}-");
        }

        // Retrieves teller information based on the provided application code and branch
        private async Task<Teller> GetTellerInfo(string applicationCode, BranchDto branch)
        {
            string tppCode = $"{applicationCode}-{branch.branchCode}";
            var teller = await _tellerRepository.FindBy(t => t.Code == tppCode && t.ActiveStatus).FirstOrDefaultAsync();
            // Check if the teller exists
            if (teller == null)
                throw new InvalidOperationException($"3PP with application code: {tppCode} does not exist.");
            return teller;
        }

        // Retrieves the teller account based on the provided teller ID
        private async Task<Account> GetTellerAccount(string tellerId)
        {
            var tellerAccount = await _accountRepository.FindBy(t => t.TellerId == tellerId).FirstOrDefaultAsync();

            // Check if the teller account exists
            if (tellerAccount == null)
            {
                var errorMessage = $"Teller account does not have an account";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Update.ToString(), tellerId, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                throw new InvalidOperationException(errorMessage);
            }
            return tellerAccount;
        }

        // Calculates currency notes based on the provided amount and reference
        private async Task<List<CurrencyNotesDto>> CalculateCurrencyNotes(decimal amount, string reference)
        {
            var notes = CurrencyNotesMapper.CalculateCurrencyNotes(amount);
            var addCurrencyNotesCommand = new AddCurrencyNotesCommand { CurrencyNote = notes, Reference = reference };
            var currencyNoteResponse = await _mediator.Send(addCurrencyNotesCommand);
            // Check if currency notes calculation was successful
            if (currencyNoteResponse.StatusCode != 200)
                throw new InvalidOperationException(currencyNoteResponse.Message);
            return currencyNoteResponse.Data;
        }

        // Prepares the bulk operation object for deposit
        private BulkDeposit PrepareBulkOperation(AddThirdPartyCashInRequestCommand request, CustomerDto customer, BranchDto branch, List<CurrencyNotesDto> currencyNote)
        {
            return new BulkDeposit
            {
                AccountNumber = request.AccountNumber,
                Amount = request.Amount,
                Branch = branch,
                Customer = customer,
                currencyNotes = currencyNote,
                CustomerId = customer.CustomerId,
                Fee = request.Charge,
                OperationType = TransactionType.DEPOSIT.ToString(),
                ExternalReference = request.ExternalReference,
                IsExternalOperation = true,
                ExternalApplicationName = _userInfoToken.FullName,
                SourceType = "3PP",
            };
        }

        // Performs the deposit operation
        private async Task<TransactionDto> PerformDeposit(BulkDeposit bulkOperation, Teller teller, Account tellerAccount, string Reference, Account customerAccount, Config config, string Name, DateTime accountingDate)
        {

            var transactionDto = await _depositServices.Deposit(teller, tellerAccount, bulkOperation, false, bulkOperation.Customer.BranchId, bulkOperation.Customer.BranchId, bulkOperation.Fee, Reference, customerAccount, config, Name, false,accountingDate,false,null);
            return transactionDto;
        }

        // Sends SMS notification to the customer
        private async Task SendSMSNotification(TransactionThirdPartyDto depositResult, string reference, CustomerDto customer, BranchDto branch, Account customerAccount)
        {
            string bankName = branch.name;
            string encryptedAccountNumber = BaseUtilities.PartiallyEncryptAccountNumber(depositResult.AccountNumber);
            string msg = $"{customer.FirstName} {customer.LastName}, Successfully cash-in of {BaseUtilities.FormatCurrency(depositResult.Amount)} to your {customerAccount.AccountType} account number: {encryptedAccountNumber}\nCurrent balance: {BaseUtilities.FormatCurrency(customerAccount.Balance)}\nTransaction Ref: {reference}.\nExternal Ref: {depositResult.ExternalReference}\nCharges: {BaseUtilities.FormatCurrency(depositResult.TotalCharge)}.\nDate and Time: {BaseUtilities.UtcToDoualaTime(DateTime.Now)}.\nThank you for banking with us.\n{bankName}.";
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.Phone
            };
            await _mediator.Send(sMSPICallCommand);
        }

        // Posts accounting entries for the transaction
        private async Task<ServiceResponse<bool>> PostAccountingEntries(decimal amount, TransactionDto depositResult, BranchDto branch)
        {
            var apiRequest = MakeAccountingPosting(amount, depositResult.Account, depositResult, branch, false);
            var result = await _mediator.Send(apiRequest);
            if (result.StatusCode == 200)
                return ServiceResponse<bool>.ReturnResultWith200(true);
            return ServiceResponse<bool>.ReturnResultWith200(false, "Error posting accounting entries.");
        }

        // Creates an accounting posting command based on the provided parameters
        private AddAccountingPostingCommand MakeAccountingPosting(decimal Amount, Account account, TransactionDto transaction, BranchDto branch, bool IsInterBranch)
        {
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
                AmountCollection = new List<AmountCollection>(),
                AmountEventCollections = new List<AmountEventCollection>(),
                Source = TellerSources.Virtual_Teller_GAV.ToString()
            };

            // Determine whether to include commission amounts based on the transaction details
            bool NotDestinationBranchCommission = false;
            bool NotSourceBranchCommission = false;
            bool both = false;
            if (transaction.DestinationBranchCommission > 0)
            {
                NotDestinationBranchCommission = true;
            }
            if (transaction.SourceBranchCommission > 0)
            {
                NotSourceBranchCommission = true;
            }

            // Add amount collections based on whether it's an inter-branch transaction
            if (IsInterBranch)
            {
                // For inter-branch transactions
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = Amount,
                    IsPrincipal = true,
                    EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = both ? IsInterBranch : false,
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = transaction.SourceBranchCommission,
                    IsPrincipal = false,
                    EventAttributeName = SharingWithPartner.SourceBrachCommission_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = NotSourceBranchCommission ? IsInterBranch : false
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = transaction.DestinationBranchCommission,
                    IsPrincipal = false,
                    EventAttributeName = SharingWithPartner.DestinationBranchCommission_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = NotDestinationBranchCommission ? IsInterBranch : false
                });
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
                    Amount = transaction.Fee,
                    IsPrincipal = false,
                    EventAttributeName = OperationEventRubbriqueName.Saving_Fee_Account.ToString(),
                    IsInterBankOperationPrincipalCommission = false
                });
            }

            return addAccountingPostingCommand;
        }


    }
}
