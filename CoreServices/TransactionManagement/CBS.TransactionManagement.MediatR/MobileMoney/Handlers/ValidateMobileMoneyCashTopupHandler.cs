using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.MediatR.MobileMoney.Commands;
using CBS.TransactionManagement.Repository.MobileMoney;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Data.Entity.MobileMoney;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MediatR.Accounting.Command;

namespace CBS.TransactionManagement.MediatR.MobileMoney.Handlers
{

    /// <summary>
    /// Handles the command to update a CashReplenishment based on UpdateCashReplenishmentCommand.
    /// </summary>
    public class ValidateMobileMoneyCashTopupHandler : IRequestHandler<ValidateMobileMoneyCashTopupCommand, ServiceResponse<MobileMoneyCashTopupDto>>
    {
        private readonly IMobileMoneyCashTopupRepository _mobileMoneyCashTopupRepository; // Repository for accessing CashReplenishment data.
        private readonly ILogger<ValidateMobileMoneyCashTopupHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountRepository _accountRepository;
        private readonly ITellerRepository _tellerRepository;

        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly ITransactionRepository _transactionRepository;
        public IMediator _mediator { get; set; }

        //_dailyTellerRepository
        /// <summary>
        /// Constructor for initializing the UpdateCashReplenishmentCommandHandler.
        /// </summary>
        /// <param name="mobileMoneyCashTopupRepository">Repository for CashReplenishment data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public ValidateMobileMoneyCashTopupHandler(
            IMobileMoneyCashTopupRepository mobileMoneyCashTopupRepository,
            ILogger<ValidateMobileMoneyCashTopupHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null
,
            IAccountRepository accountRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            ITransactionRepository transactionRepository = null,
            IMediator mediator = null,
            ITellerRepository tellerRepository = null)
        {
            _mobileMoneyCashTopupRepository = mobileMoneyCashTopupRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
            _accountRepository = accountRepository;
            _accountingDayRepository = accountingDayRepository;
            _transactionRepository = transactionRepository;
            _mediator = mediator;
            _tellerRepository = tellerRepository;
        }
      
