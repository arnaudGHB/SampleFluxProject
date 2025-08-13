using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository.AccountServices;
using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.MediatR.ThirtPartyPayment.Commands;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.MediatR.ThirtPartyPayment.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transfer Transaction.
    /// </summary>
    public class TransferThirdPartyLocalCommandHandler : IRequestHandler<TransferThirdPartyLocalCommand, ServiceResponse<TransferThirdPartyDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly ICurrencyNotesRepository _CurrencyNotesRepository;
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<TransferThirdPartyLocalCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository;
        private readonly ITTPTransferServices _tTPTransferServices;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfigRepository _configRepository;
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the TransferAndriodCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public TransferThirdPartyLocalCommandHandler(
            IAccountRepository AccountRepository,
            ITransactionRepository TransactionRepository,
            IMapper mapper,

            ILogger<TransferThirdPartyLocalCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            ITellerRepository tellerRepository = null,
            UserInfoToken userInfoToken = null,
            IConfigRepository configRepository = null,
            IMediator mediator = null,
            ICurrencyNotesRepository currencyNotesRepository = null,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository = null,
            ITTPTransferServices tTPTransferServices = null,
            IAPIUtilityServicesRepository utilityServicesRepository = null)
        {
            _AccountRepository = AccountRepository;
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _tellerProvisioningAccountRepository = tellerProvisioningAccountRepository;
            _tellerOperationRepository = tellerOperationRepository;
            _tellerRepository = tellerRepository;
            _userInfoToken = userInfoToken;
            _configRepository = configRepository;
            _mediator = mediator;
            _CurrencyNotesRepository = currencyNotesRepository;
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _tTPTransferServices = tTPTransferServices;
            _utilityServicesRepository=utilityServicesRepository;
        }

        /// <summary>
        /// Handles the transfer of funds from one account to another as a third-party transfer.
        /// </summary>
        /// <param name="request">The transfer request details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A service response containing the transaction details.</returns>
        // 1. Define the async handler to manage third-party local transfers, returning a response wrapped in a ServiceResponse.
        public async Task<ServiceResponse<TransferThirdPartyDto>> Handle(TransferThirdPartyLocalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                bool isinterBranch; // 2. Initialize a boolean to check if the transfer is within the same branch.

                // 3. Check if the system configuration for third-party transfers (TTP) is available and retrieve it.
                var config = await _configRepository.GetConfigAsync(OperationSourceType.TTP.ToString());

                // 4. Retrieve the sender's account using their account number and the specific transaction type for TTP transfers.
                var senderAccount = await _AccountRepository.GetAccount(request.SenderAccountNumber, TransactionType.TTP_TRANSFER.ToString());
                _AccountRepository.CheckAppConstraints(senderAccount, TransactionType.TRANSFER, AppType.MobileApp, ServiceType.CMONEY);

                // 5. Retrieve the receiver's account using their account number and the same transaction type.
                var receiverAccount = await _AccountRepository.GetAccount(request.ReceiverAccountNumber, TransactionType.TTP_TRANSFER.ToString());
                string receiverProductName = receiverAccount.Product.Name; // 6. Save the receiver's product name for later use.

                var membersReferences =new List<string> { senderAccount.CustomerId, receiverAccount.CustomerId };

                var customers = await _utilityServicesRepository.GetMultipleMembers(membersReferences, OperationType.Transfer.ToString(), false);

                // 7. Retrieve the customer information for the sender account.
                var sendingCustomerInformation = customers.Where(x=>x.CustomerId==senderAccount.CustomerId).FirstOrDefault();

                // 8. Retrieve the customer information for the receiver account.
                var receivingCustomerInformation = customers.Where(x => x.CustomerId==receiverAccount.CustomerId).FirstOrDefault();

                // 9. Retrieve the branch details for the sender's customer to determine branch information.
                var branch = await GetBranch(sendingCustomerInformation);

                // 10. Retrieve teller information based on application code, branch code, and branch name.
                var teller = await _tellerRepository.GetTellerByCode(request.ApplicationCode, branch.branchCode, branch.name);

                // 11. Verify that the teller has sufficient funds in scope to handle the requested transfer amount.
                await _tellerRepository.CheckIfTransferIsWithinTellerScope(teller.Code, request.Amount);

                // 12. Retrieve the sub teller account associated with the teller to perform the transfer.
                var tellerAccount = await _AccountRepository.RetrieveTellerAccount(teller);

                // 13. Ensure the teller has necessary rights to perform the transfer operation.
                await _tellerRepository.CheckTellerOperationalRights(teller, OperationType.Transfer.ToString(), false);

                // 14. Determine if the transfer is an inter-branch operation by comparing sender and receiver branch IDs.
                isinterBranch = sendingCustomerInformation.BranchId != receivingCustomerInformation.BranchId ? true : false;

                // 15. Generate a unique transaction reference based on whether the transaction is inter-branch or intra-branch.
                string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.TTP_TRANSFER_CMNEY.ToString(), isinterBranch);

                var transaction = new TransactionDto(); // 16. Initialize a TransactionDto object to store transaction details.

                // 17. Map the transfer command details to the transfer object, preparing for the transfer operation.
                var transferTTP = MapToTransferTTP(request);

                // 18. Set the fee type based on the application code: either "CMoney" or "Gav" (default).
                string FeeType = request.ApplicationCode.ToLower() == "cmoney" ? FeeOperationType.CMoney.ToString() : FeeOperationType.Gav.ToString();

                if (FeeType == FeeOperationType.CMoney.ToString())
                {
                    // 19. Perform the third-party transfer for "CMoney" application type.
                    transaction = await _tTPTransferServices.TransferTTP(
                        teller,
                        tellerAccount,
                        transferTTP,
                        reference,
                        senderAccount,
                        receiverAccount,
                        sendingCustomerInformation,
                        receivingCustomerInformation,
                        OperationSourceType.TTP.ToString(),
                        TPPtransferType.Local.ToString(), FeeOperationType.CMoney
                    );
                }
                else
                {
                    // 20. Perform the third-party transfer for "Gav" application type.
                    transaction = await _tTPTransferServices.TransferTTP(
                        teller,
                        tellerAccount,
                        transferTTP,
                        reference,
                        senderAccount,
                        receiverAccount,
                        sendingCustomerInformation,
                        receivingCustomerInformation,
                        OperationSourceType.TTP.ToString(),
                        TPPtransferType.Local.ToString(), FeeOperationType.Gav
                    );
                }

                // 21. Save all pending changes related to the transfer in the database.
                await _uow.SaveAsync();

                // 22. Send SMS notifications to both the sender and receiver with transfer details.
                await SendTransferSMSToSenderAndReceiver(
                    senderAccount,
                    receiverAccount,
                    transaction,
                    sendingCustomerInformation,
                    receivingCustomerInformation
                );

                // 23. Create an accounting posting command for the transaction to ensure proper ledger entries.
                var addTransferPostingCommand = MakeAccountingPosting(
                    request.Amount,
                    receiverAccount,
                    senderAccount,
                    transaction,
                    receiverAccount.AccountName,
                    receiverProductName, BaseUtilities.UtcNowToDoualaTime()
                );

                // 24. Send the accounting posting command through the _mediator to complete the accounting transaction.
                var result = await _mediator.Send(addTransferPostingCommand);

                // 25. Map the completed transaction details to the TransferThirdPartyDto for the response.
                var transferDto = CurrencyNotesMapper.MapTransactionToTransferThirdParty(transaction);

                // 26. Construct a success message based on the accounting posting result.
                string msg = result.Data
                    ? $"Third-party transfer for {request.ApplicationCode} completed successfully."
                    : $"Third-party transfer for {request.ApplicationCode} was completed with an error in accounting posting.";

                // 27. Log and audit the successful transfer, including the success message and action details.
                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.TransferCMONEY, LogLevelInfo.Information);

                // 28. Return a successful ServiceResponse containing the transfer details and message.
                return ServiceResponse<TransferThirdPartyDto>.ReturnResultWith200(transferDto, msg);
            }
            catch (Exception e)
            {
                // 29. Capture and log any exceptions, including the application code and error message.
                var errorMessage = $"Error during third-party transfer for {request.ApplicationCode}: {e.Message}";
                _logger.LogError(errorMessage);

                // 30. Log and audit the error for monitoring purposes.
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.TransferCMONEY, LogLevelInfo.Error);

                // 31. Return a 500 ServiceResponse with the error message indicating the failure.
                return ServiceResponse<TransferThirdPartyDto>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Maps a TransferThirdPartyCommand object to a TransferCMONEY object.
        /// </summary>
        /// <param name="command">The TransferThirdPartyCommand object to map.</param>
        /// <returns>A TransferCMONEY object mapped from the TransferThirdPartyCommand object.</returns>
        public TransferTTP MapToTransferTTP(TransferThirdPartyLocalCommand command)
        {
            return new TransferTTP
            {
                Amount = command.Amount,
                SenderAccountNumber = command.SenderAccountNumber,
                ReceiverAccountNumber = command.ReceiverAccountNumber,
                Note = command.Note,
                AccessCode = command.ApplicationCode,
                SourceType = command.SourceType
            };
        }

        // Method to retrieve customer information
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

            EnsureCustomerMembershipApproved(customer); // Ensure customer membership is approved.

            return customer; // Return customer data.
        }
        // Method to ensure customer membership is approved
        private void EnsureCustomerMembershipApproved(CustomerDto customer)
        {
            // Check if customer's membership is approved
            if (customer.MembershipApprovalStatus.ToLower() != AccountStatus.approved.ToString().ToLower())
            {
                var errorMessage = $"Customer membership is not approved, Current Status: {customer.MembershipApprovalStatus}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
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

        private async Task SendTransferSMSToSenderAndReceiver(Account senderAccount, Account receiverAccount, TransactionDto transaction, CustomerDto senderCustomer, CustomerDto receiverCustomer)
        {
            // Prepare sender SMS
            string senderMsg = $"Dear {senderCustomer.Name} {senderCustomer.Name}, You have successfully transfer the sum of {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} to account number {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)}. Transaction Reference: {transaction.TransactionReference}. Date and Time: {transaction.CreatedDate}. A charge of {BaseUtilities.FormatCurrency(transaction.Fee)} was deducted from your account {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Your current balance is now {BaseUtilities.FormatCurrency(senderAccount.Balance)}. Thank you for banking with us.";
            if (senderCustomer.Language.ToLower() == "french")
            {
                senderMsg = $"Cher {senderCustomer.Name} {senderCustomer.Name}, Vous avez transféré avec succès la somme de {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} sur le compte numéro {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)}. Référence de la transaction : {transaction.TransactionReference}. Date et heure : {transaction.CreatedDate}. Des frais de {BaseUtilities.FormatCurrency(transaction.Fee)} ont été déduits de votre compte {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Votre solde actuel est maintenant de {BaseUtilities.FormatCurrency(senderAccount.Balance)}. Merci de faire confiance à notre banque.\r\n";

            }

            // Prepare receiver SMS
            string receiverMsg = $"Dear {receiverCustomer.Name} {receiverCustomer.Name}, You have received a transfer of {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} in to you account {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)} from account number {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Transaction Reference: {transaction.TransactionReference}. Date and Time: {transaction.CreatedDate}. Your current balance is now {BaseUtilities.FormatCurrency(receiverAccount.Balance)}. Thank you for banking with us.";
            if (receiverCustomer.Language.ToLower() == "french")
            {
                receiverMsg = $"Cher {receiverCustomer.Name} {receiverCustomer.Name}, Vous avez reçu un virement de {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} sur votre compte {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)} en provenance du compte numéro {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Référence de transaction : {transaction.TransactionReference}. Date et heure : {transaction.CreatedDate}. Votre solde actuel est maintenant de {BaseUtilities.FormatCurrency(receiverAccount.Balance)}. Merci de votre confiance en notre banque.";

            }
            // Send SMS to sender
            var senderSmsCommand = new SendSMSPICallCommand
            {
                messageBody = senderMsg,
                recipient = senderCustomer.Phone
            };
            await _mediator.Send(senderSmsCommand);

            // Send SMS to receiver
            var receiverSmsCommand = new SendSMSPICallCommand
            {
                messageBody = receiverMsg,
                recipient = receiverCustomer.Phone
            };
            await _utilityServicesRepository.PushNotification(senderCustomer.CustomerId, PushNotificationTitle.TRANSFER, senderMsg);
            await _utilityServicesRepository.PushNotification(receiverCustomer.CustomerId, PushNotificationTitle.TRANSFER, receiverMsg);
            await _mediator.Send(receiverSmsCommand);
        }


        private AddTransferPostingCommand MakeAccountingPosting(
            decimal amount,
            Account receiverAccount,
            Account senderAccount,
            TransactionDto senderTransaction,
            string senderProductName,
            string receiverProductName,
            DateTime accountingDate)
        {
            var addAccountingPostingCommand = new AddTransferPostingCommand
            {
                FromProductId = senderAccount.ProductId,
                ToProductId = receiverAccount.ProductId,
                TransactionReferenceId = senderTransaction.TransactionReference,
                ExternalBranchId = senderTransaction.DestinationBrachId,
                IsInterBranchTransaction = senderTransaction.IsInterBrachOperation,
                ExternalBranchCode = receiverAccount.BranchCode,
                TransactionDate = accountingDate,
                FromMemberReference = senderAccount.CustomerId
            };

            if (senderTransaction.IsInterBrachOperation)
            {
                // Inter-branch transaction
                AddAmountCollectionItems(addAccountingPostingCommand.AmountCollection, amount, senderTransaction, true);
            }
            else
            {
                // Intra-branch transaction
                AddAmountCollectionItems(addAccountingPostingCommand.AmountCollection, amount, senderTransaction, false);
            }

            return addAccountingPostingCommand;
        }

        private void AddAmountCollectionItems(
            List<AmountCollectionItem> collection,
            decimal principalAmount,
            TransactionDto transaction,
            bool isInterBranch)
        {
            // Add Principal Amount
            collection.Add(new AmountCollectionItem(false, true, false)
            {
                Amount = principalAmount,
                IsPrincipal = true,
                Naration = $"Principal Amount Transfer: [Amount: {BaseUtilities.FormatCurrency(principalAmount)}, Transaction REF: {transaction.TransactionReference}]",
                EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                LiaisonEventName = OperationEventRubbriqueName.Liasson_Account.ToString()
            });

            if (isInterBranch)
            {
                // Source Branch Commission
                collection.Add(new AmountCollectionItem(false, true, false)
                {
                    Amount = transaction.SourceBranchCommission,
                    IsPrincipal = false,
                    Naration = $"Source Branch Commission: [Amount: {BaseUtilities.FormatCurrency(transaction.SourceBranchCommission)}, Transaction REF: {transaction.TransactionReference}]",
                    EventAttributeName = OperationEventRubbriqueName.SourceCMoneyTransferCommission.ToString(),
                    LiaisonEventName = OperationEventRubbriqueName.Liasson_Account.ToString()
                });

                // Destination Branch Commission
                collection.Add(new AmountCollectionItem(false, true, false)
                {
                    Amount = transaction.DestinationBranchCommission,
                    IsPrincipal = false,
                    Naration = $"Destination Branch Commission: [Amount: {BaseUtilities.FormatCurrency(transaction.DestinationBranchCommission)}, Transaction REF: {transaction.TransactionReference}]",
                    EventAttributeName = OperationEventRubbriqueName.ChartOfAccountIdDestinationCMoneyTransferCommission.ToString(),
                    LiaisonEventName = OperationEventRubbriqueName.Liasson_Account.ToString()
                });
            }

            // Common Fees
            AddFeeCollections(collection, transaction.Fee, transaction.TransactionReference);
        }

        private void AddFeeCollections(List<AmountCollectionItem> collection, decimal feeAmount, string transactionReference)
        {
            // Add all fee-related collections
            var feeEvents = new[]
            {
        OperationEventRubbriqueName.CamCCULShareCMoneyTransferCommission.ToString(),
        OperationEventRubbriqueName.FluxAndPTMShareCMoneyTransferCommission.ToString(),
        OperationEventRubbriqueName.HeadOfficeShareCMoneyTransferCommission.ToString(),
        OperationEventRubbriqueName.Transfer_Fee_Account.ToString()
    };

            foreach (var eventCode in feeEvents)
            {
                collection.Add(new AmountCollectionItem(false, true, false)
                {
                    Amount = feeAmount,
                    IsPrincipal = false,
                    Naration = $"Fee Allocation: [Amount: {BaseUtilities.FormatCurrency(feeAmount)}, Event: {eventCode}, Transaction REF: {transactionReference}]",
                    EventAttributeName = eventCode,
                    LiaisonEventName = OperationEventRubbriqueName.Liasson_Account.ToString()
                });
            }
        }


    }
}