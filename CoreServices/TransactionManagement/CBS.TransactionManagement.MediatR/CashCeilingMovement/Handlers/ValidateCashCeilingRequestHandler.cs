using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.CashCeilingMovement.Commands;
using CBS.TransactionManagement.Repository.CashCeilingMovement;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Repository.VaultP;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using DocumentFormat.OpenXml.ExtendedProperties;

namespace CBS.TransactionManagement.CashCeilingMovement.Handlers
{
    public class ValidateCashCeilingRequestHandler : IRequestHandler<ValidationCashCeilingRequestCommand, ServiceResponse<bool>>
    {
        private readonly ICashCeilingRequestRepository _cashCeilingRequestRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IVaultRepository _vaultRepository;
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvisioningHistoryRepository;
        private readonly IMediator _mediator;
        private readonly ITellerOperationRepository _tellerOperationRepository;

        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<ValidateCashCeilingRequestHandler> _logger;

        public ValidateCashCeilingRequestHandler(
            ICashCeilingRequestRepository cashCeilingRequestRepository,
            ITellerRepository tellerRepository,
            IAccountRepository accountRepository,
            IVaultRepository vaultRepository,
            IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository,
            IAccountingDayRepository accountingDayRepository,
            IUnitOfWork<TransactionContext> uow,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<ValidateCashCeilingRequestHandler> logger,
            ISubTellerProvisioningHistoryRepository subTellerProvisioningHistoryRepository,
            IMediator mediator,
            ITellerOperationRepository tellerOperationRepository)
        {
            _cashCeilingRequestRepository = cashCeilingRequestRepository;
            _tellerRepository = tellerRepository;
            _accountRepository = accountRepository;
            _vaultRepository = vaultRepository;
            _primaryTellerProvisioningHistoryRepository = primaryTellerProvisioningHistoryRepository;
            _accountingDayRepository = accountingDayRepository;
            _uow = uow;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _subTellerProvisioningHistoryRepository=subTellerProvisioningHistoryRepository;
            _mediator=mediator;
            _tellerOperationRepository=tellerOperationRepository;
        }

        /// <summary>
        /// Validates a cash ceiling request, ensuring correctness and performing necessary cash handling operations.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(ValidationCashCeilingRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the current accounting date for the branch.
                var accountingDate = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Step 2: Retrieve the existing cash ceiling request from the repository.
                var existingRequest = await _cashCeilingRequestRepository.FindAsync(request.Id);