        /// <summary>
        /// Handles the ValidateMobileMoneyCashTopupCommand to validate and process a Mobile Money Cash Top-up.
        /// </summary>
        /// <param name="request">The command containing updated MobileMoneyCashTopup data.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
        /// <returns>ServiceResponse containing the MobileMoneyCashTopupDto or an error message.</returns>
        public async Task<ServiceResponse<MobileMoneyCashTopupDto>> Handle(ValidateMobileMoneyCashTopupCommand request, CancellationToken cancellationToken)
        {
            string logReference = BaseUtilities.GenerateUniqueNumber();  // Generate a unique reference for tracking logs.

            try
            {
                // 1. Fetch the current accounting day.
                var accountingDay = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);
                string accountingMessage = $"Fetched current accounting day for branch '{_userInfoToken.BranchID}'.";
                _logger.LogInformation(accountingMessage);

                // 2. Fetch the MobileMoneyCashTopup entity.
                var mobileMoneyCashTopup = await _mobileMoneyCashTopupRepository.FindAsync(request.Id);
                string fetchTopupMessage = $"Fetched Mobile Money Cash Topup request with ID '{request.Id}'.";
                _logger.LogInformation(fetchTopupMessage);

                // 3. Check if the MobileMoneyCashTopup exists.
                if (mobileMoneyCashTopup == null)
                {
                    string errorMessage = $"Mobile Money Cash Topup request with ID '{request.Id}' not found.";
                    return await LogAndAuditFailure(errorMessage, request, logReference, HttpStatusCodeEnum.NotFound);
                }

                // 4. Check if the request is already approved.
                if (mobileMoneyCashTopup.RequestApprovalStatus == Status.Approved.ToString())
                {
                    string errorMessage = $"Mobile Money Cash Topup request with Transaction ID '{mobileMoneyCashTopup.MobileMoneyTransactionId}' is already approved.";
                    return await LogAndAuditFailure(errorMessage, request, logReference, HttpStatusCodeEnum.Forbidden);
                }

                // 5. Fetch the associated customer.
                var customer = await GetCustomer(mobileMoneyCashTopup.MobileMoneyMemberReference);
                string customerMessage = $"Fetched customer with Member Reference '{mobileMoneyCashTopup.MobileMoneyMemberReference}'.";
                _logger.LogInformation(customerMessage);

                // 6. Fetch the branch for the customer.
                var branch = await GetBranch(customer);
                string branchMessage = $"Fetched branch for customer '{customer.Name}'.";
                _logger.LogInformation(branchMessage);

                // 7. Map request data to the MobileMoneyCashTopup entity.
                _mapper.Map(request, mobileMoneyCashTopup);
                string mappingMessage = $"Mapped request data to MobileMoneyCashTopup entity with ID '{request.Id}'.";
                _logger.LogInformation(mappingMessage);

                // 8. Set approval details.
                mobileMoneyCashTopup.RequestApprovedBy = _userInfoToken.FullName ?? _userInfoToken.Email;
                mobileMoneyCashTopup.RequestApprovalDate = BaseUtilities.UtcNowToDoualaTime();
                string approvalInfoMessage = $"Request approved by '{mobileMoneyCashTopup.RequestApprovedBy}' on '{mobileMoneyCashTopup.RequestApprovalDate}'.";
                _logger.LogInformation(approvalInfoMessage);

                bool isApproved = false;
                decimal balance = 0;
                string transactionType = string.Empty;
                var teller = new Teller();

                // 9. Handle processing if the request is approved.
                if (mobileMoneyCashTopup.RequestApprovalStatus == Status.Approved.ToString())
                {
                    // Fetch the mobile money account for the specified teller.
                    var mobileMoneyAccount = await _accountRepository.FindBy(x => x.IsTellerAccount && x.TellerId == mobileMoneyCashTopup.TellerId).FirstOrDefaultAsync();
                    if (mobileMoneyAccount == null)
                    {
                        string errorMessage = $"No {mobileMoneyCashTopup.OperatorType} account found for Account Number '{mobileMoneyCashTopup.AccountNumber}'.";
                        return await LogAndAuditFailure(errorMessage, request, logReference, HttpStatusCodeEnum.NotFound);
                    }

                    teller = await _tellerRepository.GetTeller(mobileMoneyAccount.TellerId);
                    balance = mobileMoneyAccount.PreviousBalance + mobileMoneyCashTopup.Amount;

                    var memberAccount = await _accountRepository.GetAccountByAccountNumber(mobileMoneyCashTopup.AccountNumber);

                    // Verify the integrity of balances.
                    _accountRepository.VerifyBalanceIntegrity(mobileMoneyAccount);
                    _accountRepository.VerifyBalanceIntegrity(memberAccount);

                    // Handle balance mismatch.
                    if (memberAccount.Balance != mobileMoneyAccount.Balance)
                    {
                        string warningMessage = $"Balance mismatch detected: Member Account ({memberAccount.AccountNumber}) Balance: {memberAccount.Balance}, " +
                                                $"Mobile Money Account ({mobileMoneyAccount.AccountNumber}) Balance: {mobileMoneyAccount.Balance}. Overriding balance.";
                        _logger.LogWarning(warningMessage);
                        await BaseUtilities.LogAndAuditAsync(warningMessage, request, HttpStatusCodeEnum.OK, LogAction.ValidateMobileMoneyCashTopup, LogLevelInfo.Warning, logReference);
                        memberAccount.Balance = mobileMoneyAccount.Balance;
                    }

                    // Credit both accounts.
                    _accountRepository.CreditAccount(mobileMoneyAccount, mobileMoneyCashTopup.Amount);
                    _accountRepository.CreditAccount(memberAccount, mobileMoneyCashTopup.Amount);

                    // Determine the transaction type.
                    transactionType = mobileMoneyAccount.AccountType == AccountType.MobileMoneyMTN.ToString() ?
                                      TransactionType.MobileMoney_TopUp.ToString() :
                                      TransactionType.OrangeMoney_TopUp.ToString();

                    // Map and record the transaction.
                    var transaction = CurrencyNotesMapper.MapTransactionMobileMoneyTopUp(mobileMoneyCashTopup, _userInfoToken, memberAccount, transactionType, accountingDay);
                    transaction.Account = null;
                    _transactionRepository.Add(transaction);

                    string transactionSuccessMessage = $"Processed Mobile Money Cash Top Up transaction for Momo ID '{mobileMoneyCashTopup.MobileMoneyTransactionId}' with type '{transactionType}'.";
                    _logger.LogInformation(transactionSuccessMessage);
                    await BaseUtilities.LogAndAuditAsync(transactionSuccessMessage, request, HttpStatusCodeEnum.OK, LogAction.ValidateMobileMoneyCashTopup, LogLevelInfo.Information, logReference);

                    isApproved = true;
                }

                // 10. Commit the transaction.
                await _uow.SaveAsync();
                string saveChangesMessage = $"Changes saved for Mobile Money Cash Top Up ID '{mobileMoneyCashTopup.MobileMoneyTransactionId}'.";
                _logger.LogInformation(saveChangesMessage);
                // 11. Send SMS notification.
                await SendSMS(mobileMoneyCashTopup.MobileMoneyTransactionId, customer.Phone, mobileMoneyCashTopup.Amount, mobileMoneyCashTopup.RequestReference, branch, $"{customer.FirstName} {customer.LastName}", mobileMoneyCashTopup.OperatorType, balance);
                string smsMessage = $"SMS notification sent for Mobile Money Cash Top Up request with amount '{mobileMoneyCashTopup.Amount}'.";
                _logger.LogInformation(smsMessage);
                await BaseUtilities.LogAndAuditAsync(smsMessage, request, HttpStatusCodeEnum.OK, LogAction.ValidateMobileMoneyCashTopup, LogLevelInfo.Information, logReference);

                // 12. Handle accounting posting.
                if (isApproved)
                {
                    await MakeAccountingPosting(mobileMoneyCashTopup.Amount, teller, accountingDay, mobileMoneyCashTopup.SourceType, mobileMoneyCashTopup.RequestReference);
                }

                // 13. Return success response.
                string successMessage = $"{transactionType} successfully {request.RequestApprovalStatus} for Mobile Money Cash Top Up request '{mobileMoneyCashTopup.MobileMoneyTransactionId}'.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.ValidateMobileMoneyCashTopup, LogLevelInfo.Information, logReference);

                return ServiceResponse<MobileMoneyCashTopupDto>.ReturnResultWith200(_mapper.Map<MobileMoneyCashTopupDto>(mobileMoneyCashTopup), successMessage);
            }
            catch (Exception e)
            {
                string errorMessage = $"Error occurred during Mobile Money Cash Top Up validation: {e.Message}";
                return await LogAndAuditFailure(errorMessage, request, logReference, HttpStatusCodeEnum.InternalServerError);
            }
        }

        /// <summary>
        /// Helper method to log and audit failures during the Mobile Money Cash Top Up validation process.
        /// </summary>
        private async Task<ServiceResponse<MobileMoneyCashTopupDto>> LogAndAuditFailure(string errorMessage, ValidateMobileMoneyCashTopupCommand request, string logReference, HttpStatusCodeEnum statusCode)
        {
            _logger.LogError(errorMessage);
            await BaseUtilities.LogAndAuditAsync(errorMessage, request, statusCode, LogAction.ValidateMobileMoneyCashTopup, LogLevelInfo.Error, logReference);
            return ServiceResponse<MobileMoneyCashTopupDto>.Return500(errorMessage);
        }

        /// <summary>
        /// Handles the ValidateMobileMoneyCashTopupCommand to validate a MobileMoneyCashTopup.
        /// </summary>
        /// <param name="request">The ValidateMobileMoneyCashTopupCommand containing updated CashTopup data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        //public async Task<ServiceResponse<MobileMoneyCashTopupDto>> Handle(ValidateMobileMoneyCashTopupCommand request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        // 1. Fetch the current accounting day for the branch of the user
        //        var accountingDay = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);
        //        _logger.LogInformation($"Fetching current accounting day for branch '{_userInfoToken.BranchID}'.");

        //        // 2. Fetch the MobileMoneyCashTopup entity based on the provided request Id
        //        var mobileMoneyCashTopup = await _mobileMoneyCashTopupRepository.FindAsync(request.Id);
        //        _logger.LogInformation($"Fetching Mobile Money Cash Topup for request ID '{request.Id}'.");

        //        // 3. Check if the CashReplenishment (MobileMoneyCashTopup) entity exists
        //        if (mobileMoneyCashTopup == null)
        //        {
        //            // Log and return 404 Not Found if the entity does not exist
        //            string errorMessage = $"Mobile Money Cash Topup request with ID '{request.Id}' was not found.";
        //            _logger.LogError(errorMessage);  // Log the error message for future debugging
        //            return ServiceResponse<MobileMoneyCashTopupDto>.Return404(errorMessage);  // Return 404 response
        //        }

        //        // 4. Check if the request is already approved (prevents further changes)
        //        if (mobileMoneyCashTopup.RequestApprovalStatus == Status.Approved.ToString())
        //        {
        //            // Log and return a 403 Forbidden response if the request has already been approved
        //            string errorMessage = $"Mobile Money Cash Topup request with Transaction ID '{mobileMoneyCashTopup.MobileMoneyTransactionId}' has already been approved.";
        //            _logger.LogError(errorMessage);  // Log the error message to track repeated attempts to modify an approved request
        //            return ServiceResponse<MobileMoneyCashTopupDto>.Return403(errorMessage);  // Return 403 response (Forbidden)
        //        }

        //        // 5. Fetch the customer associated with the MobileMoneyCashTopup (based on member reference)
        //        var customer = await GetCustomer(mobileMoneyCashTopup.MobileMoneyMemberReference);
        //        _logger.LogInformation($"Customer for Mobile Money Cash Topup with Member Reference '{mobileMoneyCashTopup.MobileMoneyMemberReference}' fetched.");

        //        // 6. Fetch the branch information for the customer
        //        var branch = await GetBranch(customer);
        //        _logger.LogInformation($"Branch for customer '{customer.Name}' fetched.");

        //        // 7. Map the request data to the existing MobileMoneyCashTopup entity
        //        _mapper.Map(request, mobileMoneyCashTopup);
        //        _logger.LogInformation($"Mapped request data to Mobile Money Cash Topup for request ID '{request.Id}'.");

        //        // 8. Set the approval information (Approved by the current user and approval date)
        //        mobileMoneyCashTopup.RequestApprovedBy = _userInfoToken.FullName ?? _userInfoToken.Email;  // Use FullName if available, otherwise Email
        //        mobileMoneyCashTopup.RequestApprovalDate = BaseUtilities.UtcNowToDoualaTime();  // Set the current date and time in Douala time zone
        //        _logger.LogInformation($"Request approved by '{mobileMoneyCashTopup.RequestApprovedBy}' on '{mobileMoneyCashTopup.RequestApprovalDate}'.");

        //        // 9. Initialize variables for transaction type and balance
        //        string transactionType = string.Empty;
        //        decimal balance = 0;
        //        var teller = new Teller();
        //        bool isApproved = false;
        //        // 10. Check if the request is approved for processing
        //        if (mobileMoneyCashTopup.RequestApprovalStatus == Status.Approved.ToString())
        //        {
        //            // 10.1. Fetch the mobile money account for the specified account number
        //            var mobileMoneyAccount = await _accountRepository.FindBy(x => x.IsTellerAccount && x.TellerId == mobileMoneyCashTopup.TellerId).FirstOrDefaultAsync();
        //            if (mobileMoneyAccount == null)
        //            {
        //                // Handle case where the account is not found
        //                string errorMessage = $"No {mobileMoneyCashTopup.OperatorType} account found for Account Number '{mobileMoneyCashTopup.AccountNumber}'.";
        //                _logger.LogWarning(errorMessage);
        //                return ServiceResponse<MobileMoneyCashTopupDto>.Return404(errorMessage);  // Return 404 response (Not Found)
        //            }

        //            teller = await _tellerRepository.GetTeller(mobileMoneyAccount.TellerId);

        //            decimal previouseBalance = mobileMoneyAccount.PreviousBalance;
        //            balance = previouseBalance + mobileMoneyCashTopup.Amount;

        //            // 10.2. Fetch the member account (receiver account) associated with the member reference and account type
        //            var memberAccount = await _accountRepository.GetAccountByAccountNumber(mobileMoneyCashTopup.AccountNumber);

        //            // 10.3. Verify the integrity of both sender and receiver balances
        //            _accountRepository.VerifyBalanceIntegrity(mobileMoneyAccount);
        //            _accountRepository.VerifyBalanceIntegrity(memberAccount);

        //            // 10.3.1. Check if balances are different and log/override if necessary
        //            if (memberAccount.Balance != mobileMoneyAccount.Balance)
        //            {
        //                string mismatchLogMessage = $"Balance mismatch detected: Member Account ({memberAccount.AccountNumber}) Balance: {memberAccount.Balance}, " +
        //                                            $"Mobile Money Account ({mobileMoneyAccount.AccountNumber}) Balance: {mobileMoneyAccount.Balance}. " +
        //                                            $"Overriding member account balance with mobile money account balance.";

        //                _logger.LogWarning(mismatchLogMessage);

        //                // Override the balance of the member account
        //                memberAccount.Balance = mobileMoneyAccount.Balance;
        //            }

        //            // 10.4. Credit the accounts (both sender and receiver) with the specified amount
        //            _accountRepository.CreditAccount(mobileMoneyAccount, mobileMoneyCashTopup.Amount);
        //            _accountRepository.CreditAccount(memberAccount, mobileMoneyCashTopup.Amount);

        //            // 10.5. Determine the transaction type based on the account type
        //            transactionType = mobileMoneyAccount.AccountType == AccountType.MobileMoneyMTN.ToString()
        //                ? TransactionType.MobileMoney_TopUp.ToString()
        //                : TransactionType.OrangeMoney_TopUp.ToString();

        //            // 10.6. Map and record the transaction in the transaction repository
        //            var transaction = CurrencyNotesMapper.MapTransactionMobileMoneyTopUp(mobileMoneyCashTopup, _userInfoToken, memberAccount, transactionType, accountingDay);
        //            transaction.Account = null;
        //            _transactionRepository.Add(transaction);

        //            _logger.LogInformation($"Mobile Money Cash Top Up transaction for Momo ID '{mobileMoneyCashTopup.MobileMoneyTransactionId}' processed with transaction type '{transactionType}'.");

        //            isApproved = true;
        //        }

        //        // 11. Save changes to the database (commit the transaction)
        //        await _uow.SaveAsync();
        //        _logger.LogInformation($"Changes for Mobile Money Cash Top Up for Momo ID '{mobileMoneyCashTopup.MobileMoneyTransactionId}' saved successfully.");

        //        // 12. Send SMS notification to the customer regarding the top-up
        //        await SendSMS(mobileMoneyCashTopup.MobileMoneyTransactionId,customer.Phone, mobileMoneyCashTopup.Amount, mobileMoneyCashTopup.RequestReference, branch, $"{customer.FirstName} {customer.LastName} ", mobileMoneyCashTopup.OperatorType, balance);
        //        _logger.LogInformation($"SMS sent to customer at phone number '{mobileMoneyCashTopup.PhoneNumber}' regarding the top-up of amount '{mobileMoneyCashTopup.Amount}'.");

        //        //MobileMoneyTupUpCommand
        //        if (isApproved)
        //        {
        //            await MakeAccountingPosting(mobileMoneyCashTopup.Amount, teller, accountingDay, mobileMoneyCashTopup.SourceType, mobileMoneyCashTopup.RequestReference);

        //        }
        //        // 13. Log success and return the response with a 200 OK status
        //        string successMessage = $"{transactionType} was successfully {request.RequestApprovalStatus} for Mobile Money Cash Top Up request with reference '{mobileMoneyCashTopup.MobileMoneyTransactionId}'.";
        //        var response = ServiceResponse<MobileMoneyCashTopupDto>.ReturnResultWith200(_mapper.Map<MobileMoneyCashTopupDto>(mobileMoneyCashTopup), successMessage);
        //        _logger.LogInformation(successMessage);

        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        // 14. Handle any errors that occur during the process
        //        string errorMessage = $"Error occurred while updating Mobile Money Cash Top Up: {e.Message}";
        //        _logger.LogError(errorMessage);
        //        return ServiceResponse<MobileMoneyCashTopupDto>.Return500(errorMessage);  // Return 500 Internal Server Error response
        //    }
        //}

        private async Task<bool> MakeAccountingPosting(decimal amount, Teller teller, DateTime accountingDate, string directionType, string reference)
        {
            // Create a new instance of MobileMoneyTupUpCommand and initialize its properties

            string ChartOfAccountIdFrom = string.Empty;
            string ChartOfAccountIdTo = string.Empty;

            // Determine ChartOfAccountIdFrom and ChartOfAccountIdTo based on directionType and account configuration
            switch (Enum.Parse<DirectionType>(directionType))
            {
                case DirectionType.AuxillaryToBranch:
                    ChartOfAccountIdFrom = teller.FromAuxillaryAccountNumber_A;
                    ChartOfAccountIdTo = teller.ToBranchFloatAccountNumberAuxillary_A;
                    break;
                case DirectionType.HeadOfficeToBranch:
                    ChartOfAccountIdFrom = teller.FromHeadOfficeAccountNumber_B;
                    ChartOfAccountIdTo = teller.ToBranchFloatAccountNumberHeadOffice_B;
                    break;
                case DirectionType.BranchToBranch:
                    ChartOfAccountIdFrom = teller.FromBranchAccountNumber_C;
                    ChartOfAccountIdTo = teller.ToBranchFloatAccountNumberBranch_C;
                    break;
                case DirectionType.BranchToHeadOffice:
                    ChartOfAccountIdFrom = teller.FromBranchFloatAccountNumber_D;
                    ChartOfAccountIdTo = teller.ToHeadOfficeFloatAccountNumber_D;
                    break;
                default:
                    throw new InvalidOperationException("Invalid direction type provided.");
            }

            var addAccountingPostingCommand = new MobileMoneyTupUpCommand
            {
                ChartOfAccountIdFrom = ChartOfAccountIdFrom,
                ChartOfAccountIdTo = ChartOfAccountIdTo,
                Amount = amount,
                Direction = directionType,
                TransactionDate = accountingDate,
                TransactionReferenceId = reference,
            };
            var customerResponse = await _mediator.Send(addAccountingPostingCommand); // Send command to _mediator.

            if (customerResponse.StatusCode == 200)
            {
                return true;
            }
            return false; // Return the created accounting posting command.
        }

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
    

        private async Task SendSMS(string mobilemoneyRef, string telephoneNumber, decimal amount, string reference, BranchDto branch, string name, string accountType, decimal currentBalance)
        {
            // Extract branch name and determine account type for proper message formatting
            string branchName = branch?.name ?? "Branch";
            var accountTypeDisplay = accountType == AccountType.MobileMoneyMTN.ToString() ? "Mobile Money" : "Orange Money";

            // Construct SMS message in a clear and professional manner
            string msg = $"{name} ({telephoneNumber}),\n" +
                         $"From {branchName.ToUpper()}.\nYou have successfully topped up your {accountTypeDisplay} operations account.\nAmount: {BaseUtilities.FormatCurrency(amount)}\n" +
                         $"Current Balance: {BaseUtilities.FormatCurrency(currentBalance)}\n" +
                         $"Internal Reference: {reference}\n" +
                         $"{accountTypeDisplay} Reference: {mobilemoneyRef}\n" +
                         $"Date and Time: {BaseUtilities.UtcToDoualaTime(DateTime.Now)}";
            // Create command to send SMS
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = telephoneNumber
            };

            // Send the SMS via _mediator
            await _mediator.Send(sMSPICallCommand);
        }


    }

}
