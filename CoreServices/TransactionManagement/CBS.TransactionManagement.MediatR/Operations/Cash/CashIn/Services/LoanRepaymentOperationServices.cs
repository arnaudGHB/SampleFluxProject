using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Repository.AccountServices;
using CBS.TransactionManagement.MediatR.LoanRepayment.Command;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Repository.Receipts.Payments;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;
using CBS.TransactionManagement.LoanRepayment.Command;
using DocumentFormat.OpenXml.Bibliography;
using CBS.TransactionManagement.Repository.OldLoanConfiguration;
using CBS.TransactionManagement.Data.Entity.OldLoanConfiguration;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.MediatR.Commands;
using CBS.TransactionManagement.Data.Entity;
using DocumentFormat.OpenXml.Drawing.Charts;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.NewFolder
{

    public class LoanRepaymentOperationServices : ILoanRepaymentOperationServices
    {
        private readonly IGeneralDailyDashboardRepository _generalDailyDashboardRepository; // Repository for accessing account data.
        private readonly IAccountRepository _accountRepository; // Repository for accessing account data.
        private readonly UserInfoToken _userInfoToken; // User information token.
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository; // Repository for accessing teller data.
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository; // Repository for accessing teller provisioning account data.
        private readonly ITellerRepository _tellerRepository; // Repository for accessing teller data.
        private readonly IPaymentReceiptRepository _paymentReceiptRepository;
        private readonly IMapper _mapper;
        private readonly ILoanRepaymentServices _loanRepaymentServices;
        private readonly IOldLoanAccountingMapingRepository _OldLoanAccountingMapingRepository;
        private readonly IBlockedAccountRepository _blockedAccountRepository;
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository;

        public IMediator _mediator { get; set; } // Mediator for handling requests.
        private readonly ILogger<LoanRepaymentOperationServices> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for transaction context.
        public LoanRepaymentOperationServices(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            ILogger<LoanRepaymentOperationServices> logger,
            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IPaymentReceiptRepository paymentReceiptRepository = null,
            IMapper mapper = null,
            ILoanRepaymentServices loanRepaymentServices = null,
            IOldLoanAccountingMapingRepository oldLoanAccountingMapingRepository = null,
            IGeneralDailyDashboardRepository generalDailyDashboardRepository = null,
            IBlockedAccountRepository blockedAccountRepository = null,
            IAPIUtilityServicesRepository utilityServicesRepository = null)
        {
            _userInfoToken = UserInfoToken; // Initialize user information token.
            _accountRepository = AccountRepository; // Initialize account repository.
            _logger = logger; // Initialize logger.
            _uow = uow; // Initialize unit of work.
            _tellerProvisioningAccountRepository = tellerProvisioningAccountRepository; // Initialize teller provisioning account repository.
            _tellerRepository = tellerRepository; // Initialize teller repository.
            _mediator = mediator; // Initialize _mediator.
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _mapper = mapper;
            _loanRepaymentServices = loanRepaymentServices;
            _OldLoanAccountingMapingRepository = oldLoanAccountingMapingRepository;
            _generalDailyDashboardRepository = generalDailyDashboardRepository;
            _blockedAccountRepository=blockedAccountRepository;
            _utilityServicesRepository=utilityServicesRepository;
        }





        /// <summary>
        /// Handles the loan repayment process, processes bulk deposit transactions, updates dashboards, and posts accounting entries.
        /// </summary>
        /// <param name="requests">The bulk operation request for loan repayment.</param>
        /// <param name="accountingDate">The accounting date for the loan repayment transaction.</param>
        /// <param name="config">System configuration details.</param>
        /// <returns>A service response containing the payment receipt details or an error message.</returns>
        public async Task<ServiceResponse<PaymentReceiptDto>> LoanRepayment(AddBulkOperationDepositCommand requests, DateTime accountingDate, Config config)
        {
            try
            {
                // Step 1: Retrieve loan information from the bulk operation request.
                var loanInfo = requests.BulkOperations.FirstOrDefault();
                bool isInterBranchOperation = false;
                loanInfo.LoanId = loanInfo.AccountNumber;  // Assign the loan ID to the account number.

                // Step 2: Validate if the user account serves as a teller for the current date.
                var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();

                // Step 3: Verify that the accounting day is still open.
                await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate);

                // Step 4: Retrieve teller information and check operational rights.
                var teller = await _tellerRepository.RetrieveTeller(dailyTeller);
                await _tellerRepository.CheckTellerOperationalRights(teller, requests.OperationType, requests.IsCashOperation);

                // Step 5: Retrieve teller and customer accounts.
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);
                var customer = await _utilityServicesRepository.GetCustomer(loanInfo.CustomerId);

                // Step 6: Initialize member activation policy for potential membership-related checks.
                var memberActivationPolicy = new MemberActivationPolicyDto();

                // Step 7: Retrieve branch information for the customer.
                var branch = await _utilityServicesRepository.GetBranch(customer.BranchId);

                // Step 8: Generate a unique transaction reference.
                string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.CASHIN_LOAN_REPAYMENT.ToString(), isInterBranchOperation);

                // Step 9: Handle inter-branch operations by regenerating the reference if applicable.
                if (teller.BranchId != customer.BranchId)
                {
                    isInterBranchOperation = true;
                    reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.CASHIN_LOAN_REPAYMENT.ToString(), isInterBranchOperation);
                }

                // Step 10: Calculate the total repayment amount from bulk operations.
                decimal amount = CalculateTotalAmount(requests.BulkOperations);

                // Step 11: Retrieve and record currency notes.
                var currencyNote = await RetrieveCurrencyNotes(reference, loanInfo.currencyNotes);
                var subTellerProvisioning = _subTellerProvioningHistoryRepository.CashInByDinomination(
                    amount,
                    loanInfo.currencyNotes,
                    teller.Id,
                    accountingDate,
                    tellerAccount.OpenningOfDayReference
                );

                // Step 12: Retrieve the loan account for the customer.
                var loanAccount = await _accountRepository.GetMemberLoanAccount(loanInfo.CustomerId);

                // Step 13: Map bulk operations to deposit transactions.
                var bulkDeposits = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(
                    requests.BulkOperations,
                    true,
                    loanAccount.AccountNumber
                );

                // Step 14: Process the loan repayment and retrieve refund details.
                var refund = await MakeLoanRepayment(amount, bulkDeposits.FirstOrDefault(), reference);

                // Step 15: Prepare loan payment details for the transaction.
                var loanPaymentObject = new TellerLoanPaymentObject
                {
                    AccountingDate = accountingDate,
                    Amount = amount,
                    BankId = teller.BankId,
                    BranchId = teller.BranchId,
                    CurrentBalance = tellerAccount.Balance,
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.Name,
                    DestinationBrachId = customer.BranchId,
                    EvenetType = TransactionType.CASHIN_LOAN_REPAYMENT.ToString(),
                    Interest = refund.Interest,
                    IsInterBranch = isInterBranchOperation,
                    OperationType = TransactionType.CASHIN_LOAN_REPAYMENT.ToString(),
                    Principal = refund.Principal,
                    LoanRepaymentType = requests.DepositType,
                    SourceBranchId = teller.BranchId,
                    TransactionReference = reference,
                    Vat = refund.Tax
                };

                // Step 16: Process bulk transactions for the loan repayment.
                var transactions = await ProcessBulkTransactions(bulkDeposits, teller, tellerAccount, customer, currencyNote.Data.ToList(), branch, loanPaymentObject);

                // Step 17: Prepare payment receipt details.
                var paymentReceipts = new List<PaymentDetailObject>
        {
            new PaymentDetailObject
            {
                AccountNumber = refund.LoanId,
                Fee = 0,
                Amount = refund.Amount,
                Interest = refund.Interest,
                LoanCapital = refund.Principal,
                SericeOrEventName = TransactionType.CASHIN_LOAN_REPAYMENT.ToString(),
                VAT = refund.Tax,
                Balance = refund.Balance
            }
        };

                // Step 18: Create a payment processing request.
                var paymentProcessing = new PaymentProcessingRequest
                {
                    AccountingDay = accountingDate,
                    Amount = amount,
                    MemberName = customer.Name,
                    NotesRequest = loanInfo.currencyNotes,
                    OperationType = OperationType.Cash.ToString(),
                    OperationTypeGrouping = TransactionType.CASH_IN.ToString(),
                    PortalUsed = OperationSourceType.Web_Portal.ToString(),
                    PaymentDetails = paymentReceipts,
                    ServiceType = TransactionType.CASHIN_LOAN_REPAYMENT.ToString(),
                    SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
                    TotalAmount = amount,
                    TotalCharges = transactions.Sum(x => x.Fee),
                    Transactions = transactions
                };

                // Step 19: Process the payment and retrieve the payment receipt.
                var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);

                // Step 20: Update the dashboard with the cash operation details.
                var tellerBranch = await _utilityServicesRepository.GetBranch(teller.BranchId);
                var cashOperation = new CashOperation(
                    teller.BranchId,
                    paymentProcessing.Amount,
                    paymentProcessing.TotalCharges,
                    tellerBranch.name,
                    tellerBranch.branchCode,
                    CashOperationType.LoanRepayment,
                    LogAction.LoanRepayment,
                    subTellerProvisioning
                );

                // Step 21: Save changes to the database.
                await _uow.SaveAsync();

                // Step 22: If the request is not partial, send an SMS notification.
                if (!requests.IsPartialRequest)
                {
                    await SendSMS(paymentReceipts.FirstOrDefault(), reference, customer, branch);
                }

                // Step 23: Handle old loan repayment logic if applicable.
                OldLoanAccountingMaping oldLoanParam = new OldLoanAccountingMaping();
                if (refund.Loan.IsUpload)
                {
                    oldLoanParam = await _OldLoanAccountingMapingRepository
                        .FindBy(x => x.LoanTypeName == refund.Loan.LoanCategory && !x.IsDeleted)
                        .FirstOrDefaultAsync() ?? new Data.Entity.OldLoanConfiguration.OldLoanAccountingMaping();
                }

                // Step 24: Post accounting entries and get response messages.
                var accountingResponseMessages = await PostAccounting(refund, customer, reference, accountingDate, oldLoanParam);

                // Step 25: Prepare success message or fallback to default success message.
                accountingResponseMessages ??= $"Account Posting: Successful. A cash deposit of {BaseUtilities.FormatCurrency(amount)} was successfully processed for loan repayment by {teller.Name} [User: {dailyTeller.UserName}] on behalf of {customer.Name}.";

                // Step 26: Log and audit the success operation.
                await BaseUtilities.LogAndAuditAsync(accountingResponseMessages, requests, HttpStatusCodeEnum.OK, LogAction.LoanRepayment, LogLevelInfo.Information);
                _logger.LogInformation(accountingResponseMessages);

                // Return success response with the payment receipt DTO.
                var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);
                return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, accountingResponseMessages);
            }
            catch (Exception e)
            {
                // Step 27: Handle and log any errors that occur.
                string errorMessage = $"Error: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.InternalServerError, LogAction.LoanRepayment, LogLevelInfo.Error);

                // Return error response.
                return ServiceResponse<PaymentReceiptDto>.Return500(errorMessage);
            }
        }







        /// <summary>
        /// Processes loan repayment transactions, including validations, accounting entries, and notifications.
        /// </summary>
        /// <param name="requests">The bulk operation deposit command containing repayment details.</param>
        /// <param name="accountingDate">The accounting date for the transaction.</param>
        /// <param name="config">The configuration object for the operation.</param>
        /// <returns>A ServiceResponse containing the payment receipt or an error.</returns>



        //public async Task<ServiceResponse<PaymentReceiptDto>> LoanRepayment(AddBulkOperationDepositCommand requests, DateTime accountingDate, Config config)
        //{
        //    try
        //    {
        //        // Step 1: Retrieve loan information.
        //        var loanInfo = requests.BulkOperations.FirstOrDefault();
        //        bool isInterBranchOperation = false;
        //        loanInfo.LoanId = loanInfo.AccountNumber;

        //        // Step 2: Validate if the user account serves as a teller for the current date.
        //        var dailyTeller = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate();

        //        // Step 3: Verify the accounting day is still open.
        //        await _subTellerProvioningHistoryRepository.CheckIfDayIsStillOpened(dailyTeller.TellerId, accountingDate);

        //        // Step 4: Retrieve teller information and check operational rights.
        //        var teller = await _tellerRepository.RetrieveTeller(dailyTeller);
        //        await _tellerRepository.CheckTellerOperationalRights(teller, requests.OperationType, requests.IsCashOperation);

        //        // Step 5: Retrieve teller and customer accounts.
        //        var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);
        //        var customer = await _utilityServicesRepository.GetCustomer(requests.BulkOperations.FirstOrDefault().CustomerId);

        //        // Step 6: Initialize member activation policy.
        //        var memberActivationPolicy = new MemberActivationPolicyDto();

        //        // Step 7: Retrieve branch information.
        //        var branch = await _utilityServicesRepository.GetBranch(customer.BranchId);

        //        // Step 8: Generate transaction reference.
        //        string reference = CurrencyNotesMapper.GenerateTransactionReference(
        //            _userInfoToken,
        //            TransactionType.CASHIN_LOAN_REPAYMENT.ToString(),
        //            isInterBranchOperation);

        //        // Handle inter-branch operations.
        //        if (teller.BranchId != customer.BranchId)
        //        {
        //            isInterBranchOperation = true;
        //            reference = CurrencyNotesMapper.GenerateTransactionReference(
        //                _userInfoToken,
        //                TransactionType.CASHIN_LOAN_REPAYMENT.ToString(),
        //                isInterBranchOperation);
        //        }

        //        // Step 9: Calculate the total repayment amount.
        //        decimal amount = CalculateTotalAmount(requests.BulkOperations);

        //        // Step 10: Retrieve and record currency notes.
        //        var currencyNote = await RetrieveCurrencyNotes(reference, loanInfo.currencyNotes);
        //        var subTellerProvioning = _subTellerProvioningHistoryRepository.CashInByDinomination(
        //            amount,
        //            loanInfo.currencyNotes,
        //            teller.Id,
        //            accountingDate,
        //            tellerAccount.OpenningOfDayReference);

        //        // Step 11: Retrieve loan account information.
        //        var loanAccount = await _accountRepository.GetMemberLoanAccount(loanInfo.CustomerId);

        //        // Step 12: Map bulk operations to bulk deposits.
        //        var bulkDeposits = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(
        //            requests.BulkOperations,
        //            true,
        //            loanAccount.AccountNumber);

        //        // Step 13: Process the loan repayment.
        //        var refund = await MakeLoanRepayment(amount, bulkDeposits.FirstOrDefault(), reference);

        //        // Step 14: Prepare loan payment details.
        //        var loanPaymentObject = new TellerLoanPaymentObject
        //        {
        //            AccountingDate = accountingDate,
        //            Amount = amount,
        //            BankId = teller.BankId,
        //            BranchId = teller.BranchId,
        //            CurrentBalance = tellerAccount.Balance,
        //            CustomerId = customer.CustomerId,
        //            CustomerName = customer.Name,
        //            DestinationBrachId = customer.BranchId,
        //            EvenetType = TransactionType.CASHIN_LOAN_REPAYMENT.ToString(),
        //            Interest = refund.Interest,
        //            IsInterBranch = isInterBranchOperation,
        //            OperationType = TransactionType.CASHIN_LOAN_REPAYMENT.ToString(),
        //            Principal = refund.Principal,
        //            LoanRepaymentType = requests.DepositType,
        //            SourceBranchId = teller.BranchId,
        //            TransactionReference = reference,
        //            Vat = refund.Tax
        //        };

        //        // Step 15: Process bulk transactions.
        //        var transactions = await ProcessBulkTransactions(bulkDeposits, teller, tellerAccount, customer, currencyNote.Data.ToList(), branch, loanPaymentObject);

        //        // Step 16: Prepare payment receipt details.
        //        var paymentReceipts = new List<PaymentDetailObject>
        //{
        //    new PaymentDetailObject
        //    {
        //        AccountNumber = refund.LoanId,
        //        Fee = 0,
        //        Amount = refund.Amount,
        //        Interest = refund.Interest,
        //        LoanCapital = refund.Principal,
        //        SericeOrEventName = TransactionType.CASHIN_LOAN_REPAYMENT.ToString(),
        //        VAT = refund.Tax,
        //        Balance = refund.Balance
        //    }

        //};

        //        // Step 17: Create payment processing request.
        //        var paymentProcessing = new PaymentProcessingRequest
        //        {
        //            AccountingDay = accountingDate,
        //            Amount = amount,
        //            MemberName = customer.Name,
        //            NotesRequest = requests.BulkOperations.FirstOrDefault().currencyNotes,
        //            OperationType = OperationType.Cash.ToString(),
        //            OperationTypeGrouping = TransactionType.CASH_IN.ToString(),
        //            PortalUsed = OperationSourceType.Web_Portal.ToString(),
        //            PaymentDetails = paymentReceipts,
        //            ServiceType = TransactionType.CASHIN_LOAN_REPAYMENT.ToString(),
        //            SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
        //            TotalAmount = amount,
        //            TotalCharges = transactions.Sum(x => x.Fee),
        //            Transactions = transactions
        //        };

        //        // Step 18: Process payment and retrieve receipt.
        //        var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);

        //        // Step 19: Update dashboard with the cash operation.
        //        var TellerBranch = await _utilityServicesRepository.GetBranch(teller.BranchId);
        //        var cashOperation = new CashOperation(
        //            teller.BranchId,
        //            paymentProcessing.Amount,
        //            paymentProcessing.TotalCharges,
        //            TellerBranch.name,
        //            TellerBranch.branchCode,
        //            CashOperationType.LoanRepayment,
        //            LogAction.LoanRepayment,
        //            subTellerProvioning);

        //        //await _generalDailyDashboardRepository.UpdateOrCreateDashboardAsync(cashOperation);

        //        // Step 20: Save changes to the database.
        //        await _uow.SaveAsync();

        //        if (!requests.IsPartialRequest)
        //        {
        //            // Step 21: Send SMS notification for the repayment.
        //            await SendSMS(paymentReceipts.FirstOrDefault(), reference, customer, branch);
        //        }

        //        // Step 22: Checking if loan repayment is for old loan
        //        OldLoanAccountingMaping oldLoanParam = new OldLoanAccountingMaping();
        //        if (refund.Loan.IsUpload)
        //        {
        //            // Step 23:Getting old loan from system.
        //            oldLoanParam = await _OldLoanAccountingMapingRepository
        //            .FindBy(x => x.LoanTypeName == refund.Loan.LoanCategory && !x.IsDeleted)
        //            .FirstOrDefaultAsync() ?? new Data.Entity.OldLoanConfiguration.OldLoanAccountingMaping();
        //        }
        //        // Step 23: Post accounting entries for the transaction.
        //        var accountingResponseMessages = await PostAccounting(refund, customer, reference, accountingDate, oldLoanParam);

        //        // Step 24: Prepare the success response message.
        //        var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);
        //        accountingResponseMessages ??= $"Account Posting: Successful. A cash deposit of {BaseUtilities.FormatCurrency(amount)} has been successfully processed for loan repayment by {teller.Name} [User: {dailyTeller.UserName}] on behalf of {customer.Name}.";

        //        // Log and audit the success operation.
        //        await BaseUtilities.LogAndAuditAsync(accountingResponseMessages, requests, HttpStatusCodeEnum.OK, LogAction.LoanRepayment, LogLevelInfo.Information);
        //        _logger.LogInformation(accountingResponseMessages);

        //        // Return success response with the payment receipt DTO.
        //        return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, accountingResponseMessages);
        //    }
        //    catch (Exception e)
        //    {
        //        // Log and audit the error.
        //        var errorMessage = $"Error: {e.Message}";
        //        _logger.LogError(errorMessage);
        //        await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.InternalServerError, LogAction.LoanRepayment, LogLevelInfo.Error);

        //        // Return error response.
        //        return ServiceResponse<PaymentReceiptDto>.Return500(e);
        //    }
        //}



        /// <summary>
        /// Processes the loan repayment and handles associated operations such as releasing blocked accounts.
        /// </summary>
        /// <param name="amount">The total repayment amount.</param>
        /// <param name="deposit">Details of the deposit for the repayment.</param>
        /// <param name="TransactionCode">The transaction reference code.</param>
        /// <returns>A <see cref="RefundDto"/> containing the repayment details.</returns>
        private async Task<RefundDto> MakeLoanRepayment(decimal amount, BulkDeposit deposit, string TransactionCode)
        {
            try
            {
                // Step 1: Initialize the deposit details.
                deposit.Principal = deposit.Amount;

                // Step 2: Prepare the repayment command.
                var addRepaymentCommand = new AddLoanRepaymentCommandDetails
                {
                    Amount = amount,
                    Comment = deposit.Note ?? "Loan Repayment", // Default comment if none provided.
                    LoanId = deposit.LoanId,
                    Interest = deposit.Interest,
                    PaymentChannel = deposit.PaymentMethod ?? "Web_Portal", // Default payment channel.
                    Penalty = deposit.Penalty,
                    Principal = deposit.Principal,
                    Tax = deposit.Tax,
                    PaymentMethod = deposit.PaymentMethod ?? "Cash", // Default payment method.
                    TransactionCode = TransactionCode
                };
                
                // Step 3: Send the repayment command via mediator.
                var addRepaymentCommandResponse = await _mediator.Send(addRepaymentCommand);

                // Step 4: Validate the response from the repayment API.
                if (addRepaymentCommandResponse == null || !addRepaymentCommandResponse.StatusCode.Equals(200))
                {
                    var errorMessage = $"Loan Repayment API Failed: {addRepaymentCommandResponse?.Message ?? "No response received"}";
                    _logger.LogError(errorMessage); // Log error message.
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        addRepaymentCommand,
                        HttpStatusCodeEnum.InternalServerError,
                        LogAction.LoanRepayment,
                        LogLevelInfo.Error);

                    throw new InvalidOperationException(errorMessage); // Throw exception to halt the process.
                }

                // Step 5: Retrieve the refund details from the response.
                var refund = addRepaymentCommandResponse.Data;

                // Step 6: Handle blocked accounts if the repayment is complete.
                if (refund.Balance==0)
                {
                    // Step 6.1: Retrieve all blocked accounts associated with the loan application.
                    var blockedAccounts = await _blockedAccountRepository
                        .FindBy(x => x.LoanApplicationId == refund.Loan.LoanApplicationId)
                        .ToListAsync();

                    if (blockedAccounts.Any())
                    {
                        // Step 6.2: Prepare the list of accounts to release.
                        var accountToBlockedList = blockedAccounts
                            .Select(blockedAccount => new AccountToBlocked
                            {
                                AccountNumber = blockedAccount.AccountNumber,
                                Amount = blockedAccount.Amount,
                                Reason = $"Releasing blocked amount of {blockedAccount.Amount}"
                            })
                            .ToList();

                        // Step 6.3: Create and send the command to release blocked amounts.
                        var blockListOfAccountBalanceCommand = new BlockListOfAccountBalanceCommand
                        {
                            AccountToBlockeds = accountToBlockedList,
                            IsRelease = true,
                            LoanApplicationId = refund.Loan.LoanApplicationId
                        };

                        var releaseResult = await _mediator.Send(blockListOfAccountBalanceCommand);

                        // Step 6.4: Log success or failure of the release operation.
                        if (releaseResult == null || !releaseResult.StatusCode.Equals(200))
                        {
                            var warningMessage = $"Failed to release blocked accounts for Loan Application ID: {refund.Loan.LoanApplicationId}";
                            _logger.LogWarning(warningMessage);
                            await BaseUtilities.LogAndAuditAsync(
                                warningMessage,
                                blockListOfAccountBalanceCommand,
                                HttpStatusCodeEnum.InternalServerError,
                                LogAction.LoanRepayment,
                                LogLevelInfo.Warning);
                        }
                        else
                        {
                            var successMessage = $"Successfully released blocked accounts for Loan Application ID: {refund.Loan.LoanApplicationId}";
                            _logger.LogInformation(successMessage);
                            await BaseUtilities.LogAndAuditAsync(
                                successMessage,
                                blockListOfAccountBalanceCommand,
                                HttpStatusCodeEnum.OK,
                                LogAction.LoanRepayment,
                                LogLevelInfo.Information);
                        }
                    }
                }

                // Step 7: Log the successful loan repayment and return the refund details.
                var successLogMessage = $"Loan repayment successful for Loan ID: {refund.Loan.LoanApplicationId}, Amount: {BaseUtilities.FormatCurrency(refund.Amount)}";
                _logger.LogInformation(successLogMessage);
                await BaseUtilities.LogAndAuditAsync(
                    successLogMessage,
                    addRepaymentCommand,
                    HttpStatusCodeEnum.OK,
                    LogAction.LoanRepayment,
                    LogLevelInfo.Information);

                return refund;
            }
            catch (Exception ex)
            {
                // Log and rethrow the exception for upstream handling.
                var errorMessage = $"Error occurred during loan repayment processing: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    deposit,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.LoanRepayment,
                    LogLevelInfo.Error);

                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
        }

        //BlockListOfAccountBalanceCommand
        // Method to calculate total amount from bulk operations
        private decimal CalculateTotalAmount(IEnumerable<BulkOperation> bulkOperations)
        {
            return bulkOperations.Sum(x => x.Total); // Calculate total amount.
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
        /// <summary>
        /// Processes bulk transactions for loan repayments by iterating over a list of deposit requests.
        /// </summary>
        /// <param name="requests">List of bulk deposit requests.</param>
        /// <param name="teller">Teller information.</param>
        /// <param name="tellerAccount">Teller's account details.</param>
        /// <param name="customer">Customer details.</param>
        /// <param name="currencyNotes">Details of the currency notes used in the transaction.</param>
        /// <param name="branch">Branch information.</param>
        /// <param name="paymentObject">Loan payment object containing transaction details.</param>
        /// <returns>List of processed transactions as <see cref="TransactionDto"/>.</returns>
        private async Task<List<TransactionDto>> ProcessBulkTransactions(
            List<BulkDeposit> requests,
            Teller teller,
            Account tellerAccount,
            CustomerDto customer,
            List<CurrencyNotesDto> currencyNotes,
            BranchDto branch,
            TellerLoanPaymentObject paymentObject)
        {
            var transactions = new List<TransactionDto>(); // Initialize list to store transactions.
            string transactionType = TransactionType.CASHIN_LOAN_REPAYMENT.ToString(); // Define transaction type.

            // Iterate through each deposit request in the list.
            foreach (var request in requests)
            {
                try
                {
                    // Step 1: Get customer account information for the transaction type.
                    var customerAccount = await _accountRepository.GetAccount(request.AccountNumber, transactionType);
                    await BaseUtilities.LogAndAuditAsync(
                        $"Retrieved account {request.AccountNumber} for loan repayment transaction.",
                        request,
                        HttpStatusCodeEnum.OK,
                        LogAction.LoanRepayment,
                        LogLevelInfo.Information, paymentObject.TransactionReference);

                    // Step 2: Set request parameters with additional details.
                    request.Customer = customer; // Attach customer details to the request.
                    request.currencyNotes = currencyNotes; // Attach currency notes to the request.
                    request.Branch = branch; // Attach branch information to the request.
                    request.IsExternalOperation = false; // Mark as internal operation.
                    request.ExternalApplicationName = "N/A"; // No external application involved.
                    request.ExternalReference = "N/A"; // No external reference.
                    request.SourceType = OperationSourceType.Web_Portal.ToString(); // Define the source type.

                    // Step 3: Perform the loan deposit operation.
                    var transaction = await _loanRepaymentServices.LoanDepositCash(
                        request,
                        teller,
                        tellerAccount,
                        customerAccount,
                        paymentObject);

                    // Log successful deposit operation.
                    await BaseUtilities.LogAndAuditAsync(
                        $"Successfully processed loan deposit for Account: {request.AccountNumber}, Amount: {BaseUtilities.FormatCurrency(request.Amount)}.",
                        request,
                        HttpStatusCodeEnum.OK,
                        LogAction.LoanRepayment,
                        LogLevelInfo.Information, paymentObject.TransactionReference);

                    // Step 4: Set additional flags and add transaction to the list.
                    transaction.IsChargesIclussive = request.IsChargesInclussive; // Indicate if charges are inclusive.
                    transactions.Add(transaction); // Add the transaction to the list.
                }
                catch (Exception ex)
                {
                    // Log error if the transaction fails.
                    var errorMessage = $"Failed to process loan deposit for Account: {request.AccountNumber}. Error: {ex.Message}";
                    _logger.LogError(ex, errorMessage);

                    // Log and audit the error.
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.InternalServerError,
                        LogAction.LoanRepayment,
                        LogLevelInfo.Error, paymentObject.TransactionReference);

                    // Optionally rethrow the exception or continue processing other transactions.
                    throw; // Or handle gracefully if necessary.
                }
            }

            // Return the list of processed transactions.
            return transactions;
        }
        // Method to process bulk transactions
        private async Task SendSMS(PaymentDetailObject paymentDetail, string reference, CustomerDto customer, BranchDto branch)
        {
            // Initialize message variable
            string msg;
            string branchName = branch?.name ?? "the branch";

            // Check the customer's language preference
            if (customer.Language.ToLower() == "english")
            {
                // Construct the SMS message in English for loan repayment
                msg = $"Loan repayment for {customer.FirstName} processed at {branchName}:\n" +
                      $"Refund Amount: {BaseUtilities.FormatCurrency(paymentDetail.Amount)}\n" +
                      $"Capital: {BaseUtilities.FormatCurrency(paymentDetail.LoanCapital)}\n" +
                      $"Interest: {BaseUtilities.FormatCurrency(paymentDetail.Interest)}\n" +
                      $"VAT: {BaseUtilities.FormatCurrency(paymentDetail.VAT)}\n" +
                      //$"Remaining Balance to Repay: {BaseUtilities.FormatCurrency(paymentDetail.Balance)}\n" +
                      $"Reference: {reference}. Date: {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}.";
            }
            else
            {
                // Construct the SMS message in French for loan repayment
                msg = $"Remboursement de prêt pour {customer.FirstName} traité au {branchName} :\n" +
                      $"Montant du remboursement : {BaseUtilities.FormatCurrency(paymentDetail.Amount)}\n" +
                      $"Capital : {BaseUtilities.FormatCurrency(paymentDetail.LoanCapital)}\n" +
                      $"Intérêt : {BaseUtilities.FormatCurrency(paymentDetail.Interest)}\n" +
                      $"TVA : {BaseUtilities.FormatCurrency(paymentDetail.VAT)}\n" +
                      //$"Solde restant à rembourser : {BaseUtilities.FormatCurrency(paymentDetail.Balance)}\n" +
                      $"Référence : {reference}. Date : {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}.";
            }

            // Create command to send SMS
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.Phone
            };
            await _mediator.Send(sMSPICallCommand);
            var sMSPICallCommandx = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = "237650535634"//customer.Phone
            };
            await _utilityServicesRepository.PushNotification(customer.CustomerId, PushNotificationTitle.LOAN_REPAYMENT, msg);
            // Send command to _mediator
            await _mediator.Send(sMSPICallCommandx);
        }
        /// <summary>
        /// Handles the accounting posting for a loan refund.
        /// </summary>
        /// <param name="refund">Details of the refund including amounts and loan details.</param>
        /// <param name="customer">Details of the customer associated with the loan.</param>
        /// <param name="ReferenceNumber">The transaction reference number.</param>
        /// <param name="accountingDate">The date of the accounting transaction.</param>
        /// <param name="oldLoanAccounting">Mapping details for old system loans (if applicable).</param>
        /// <returns>A message string indicating the success or failure of the accounting posting.</returns>
        private async Task<string> PostAccounting(
            RefundDto refund,
            CustomerDto customer,
            string ReferenceNumber,
            DateTime accountingDate,
            OldLoanAccountingMaping oldLoanAccounting)
        {
            string accountingResponseMessages = null; // Initialize response message variable.

            // Create a new command for loan refund posting.
            var addAccountingPostingCommand = new AddLoanRefundPostingCommand
            {
                AccountNumber = refund.LoanProduct?.AccountNumber, // Set the account number.
                LoanProductId = refund.LoanProduct?.Id, // Set the loan product ID.
                Naration = refund.Loan.IsUpload
                    ? $"Migrated Loan Repayment | Amount: {BaseUtilities.FormatCurrency(refund.Amount)} | Member Reference: {customer.CustomerId} | Member Name: {customer.FirstName} {customer.LastName} | REF: {ReferenceNumber} | Date: {accountingDate}"
                    : $"TSC Loan Repayment | Amount: {BaseUtilities.FormatCurrency(refund.Amount)} | Member Reference: {customer.CustomerId} | Member Name: {customer.FirstName} {customer.LastName} | REF: {ReferenceNumber} | Date: {accountingDate}", // Set narration based on loan type.
                TransactionReferenceId = ReferenceNumber, // Set the transaction reference ID.
                AmountCollection = new List<LoanRefundCollection>(), // Initialize amount collection.
                Amount = refund.Amount, // Total refund amount.
                TellerSource = TellerSources.Physical_Teller.ToString(), // Set teller source.
                IsOldSystemLoan = refund.Loan.IsUpload, // Indicate if it's an old system loan.
                BranchId = refund.BranchId, // Branch ID.
                TransactionDate = accountingDate, // Transaction date.
                MemberReference = customer.CustomerId, // Set member reference.
            };

            if (addAccountingPostingCommand.IsOldSystemLoan)
            {
                // Handle old system loan accounting.
                addAccountingPostingCommand.LoanRefundCollectionAlpha = new LoanRefundCollectionAlpha
                {
                    InterestAccountNumber = oldLoanAccounting.ChartOfAccountIdForInterest ?? "N/A", // Interest account number.
                    VatAccountNumber = oldLoanAccounting.ChartOfAccountIdForVAT ?? "N/A", // VAT account number.
                    AmountAccountNumber = oldLoanAccounting.ChartOfAccountIdForCapital ?? "N/A", // Capital account number.
                    AmountInterest = refund.Interest, // Interest amount.
                    AmountVAT = refund.Tax, // VAT amount.
                    AmountCapital = refund.Principal, // Principal amount.
                    InterestNaration = $"Migrated Loan Repayment | Type: Interest | Amount: {BaseUtilities.FormatCurrency(refund.Interest)} | REF: {ReferenceNumber} | Member Reference: {customer.CustomerId} | Member Name: {customer.FirstName} {customer.LastName}",
                    VatNaration = $"Migrated Loan Repayment | Type: VAT | Amount: {BaseUtilities.FormatCurrency(refund.Tax)} | REF: {ReferenceNumber} | Member Reference: {customer.CustomerId} | Member Name: {customer.FirstName} {customer.LastName}",
                };
            }
            else
            {
                // Handle regular branch transactions.
                addAccountingPostingCommand.AmountCollection.Add(new LoanRefundCollection
                {
                    Amount = refund.Principal, // Principal amount.
                    EventAttributeName = OperationEventRubbriqueNameForLoan.Loan_Principal_Account.ToString(), // Event attribute name for principal.
                    Naration = $"Loan Repayment | Type: Principal | Amount: {BaseUtilities.FormatCurrency(refund.Principal)} | REF: {ReferenceNumber} | Member Reference: {customer.CustomerId} | Member Name: {customer.FirstName} {customer.LastName}"
                });

                addAccountingPostingCommand.AmountCollection.Add(new LoanRefundCollection
                {
                    Amount = refund.Interest, // Interest amount.
                    EventAttributeName = OperationEventRubbriqueNameForLoan.Loan_Interest_Recieved_Account.ToString(), // Event attribute name for interest.
                    Naration = $"Loan Repayment | Type: Interest | Amount: {BaseUtilities.FormatCurrency(refund.Interest)} | REF: {ReferenceNumber} | Member Reference: {customer.CustomerId} | Member Name: {customer.FirstName} {customer.LastName}"
                });

                addAccountingPostingCommand.AmountCollection.Add(new LoanRefundCollection
                {
                    Amount = refund.Tax, // Tax amount.
                    EventAttributeName = OperationEventRubbriqueNameForLoan.Loan_VAT_Account.ToString(), // Event attribute name for VAT.
                    Naration = $"Loan Repayment | Type: VAT | Amount: {BaseUtilities.FormatCurrency(refund.Tax)} | REF: {ReferenceNumber} | Member Reference: {customer.CustomerId} | Member Name: {customer.FirstName} {customer.LastName}"
                });

                // Set default values for non-applicable accounts in LoanRefundCollectionAlpha.
                addAccountingPostingCommand.LoanRefundCollectionAlpha = new LoanRefundCollectionAlpha
                {
                    InterestAccountNumber = "N/A",
                    VatAccountNumber = "N/A",
                    AmountAccountNumber = "N/A",
                    AmountInterest = refund.Interest,
                    AmountVAT = refund.Tax,
                    AmountCapital = refund.Principal,
                    InterestNaration = "N/A",
                    VatNaration = "N/A",
                };
            }

            // Send the command for processing and capture the result.
            var result = await _mediator.Send(addAccountingPostingCommand);

            if (result.StatusCode != 200)
            {
                // Capture the error message if the posting failed.
                accountingResponseMessages = $"{result.Message}, ";

                // Log and audit the error.
                string errorMessage = $"Accounting posting failed | REF: {ReferenceNumber} | Error: {result.Message}";
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    refund,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.LoanRepayment,
                    LogLevelInfo.Error,
                    ReferenceNumber);
            }
            else
            {
                // Log and audit the success.
                string successMessage = $"Accounting posting successful | REF: {ReferenceNumber} | Amount: {BaseUtilities.FormatCurrency(refund.Amount)}";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    refund,
                    HttpStatusCodeEnum.OK,
                    LogAction.LoanRepayment,
                    LogLevelInfo.Information,
                    ReferenceNumber);
            }

            // Return the accounting response messages.
            return accountingResponseMessages;
        }

    }
}
