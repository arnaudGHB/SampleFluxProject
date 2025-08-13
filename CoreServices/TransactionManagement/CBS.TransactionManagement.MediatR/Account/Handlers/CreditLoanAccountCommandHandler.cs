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
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Handler;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class CreditLoanAccountCommandHandler : IRequestHandler<CreditLoanAccountCommand, ServiceResponse<CreditLoanAccountCommandDto>>
    {
        // Dependencies
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountRepository _AccountRepository;
        private readonly IMediator _mediator;
        private readonly ITellerRepository _tellerRepository;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly ITransactionRepository _TransactionRepository;
        private readonly ILogger<CreditLoanAccountCommandHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;

        // Constructor to initialize dependencies
        public CreditLoanAccountCommandHandler(
            IAccountRepository AccountRepository,
            UserInfoToken userInfoToken,
            ILogger<CreditLoanAccountCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            ITransactionRepository transactionRepository = null,
            ITellerRepository tellerRepository = null,
            IAccountingDayRepository accountingDayRepository = null)
        {
            _AccountRepository = AccountRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _mediator = mediator;
            _TransactionRepository = transactionRepository;
            _tellerRepository = tellerRepository;
            _accountingDayRepository = accountingDayRepository;
        }

        // Method to handle the command
        public async Task<ServiceResponse<CreditLoanAccountCommandDto>> Handle(CreditLoanAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(request.BranchId);

                var teller = await _tellerRepository.GetPrimaryTeller(request.BranchId);
                var account = new Account();
                // Get the loan account
                var isLoan = await _AccountRepository.CheckIfMemberHaveLoanAccount(request.CustomerId);
                if (isLoan)
                {
                    account = await _AccountRepository.GetMemberLoanAccount(request.CustomerId);
                }
                else
                {
                    var addLoanAccount = new AddLoanAccountCommand { BankId = request.BankId, BranchId = request.BranchId, CustomerId = request.CustomerId };
                    var serviceResponse = await _mediator.Send(addLoanAccount);
                    if (serviceResponse.StatusCode==200)
                    {
                        account = await _AccountRepository.GetMemberLoanAccount(request.CustomerId);

                    }
                    else
                    {
                        return ServiceResponse<CreditLoanAccountCommandDto>.Return403(serviceResponse.Message);

                    }
                }
                var transactions = await CreateSenderTransactionEntity(request, account, teller.Id);
                _AccountRepository.CreditAccount(account, request.Amount);
                _TransactionRepository.Add(transactions);
                await _uow.SaveAsync();

                var apiRequest = MakeAccountingPosting(request, _userInfoToken.BranchID,  account, accountingDate);
                var postingResult = await _mediator.Send(apiRequest);
                string successMessage = "Loan was successfully approved.";
                var creditLoan = new CreditLoanAccountCommandDto { AccountingDate = accountingDate, AccountName = account.AccountName, AccountNumber = account.AccountNumber };

                if (postingResult.StatusCode != 200)
                {
                    successMessage= $"{successMessage} However, there was an error with accounting entries: {postingResult.Message}";
                    _logger.LogInformation(successMessage);
                    return ServiceResponse<CreditLoanAccountCommandDto>.ReturnResultWith200(creditLoan, successMessage);

                }
                else
                {
                    string msg = $"Member's loan account credited successfully by {request.Amount}.";
                    await LogAndAudit(msg, 200, request);
                    return ServiceResponse<CreditLoanAccountCommandDto>.ReturnResultWith200(creditLoan, msg);
                }
           
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred: {e.Message}";
                await LogAndAudit(errorMessage, 500, request);
                return ServiceResponse<CreditLoanAccountCommandDto>.Return500(e);
            }
        }
        private AdLoanApprovalCommand MakeAccountingPosting(CreditLoanAccountCommand creditLoanAccount, string branchId, Account account, DateTime accountingDate)
        {
            // Create a new AddAccountingPostingCommand instance
            var addAccountingPostingCommand = new AdLoanApprovalCommand
            {
                AccountNumber = account.AccountNumber,
                Naration = $"Loan Approval For {creditLoanAccount.LoanApplicationType} [Member-Reference: {account.CustomerId}, Name: {account.CustomerName}, Amount: {BaseUtilities.FormatCurrency(creditLoanAccount.Amount)}, REF: {creditLoanAccount.ReferenceNumber}]",
                MemberReference=account.CustomerId,
                TransactionReferenceId = creditLoanAccount.ReferenceNumber,
                Amount = creditLoanAccount.Amount,
                LoanProductId = creditLoanAccount.LoanProductId,
                LoanProductName = creditLoanAccount.LoanProductName,
                BranchId = branchId,
                TransactionDate = accountingDate
            };

            return addAccountingPostingCommand;
        }
        private async Task<Transaction> CreateSenderTransactionEntity(CreditLoanAccountCommand request, Account senderAccount, string tellerid)
        {

            decimal Charges = 0;
            decimal Balance = senderAccount.Balance + request.Amount;

            string N_A = "N/A";
            var transaction = new Transaction
            {
                Id = BaseUtilities.GenerateUniqueNumber(), // Generate unique transaction ID
                CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow), // Set transaction creation date
                Status = TransactionStatus.COMPLETED.ToString(), // Set transaction status to completed
                TransactionReference = request.ReferenceNumber, // Set transaction reference
                ExternalReference = request.ReferenceNumber, // Set external reference
                IsExternalOperation = false, // Set external operation flag
                ExternalApplicationName = "LoanMS", // Set external application name
                SourceType = OperationSourceType.Web_Portal.ToString(), // Set source type
                Currency = "XAF", // Set currency
                TransactionType = TransactionType.TRANSFER.ToString(),
                AccountNumber = senderAccount.AccountNumber, // Set account number
                PreviousBalance = senderAccount.Balance, // Set previous balance
                AccountId = senderAccount.Id, // Set account ID
                CustomerId = senderAccount.CustomerId, // Set customer ID
                ProductId = senderAccount.ProductId, // Set product ID
                SenderAccountId = senderAccount.Id, // Set sender account ID
                ReceiverAccountId = senderAccount.Id, // Set receiver account ID
                BankId = senderAccount.BankId, // Set bank ID
                Operation = TransactionType.Loan_Disbursement.ToString(), // Set operation type (deposit)
                BranchId = senderAccount.BranchId, // Set branch ID
                OriginalDepositAmount = request.Amount, // Set original deposit amount including fees
                Fee = Charges,
                Tax = 0, // Set tax (assuming tax is not applicable)
                Amount = request.Amount - Charges, // Set amount (excluding fees)
                Note = $"The sum of {BaseUtilities.FormatCurrency(request.Amount)} was successfully  credited to loan transit account. Charge: {BaseUtilities.FormatCurrency(Charges)}",
                OperationType = OperationType.Debit.ToString(), // Set operation type (credit)
                FeeType = Events.None.ToString(), // Set fee type
                TellerId = tellerid, // Set teller ID
                DepositerNote = N_A, // Set depositer note
                DepositerTelephone = N_A, // Set depositer telephone
                DepositorIDNumber = N_A, // Set depositor ID number
                DepositorIDExpiryDate = N_A, // Set depositor ID expiry date
                DepositorIDIssueDate = N_A, // Set depositor ID issue date
                DepositorIDNumberPlaceOfIssue = N_A, // Set depositor ID place of issue
                IsDepositDoneByAccountOwner = true, // Set flag indicating if deposit is done by account owner
                DepositorName = N_A, // Set depositor name
                Balance = Balance, // Set balance after deposit (including original amount)
                Credit = request.Amount, // Set credit amount (including fees if IsOfflineCharge is true, otherwise excluding fees)
                Debit = 0, // Set debit amount (assuming no debit)
                DestinationBrachId = senderAccount.BranchId,
                OperationCharge = 0,
                WithrawalFormCharge = 0,  // Set destination branch ID
                SourceBrachId = senderAccount.BranchId, // Set source branch ID
                IsInterBrachOperation = false, // Set flag indicating if inter-branch operation
                DestinationBranchCommission = 0,
                SourceBranchCommission = 0,
                ReceiptTitle = $"Loan Disbursement Receipt Reference: " + request.ReferenceNumber,

            };
            _TransactionRepository.Add(transaction);

            return transaction; // Return the transaction entity
        }


        // Log and audit the action
        private async Task LogAndAudit(string message, int statusCode, CreditLoanAccountCommand command)
        {
            _logger.LogError(message);
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), command, message, LogLevelInfo.Error.ToString(), statusCode, _userInfoToken.Token);
        }
    }

}
