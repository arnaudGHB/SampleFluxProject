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
using CBS.TransactionManagement.Dto.Entity.ThirtPartyPayment;
using CBS.TransactionManagement.Repository.ThirtPartyPayment;
using CBS.TransactionManagement.Data.Entity.ThirtPartyPayment;

namespace CBS.TransactionManagement.MediatR.ThirtPartyPayment.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transfer Transaction.
    /// </summary>
    public class GimacPaymentCommandHandler : IRequestHandler<GimacPaymentCommand, ServiceResponse<GimacPaymentDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly IGimacPaymentRepository _gimacPaymentRepository;
        private readonly ICurrencyNotesRepository _CurrencyNotesRepository;
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GimacPaymentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository;
        private readonly ITTPTransferServices _tTPTransferServices;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfigRepository _configRepository;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the TransferAndriodCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GimacPaymentCommandHandler(
            IAccountRepository AccountRepository,
            ITransactionRepository TransactionRepository,
            IMapper mapper,

            ILogger<GimacPaymentCommandHandler> logger,
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
            IGimacPaymentRepository gimacPaymentRepository = null)
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
            _gimacPaymentRepository = gimacPaymentRepository;
        }

        /// <summary>
        /// Handles the transfer of funds from one account to another as a third-party transfer.
        /// </summary>
        /// <param name="request">The transfer request details.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A service response containing the transaction details.</returns>
        public async Task<ServiceResponse<GimacPaymentDto>> Handle(GimacPaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Check if system configuration is set
                var config = await _configRepository.GetConfigAsync(OperationSourceType.TTP.ToString());

                // 2. Retrieve the sender account for the TTP transfer operation
                var senderAccount = await _AccountRepository.GetAccount(request.AccountNumber, TransactionType.TTP_TRANSFER.ToString());

                // 3. Retrieve customer information for the sender account
                var sendingCustomerInformation = await GetCustomer(senderAccount.CustomerId);

                // 4. Retrieve customer information for the receiver account
                var receivingCustomerInformation = await GetCustomer($"{_userInfoToken.BranchCode}0000{request.ApplicationCode}");

                // 5. Retrieve branch information
                var branch = await GetBranch(sendingCustomerInformation);

                // 6. Retrieve teller information
                var teller = await _tellerRepository.GetTellerByCode(request.ApplicationCode, branch.branchCode, branch.name);

                // 7. Check if operation is within teller scope
                await _tellerRepository.CheckIfTransferIsWithinTellerScope(teller.Code, request.Amount);

                // 8. Retrieve sub teller account
                var tellerAccount = await _AccountRepository.RetrieveTellerAccount(teller);

                // 9. Retrieve the receiver account for the TTP transfer operation
                var receiverAccount = await _AccountRepository.Get3PPDepositAccount(receivingCustomerInformation.CustomerId);
                string receiverProductName = receiverAccount.Product.Name;

                // 10. Check teller operational rights
                await _tellerRepository.CheckTellerOperationalRights(teller, OperationType.Transfer.ToString(), true);

                // 11. Generate transaction reference based on branch type
                string reference = CurrencyNotesMapper.GenerateTransactionReference(_userInfoToken, TransactionType.TTP_TRANSFER.ToString(), false);

                // 12. Map GimacPaymentCommand to TransferCMONEY object
                var transferTTP = MapToTransferTTP(request, receiverAccount.AccountNumber);

                // 13. Perform the TTP transfer operation
                var transaction = await _tTPTransferServices.TransferTTP(teller, tellerAccount, transferTTP, reference, senderAccount, receiverAccount, sendingCustomerInformation, receivingCustomerInformation, OperationSourceType.TTP.ToString(), TPPtransferType.GimacPayment.ToString(), FeeOperationType.Gav);

                // 14. Save GmacPayment
                var gimacPayment = SaveGmacPayment(request, sendingCustomerInformation, receiverAccount, reference);
                await _uow.SaveAsync(); // Save changes to the database

                // 15. Send SMS notification to members
                await SendTransferSMSToSenderAndReceiver(senderAccount, receiverAccount, transaction, sendingCustomerInformation, receivingCustomerInformation);

                // 16. Post accounting entries for transactions
                var addTransferPostingCommand = MakeAccountingPosting(request.Amount, receiverAccount, senderAccount, transaction, receiverAccount.AccountName, receiverProductName, BaseUtilities.UtcNowToDoualaTime());
                var result = await _mediator.Send(addTransferPostingCommand);

                // 17. Map and return result
                var gimacPaymentDto = _mapper.Map<GimacPaymentDto>(gimacPayment);
                string msg = result.Data
                          ? $"Third-party transfer for {request.ApplicationCode} completed successfully."
                          : $"Third-party transfer for {request.ApplicationCode} was completed with an error in accounting posting.";


                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.Transfer3PP, LogLevelInfo.Information);

                return ServiceResponse<GimacPaymentDto>.ReturnResultWith200(gimacPaymentDto, msg);
            }
            catch (Exception e)
            {
                // Capture and log error with detailed information
                var errorMessage = $"Error during third-party transfer for {request.ApplicationCode}: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Transfer3PP, LogLevelInfo.Error);
                return ServiceResponse<GimacPaymentDto>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Maps a GimacPaymentCommand object to a TransferCMONEY object.
        /// </summary>
        /// <param name="command">The GimacPaymentCommand object to map.</param>
        /// <returns>A TransferCMONEY object mapped from the GimacPaymentCommand object.</returns>
        public TransferTTP MapToTransferTTP(GimacPaymentCommand command, string ReceiverAccountNumber)
        {
            return new TransferTTP
            {
                Amount = command.Amount,
                SenderAccountNumber = command.AccountNumber,
                ReceiverAccountNumber = ReceiverAccountNumber,
                Note = command.Note,
                AccessCode = command.ApplicationCode,
                SourceType = command.SourceType
            };
        }

        public GimacPayment SaveGmacPayment(GimacPaymentCommand command, CustomerDto member, Account recivingAccount, string reference)
        {
            var gimacPayment = new GimacPayment
            {
                AccountNumber = command.AccountNumber,
                Amount = command.Amount,
                ApplicationName = command.ApplicationName,
                BeneficialAccountNumber = recivingAccount.AccountNumber,
                Date = BaseUtilities.UtcNowToDoualaTime(),
                ExternalTransactionReference = command.ExternalTransactionReference,
                Id = BaseUtilities.GenerateUniqueNumber(),
                InternalTransactionReference = reference,
                ItemType = command.ItemType,
                MemberName = member.Name,
                MemberReferenceId = member.CustomerId,
                Msisdn = member.Phone,
                Note = command.Note,
                SourceType = command.SourceType,
                Status = Status.Completed.ToString(),
            };
            _gimacPaymentRepository.Add(gimacPayment);
            return gimacPayment;
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
            string senderMsg = $"Dear {senderCustomer.FirstName} {senderCustomer.LastName}, You have successfully transfer the sum of {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} to account number {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)}. Transaction Reference: {transaction.TransactionReference}. Date and Time: {transaction.CreatedDate}. A charge of {BaseUtilities.FormatCurrency(transaction.Fee)} was deducted from your account {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Your current balance is now {BaseUtilities.FormatCurrency(senderAccount.Balance)}. Thank you for banking with us.";
            if (senderCustomer.Language.ToLower() == "french")
            {
                senderMsg = $"Cher {senderCustomer.FirstName} {senderCustomer.LastName}, Vous avez transféré avec succès la somme de {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} sur le compte numéro {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)}. Référence de la transaction : {transaction.TransactionReference}. Date et heure : {transaction.CreatedDate}. Des frais de {BaseUtilities.FormatCurrency(transaction.Fee)} ont été déduits de votre compte {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Votre solde actuel est maintenant de {BaseUtilities.FormatCurrency(senderAccount.Balance)}. Merci de faire confiance à notre banque.\r\n";

            }

            // Prepare receiver SMS
            string receiverMsg = $"Dear {receiverCustomer.FirstName} {receiverCustomer.LastName}, You have received a transfer of {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} in to you account {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)} from account number {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Transaction Reference: {transaction.TransactionReference}. Date and Time: {transaction.CreatedDate}. Your current balance is now {BaseUtilities.FormatCurrency(receiverAccount.Balance)}. Thank you for banking with us.";
            if (receiverCustomer.Language.ToLower() == "french")
            {
                receiverMsg = $"Cher {receiverCustomer.FirstName} {receiverCustomer.LastName}, Vous avez reçu un virement de {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} sur votre compte {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)} en provenance du compte numéro {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Référence de transaction : {transaction.TransactionReference}. Date et heure : {transaction.CreatedDate}. Votre solde actuel est maintenant de {BaseUtilities.FormatCurrency(receiverAccount.Balance)}. Merci de votre confiance en notre banque.";

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
            await _mediator.Send(receiverSmsCommand);
        }


        /// <summary>
        /// Creates and initializes an AddTransferPostingCommand instance based on the provided transaction details.
        /// Handles both inter-branch and intra-branch transfer postings with respective commissions and principal amounts.
        /// </summary>
        /// <param name="Amount">The amount to be transferred.</param>
        /// <param name="reciverAccount">The receiving account details.</param>
        /// <param name="senderAccount">The sending account details.</param>
        /// <param name="senderTransaction">The transaction details from the sender's perspective.</param>
        /// <param name="sendderProductName">The product name associated with the sender's account.</param>
        /// <param name="reciverProductName">The product name associated with the receiver's account.</param>
        /// <param name="accountingDate">The date for the accounting entry.</param>
        /// <returns>An initialized AddTransferPostingCommand object with the transaction details.</returns>
        private AddTransferPostingCommand MakeAccountingPosting(decimal Amount, Account reciverAccount, Account senderAccount, TransactionDto senderTransaction, string sendderProductName, string reciverProductName, DateTime accountingDate)
        {
            var addAccountingPostingCommand = new AddTransferPostingCommand
            {
                FromProductId = senderAccount.ProductId,
                ToProductId = reciverAccount.ProductId,
                FromMemberReference=senderAccount.CustomerId, 
                TransactionReferenceId = senderTransaction.TransactionReference,
                ExternalBranchId = senderTransaction.DestinationBrachId,
                IsInterBranchTransaction = senderTransaction.IsInterBrachOperation,
                ExternalBranchCode = reciverAccount.BranchCode,
                TransactionDate = accountingDate,
                AmountCollection = new List<AmountCollectionItem>()
            };

            // Add principal amount
            AddPrincipalAmount(addAccountingPostingCommand.AmountCollection, Amount, senderTransaction.IsInterBrachOperation, senderAccount,senderTransaction.TransactionReference);

            if (senderTransaction.IsInterBrachOperation)
            {
                AddInterBranchCommissions(addAccountingPostingCommand.AmountCollection, senderTransaction);
            }
            else
            {
                AddIntraBranchCommissions(addAccountingPostingCommand.AmountCollection, senderTransaction);
            }

            return addAccountingPostingCommand;
        }

        /// <summary>
        /// Adds the principal amount to the transaction collection.
        /// </summary>
        /// <param name="amountCollection">The collection to which the principal amount will be added.</param>
        /// <param name="amount">The principal amount of the transaction.</param>
        /// <param name="isInterBranch">Indicates if the transaction is inter-branch.</param>
        private void AddPrincipalAmount(List<AmountCollectionItem> amountCollection, decimal amount, bool isInterBranch, Account SenderAccount, string Reference)
        {
            amountCollection.Add(new AmountCollectionItem(false, true, false)
            {
                Amount = amount,
                IsPrincipal = true,
                Naration = $"GIMAC Pay Transfer: [Amount: {BaseUtilities.FormatCurrency(amount)}, " +
                   $"Member Reference: {SenderAccount.CustomerId}, Name: {SenderAccount.CustomerName}, Transaction REF: {Reference}]",
                EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
                LiaisonEventName = isInterBranch ? OperationEventRubbriqueName.Liasson_Account.ToString() : null
            });
        }

        /// <summary>
        /// Adds inter-branch specific commissions to the transaction collection.
        /// </summary>
        /// <param name="amountCollection">The collection to which the commissions will be added.</param>
        /// <param name="senderTransaction">The transaction details.</param>
        private void AddInterBranchCommissions(List<AmountCollectionItem> amountCollection, TransactionDto senderTransaction)
        {
            amountCollection.AddRange(new[]
            {
        CreateAmountCollectionItem(senderTransaction.SourceBranchCommission, SharingWithPartner.SourceBrachCommission_Account.ToString(),senderTransaction.TransactionReference),
        CreateAmountCollectionItem(senderTransaction.SourceBranchCommission, SharingWithPartner.DestinationBranchCommission_Account.ToString(),senderTransaction.TransactionReference),
        CreateAmountCollectionItem(senderTransaction.FluxAndPTMCommission, OperationEventRubbriqueName.FluxAndPTMShareTransferCommission.ToString(),senderTransaction.TransactionReference),
        CreateAmountCollectionItem(senderTransaction.HeadOfficeCommission, OperationEventRubbriqueName.HeadOfficeShareTransferCommission.ToString(),senderTransaction.TransactionReference),
        CreateAmountCollectionItem(senderTransaction.CamCCULCommission, OperationEventRubbriqueName.CamCCULShareTransferCommission.ToString(),senderTransaction.TransactionReference)
    });
        }

        /// <summary>
        /// Adds intra-branch specific commissions to the transaction collection.
        /// </summary>
        /// <param name="amountCollection">The collection to which the commissions will be added.</param>
        /// <param name="senderTransaction">The transaction details.</param>
        private void AddIntraBranchCommissions(List<AmountCollectionItem> amountCollection, TransactionDto senderTransaction)
        {
            amountCollection.AddRange(new[]
            {
        CreateAmountCollectionItem(senderTransaction.Fee, OperationEventRubbriqueName.Transfer_Fee_Account.ToString(),senderTransaction.TransactionReference),
        CreateAmountCollectionItem(senderTransaction.FluxAndPTMCommission, OperationEventRubbriqueName.FluxAndPTMShareTransferCommission.ToString(),senderTransaction.TransactionReference),
        CreateAmountCollectionItem(senderTransaction.HeadOfficeCommission, OperationEventRubbriqueName.HeadOfficeShareTransferCommission.ToString(),senderTransaction.TransactionReference),
        CreateAmountCollectionItem(senderTransaction.CamCCULCommission, OperationEventRubbriqueName.CamCCULShareTransferCommission.ToString(),senderTransaction.TransactionReference)
    });
        }

        /// <summary>
        /// Creates an AmountCollectionItem instance.
        /// </summary>
        /// <param name="amount">The amount for the collection item.</param>
        /// <param name="eventAttributeName">The event attribute name for the collection item.</param>
        /// <returns>An initialized AmountCollectionItem.</returns>
        private AmountCollectionItem CreateAmountCollectionItem(decimal amount, string eventAttributeName, string Reference)
        {
            return new AmountCollectionItem(false, true, false)
            {
                Amount = amount,
                IsPrincipal = false,
                Naration = $"GIMAC Pay Transfer Commission: [Amount: {BaseUtilities.FormatCurrency(amount)}, " +
                   $"Event: {eventAttributeName}, Transaction REF: {Reference}]", // Set narration with formatted details
                EventAttributeName = eventAttributeName,
                LiaisonEventName = OperationEventRubbriqueName.Liasson_Account.ToString()
            };
        }


    }
}