                // Step 3: Validate the existence of the cash ceiling request.
                // If the request is not found, log an error, audit the operation, and return a 404 response.
                if (existingRequest == null)
                {
                    string errorMessage = $"Cash ceiling request with ID {request.Id} was not found.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.CashCeillingCashOutValidation, LogLevelInfo.Warning, request.Id);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Step 4: Check if the cash ceiling request is already approved.
                // If the request is already approved, log a warning and return a 403 response.
                if (existingRequest.ApprovedStatus == Status.Approved.ToString())
                {
                    string errorMessage = $"Request {existingRequest.TransactionReference} has already been validated.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.CashCeillingCashOutValidation, LogLevelInfo.Warning, request.Id);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }

                // Step 5: Retrieve associated teller information.
                // This fetches the details of the teller responsible for the cash ceiling request.
                var teller = await _tellerRepository.GetTeller(existingRequest.TellerId);

                // Step 6: Update the cash ceiling request with approval details.
                // Map the incoming request data to the existing entity, updating key properties.
                _mapper.Map(request, existingRequest);
                existingRequest.ApprovedBy = _userInfoToken.FullName ?? _userInfoToken.Email; // Set the approver's name or email.
                existingRequest.ApprovedDate = BaseUtilities.UtcNowToDoualaTime(); // Set the approval date in the appropriate timezone.

                // Step 7: Handle cash operations based on the request type.
                // Check if the request's status indicates it's approved for processing.
                if (existingRequest.ApprovedStatus == Status.Approved.ToString())
                {
                    // Step 7a: Retrieve the sub-teller's account balance.
                    var accountBalanceSubTeller = await _accountRepository.RetrieveTellerAccount(teller);

                    // Step 7b: Verify the integrity of the account balance.
                    // Ensures that there are no discrepancies in the account balance.
                    _accountRepository.VerifyBalanceIntegrity(accountBalanceSubTeller);

                    // Step 8: Process cash transfer if the request type is "Cash_To_Vault".
                    if (existingRequest.RequestType == EventCode.Cash_To_Vault.ToString())
                    {
                        // Step 8a: Check for sufficient funds in the sub-teller account.
                        if (accountBalanceSubTeller.Balance < existingRequest.CashoutRequestAmount)
                        {
                            string errorMessage = $"Insufficient funds: The primary teller account does not have enough balance to process the transfer.";
                            await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.CashCeillingCashOutValidation, LogLevelInfo.Warning, existingRequest.TransactionReference);
                            return ServiceResponse<bool>.Return400(errorMessage);
                        }

                        // Step 8b: Retrieve the primary teller's provisioning history.
                        var primaryProvisioning = await _primaryTellerProvisioningHistoryRepository.GetLastUpdatedRecordForPrimaryProvisioningHistory(teller.Id);

                        // Step 8c: Validate the existence of the primary teller's provisioning history.
                        if (primaryProvisioning == null)
                        {
                            string errorMessage = $"No primary teller provisioning history found for Teller ID: {teller.Id}.";
                            await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.CashCeillingCashOutValidation, LogLevelInfo.Warning, existingRequest.TransactionReference);
                            return ServiceResponse<bool>.Return404(errorMessage);
                        }

                        // Step 8d: Perform cash-out operation from the primary teller.
                        _primaryTellerProvisioningHistoryRepository.CashOutByDinomination(existingRequest.CashoutRequestAmount, request.CurrencyNote, primaryProvisioning);

                        // Step 8e: Perform cash-in operation into the vault.
                        _vaultRepository.CashInByDenomination(existingRequest.CashoutRequestAmount, request.CurrencyNote, existingRequest.BranchId, existingRequest.TransactionReference, EventCode.Cash_To_Vault.ToString(), 0, 0, "N/A", true);

                        // Step 8f: Debit the sub-teller's account.
                        _accountRepository.DebitAccount(accountBalanceSubTeller, existingRequest.CashoutRequestAmount);
                    }
                    else
                    {
                        // Step 9: Process cash transfer from sub-teller to primary teller.
                        var subTellerProvisioning = await _subTellerProvisioningHistoryRepository.GetLastUpdatedRecordForSubTellerProvisioningHistory(teller.Id);

                        // Step 9a: Validate the existence of the sub-teller's provisioning history.
                        if (subTellerProvisioning == null)
                        {
                            string errorMessage = $"No sub-teller provisioning history found for Teller ID: {teller.Id}.";
                            await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.CashCeillingCashOutValidation, LogLevelInfo.Warning, existingRequest.TransactionReference);
                            return ServiceResponse<bool>.Return404(errorMessage);
                        }

                        // Step 9b: Check if the sub-teller has sufficient cash on hand.
                        if (subTellerProvisioning.CashAtHand < existingRequest.CashoutRequestAmount)
                        {
                            string errorMessage = $"Insufficient funds in sub-teller: The requested amount exceeds the available cash on hand.";
                            await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.CashCeillingCashOutValidation, LogLevelInfo.Warning, existingRequest.TransactionReference);
                            return ServiceResponse<bool>.Return400(errorMessage);
                        }

                        // Step 9c: Retrieve primary teller details and their account balance.
                        var primaryTeller = await _tellerRepository.GetPrimaryTeller(existingRequest.BranchId);
                        var accountBalancePrimary = await _accountRepository.RetrieveTellerAccount(primaryTeller);
                        _accountRepository.VerifyBalanceIntegrity(accountBalancePrimary);

                        // Step 9d: Perform cash-in operation for the primary teller.
                        _primaryTellerProvisioningHistoryRepository.CashInByDinomination(existingRequest.CashoutRequestAmount, request.CurrencyNote, primaryTeller.Id, accountingDate, accountBalanceSubTeller.OpenningOfDayReference);

                        // Step 9e: Update account balances for both tellers.
                        _accountRepository.CreditAccount(accountBalancePrimary, existingRequest.CashoutRequestAmount);
                        _accountRepository.DebitAccount(accountBalanceSubTeller, existingRequest.CashoutRequestAmount);

                        // Step 9f: Perform cash-out operation for the sub-teller.
                        _subTellerProvisioningHistoryRepository.CashOutByDinomination(existingRequest.CashoutRequestAmount, request.CurrencyNote, subTellerProvisioning);

