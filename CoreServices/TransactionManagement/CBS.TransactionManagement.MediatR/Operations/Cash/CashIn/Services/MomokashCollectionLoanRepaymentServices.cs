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
using CBS.TransactionManagement.Repository.Receipts.Payments;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.MediatR.LoanRepayment.Command;
using DocumentFormat.OpenXml.Bibliography;
using CBS.TransactionManagement.Repository.OldLoanConfiguration;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data.Entity.OldLoanConfiguration;
using CBS.TransactionManagement.LoanRepayment.Command;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.NewFolder
{

    public class MomokashCollectionLoanRepaymentServices : IMomokashCollectionLoanRepaymentServices
    {
        private readonly IAccountRepository _accountRepository; // Repository for accessing account data.
        private readonly UserInfoToken _userInfoToken; // User information token.
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository; // Repository for accessing teller data.
        private readonly ITellerRepository _tellerRepository; // Repository for accessing teller data.
        private readonly IConfigRepository _configRepository;
        private readonly IDepositServices _depositServices;
        private readonly IPaymentReceiptRepository _paymentReceiptRepository;
        private readonly IMapper _mapper;
        private readonly ILoanRepaymentServices _loanRepaymentServices;
        private readonly IOldLoanAccountingMapingRepository _OldLoanAccountingMapingRepository;
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository;
        public IMediator _mediator { get; set; } // Mediator for handling requests.
        private readonly ILogger<MomokashCollectionLoanRepaymentServices> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for transaction context.

        public MomokashCollectionLoanRepaymentServices(
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            ILogger<MomokashCollectionLoanRepaymentServices> logger,
            IUnitOfWork<TransactionContext> uow,
            ITellerRepository tellerRepository = null,
            IMediator mediator = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            IDepositServices depositServices = null,
            IConfigRepository configRepository = null,
            IPaymentReceiptRepository paymentReceiptRepository = null,
            IMapper mapper = null,
            ILoanRepaymentServices loanRepaymentServices = null,
            IOldLoanAccountingMapingRepository oldLoanAccountingMapingRepository = null,
            IAPIUtilityServicesRepository utilityServicesRepository = null)
        {
            _userInfoToken = UserInfoToken; // Initialize user information token.
            _accountRepository = AccountRepository; // Initialize account repository.
            _logger = logger; // Initialize logger.
            _uow = uow; // Initialize unit of work.
            _tellerRepository = tellerRepository; // Initialize teller repository.
            _mediator = mediator; // Initialize _mediator.
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _depositServices = depositServices;
            _configRepository = configRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _mapper = mapper;
            _loanRepaymentServices = loanRepaymentServices;
            _OldLoanAccountingMapingRepository = oldLoanAccountingMapingRepository;
            _utilityServicesRepository=utilityServicesRepository;
        }


        /// <summary>
        /// Handles the loan repayment process through Momokash collection. 
        /// Manages loan repayment transactions, payment processing, and accounting entries.
        /// </summary>
        /// <param name="requests">Bulk operation requests for loan repayment.</param>
        /// <param name="accountingDate">The accounting day for the transaction.</param>
        /// <returns>A service response containing the payment receipt details.</returns>
        public async Task<ServiceResponse<PaymentReceiptDto>> LoanRepaymentMomokashCollection(AddBulkOperationDepositCommand requests, DateTime accountingDate)
        {
            try
            {
                // Step 1: Get loan information from the requests.
                var loanInfo = requests.BulkOperations.FirstOrDefault();

                bool isInterBranchOperation = false; // Flag to indicate if the operation is inter-branch.

                // Step 2: Check if system configuration is set.
                var config = await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString());

                // Step 3: Retrieve teller information.
                var teller = await _tellerRepository.GetTellerByType(loanInfo.SourceType);

                // Step 4: Check teller rights for the requested operation.
                await _tellerRepository.CheckTellerOperationalRights(teller, requests.OperationType, requests.IsCashOperation);

                // Step 5: Retrieve sub-teller account.
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Step 6: Retrieve customer information.
                var customer = await GetCustomer(requests);

                // Step 7: Retrieve branch information.
                var branch = await GetBranch(customer);

                // Step 8: Generate a unique transaction reference.
                string reference = CurrencyNotesMapper.GenerateTransactionReference(
                    _userInfoToken,
                    TransactionType.MomokcashCollection_Loan_Repayment.ToString(),
                    isInterBranchOperation
                );

                // Step 9: Calculate total amount for the loan repayment.
                decimal amount = CalculateTotalAmount(requests.BulkOperations);

                // Step 10: Retrieve loan account information.
                var loanAccount = await _accountRepository.GetMemberLoanAccount(loanInfo.CustomerId);

                // Step 11: Map bulk operations to bulk deposit objects for processing.
                var bulkDeposits = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(
                    requests.BulkOperations,
                    true,
                    loanAccount.AccountNumber
                );

                // Step 12: Check if the operation is inter-branch and handle appropriately.
                if (teller.BranchId != customer.BranchId)
                {
                    isInterBranchOperation = true;
                    var errorMessage = "Inter-branch operation is not allowed for Momo cash collection.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.Forbidden, LogAction.MomocashCollectionLoanRepayment, LogLevelInfo.Error);
                    return ServiceResponse<PaymentReceiptDto>.Return403(errorMessage);
                }

                // Step 13: Process the loan repayment using mapped bulk deposits.
                var refund = await MakeLoanRepayment(amount, bulkDeposits.FirstOrDefault(), reference, loanInfo);

                // Step 14: Prepare loan payment object for transactions.
                string sourceType = loanInfo.SourceType == AccountType.MobileMoneyMTN.ToString() ? "MTN Money" : "Orange Money";
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
                    DestinationBranchCommission = 0,
                    EvenetType = TransactionType.MomokcashCollection_Loan_Repayment.ToString(),
                    Interest = refund.Interest,
                    IsInterBranch = isInterBranchOperation,
                    OperationType = TransactionType.MomokcashCollection_Loan_Repayment.ToString(),
                    Principal = refund.Principal,
                    SourceBranchCommission = 0,
                    LoanRepaymentType = requests.DepositType,
                    SourceBranchId = teller.BranchId,
                    TransactionReference = reference,
                    Vat = refund.Tax
                };

                // Step 15: Process bulk transactions for the loan repayment.
                var transactions = await ProcessBulkTransactions(
                    bulkDeposits,
                    teller,
                    tellerAccount,
                    customer,
                    branch,
                    loanPaymentObject
                );

                // Step 16: Prepare payment receipt details.
                var paymentReceipts = new List<PaymentDetailObject>
        {
            new PaymentDetailObject
            {
                AccountNumber = refund.LoanId,
                Fee = 0,
                Amount = refund.Amount,
                Interest = refund.Interest,
                LoanCapital = refund.Principal,
                SericeOrEventName = TransactionType.MomokcashCollection_Loan_Repayment.ToString(),
                VAT = refund.Tax,
                Balance = refund.Balance
            }
        };

                // Step 17: Create a payment processing request.
                var paymentProcessing = new PaymentProcessingRequest
                {
                    AccountingDay = accountingDate,
                    Amount = amount,
                    MemberName = customer.Name,
                    NotesRequest = new CurrencyNotesRequest(),
                    OperationType = OperationType.Cash.ToString(),
                    OperationTypeGrouping = TransactionType.CASH_IN.ToString(),
                    PortalUsed = OperationSourceType.Web_Portal.ToString(),
                    PaymentDetails = paymentReceipts,
                    ServiceType = TransactionType.MomokcashCollection_Loan_Repayment.ToString(),
                    SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
                    TotalAmount = amount,
                    TotalCharges = transactions.Sum(x => x.Fee),
                    Transactions = transactions
                };

                // Step 18: Process payment and get the receipt.
                var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);

                // Step 19: Save changes to the database.
                await _uow.SaveAsync();

                // Step 20: Send SMS notification to the customer.
                await SendSMS(paymentReceipts.FirstOrDefault(), reference, customer, branch);

                // Step 21: Retrieve loan configuration for accounting.
                var oldLoanParam = await _OldLoanAccountingMapingRepository
                    .FindBy(x => x.LoanTypeName == refund.Loan.LoanCategory && !x.IsDeleted)
                    .FirstOrDefaultAsync() ?? new Data.Entity.OldLoanConfiguration.OldLoanAccountingMaping();

                // Step 22: Post accounting entries for the loan repayment.
                var accountingResponseMessages = await PostAccounting(refund, customer, reference, accountingDate, sourceType, oldLoanParam);

                // Step 23: Prepare and log the final success message.
                var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);
                string finalMessage = accountingResponseMessages == null
                    ? $"Account Posting: Successful. A cash deposit of {BaseUtilities.FormatCurrency(amount)} was successfully processed for loan repayment on behalf of Member: {customer.Name}."
                    : $"Account Posting: Failed. A cash deposit of {BaseUtilities.FormatCurrency(amount)} was processed for loan repayment on behalf of Member: {customer.Name}.";

                await BaseUtilities.LogAndAuditAsync(finalMessage, requests, HttpStatusCodeEnum.OK, LogAction.MomocashCollectionLoanRepayment, LogLevelInfo.Information);
                _logger.LogInformation(finalMessage);

                // Return success response.
                return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, finalMessage);
            }
            catch (Exception e)
            {
                // Step 24: Handle errors and log the exception.
                string errorMessage = $"Error during loan repayment: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.InternalServerError, LogAction.MomocashCollectionLoanRepayment, LogLevelInfo.Error);
                return ServiceResponse<PaymentReceiptDto>.Return500(errorMessage);
            }
        }


        //public async Task<ServiceResponse<PaymentReceiptDto>> LoanRepaymentMomokashCollection(AddBulkOperationDepositCommand requests, DateTime accountingDate)
        //{

        //    try
        //    {
        //        // Step 1: Get loan information from the requests.
        //        var loanInfo = requests.BulkOperations.FirstOrDefault();


        //        bool isInterBranchOperation = false; // Flag to indicate if the operation is inter-branch.


        //        // Check if system configuration is set.
        //        var config = await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString());

        //        // Retrieve teller information.
        //        var teller = await _tellerRepository.GetTellerByType(requests.BulkOperations.FirstOrDefault().SourceType);
        //        // Check teller rights.
        //        await _tellerRepository.CheckTellerOperationalRights(teller, requests.OperationType, requests.IsCashOperation);

        //        // Retrieve sub teller account.
        //        var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

        //        // Retrieve customer information.
        //        var customer = await GetCustomer(requests);

        //        // Step 11: Retrieve branch information.
        //        var branch = await GetBranch(customer);


        //        // Step 12: Generate transaction reference based on branch type.
        //        string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.MomokcashCollection_Loan_Repayment.ToString(), isInterBranchOperation);

        //        // Step 13: Calculate total amount and charges.
        //        decimal amount = CalculateTotalAmount(requests.BulkOperations);

        //        // Step 14: Retrieve currency notes.
        //        //var currencyNote = await RetrieveCurrencyNotes(reference, requests.BulkOperations.FirstOrDefault().currencyNotes);

        //        // Step 15: Record cash-in by denomination.
        //        //_subTellerProvioningHistoryRepository.CashInByDinomination(amount, requests.BulkOperations.FirstOrDefault().currencyNotes, teller.Id, accountingDate, tellerAccount.OpenningOfDayReference);


        //        // Step 16: Retrieve loan account information.
        //        var loanAccount = await _accountRepository.GetMemberLoanAccount(loanInfo.CustomerId);

        //        // Step 17: Map the list of BulkOperation objects to a list of corresponding BulkDeposit objects.
        //        var bulkDeposits = CurrencyNotesMapper.MapBulkOperationListToBulkDepositList(requests.BulkOperations, true, loanAccount.AccountNumber);

        //        // Step 18: Check if the operation is inter-branch and update reference if needed.
        //        if (teller.BranchId != customer.BranchId)
        //        {
        //            // Set the operation as inter-branch
        //            isInterBranchOperation = true;

        //            // Prepare an error message
        //            var errorMessage = $"Inter-branch operation is not allowed for Momo cash collection.";

        //            // Log and audit the error message
        //            await BaseUtilities.LogAndAuditAsync(
        //                errorMessage,
        //                requests,
        //                HttpStatusCodeEnum.Forbidden,
        //                LogAction.DepositProcessed,
        //                LogLevelInfo.Error
        //            );

        //            // Return a 403 service response with the error details
        //            return ServiceResponse<PaymentReceiptDto>.Return403(errorMessage);
        //        }

        //        // Step 21: Make the loan repayment.
        //        //var refund = await MakeLoanRepayment(loanInfo.AccountNumber, loanInfo.Amount, reference);
        //        var refund = await MakeLoanRepayment(amount, bulkDeposits.FirstOrDefault(), reference, loanInfo);

        //        // Step 19: Process each transaction in the bulk.
        //        string soureType = requests.BulkOperations.FirstOrDefault().SourceType == AccountType.MobileMoneyMTN.ToString() ? "MTN Money" : "Orange Money";
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
        //            DestinationBranchCommission = 0,
        //            EvenetType = TransactionType.MomokcashCollection_Loan_Repayment.ToString(),
        //            Interest = refund.Interest,
        //            IsInterBranch = isInterBranchOperation,
        //            OperationType = TransactionType.MomokcashCollection_Loan_Repayment.ToString(),
        //            Principal = refund.Principal,
        //            SourceBranchCommission = 0,
        //            LoanRepaymentType = requests.DepositType,
        //            SourceBranchId = teller.BranchId,
        //            TransactionReference = reference,
        //            Vat = refund.Tax
        //        };

        //        var transactions = await ProcessBulkTransactions(bulkDeposits, teller, tellerAccount, customer, branch, loanPaymentObject);


        //        // Step 22: Prepare payment receipt details.
        //        var accountDeposits = new List<AccountDeposit>();
        //        var paymentReciepts = new List<PaymentDetailObject>();

        //        paymentReciepts.Add(new PaymentDetailObject
        //        {
        //            AccountNumber = refund.LoanId,
        //            Fee = 0,
        //            Amount = refund.Amount,
        //            Interest = refund.Interest,
        //            LoanCapital = refund.Principal,
        //            SericeOrEventName = TransactionType.MomokcashCollection_Loan_Repayment.ToString(),
        //            VAT = refund.Tax,
        //            Balance = refund.Balance
        //        });

        //        // Step 23: Create payment processing request object.
        //        var paymentProcessing = new PaymentProcessingRequest
        //        {
        //            AccountingDay = accountingDate,
        //            Amount = amount,
        //            MemberName = customer.Name,
        //            NotesRequest = new CurrencyNotesRequest(),
        //            OperationType = OperationType.Cash.ToString(),
        //            OperationTypeGrouping = TransactionType.CASH_IN.ToString(),
        //            PortalUsed = OperationSourceType.Web_Portal.ToString(),
        //            PaymentDetails = paymentReciepts,
        //            ServiceType = TransactionType.MomokcashCollection_Loan_Repayment.ToString(),
        //            SourceOfRequest = OperationSourceType.Physical_Teller.ToString(),
        //            TotalAmount = amount,
        //            TotalCharges = transactions.Sum(x => x.Fee),
        //            Transactions = transactions
        //        };

        //        // Step 24: Process payment and get receipt.
        //        var paymentReceipt = _paymentReceiptRepository.ProcessPaymentAsync(paymentProcessing);

        //        // Step 25: Save changes to the database.
        //        await _uow.SaveAsync();

        //        // Step 27: Send SMS notification to customer.
        //        await SendSMS(paymentReciepts.FirstOrDefault(), reference, customer, branch);

        //        var oldLoanPAram = await _OldLoanAccountingMapingRepository.FindBy(x => x.LoanTypeName == refund.Loan.LoanCategory && x.IsDeleted == false).FirstOrDefaultAsync();
        //        if (oldLoanPAram == null)
        //        {
        //            oldLoanPAram = new Data.Entity.OldLoanConfiguration.OldLoanAccountingMaping();
        //        }
        //        // Step 28: Post accounting entries for transactions.
        //        var accountingResponseMessages = await PostAccounting(refund, customer, reference, accountingDate, soureType, oldLoanPAram);

        //         var paymentReceiptDto = _mapper.Map<PaymentReceiptDto>(paymentReceipt);

        //        if (accountingResponseMessages == null)
        //        {
        //            accountingResponseMessages = $"Account Posting: Successful. A cash deposit of {BaseUtilities.FormatCurrency(amount)} has been successfully processed for the repayment of the loan on behalf of Member: {customer.Name}.";
        //        }
        //        else
        //        {
        //            accountingResponseMessages = $"Account Posting: Failed. A cash deposit of {BaseUtilities.FormatCurrency(amount)} has been successfully processed for the repayment of the loan on behalf of Member: {customer.Name}.";
        //        }
        //        await BaseUtilities.LogAndAuditAsync(accountingResponseMessages, requests, HttpStatusCodeEnum.OK, LogAction.LoanRepayment, LogLevelInfo.Information);

        //        _logger.LogInformation(accountingResponseMessages);

        //        // Return success response.
        //        return ServiceResponse<PaymentReceiptDto>.ReturnResultWith200(paymentReceiptDto, accountingResponseMessages);
        //    }
        //    catch (Exception e)
        //    {
        //        // Log error and return 500 Internal Server Error response with error message.
        //        var errorMessage = $"Error: {e.Message}";
        //        _logger.LogError(errorMessage);
        //        await BaseUtilities.LogAndAuditAsync(errorMessage, requests, HttpStatusCodeEnum.InternalServerError, LogAction.LoanRepayment, LogLevelInfo.Error);
        //        return ServiceResponse<PaymentReceiptDto>.Return500(e);
        //    }
        //}
        private async Task<RefundDto> MakeLoanRepayment(decimal amount, BulkDeposit deposit, string TransactionCode, BulkOperation loanInfo)
        {
            deposit.Principal = deposit.Amount;
            var addRepaymentCommand = new AddLoanRepaymentCommandDetails { Amount = amount, Comment = deposit.Note == null ? "Loan Repayment Momocash" : deposit.Note, LoanId = loanInfo.AccountNumber, Interest = deposit.Interest, PaymentChannel = deposit.PaymentMethod == null ? "Web_Portal" : deposit.PaymentMethod, Penalty = deposit.Penalty, Principal = loanInfo.Amount, Tax = deposit.Tax, PaymentMethod = deposit.PaymentMethod == null ? "Cash" : deposit.PaymentMethod, TransactionCode = TransactionCode };
            var addRepaymentCommandResponse = await _mediator.Send(addRepaymentCommand);
            if (addRepaymentCommandResponse == null)
            {
                var errorMessage = $"Loan API Failed: {addRepaymentCommandResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return addRepaymentCommandResponse.Data; // Return branch data.

        }
        private async Task<RefundDto> MakeLoanRepayment(string loanId, decimal Amount, string TransactionCode)
        {
            var addRepaymentCommand = new AddRepaymentCommand { LoanId = loanId, Amount = Amount, TransactionCode = TransactionCode, PaymentChannel = OperationSourceType.Web_Portal.ToString(), PaymentMethod = "Cash" }; // Create command to get branch.
            var addRepaymentCommandResponse = await _mediator.Send(addRepaymentCommand); // Send command to _mediator.
            // Check if branch information retrieval was successful
            if (addRepaymentCommandResponse == null)
            {
                var errorMessage = $"Loan API Failed: {addRepaymentCommandResponse.Message}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return addRepaymentCommandResponse.Data; // Return branch data.
        }
        // Method to send SMS notification
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
                      $"Remaining Balance to Repay: {BaseUtilities.FormatCurrency(paymentDetail.Balance)}\n" +
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
                      $"Solde restant à rembourser : {BaseUtilities.FormatCurrency(paymentDetail.Balance)}\n" +
                      $"Référence : {reference}. Date : {BaseUtilities.UtcToDoualaTime(DateTime.UtcNow)}.";
            }

            // Create command to send SMS
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = "237650535634"//customer.Phone
            };
            await _utilityServicesRepository.PushNotification(customer.CustomerId, PushNotificationTitle.LOAN_REPAYMENT_MOMO_CASH, msg);
            // Send command to _mediator
            await _mediator.Send(sMSPICallCommand);
        }

        // Method to retrieve customer information
        private async Task<CustomerDto> GetCustomer(AddBulkOperationDepositCommand requests)
        {
            string customerId = requests.BulkOperations.FirstOrDefault().CustomerId; // Get customer ID from request.
            var customerCommandQuery = new GetCustomerCommandQuery { CustomerID = customerId }; // Create command to get customer.
            var customerResponse = await _mediator.Send(customerCommandQuery); // Send command to _mediator.

            if (customerResponse == null)
            {
                var errorMessage = "Error; Null";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Check if customer information retrieval was successful
            if (customerResponse.StatusCode != 200)
            {
                var errorMessage = "Failed getting member's information";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            var customer = customerResponse.Data; // Get customer data from response.
            customer.Name = $"{customer.FirstName} {customer.LastName}";

            return customer; // Return customer data.
        }

        // Method to retrieve branch information
        private async Task<BranchDto> GetBranch(CustomerDto customer)
        {
            var branchCommandQuery = new GetBranchByIdCommand { BranchId = customer.BranchId }; // Create command to get branch.
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
        // Method to calculate total amount from bulk operations
        private decimal CalculateTotalAmount(IEnumerable<BulkOperation> bulkOperations)
        {
            return bulkOperations.Sum(x => x.Total); // Calculate total amount.
        }
        // Method to calculate total charges from bulk operations
        private decimal CalculateTotalCharges(IEnumerable<BulkOperation> bulkOperations)
        {
            return bulkOperations.Sum(x => x.Fee); // Calculate total charges.
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
        // Method to process bulk transactions
        private async Task<List<TransactionDto>> ProcessBulkTransactions(List<BulkDeposit> requests, Teller teller, Account tellerAccount, CustomerDto customer, BranchDto branch, TellerLoanPaymentObject paymentObject)
        {
            var transactions = new List<TransactionDto>(); // Initialize list to store transactions.
            string transactionType = TransactionType.CASHIN_LOAN_REPAYMENT.ToString();
            foreach (var request in requests)
            {

                var customerAccount = await _accountRepository.GetAccount(request.AccountNumber, transactionType); // Get customer account information.
                request.Customer = customer;
                request.currencyNotes = new List<CurrencyNotesDto>();
                request.Branch = branch;
                request.IsExternalOperation = false;
                request.ExternalApplicationName = "N/A";
                request.ExternalReference = "N/A";
                request.SourceType = OperationSourceType.Web_Portal.ToString();
                // Deposit amount into account
                var transaction = await _loanRepaymentServices.LoanDepositCash(request, teller, tellerAccount, customerAccount, paymentObject);
                transaction.IsChargesIclussive = request.IsChargesInclussive;
                transactions.Add(transaction); // Add transaction to list.
            }
            return transactions; // Return list of transactions.
        }



        private async Task<string> PostAccounting(
            RefundDto refund,
            CustomerDto customer,
            string ReferenceNumber,
            DateTime accountingDate,
            string MomoOperatorType,
            OldLoanAccountingMaping oldLoanAccounting)
        {
            string accountingResponseMessages = null;

            // Create a new instance of MomocashCollectionLoanRepaymentCommand and initialize its properties
            var addAccountingPostingCommand = new MomocashCollectionLoanRepaymentCommand
            {
                AccountNumber = refund.LoanProduct?.AccountNumber, // Set the account number
                LoanProductId = refund.LoanProduct?.Id, // Set the product ID
                Naration = refund.Loan.IsUpload
                    ? $"Migrated Loan Repayment | Member: {customer.Name} | Member Reference: {customer.CustomerId} | Transaction Reference: {ReferenceNumber}"
                    : $"TSC Loan Repayment | Member: {customer.Name} | Member Reference: {customer.CustomerId} | Transaction Reference: {ReferenceNumber}", // Set the transaction narration
                TransactionReferenceId = ReferenceNumber, // Set the transaction reference ID
                Amount = refund.Amount,
                TellerSource = TellerSources.Virtual_Teller_Momo_cash_Collection.ToString(),
                IsOldSystemLoan = refund.Loan.IsUpload,
                BranchId = refund.BranchId,
                TransactionDate = accountingDate,
                MomoOperatorType = MomoOperatorType,
            };

            if (addAccountingPostingCommand.IsOldSystemLoan)
            {
                // For old system loans
                addAccountingPostingCommand.LoanRefundCollectionAlpha = new LoanRefundCollectionAlpha
                {
                    InterestAccountNumber = oldLoanAccounting.ChartOfAccountIdForInterest ?? "N/A",
                    VatAccountNumber = oldLoanAccounting.ChartOfAccountIdForVAT ?? "N/A",
                    AmountAccountNumber = oldLoanAccounting.ChartOfAccountIdForCapital ?? "N/A",
                    AmountInterest = refund.Interest,
                    AmountVAT = refund.Tax,
                    AmountCapital = refund.Principal,
                    InterestNaration = $"Migrated Loan Repayment | Type: Interest | Amount: {BaseUtilities.FormatCurrency(refund.Interest)} | Reference: {ReferenceNumber} | Member: {customer.Name}",
                    VatNaration = $"Migrated Loan Repayment | Type: VAT | Amount: {BaseUtilities.FormatCurrency(refund.Tax)} | Reference: {ReferenceNumber} | Member: {customer.Name}",
                };
            }
            else
            {
                // For regular branch transactions
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = refund.Principal, // Set the principal amount
                    EventAttributeName = OperationEventRubbriqueNameForLoan.Loan_Principal_Account.ToString(), // Set the event attribute name
                    Naration = $"Loan Repayment | Type: Principal | Amount: {BaseUtilities.FormatCurrency(refund.Principal)} | Reference: {ReferenceNumber} | Member: {customer.Name}"
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = refund.Interest, // Set the interest amount
                    EventAttributeName = OperationEventRubbriqueNameForLoan.Loan_Interest_Recieved_Account.ToString(), // Set the event attribute name
                    Naration = $"Loan Repayment | Type: Interest | Amount: {BaseUtilities.FormatCurrency(refund.Interest)} | Reference: {ReferenceNumber} | Member: {customer.Name}"
                });
                addAccountingPostingCommand.AmountCollection.Add(new AmountCollection
                {
                    Amount = refund.Tax, // Set the VAT amount
                    EventAttributeName = OperationEventRubbriqueNameForLoan.Loan_VAT_Account.ToString(), // Set the event attribute name
                    Naration = $"Loan Repayment | Type: VAT | Amount: {BaseUtilities.FormatCurrency(refund.Tax)} | Reference: {ReferenceNumber} | Member: {customer.Name}"
                });

                addAccountingPostingCommand.LoanRefundCollectionAlpha = new LoanRefundCollectionAlpha
                {
                    InterestAccountNumber = "N/A",
                    VatAccountNumber = "N/A",
                    AmountAccountNumber = "N/A",
                    AmountInterest = refund.Interest,
                    AmountVAT = refund.Tax,
                    AmountCapital = refund.Principal,
                    InterestNaration = $"Loan Repayment | Type: Interest | Amount: {BaseUtilities.FormatCurrency(refund.Interest)} | Reference: {ReferenceNumber} | Member: {customer.Name}",
                    VatNaration = $"Loan Repayment | Type: VAT | Amount: {BaseUtilities.FormatCurrency(refund.Tax)} | Reference: {ReferenceNumber} | Member: {customer.Name}"
                };
            }

            // Send the accounting command for processing
            var result = await _mediator.Send(addAccountingPostingCommand);

            if (result.StatusCode != 200)
            {
                // Append error message to response messages
                accountingResponseMessages = $"{result.Message}, ";
            }

            return accountingResponseMessages; // Return accounting response messages
        }
    }
}
