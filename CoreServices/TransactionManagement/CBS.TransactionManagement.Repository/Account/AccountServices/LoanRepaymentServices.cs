using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Repository.AccountServices
{
    public class LoanRepaymentServices : ILoanRepaymentServices
    {
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly IAccountRepository _accountRepository;
        public IMediator _mediator { get; set; }
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly ILogger<AccountRepository> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        public LoanRepaymentServices(ITellerOperationRepository tellerOperationRepository = null, IMediator mediator = null, ITransactionRepository transactionRepository = null, ILogger<AccountRepository> logger = null, IAccountRepository accountRepository = null, IUnitOfWork<TransactionContext> uow = null, ISavingProductFeeRepository savingProductFeeRepository = null, UserInfoToken userInfoToken = null, ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null)
        {
            _tellerOperationRepository = tellerOperationRepository;
            _mediator = mediator;
            _TransactionRepository = transactionRepository;
            _logger = logger;
            _accountRepository = accountRepository;
            _userInfoToken = userInfoToken;
        }

        public async Task<TransactionDto> LoanDepositCash(BulkDeposit request, Teller teller, Account tellerAccount, Account loanTransitAccount, TellerLoanPaymentObject paymentObject)
        {//LoanRepaymentMomocashCollection
            if (paymentObject.LoanRepaymentType != "LoanRepaymentMomocashCollection")
            {
                // 1. Credit the teller's account with the deposit amount specified in the paymentObject.
                _accountRepository.CreditAccount(tellerAccount, paymentObject.Amount);
                // 2. Create a TellerOperation entity for the deposit action using the provided teller, teller account, and payment details.
                var tellerOperationDeposit = CreateTellerOperation(teller, tellerAccount, paymentObject);

                // 3. Add the newly created TellerOperation entity to the repository for further processing or saving.
                _tellerOperationRepository.Add(tellerOperationDeposit);

            }


            // 4. Create a Transaction entity for the deposit, specifying the request, loan transit account, teller, and payment details.
            var transaction = CreateTransaction(request, loanTransitAccount, teller, paymentObject);
            transaction.Account = loanTransitAccount;
            // 5. Map the Transaction entity to a TransactionDto object, including currency notes and user information.
            var transactionDto = CurrencyNotesMapper.MapTransactionToDto(transaction, request.currencyNotes, _userInfoToken);
            transaction.Account = null;

            // 6. Assign the current teller to the transactionDto for record-keeping and reference.
            transactionDto.Teller = teller;
            // 7. Log the successful deposit operation, including the Teller ID and the transaction reference.
            _logger?.LogInformation("Deposit successful for {TellerId}, Transaction Reference: {TransactionReference}", teller.Id, paymentObject.TransactionReference);

            // 8. Return the created transaction DTO to the caller.
            return transactionDto;
        }

        private Transaction CreateTransaction(BulkDeposit request, Account account, Teller teller, TellerLoanPaymentObject tellerLoan)
        {

            decimal balance = 0;
            decimal credit = tellerLoan.Amount;
            decimal originalAmount = tellerLoan.Amount;
            string statement = null;
            string note = null;
            string accountType = account.AccountType; // Example value
            string requestNote = request.Note;
            string accountNumber = account.AccountNumber;
            string reference = tellerLoan.TransactionReference;
            string receiptTitle;
            if (request.OperationType == "LoanRepaymentMomocashCollection")
            {
                receiptTitle = "Momocash Collection Loan Repayment Reference: " + tellerLoan.TransactionReference;
                note = $"Note: {requestNote} - Statement: Momocash Collection loan repayment of {BaseUtilities.FormatCurrency(originalAmount)} with charge: 0 was completed on account number {accountNumber} [{accountType}], Reference: {reference}, Charges inclusive? flase";

            }
            else
            {
                receiptTitle = "Loan Repayment Receipt Reference: " + tellerLoan.TransactionReference;
                note = $"Note: {requestNote} - Statement: A repayment of {BaseUtilities.FormatCurrency(originalAmount)} was completed on account number {accountNumber} [{accountType}], Reference: {reference}, Charges inclusive? false"; // Modify this with the actual statement logic if needed

            }
            // Set the transaction note or statement based on the account type
            string transactionNoteOrStatement = note;
            // Create the transaction entity
            var transactionEntityEntryFee = new Transaction
            {
                Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique transaction ID
                CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow), // Set transaction creation date
                Status = TransactionStatus.COMPLETED.ToString(), // Set transaction status to completed
                TransactionReference = tellerLoan.TransactionReference, // Set transaction reference
                ExternalReference = request.ExternalReference, // Set external reference
                IsExternalOperation = request.IsExternalOperation, // Set external operation flag
                ExternalApplicationName = request.ExternalApplicationName, // Set external application name
                SourceType = request.SourceType, // Set source type
                Currency = request.Currency, // Set currency
                TransactionType = TransactionType.CASHIN_LOAN_REPAYMENT.ToString(), // Set transaction type (deposit)
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
                Fee = 0, // Set fee (charges)
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
                Balance = 0, // Set balance after deposit (including original amount)
                Credit = credit, // Set credit amount (including fees if IsOfflineCharge is true, otherwise excluding fees)
                Debit = 0, // Set debit amount (assuming no debit)
                DestinationBrachId = tellerLoan.DestinationBrachId,
                OperationCharge = 0, 
                WithrawalFormCharge = 0,  // Set destination branch ID
                SourceBrachId = tellerLoan.SourceBranchId, // Set source branch ID
                IsInterBrachOperation = tellerLoan.IsInterBranch, // Set flag indicating if inter-branch operation
                DestinationBranchCommission = 0,
                SourceBranchCommission = 0,
                ReceiptTitle = receiptTitle,
                IsSWS = request.IsSWS,
                AccountingDate = tellerLoan.AccountingDate,
                CheckNumber = request.CheckNumber,
                CheckName = request.CheckName,
            };
            _TransactionRepository.Add(transactionEntityEntryFee);
            return transactionEntityEntryFee; // Return the transaction entity
        }
        private TellerOperation CreateTellerOperation(Teller teller, Account tellerAccount, TellerLoanPaymentObject payment)
        {
            // Generate common teller operation fields
            var tellerOperation = new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(), // Unique ID for the operation
                AccountID = tellerAccount.Id, // Account ID associated with the operation
                AccountNumber = tellerAccount.AccountNumber, // Account number associated with the operation
                Amount = payment.Amount, // Amount for the operation
                CurrentBalance = payment.CurrentBalance, // Current balance of the account
                Date = payment.AccountingDate, // Operation date converted to local time
                PreviousBalance = tellerAccount.PreviousBalance, // Previous balance of the account
                TellerID = teller.Id, // Teller ID performing the operation
                TransactionReference = payment.TransactionReference, // Transaction reference
                UserID = _userInfoToken.Id, // User ID performing the operation
                EventName = payment.EvenetType, // Event type associated with the operation
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode), // Generated reference number
                CustomerId = payment.CustomerId, // Customer ID associated with the transaction
                MemberAccountNumber = payment.LoanRepaymentType == LoanRepaymentType.LocalAccount.ToString() ? payment.LocalAccountNumber : "N/A", // Member's account number associated with the transaction
                DestinationBrachId = payment.DestinationBrachId, // Destination branch ID
                SourceBranchId = payment.SourceBranchId, // Source branch ID
                IsInterBranch = payment.IsInterBranch, // Flag indicating if the operation is inter-branch
                MemberName = payment.CustomerName, // Member's name
                DestinationBranchCommission = payment.DestinationBranchCommission, // Destination branch commission
                SourceBranchCommission = payment.SourceBranchCommission, // Source branch commission
                BranchId = payment.BranchId, // Source branch commission
                BankId = payment.BankId,
                AccountingDate = payment.AccountingDate,
                OpenOfDayReference = tellerAccount.OpenningOfDayReference,
                IsCashOperation = (payment.LoanRepaymentType == LoanRepaymentType.LoanRepaymentMomocashCollection.ToString() || payment.LoanRepaymentType == LoanRepaymentType.LocalAccount.ToString()) ? false : true,
            };
            tellerOperation.Description = $"{TransactionType.CASH_IN} of {BaseUtilities.FormatCurrency(payment.Amount)} for the repayment of loan.";
            tellerOperation.OperationType = OperationType.Credit.ToString();
            tellerOperation.TransactionType = TransactionType.CASHIN_LOAN_REPAYMENT.ToString();
            return tellerOperation; // Return the created TellerOperation object
        }

    }
}