                        // Step 9g: Create a teller operation record for audit purposes.
                        CreateTellerOperation(existingRequest.CashoutRequestAmount, OperationType.Debit, subTellerProvisioning.UserIdInChargeOfThisTeller, accountBalanceSubTeller, existingRequest.TransactionReference, EventCode.Cashout_To_Primary_Teller.ToString(), EventCode.Cashout_To_Primary_Teller.ToString(), accountingDate);
                    }
                }

                // Step 10: Update the cash ceiling request entity and save changes to the database.
                _cashCeilingRequestRepository.Update(existingRequest);
                await _uow.SaveAsync();

                // Step 11: Perform accounting posting for cash transfers to the vault.
                if (existingRequest.RequestType == EventCode.Cash_To_Vault.ToString() && request.ApprovedStatus == Status.Approved.ToString())
                {
                    await AccountinPostingCashToVault(existingRequest.TransactionReference, existingRequest.CashoutRequestAmount);
                }

                // Step 12: Log the successful validation of the request.
                string successMessage = $"Cash ceiling request for teller {teller.Name} successfully validated by {_userInfoToken.FullName}.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.CashCeillingCashOutValidation, LogLevelInfo.Information, existingRequest.TransactionReference);

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Step 13: Handle unexpected errors by logging and auditing the exception details.
                string errorMessage = $"An error occurred during validation: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CashCeillingCashOutValidation, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(ex);
            }
        }

        /// <summary>
        /// Posts accounting for cash transferred to the vault.
        /// </summary>
        /// <param name="reference">Transaction reference for the posting.</param>
        /// <param name="amount">Amount being posted to the vault.</param>
        /// <returns>Returns the service response indicating the success or failure of the posting.</returns>
        private async Task<ServiceResponse<bool>> AccountinPostingCashToVault(string reference, decimal amount)
        {
            // Construct a detailed narration for the cash replenishment posting.
            string narration = $"Cash Transfer From 57 To Vault for {_userInfoToken.BranchName} | Reference: {reference} | Amount: {BaseUtilities.FormatCurrency(amount)} | Event: Cash to Vault";

            // Create command to perform the cash replenishment posting.
            var addOpenAndCloseOfDay = new InternalAutoPostingEventCommand(
                EventCode.Cash_To_Vault.ToString(),
                amount,
                reference,
                narration
            );

            // Send command to _mediator.
            var addOpenAndCloseOfDayResponse = await _mediator.Send(addOpenAndCloseOfDay);

            // Handle failure case with detailed logging and exception.
            if (addOpenAndCloseOfDayResponse.StatusCode != 200)
            {
                var errorMessage = $"Cash Transfer From 57 To Vault for {_userInfoToken.BranchName} failed for Reference: {reference} | Amount: {BaseUtilities.FormatCurrency(amount)} | StatusCode: {addOpenAndCloseOfDayResponse.StatusCode}.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Log success.
            _logger.LogInformation($"Cash Transfer From 57 To Vault for {_userInfoToken.BranchName} succeeded | Reference: {reference} | Amount: {BaseUtilities.FormatCurrency(amount)}.");

            // Return response.
            return addOpenAndCloseOfDayResponse;
        }
        private void CreateTellerOperation(decimal amount, OperationType operationType, string UserNameTeller, Account tellerAccount, string TransactionReference, string TransactionType, string eventType, DateTime accountingDate)
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
                OpenOfDayReference = tellerAccount.OpenningOfDayReference,
                CurrentBalance = tellerAccount.Balance,
                Date = accountingDate,
                AccountingDate = accountingDate,
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = tellerAccount.TellerId,
                TransactionReference = TransactionReference,
                TransactionType = TransactionType,
                UserID = UserNameTeller,
                Description = $"{TransactionType} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                ReferenceId = ReferenceNumberGenerator.GenerateReferenceNumber(_userInfoToken.BranchCode),
                CustomerId = "N/A",
                MemberAccountNumber = "N/A",
                EventName = eventType,
                DestinationBrachId = tellerAccount.BranchId,
                SourceBranchId = tellerAccount.BranchId,
                IsInterBranch = false,
                DestinationBranchCommission = 0,
                SourceBranchCommission = 0,
                IsCashOperation = true,
                MemberName = UserNameTeller,

            };
            _tellerOperationRepository.Add(tellerOperation);
        }
    }
}
