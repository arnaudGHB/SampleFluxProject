using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.ChargesWaivedP;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data.Entity.RemittanceP;
using CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Repository.HolyDayP;
using DocumentFormat.OpenXml.Features;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.AccountServices
{
    public class DepositServices : IDepositServices
    {
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ISavingProductFeeRepository _savingProductFeeRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository;
        private readonly IHolyDayRepository _holyDayRepository;

        public IMediator _mediator { get; set; }
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly ILogger<AccountRepository> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        public DepositServices(ITellerOperationRepository tellerOperationRepository = null, IMediator mediator = null, ITransactionRepository transactionRepository = null, ILogger<AccountRepository> logger = null, IAccountRepository accountRepository = null, IUnitOfWork<TransactionContext> uow = null, ISavingProductFeeRepository savingProductFeeRepository = null, UserInfoToken userInfoToken = null, ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null, IHolyDayRepository holyDayRepository = null)
        {
            _tellerOperationRepository = tellerOperationRepository;
            _mediator = mediator;
            _TransactionRepository = transactionRepository;
            _logger = logger;
            _accountRepository = accountRepository;
            _uow = uow;
            _savingProductFeeRepository = savingProductFeeRepository;
            _userInfoToken = userInfoToken;
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _holyDayRepository = holyDayRepository;
        }

        public async Task<TransactionDto> Deposit(Teller teller, Account tellerAccount, BulkDeposit request, bool isInterBranchOperation, string sourceBranchId, string destinationBranchId, decimal customCharge, string Reference, Account customerAccount, Config config, string Name, bool IsMomocashCollection, DateTime accountingDate, bool isRemittance, Remittance remittance)
        {
            if (customerAccount.AccountType == AccountType.MobileMoneyMTN.ToString() || customerAccount.AccountType == AccountType.MobileMoneyORANGE.ToString())
            {

                // Log an error indicating insufficient balance for physical person
                var errorMessage = $"The Cashin treatement of {customerAccount.AccountType} must be done as other cash operations. Go to CashDesk>>O Cash Operations>>Mobile Money. You can contact your administrator for assistance.";
                _logger.LogError(errorMessage);
                // Log an audit trail for the error
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 403, _userInfoToken.Token);
                // Throw an exception indicating insufficient balance for physical person
                throw new InvalidOperationException(errorMessage);
            }
            // Determine the type of inter-branch operation
            string interOperationType = CurrencyNotesMapper.DetermineOperationType(isInterBranchOperation, request.OperationType);

            // Retrieve deposit limit for the customer account
            var depositLimit = await GetDepositLimit(customerAccount.Product, request.Amount, interOperationType, isInterBranchOperation);

            // Create transaction history for customer
            var transactionEntity = await CreateTransactionHistory(request, customerAccount, teller, tellerAccount, depositLimit, request.Customer, isInterBranchOperation, sourceBranchId, destinationBranchId, customCharge, Reference, config, IsMomocashCollection, request.IsChargesInclussive, accountingDate,isRemittance,remittance);

            // Map the Transaction entity to TransactionDto and return it with a success response
            var transactionDto = CurrencyNotesMapper.MapTransactionToDto(transactionEntity, request.currencyNotes, _userInfoToken);
            transactionDto.Teller = teller;
            return transactionDto;
        }

        

        public async Task<TransactionDto> DepositWithdrawalFormFee(Teller teller, Account tellerAccount, WithdrawalNotification withdrawal, List<CurrencyNotesDto> currencyNotes, bool IsInterBranchOperation, string sourceBranchId, string destinationBranchId, string Reference, Account customerAccount, string Name, bool IsMomocashCollection, bool isChargeIclussive, DateTime accountingDate)
        {

            // Create transaction history for customer
            var transactionEntity = await CreateTransactionHistory(withdrawal.FormNotificationCharge, customerAccount, teller, tellerAccount, customerAccount.Product.CashDepositParameters.FirstOrDefault(), IsInterBranchOperation, sourceBranchId, destinationBranchId, Reference, Name, IsMomocashCollection, isChargeIclussive, accountingDate, false, null);

            // Map the Transaction entity to TransactionDto and return it with a success response
            //customerAccount.Product = null;
            //transactionEntity.Teller = teller;
            //transactionEntity.Account = customerAccount;
            transactionEntity.Account = customerAccount;
            var transactionDto = CurrencyNotesMapper.MapTransactionToDto(transactionEntity, currencyNotes, _userInfoToken);
            transactionDto.Teller = teller;
            return transactionDto;
        }

        // Helper method to determine the type of inter-branch operation
        private async Task<Transaction> CreateTransactionHistory(decimal Amount, Account account, Teller teller, Account tellerAccount, CashDepositParameter depositLimit, bool isInterbranch, string sourceBranchId, string destinationBranchId, string Reference, string Name, bool IsMomocashCollection, bool isChargeIclussive, DateTime accountingDate, bool isRemittance,Remittance remittance)
        {


            // Create transaction history for customer
            Transaction transactionEntity = CreateTransaction(Amount, account, teller, depositLimit, 0, sourceBranchId, destinationBranchId, isInterbranch, Reference, accountingDate);
            //CreateTransaction(sourceBranchId, string destinationBranchId, bool isInterBranch, string Reference,decimal MemberActivationFee)

            try
            {
                // Add the new Transaction entity to the repository
                transactionEntity.Account = null;
                account.Product = null;
                if (transactionEntity.FeeType != "Charge_Of_Saving_Withdrawal_Form")
                {
                    _TransactionRepository.Add(transactionEntity);
                }
                //Update Teller Account
                UpdateTellerAccountBalance(Amount, teller, tellerAccount, transactionEntity, Events.Charge_Of_Saving_Withdrawal_Form.ToString(), isInterbranch, sourceBranchId, destinationBranchId, Name, IsMomocashCollection, isChargeIclussive, accountingDate, account, isRemittance);

                // Update customer account
                UpdateCustomerAccount(account, transactionEntity, null);


                return transactionEntity;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                _logger.LogError($"Error occurred while creating transaction history: {ex.Message}");
                throw; // Rethrow the exception for handling at a higher level
            }
        }

        private async Task<Transaction> CreateTransactionHistory(BulkDeposit request, Account account, Teller teller, Account tellerAccount, CashDepositParameter depositLimit, CustomerDto customer, bool isInterbranch, string sourceBranchId, string destinationBranchId, decimal customCharge, string Reference, Config config, bool IsMomocashCollection, bool isChargeIclussive, DateTime accountingDate, bool isRemittance,Remittance remittance)
        {
            // Calculate charges
            decimal charges = customCharge;

            if (!isRemittance)
            {
                charges = await CalculateCharges(request.Amount, depositLimit, customCharge, config, request.MembershipActivationAmount, accountingDate);
            }
            // Check if the charge is an offline charge
            bool isOfflineCharge = customCharge > 0;

            // Create transaction history for customer
            Transaction transactionEntity = CreateTransaction(request, account, teller, depositLimit, charges, sourceBranchId, destinationBranchId, isInterbranch, Reference, isOfflineCharge, request.MembershipActivationAmount, accountingDate,isRemittance,remittance);

            try
            {
                // Add the new Transaction entity to the repository
                _TransactionRepository.Add(transactionEntity);
                //Update Teller Account
                UpdateTellerAccountBalance(transactionEntity.Amount, teller, tellerAccount, transactionEntity, charges != 0 ? TransactionType.CASH_IN_MMK.ToString() : Events.Deposit.ToString(), isInterbranch, sourceBranchId, destinationBranchId, customer.Name, IsMomocashCollection, isChargeIclussive, accountingDate, account,isRemittance );
                // Update customer account
                UpdateCustomerAccount(account, transactionEntity, customer);

                return transactionEntity;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                _logger.LogError($"Error occurred while creating transaction history: {ex.Message}");
                throw; // Rethrow the exception for handling at a higher level
            }
        }
        private async Task<decimal> CalculateCharges(decimal amount, CashDepositParameter depositLimit, decimal customCharge, Config config, decimal MembershipActivationAmount, DateTime accountingDate)
        {
            // Check if the automatic charging system is used
            if (config.UseAutomaticChargingSystem)
            {
                // Determine if today is a holiday
                //bool isHoliday = CurrencyNotesMapper.IsTodayHoliday(config, accountingDate);
                bool isHoliday = await _holyDayRepository.IsHoliday(accountingDate);

                // Retrieve the saving product fees applicable for deposit on the current day
                var SavingProductFees = await _savingProductFeeRepository
                    .FindBy(x => x.SavingProductId == depositLimit.ProductId && x.Fee.IsAppliesOnHoliday == isHoliday && x.FeeType.ToLower() == "deposit" && x.IsDeleted == false)
                    .Include(x => x.Fee)
                    .Include(x => x.Fee.FeePolicies)
                    .ToListAsync();
                // Generate a holiday message for error reporting
                var holidayMessage = isHoliday ? "holiday" : "non-holiday";

                // If no saving product fees are found, throw an error
                if (!SavingProductFees.Any())
                {
                    var errorMessage = $"Cash-in charges are not configured for {depositLimit.Product.Name} on {holidayMessage} days. Please contact your administrator to configure charges for {depositLimit.Product.Name}.";
                    _logger.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Retrieve applicable fees based on the operation type and holiday status
                var Fees = CurrencyNotesMapper.GetSavingProductFee(SavingProductFees, OperationType.Deposit.ToString(), isHoliday);

                // Retrieve fees by type (range and rate)
                var range = CurrencyNotesMapper.GetFeeByFeeType(Fees, "Range", false);
                var rateX = CurrencyNotesMapper.GetFeeByFeeType(Fees, "Rate", false);

                // Calculate the rate based on the fee policies and custom charge
                decimal rate = rateX != null && customCharge > 0 ? 0 : rateX?.Fee?.FeePolicies?.FirstOrDefault()?.Value ?? 0;

                // Calculate the range charge based on the fee policies and amount
                decimal rangeChargeCalculated = range != null ? CurrencyNotesMapper.GetFeePolicyRange(range.Fee.FeePolicies.ToList(), amount).Charge : 0;

                // Determine the range charge based on the custom charge or calculated range charge
                decimal rangeCharge = customCharge > 0 ? customCharge : rangeChargeCalculated;

                // Calculate and return the total customer charges
                return XAFWallet.CalculateCustomerCharges(rate, rangeCharge, 0, amount, MembershipActivationAmount);
            }
            else
            {
                // Calculate and return the total customer charges when the automatic charging system is not used
                return XAFWallet.CalculateCustomerCharges(0, customCharge, 0, amount, MembershipActivationAmount);
            }
        }
        private Transaction CreateTransaction(decimal Amount, Account account, Teller teller, CashDepositParameter deposit, decimal charges, string sourceBranchId, string destinationBranchId, bool isInterBranch, string Reference, DateTime accountingDate)
        {
            if (account.AccountType == AccountType.MobileMoneyMTN.ToString() || account.AccountType == AccountType.MobileMoneyORANGE.ToString())
            {
                // Calculate balance and credit based on original amount and fees, if IsOfflineCharge is true
                decimal balance = account.Balance;
                decimal originalAmount = Amount;
                string N_A = "N/A";
                // Create the transaction entity
                var transactionEntityEntryFee = new Transaction
                {
                    Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique transaction ID
                    CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow), // Set transaction creation date
                    Status = TransactionStatus.COMPLETED.ToString(), // Set transaction status to completed
                    TransactionReference = Reference, // Set transaction reference
                    ExternalReference = N_A, // Set external reference
                    IsExternalOperation = false, // Set external operation flag
                    ExternalApplicationName = N_A, // Set external application name
                    SourceType = OperationSourceType.Web_Portal.ToString(), // Set source type
                    Currency = Currency.XAF.ToString(), // Set currency
                    TransactionType = deposit.DepositType, // Set transaction type (deposit)
                    AccountNumber = account.AccountNumber, // Set account number
                    PreviousBalance = account.Balance, // Set previous balance
                    AccountId = account.Id, // Set account ID
                    CustomerId = account.CustomerId, // Set customer ID
                    ProductId = account.ProductId, // Set product ID
                    SenderAccountId = account.Id, // Set sender account ID
                    ReceiverAccountId = account.Id, // Set receiver account ID
                    BankId = teller.BankId, // Set bank ID
                    Operation = TransactionType.WITHDRAWAL_MOBILEMONEY.ToString(), // Set operation type (deposit)
                    BranchId = teller.BranchId, // Set branch ID
                    OriginalDepositAmount = originalAmount, // Set original deposit amount including fees
                    Fee = charges, // Set fee (charges)
                    Tax = 0, // Set tax (assuming tax is not applicable)
                    Amount = -(Amount), // Set amount (excluding fees)
                    Note = $"Statement: A Withdrawal of {BaseUtilities.FormatCurrency(originalAmount)} was completed on {account.AccountType} account, Reference: {Reference}.", // Set transaction note
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
                    Debit = -(Amount), // Set debit amount (assuming no debit)
                    DestinationBrachId = destinationBranchId,
                    OperationCharge = 0,
                    AccountingDate = accountingDate,
                    ReceiptTitle = $"{account.AccountType} Cashin Receipt, Reference: " + Reference,
                    WithrawalFormCharge = Amount,  // Set destination branch ID
                    SourceBrachId = sourceBranchId, // Set source branch ID
                    IsInterBrachOperation = isInterBranch, // Set flag indicating if inter-branch operation
                    DestinationBranchCommission = isInterBranch ? XAFWallet.CalculateCommission(deposit.DestinationBranchOfficeShare, charges) : 0, // Calculate destination branch commission
                    SourceBranchCommission = isInterBranch ? XAFWallet.CalculateCommission(deposit.SourceBrachOfficeShare, charges) : charges // Calculate source branch commission
                };

                return transactionEntityEntryFee; // Return the transaction entity

            }
            else
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
                    ExternalReference = N_A, // Set external reference
                    IsExternalOperation = false, // Set external operation flag
                    ExternalApplicationName = N_A, // Set external application name
                    SourceType = OperationSourceType.Web_Portal.ToString(), // Set source type
                    Currency = Currency.XAF.ToString(), // Set currency
                    TransactionType = deposit.DepositType, // Set transaction type (deposit)
                    AccountNumber = account.AccountNumber, // Set account number
                    PreviousBalance = account.Balance, // Set previous balance
                    AccountId = account.Id, // Set account ID
                    CustomerId = account.CustomerId, // Set customer ID
                    ProductId = account.ProductId, // Set product ID
                    SenderAccountId = account.Id, // Set sender account ID
                    ReceiverAccountId = account.Id, // Set receiver account ID
                    BankId = teller.BankId, // Set bank ID
                    Operation = TransactionType.WITHDRAWAL_REQUEST_FORM_FEE.ToString(), // Set operation type (deposit)
                    BranchId = teller.BranchId, // Set branch ID
                    OriginalDepositAmount = originalAmount, // Set original deposit amount including fees
                    Fee = charges, // Set fee (charges)
                    Tax = 0, // Set tax (assuming tax is not applicable)
                    Amount = credit, // Set amount (excluding fees)
                    Note = $"Statement: A deposit of {BaseUtilities.FormatCurrency(originalAmount)} was completed for the payment of saving withdrawal notification, Reference: {Reference}.", // Set transaction note
                    OperationType = OperationType.Debit.ToString(), // Set operation type (credit)
                    FeeType = Events.Charge_Of_Saving_Withdrawal_Form.ToString(), // Set fee type
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
                    Credit = credit, // Set credit amount (including fees if IsOfflineCharge is true, otherwise excluding fees)
                    Debit = 0, // Set debit amount (assuming no debit)
                    DestinationBrachId = destinationBranchId,
                    OperationCharge = 0,
                    AccountingDate = accountingDate,
                    ReceiptTitle = "Cash Receipt, Saving Withdrawal Form Fee. Reference: " + Reference,
                    WithrawalFormCharge = Amount,  // Set destination branch ID
                    SourceBrachId = sourceBranchId, // Set source branch ID
                    IsInterBrachOperation = isInterBranch, // Set flag indicating if inter-branch operation
                    DestinationBranchCommission = isInterBranch ? XAFWallet.CalculateCommission(deposit.DestinationBranchOfficeShare, charges) : 0, // Calculate destination branch commission
                    SourceBranchCommission = isInterBranch ? XAFWallet.CalculateCommission(deposit.SourceBrachOfficeShare, charges) : charges // Calculate source branch commission
                };

                return transactionEntityEntryFee; // Return the transaction entity

            }
        }

        private Transaction CreateTransaction(BulkDeposit request, Account account, Teller teller, CashDepositParameter deposit, decimal charges, string sourceBranchId, string destinationBranchId, bool isInterBranch, string Reference, bool IsOfflineCharge, decimal MemberActivationFee, DateTime accountingDate, bool isRemittance, Remittance remittance = null)
        {

            if (account.AccountType == AccountType.MobileMoneyMTN.ToString() || account.AccountType == AccountType.MobileMoneyORANGE.ToString())
            {
                // Calculate balance and credit based on original amount and fees, if IsOfflineCharge is true

                decimal balance = IsOfflineCharge ? account.Balance - request.Amount : account.Balance - request.Amount + charges;
                decimal credit = IsOfflineCharge ? request.Amount : request.Amount - charges;
                decimal originalAmount = request.Amount;

                //if (isChargeIclussive)
                //{
                //    credit = 0;
                //    balance = 0;

                //    credit = request.Amount - charges;
                //    balance = account.Balance + credit;
                //    originalAmount = request.Amount;
                //}
                // Create the transaction entity

                var transactionEntityEntryFee = new Transaction
                {
                    Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique transaction ID
                    CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow), // Set transaction creation date
                    Status = TransactionStatus.COMPLETED.ToString(), // Set transaction status to completed
                    TransactionReference = Reference, // Set transaction reference
                    ExternalReference = request.ExternalReference, // Set external reference
                    IsExternalOperation = request.IsExternalOperation, // Set external operation flag
                    ExternalApplicationName = request.ExternalApplicationName, // Set external application name
                    SourceType = request.SourceType, // Set source type
                    Currency = request.Currency, // Set currency
                    TransactionType = deposit.DepositType, // Set transaction type (deposit)
                    AccountNumber = account.AccountNumber, // Set account number
                    PreviousBalance = account.Balance, // Set previous balance
                    AccountId = account.Id, // Set account ID
                    CustomerId = account.CustomerId, // Set customer ID
                    ProductId = account.ProductId, // Set product ID
                    SenderAccountId = account.Id, // Set sender account ID
                    ReceiverAccountId = account.Id, // Set receiver account ID
                    BankId = teller.BankId, // Set bank ID
                    Operation = TransactionType.WITHDRAWAL_MOBILEMONEY.ToString(), // Set operation type (deposit)
                    BranchId = teller.BranchId, // Set branch ID
                    OriginalDepositAmount = originalAmount, // Set original deposit amount including fees
                    Fee = charges, // Set fee (charges)
                    Tax = 0, // Set tax (assuming tax is not applicable)
                    Amount = credit, // Set amount (excluding fees)
                    Note = $"Note:{request.Note} - Statement: A cash withdrawal of {BaseUtilities.FormatCurrency(originalAmount)} with charge: {BaseUtilities.FormatCurrency(charges)} was completed on account {account.AccountType}, Reference: {Reference}", // Set transaction note
                    OperationType = OperationType.Credit.ToString(), // Set operation type (credit)
                    FeeType = Events.None.ToString(), // Set fee type
                    TellerId = teller.Id, // Set teller ID
                    DepositerNote = request.Depositer.DepositerNote, // Set depositer note
                    DepositerTelephone = request.Depositer.DepositerTelephone, // Set depositer telephone
                    DepositorIDNumber = request.Depositer.DepositorIDNumber, // Set depositor ID number
                    DepositorIDExpiryDate = request.Depositer.DepositorIDExpiryDate, // Set depositor ID expiry date
                    DepositorIDIssueDate = request.Depositer.DepositorIDIssueDate, // Set depositor ID issue date
                    DepositorIDNumberPlaceOfIssue = request.Depositer.DepositorIDNumberPlaceOfIssue, // Set depositor ID place of issue
                    IsDepositDoneByAccountOwner = request.IsDepositDoneByAccountOwner, // Set flag indicating if deposit is done by account owner
                    DepositorName = request.Depositer.DepositorName, // Set depositor name
                    Balance = balance, // Set balance after deposit (including original amount)
                    Credit = credit, // Set credit amount (including fees if IsOfflineCharge is true, otherwise excluding fees)
                    Debit = 0, // Set debit amount (assuming no debit)
                    DestinationBrachId = destinationBranchId,
                    OperationCharge = charges - MemberActivationFee,
                    WithrawalFormCharge = 0,  // Set destination branch ID
                    SourceBrachId = sourceBranchId, // Set source branch ID
                    IsInterBrachOperation = isInterBranch, // Set flag indicating if inter-branch operation
                    DestinationBranchCommission = XAFWallet.CalculateCommission(deposit.DestinationBranchOfficeShare, charges), // Calculate destination branch commission
                    SourceBranchCommission = XAFWallet.CalculateCommission(deposit.SourceBrachOfficeShare, charges), // Calculate source branch commission
                    ReceiptTitle = $"{account.AccountType} Cash-In Receipt Reference: " + Reference,
                    AccountingDate = accountingDate,
                    CheckName = request.CheckName,
                    CheckNumber = request.CheckNumber,
                    IsSWS = request.IsSWS,




                    HeadOfficeCommission = XAFWallet.CalculateCommission(deposit.HeadOfficeShare, charges), // Calculate source branch commission
                    FluxAndPTMCommission = XAFWallet.CalculateCommission(deposit.FluxAndPTMShare, charges), // Calculate source branch commission
                    CamCCULCommission = XAFWallet.CalculateCommission(deposit.CamCCULShare, charges), // Calculate source branch commission
                };

                return transactionEntityEntryFee; // Return the transaction entity
            }
            else
            {

                if (isRemittance)
                {
                    IsOfflineCharge=remittance.ChargeType==ChargeType.Inclussive.ToString() ? true : false;
                    if (IsOfflineCharge)
                    {
                        request.Amount=remittance.InitailAmount;
                    }
                    else
                    {
                        request.Amount=remittance.InitailAmount+remittance.Fee;
                    }
                }
                // Calculate balance and credit based on original amount and fees, if IsOfflineCharge is true
                decimal balance = IsOfflineCharge ? account.Balance + request.Amount : account.Balance + request.Amount - charges;
                decimal credit = IsOfflineCharge ? request.Amount : request.Amount - charges;
                decimal originalAmount = IsOfflineCharge ? charges + request.Amount : request.Amount;
                //if (isChargeIclussive)
                //{
                //    credit = 0;
                //    balance = 0;

                //    credit = request.Amount - charges;
                //    balance = account.Balance + credit;
                //    originalAmount = request.Amount;
                //}
                string statement = null;
                string note = null;

                // Assuming these variables are already defined somewhere in your code
                string accountType = account.AccountType; // Example value
                string requestNote = request.Note;
                string accountNumber = account.AccountNumber;
                string reference = Reference;
                bool isOfflineCharge = request.IsChargesInclussive;
                string receiptTitle;
                if (accountType.ToLower() == AccountType.Loan.ToString().ToLower())
                {
                    if (request.OperationType == "LoanRepaymentMomocashCollection")
                    {
                        receiptTitle = "Momocash Collection Loan Repayment Reference: " + Reference;
                        note = $"Note: {requestNote} - Statement: Momocash Collection loan repayment of {BaseUtilities.FormatCurrency(originalAmount)} with charge: {BaseUtilities.FormatCurrency(charges)} was completed on account number {accountNumber} [{accountType}], Reference: {reference}, Charges inclusive? {isOfflineCharge}";

                    }
                    else
                    {
                        receiptTitle = "Loan Repayment Receipt Reference: " + Reference;
                        note = $"Note: {requestNote} - Statement: A repayment of {BaseUtilities.FormatCurrency(originalAmount)} was completed on account number {accountNumber} [{accountType}], Reference: {reference}, Charges inclusive? {isOfflineCharge}"; // Modify this with the actual statement logic if needed

                    }
                }
                else
                {
                    if (isRemittance)
                    {
                        request.OperationType="Remittance";
                    }
                    if (request.OperationType == "CashInMomocashCollection")
                    {
                        receiptTitle = "Momocash Collection Deposit Receipt Reference: " + Reference;
                        note = $"Note: {requestNote} - Statement: Momocash Collection deposit of {BaseUtilities.FormatCurrency(originalAmount)} with charge: {BaseUtilities.FormatCurrency(charges)} was completed on account number {accountNumber} [{accountType}], Reference: {reference}, Charges inclusive? {isOfflineCharge}";

                    }
                    else if (request.OperationType == "LoanRepaymentMomocashCollection")
                    {
                        receiptTitle = "Momocash Collection Loan Repayment Receipt Reference: " + Reference;
                        note = $"Note: {requestNote} - Statement: Momocash Collection Loan Repayment deposit of {BaseUtilities.FormatCurrency(originalAmount)} with charge: {BaseUtilities.FormatCurrency(charges)} was completed on account number {accountNumber} [{accountType}], Reference: {reference}, Charges inclusive? {isOfflineCharge}";

                    }
                    else if (request.OperationType=="Remittance")
                    {
                        receiptTitle = "Remittance Receipt, Reference: " + Reference;

                        note= $"Note: {requestNote} - A remittance transfer of {BaseUtilities.FormatCurrency(originalAmount)} was successfully initiated by {remittance.SenderName} to {remittance.ReceiverName}. " + $"ference: {reference}. Charges applied: {BaseUtilities.FormatCurrency(remittance.Fee)}";

                        // Create the transaction entity
                        var transaction = new Transaction
                        {
                            Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique transaction ID
                            CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow), // Set transaction creation date
                            Status = TransactionStatus.COMPLETED.ToString(), // Set transaction status to completed
                            TransactionReference = Reference, // Set transaction reference
                            ExternalReference = request.ExternalReference, // Set external reference
                            IsExternalOperation = request.IsExternalOperation, // Set external operation flag
                            ExternalApplicationName = request.ExternalApplicationName, // Set external application name
                            SourceType = request.SourceType, // Set source type
                            Currency = request.Currency, // Set currency
                            TransactionType = deposit.DepositType, // Set transaction type (deposit)
                            AccountNumber = account.AccountNumber, // Set account number
                            PreviousBalance = account.Balance, // Set previous balance
                            AccountId = account.Id, // Set account ID
                            CustomerId = account.CustomerId, // Set customer ID
                            ProductId = account.ProductId, // Set product ID
                            SenderAccountId = account.Id, // Set sender account ID
                            ReceiverAccountId = account.Id, // Set receiver account ID
                            BankId = teller.BankId, // Set bank ID
                            Operation = TransactionType.CASH_IN.ToString(), // Set operation type (deposit)
                            BranchId = teller.BranchId, // Set branch ID
                            OriginalDepositAmount = originalAmount, // Set original deposit amount including fees
                            Fee = charges, // Set fee (charges)
                            Tax = 0, // Set tax (assuming tax is not applicable)
                            Amount = credit, // Set amount (excluding fees)
                            Note = note,
                            OperationType = OperationType.Credit.ToString(), // Set operation type (credit)
                            FeeType = Events.ChargeOfDeposit.ToString(), // Set fee type
                            TellerId = teller.Id, // Set teller ID
                            DepositerNote = request.Depositer.DepositerNote, // Set depositer note
                            DepositerTelephone = request.Depositer.DepositerTelephone, // Set depositer telephone
                            DepositorIDNumber = request.Depositer.DepositorIDNumber, // Set depositor ID number
                            DepositorIDExpiryDate = request.Depositer.DepositorIDExpiryDate, // Set depositor ID expiry date
                            DepositorIDIssueDate = request.Depositer.DepositorIDIssueDate, // Set depositor ID issue date
                            DepositorIDNumberPlaceOfIssue = request.Depositer.DepositorIDNumberPlaceOfIssue, // Set depositor ID place of issue
                            IsDepositDoneByAccountOwner = request.IsDepositDoneByAccountOwner, // Set flag indicating if deposit is done by account owner
                            DepositorName = request.Depositer.DepositorName, // Set depositor name
                            Balance = balance, // Set balance after deposit (including original amount)
                            Credit = credit, // Set credit amount (including fees if IsOfflineCharge is true, otherwise excluding fees)
                            Debit = 0, // Set debit amount (assuming no debit)
                            DestinationBrachId = destinationBranchId,
                            OperationCharge = charges - MemberActivationFee,
                            WithrawalFormCharge = 0,  // Set destination branch ID
                            SourceBrachId = sourceBranchId, // Set source branch ID
                            IsInterBrachOperation = isInterBranch, // Set flag indicating if inter-branch operation
                            DestinationBranchCommission = isRemittance? charges-XAFWallet.CalculateCommission(deposit.SourceBrachOfficeShare, charges): XAFWallet.CalculateCommission(deposit.DestinationBranchOfficeShare, charges), // Calculate destination branch commission
                            SourceBranchCommission = XAFWallet.CalculateCommission(deposit.SourceBrachOfficeShare, charges), // Calculate source branch commission
                            ReceiptTitle = receiptTitle,
                            IsSWS = request.IsSWS,
                            AccountingDate = accountingDate,
                            CheckNumber = request.CheckNumber,
                            CheckName = request.CheckName,
                            HeadOfficeCommission = XAFWallet.CalculateCommission(deposit.HeadOfficeShare, charges), // Calculate source branch commission
                            FluxAndPTMCommission = XAFWallet.CalculateCommission(deposit.FluxAndPTMShare, charges), // Calculate source branch commission
                            CamCCULCommission = XAFWallet.CalculateCommission(deposit.CamCCULShare, charges), // Calculate source branch commission


                        };

                        return transaction; // Return the transaction entity



                    }
                    else
                    {
                        receiptTitle = "Cash Deposit Receipt Reference: " + Reference;
                        note = $"Note: {requestNote} - Statement: A deposit of {BaseUtilities.FormatCurrency(originalAmount)} with charge: {BaseUtilities.FormatCurrency(charges)} was completed on account number {accountNumber} [{accountType}], Reference: {reference}, Charges inclusive? {isOfflineCharge}";

                    }
                }



                // Set the transaction note or statement based on the account type
                string transactionNoteOrStatement = note;
                // Create the transaction entity
                var transactionEntityEntryFee = new Transaction
                {
                    Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique transaction ID
                    CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow), // Set transaction creation date
                    Status = TransactionStatus.COMPLETED.ToString(), // Set transaction status to completed
                    TransactionReference = Reference, // Set transaction reference
                    ExternalReference = request.ExternalReference, // Set external reference
                    IsExternalOperation = request.IsExternalOperation, // Set external operation flag
                    ExternalApplicationName = request.ExternalApplicationName, // Set external application name
                    SourceType = request.SourceType, // Set source type
                    Currency = request.Currency, // Set currency
                    TransactionType = deposit.DepositType, // Set transaction type (deposit)
                    AccountNumber = account.AccountNumber, // Set account number
                    PreviousBalance = account.Balance, // Set previous balance
                    AccountId = account.Id, // Set account ID
                    CustomerId = account.CustomerId, // Set customer ID
                    ProductId = account.ProductId, // Set product ID
                    SenderAccountId = account.Id, // Set sender account ID
                    ReceiverAccountId = account.Id, // Set receiver account ID
                    BankId = teller.BankId, // Set bank ID
                    Operation = TransactionType.CASH_IN.ToString(), // Set operation type (deposit)
                    BranchId = teller.BranchId, // Set branch ID
                    OriginalDepositAmount = originalAmount, // Set original deposit amount including fees
                    Fee = charges, // Set fee (charges)
                    Tax = 0, // Set tax (assuming tax is not applicable)
                    Amount = credit, // Set amount (excluding fees)
                    Note = transactionNoteOrStatement,
                    OperationType = OperationType.Credit.ToString(), // Set operation type (credit)
                    FeeType = Events.ChargeOfDeposit.ToString(), // Set fee type
                    TellerId = teller.Id, // Set teller ID
                    DepositerNote = request.Depositer.DepositerNote, // Set depositer note
                    DepositerTelephone = request.Depositer.DepositerTelephone, // Set depositer telephone
                    DepositorIDNumber = request.Depositer.DepositorIDNumber, // Set depositor ID number
                    DepositorIDExpiryDate = request.Depositer.DepositorIDExpiryDate, // Set depositor ID expiry date
                    DepositorIDIssueDate = request.Depositer.DepositorIDIssueDate, // Set depositor ID issue date
                    DepositorIDNumberPlaceOfIssue = request.Depositer.DepositorIDNumberPlaceOfIssue, // Set depositor ID place of issue
                    IsDepositDoneByAccountOwner = request.IsDepositDoneByAccountOwner, // Set flag indicating if deposit is done by account owner
                    DepositorName = request.Depositer.DepositorName, // Set depositor name
                    Balance = balance, // Set balance after deposit (including original amount)
                    Credit = credit, // Set credit amount (including fees if IsOfflineCharge is true, otherwise excluding fees)
                    Debit = 0, // Set debit amount (assuming no debit)
                    DestinationBrachId = destinationBranchId,
                    OperationCharge = charges - MemberActivationFee,
                    WithrawalFormCharge = 0,  // Set destination branch ID
                    SourceBrachId = sourceBranchId, // Set source branch ID
                    IsInterBrachOperation = isInterBranch, // Set flag indicating if inter-branch operation
                    DestinationBranchCommission = XAFWallet.CalculateCommission(deposit.DestinationBranchOfficeShare, charges), // Calculate destination branch commission
                    SourceBranchCommission = XAFWallet.CalculateCommission(deposit.SourceBrachOfficeShare, charges), // Calculate source branch commission
                    ReceiptTitle = receiptTitle,
                    IsSWS = request.IsSWS,
                    AccountingDate = accountingDate,
                    CheckNumber = request.CheckNumber,
                    CheckName = request.CheckName,
                    HeadOfficeCommission = XAFWallet.CalculateCommission(deposit.HeadOfficeShare, charges), // Calculate source branch commission
                    FluxAndPTMCommission = XAFWallet.CalculateCommission(deposit.FluxAndPTMShare, charges), // Calculate source branch commission
                    CamCCULCommission = XAFWallet.CalculateCommission(deposit.CamCCULShare, charges), // Calculate source branch commission


                };

                return transactionEntityEntryFee; // Return the transaction entity
            }


        }

        /// <summary>
        /// Updates the teller's account balance based on the deposit amount and processes related transactions.
        /// </summary>
        /// <param name="depositAmount">The amount to deposit.</param>
        /// <param name="teller">The teller performing the operation.</param>
        /// <param name="tellerAccount">The teller's account to be credited.</param>
        /// <param name="transaction">Transaction details including fees and charges.</param>
        /// <param name="eventType">The type of event (e.g., deposit, withdrawal).</param>
        /// <param name="isInterbranch">Indicates if the transaction is inter-branch.</param>
        /// <param name="sourceBranchId">Source branch ID for inter-branch operations.</param>
        /// <param name="destinationBranchId">Destination branch ID for inter-branch operations.</param>
        /// <param name="Name">The name of the entity/person involved in the transaction.</param>
        /// <param name="IsMomokashCollection">Indicates if this is a Momokash collection.</param>
        /// <param name="IsChargesInclusive">Indicates if charges are included in the deposit amount.</param>
        /// <param name="accountingDate">The accounting date of the transaction.</param>
        private void UpdateTellerAccountBalance(decimal depositAmount, Teller teller, Account tellerAccount, Transaction transaction, string eventType, bool isInterbranch, string sourceBranchId, string destinationBranchId, string Name, bool IsMomokashCollection, bool IsChargesInclusive, DateTime accountingDate, Account account,bool isRemittance)
        {
          
            tellerAccount.Product = null;
            account.Product = null;
            if (IsChargesInclusive)
            {
                depositAmount-=transaction.Fee;
                _accountRepository.CreditAccount(tellerAccount, depositAmount);
            }
            else
            {
                if (isRemittance)
                {
                    _accountRepository.CreditAccount(tellerAccount, depositAmount);

                }
                else
                {
                    var totalAmount = depositAmount + transaction.Fee;
                    _accountRepository.CreditAccount(tellerAccount, totalAmount);

                }
            }
            bool isMembersubscriptionAccount = false;
            if (AccountType.Membership.ToString() == account.AccountType)
            {
                isMembersubscriptionAccount = true;
            }
            // Step 2: Create a teller operation for the deposit and add it to the repository.
            var tellerOperationDeposit = CreateTellerOperation(
                depositAmount,
                OperationType.Credit,
                teller,
                tellerAccount,
                tellerAccount.Balance,
                transaction,
                eventType,
                isInterbranch,
                sourceBranchId,
                destinationBranchId,
                Name,
                IsMomokashCollection, false,
                accountingDate, isMembersubscriptionAccount,isRemittance
            );
            _tellerOperationRepository.Add(tellerOperationDeposit);

            // Step 3: Handle operation charges if applicable.
            // If there is an operation charge (e.g., service fee, withdrawal charge), process it.
            if (transaction.Fee > 0)
            {
                // If charges are inclusive, no additional operation is created since it's already deducted.
                if (!IsChargesInclusive)
                {
                    // Create a teller operation for the operation charge and add it to the repository.
                    var chargeOperation = CreateTellerOperation(
                        transaction.Fee,
                        OperationType.Credit, // Assume the operation charge is credited to the teller's account
                        teller,
                        tellerAccount,
                        tellerAccount.Balance,
                        transaction,
                        eventType,
                        isInterbranch,
                        sourceBranchId,
                        destinationBranchId,
                        Name,
                        IsMomokashCollection,
                        true, // This indicates it is a charge operation
                        accountingDate, isMembersubscriptionAccount,isRemittance
                    );
                    _tellerOperationRepository.Add(chargeOperation);
                }
            }

        }

       
        private TellerOperation CreateTellerOperation(decimal amount, OperationType operationType, Teller teller, Account tellerAccount, decimal currentBalance, Transaction transaction, string eventType, bool isInterBranch, string sourceBranchId, string destinationBranchId, string name, bool isMomokashCollection, bool hasCharge, DateTime accountingDate, bool isMembersubscriptionAccount,bool isRemittance)
        {
            // Generate common teller operation fields
            var tellerOperation = new TellerOperation
            {
                OpenOfDayReference = tellerAccount.OpenningOfDayReference == null ? transaction.TransactionReference : tellerAccount.OpenningOfDayReference,
                Id = BaseUtilities.GenerateUniqueNumber(), // Unique ID for the operation
                AccountID = tellerAccount.Id, // Account ID associated with the operation
                AccountNumber = tellerAccount.AccountNumber, // Account number associated with the operation
                Amount = amount, // Amount for the operation
                CurrentBalance = currentBalance, // Current balance of the account
                Date = accountingDate, // Operation date converted to local time
                PreviousBalance = tellerAccount.PreviousBalance, // Previous balance of the account
                TellerID = teller.Id, // Teller ID performing the operation
                TransactionReference = transaction.TransactionReference, // Transaction reference
                UserID = _userInfoToken.Id, // User ID performing the operation
                EventName = eventType, // Event type associated with the operation
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode), // Generated reference number
                CustomerId = transaction.CustomerId, // Customer ID associated with the transaction
                MemberAccountNumber = transaction.AccountNumber, // Member's account number associated with the transaction
                DestinationBrachId = destinationBranchId, // Destination branch ID
                SourceBranchId = sourceBranchId, // Source branch ID
                IsInterBranch = isInterBranch, // Flag indicating if the operation is inter-branch
                MemberName = name, // Member's name
                DestinationBranchCommission = transaction.DestinationBranchCommission, // Destination branch commission
                SourceBranchCommission = transaction.SourceBranchCommission, // Source branch commission
                BranchId = transaction.BranchId, // Source branch commission
                BankId = transaction.BankId,
                AccountingDate = accountingDate,
                IsCashOperation = isMomokashCollection ? false : true // Source branch commission
            };

            // Set specific fields for Momokash collections
            if (isMomokashCollection)
            {
                tellerOperation.Description = $"{TransactionType.CASH_IN_MMK} of {BaseUtilities.FormatCurrency(amount)} to Account Number: {tellerAccount.AccountNumber}";
                tellerOperation.OperationType = OperationType.Credit.ToString();
                tellerOperation.TransactionType = TransactionType.CASH_IN_MMK.ToString();
            }
            else if (isRemittance)
            {
                if (hasCharge)
                {
                    tellerOperation.Description = $"{TransactionType.FEE_Remittance} of {BaseUtilities.FormatCurrency(transaction.Fee)} to Account Number: {tellerAccount.AccountNumber}";
                    tellerOperation.OperationType = operationType.ToString();
                    tellerOperation.TransactionType = TransactionType.FEE_Remittance.ToString();
                }
                else
                {
                    tellerOperation.Description = $"{TransactionType.CASH_IN_Remittance} of {BaseUtilities.FormatCurrency(amount)} to Account Number: {tellerAccount.AccountNumber}";
                    tellerOperation.OperationType = OperationType.Credit.ToString();
                    tellerOperation.TransactionType = TransactionType.CASH_IN_Remittance.ToString();
                }


            }
            else // Set fields for regular operations
            {

                if (hasCharge)
                {
                    tellerOperation.Description = $"{TransactionType.FEE} of {BaseUtilities.FormatCurrency(transaction.Fee)} to Account Number: {tellerAccount.AccountNumber}";
                    tellerOperation.OperationType = operationType.ToString();
                    tellerOperation.TransactionType = TransactionType.FEE.ToString();
                }
                else
                {
                    if (eventType == Events.Charge_Of_Saving_Withdrawal_Form.ToString())
                    {
                        tellerOperation.Description = $"{TransactionType.CASHIN_FORM_FEE} of {BaseUtilities.FormatCurrency(amount)} to Account Number: {tellerAccount.AccountNumber}";
                        tellerOperation.OperationType = operationType.ToString();
                        tellerOperation.TransactionType = TransactionType.CASHIN_FORM_FEE.ToString();

                    }
                    else
                    {
                        if (isMembersubscriptionAccount)
                        {
                            tellerOperation.Description = $"{TransactionType.CASH_IN_MEMBER_SUB_FEE} of {BaseUtilities.FormatCurrency(amount)} to Account Number: {tellerAccount.AccountNumber}";
                            tellerOperation.OperationType = operationType.ToString();
                            tellerOperation.TransactionType = TransactionType.CASH_IN_MEMBER_SUB_FEE.ToString();
                        }
                        else
                        {
                            tellerOperation.Description = $"{TransactionType.CASH_IN} of {BaseUtilities.FormatCurrency(amount)} to Account Number: {tellerAccount.AccountNumber}";
                            tellerOperation.OperationType = operationType.ToString();
                            tellerOperation.TransactionType = TransactionType.CASH_IN.ToString();
                        }
                    }
                }
            }


            return tellerOperation; // Return the created TellerOperation object
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
                //account.AccountName = $"{account.Product.Name} {customer.FirstName} {customer.LastName}"; // Update account name
                account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance
                account.BranchCode = customer.BranchCode ?? "000";
                account.CustomerName = $"{customer.FirstName} {customer.LastName}";
                _accountRepository.Update(account);

            }
            else
            {
                // If not in progress, update account details for normal transaction

                if (customer != null)
                {
                    account.PreviousBalance = account.Balance; // Set previous balance to current balance
                    account.Balance = transactionEntityEntryFee.Balance; // Set balance to the balance brought forward from the transaction
                    account.BranchCode = customer.BranchCode ?? "000";
                    account.CustomerName = $"{customer.FirstName} {customer.LastName}";
                    account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance
                    _accountRepository.Update(account);

                }

            }
            // Update the account in the repository

        }
        private async Task<CashDepositParameter> GetDepositLimit(SavingProduct savingProduct, decimal Amount, string interOperationType, bool IsInterBranchOperation)
        {
            var depositLimits = savingProduct.CashDepositParameters.ToList();
            var depositLimit = depositLimits.FirstOrDefault(dl => dl.DepositType == interOperationType);
            if (depositLimit == null)
            {
                var errorMessage = $"Deposit failed. Deposit limits are not configure for {interOperationType} operation on ordinary account {savingProduct.AccountType}. Please contact your adminitrator set the limits";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            if (DecimalComparer.IsLessThan(Amount, depositLimit.MinAmount) || DecimalComparer.IsGreaterThan(Amount, depositLimit.MaxAmount))
            {
                var errorMessage = $"Error occurred while initiating deposit with amount: {Amount}. Deposit amount must be between {depositLimit.MinAmount} and {depositLimit.MaxAmount}.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            if (!savingProduct.IsDepositAllowedDirectlyTothisAccount)
            {
                var errorMessage = $"This account type: {savingProduct.AccountType} does not permit direct deposit.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            if (!savingProduct.AllowInterbranchDeposit && IsInterBranchOperation)
            {
                var errorMessage = $"This account type: {savingProduct.AccountType} does not support inter-branch operation";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            return depositLimit;
        }




    }
}
