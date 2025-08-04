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
using CBS.TransactionManagement.Helper.Helper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices;
using CBS.TransactionManagement.MediatR;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Queries;
using System.Diagnostics;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transfer Transaction.
    /// </summary>
    public class TransferTransactionCommandHandler : IRequestHandler<TransferTransactionCommand, ServiceResponse<TransactionDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly ICurrencyNotesRepository _CurrencyNotesRepository;
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<TransferTransactionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IDailyTellerRepository _tellerProvisioningAccountRepository;
        private readonly ITellerOperationRepository _tellerOperationRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfigRepository _ConfigRepository;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the TransferTransactionCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public TransferTransactionCommandHandler(
            IAccountRepository AccountRepository,
            ITransactionRepository TransactionRepository,
            IMapper mapper,

            ILogger<TransferTransactionCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IDailyTellerRepository tellerProvisioningAccountRepository = null,
            ITellerOperationRepository tellerOperationRepository = null,
            ITellerRepository tellerRepository = null,
            UserInfoToken userInfoToken = null,
            IConfigRepository configRepository = null,
            IMediator mediator = null,
            ICurrencyNotesRepository currencyNotesRepository = null)
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
            _ConfigRepository = configRepository;
            _mediator = mediator;
            _CurrencyNotesRepository = currencyNotesRepository;
        }

        /// <summary>
        /// Handles the TransferTransactionCommand to add a new Transfer Transaction.
        /// </summary>
        /// <param name="request">The TransferTransactionCommand containing Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>


        public async Task<ServiceResponse<TransactionDto>> Handle(TransferTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Check if the user account serves as a teller today


                bool isInternalTransfer = false;
                string transferType = TransferType.Local.ToString();


                var tellerProvision = await _tellerProvisioningAccountRepository.GetAnActiveSubTellerForTheDate(); //GetTellerProvision(request.TellerId);

                if (tellerProvision == null)
                {
                    return ServiceResponse<TransactionDto>.Return400("Your account does not serve the purpose of a teller today. Contact your primary teller for today's configuration (Open of the day.)");
                }

                // Step 2: Retrieve teller information
                var teller = await GetTeller(tellerProvision.TellerId);

                if (teller == null)
                {
                    return ServiceResponse<TransactionDto>.Return400($"Teller {tellerProvision.TellerId} does not exist.");
                }

                if (teller.MaximumTransferAmount < request.Amount)
                {
                    var errorMessage = $"Teller {teller.Name} does not have sufficient right to transfer this amount. Maximum transfer amount is {BaseUtilities.FormatCurrency(teller.MaximumTransferAmount)} ";
                    await LogErrorAndAudit(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<TransactionDto>.Return403(errorMessage);
                }
                // Step 3: Retrieve sub teller account
                var tellerAccount = await GetTellerAccount(tellerProvision.TellerId);

                if (tellerAccount == null)
                {
                    var errorMessage = $"Teller {teller.Name} does not have an account";
                    await LogErrorAndAudit(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<TransactionDto>.Return404();
                }

                // Step 5: Check if year and day are open
                if (!await AreYearAndDayOpen())
                {
                    return ServiceResponse<TransactionDto>.Return404("Year or day is not open");
                }

                // Step 6: Retrieve the sender account details along with associated product, deposit limits, and withdrawal limits
                var senderAccount = await GetSenderAccount(request.SenderAccountNumber);
                string senderProductName = senderAccount.Product.Name;

                if (senderAccount == null || !senderAccount.Status.Equals(AccountStatus.Active.ToString()))
                {
                    var errorMessage = senderAccount == null
                        ? $"Error occurred while initiating Transaction, unknown customer account {request.SenderAccountNumber}"
                        : $"Error occurred while initiating transfer, Account {request.SenderAccountNumber} is not active";

                    await LogErrorAndAudit(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 403, _userInfoToken.Token);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }
                var getCustomerCommand = new GetCustomerCommandQuery { CustomerID = senderAccount.CustomerId };
                var serviceResponse = await _mediator.Send(getCustomerCommand);
                if (serviceResponse.StatusCode == 200)
                {
                    var senderCustomer = serviceResponse.Data;
                    if (senderCustomer.MembershipApprovalStatus.ToLower() == AccountStatus.Approved.ToString().ToLower())
                    {


                        // Step 7: Verify the integrity of the sender account balance
                        if (!VerifySenderBalanceIntegrity(senderAccount))
                        {
                            return ServiceResponse<TransactionDto>.Return400($"Error occurred while initiating transfer to sender, Account balance {senderAccount.Balance} of sender has been tampered with");
                        }



                        // Step 10: Retrieve the receiver account details along with associated product, deposit limits, and withdrawal limits
                        var receiverAccount = await GetReceiverAccount(request.ReceiverAccountNumber);
                        string reciverProductName = receiverAccount.Product.Name;
                        if (receiverAccount == null)
                        {
                            return ServiceResponse<TransactionDto>.Return400($"Error occurred while initiating transfer, unknown account number {request.ReceiverAccountNumber}");
                        }
                        var getCustomerCommand1 = new GetCustomerCommandQuery { CustomerID = receiverAccount.CustomerId };
                        var receiverCustomerx = await _mediator.Send(getCustomerCommand1);
                        if (receiverCustomerx.StatusCode != 200)
                        {
                            return ServiceResponse<TransactionDto>.Return403($"Error getting reciver customer details");

                        }
                        var receiverCustomer = receiverCustomerx.Data;
                        if (senderAccount.AccountNumber == receiverAccount.AccountNumber)
                        {
                            return ServiceResponse<TransactionDto>.Return400($"Transfer can not be done to the same account number. Sender: {senderAccount.AccountNumber}, Reciver: {receiverAccount.AccountNumber}");

                        }
                        if (senderCustomer.BranchId != receiverCustomer.BranchId)
                        {
                            isInternalTransfer = true;
                            transferType = TransferType.Inter_Branch.ToString();
                        }
                        var transferLimit = GetTransferLimit(senderAccount.Product.TransferParameters, transferType);

                        if (transferLimit == null)
                        {
                            return ServiceResponse<TransactionDto>.Return400("Error occurred while initiating transfer. Transfer limit is not configured");
                        }

                        // Step 9: Check if there is enough fund to be transferred
                        if (!HasEnoughFundsForTransfer(senderAccount, transferLimit, request.Amount))
                        {
                            return ServiceResponse<TransactionDto>.Return400($"Error occurred while initiating a transfer of amount: {request.Amount}. Insufficient fund.");
                        }
                        // Step 11: Verify the integrity of the receiver account balance
                        if (!VerifyReceiverBalanceIntegrity(receiverAccount))
                        {
                            return ServiceResponse<TransactionDto>.Return400($"Error occurred while initiating transfer on receiver account number {receiverAccount.AccountNumber}, Account balance {receiverAccount.Balance} has been tampered with");
                        }

                        // Step 12: Validate the transfer amount lies within transfer limits
                        if (!IsTransferAmountWithinLimits(request.Amount, transferLimit))
                        {
                            return ServiceResponse<TransactionDto>.Return400($"Error occurred while initiating deposit with amount: {request.Amount}. Deposit amount must be between {transferLimit.MinAmount} and {transferLimit.MaxAmount}.");
                        }
                        var charges = TransferCharges(transferLimit, request.Amount);

                        // Step 13: Create and process sender transaction
                        var senderTransactionEntity = await CreateSenderTransactionEntity(request, senderAccount, transferLimit, receiverAccount, charges, string.Empty, senderCustomer.BranchId, receiverCustomer.BranchId, isInternalTransfer, teller.Id);

                        // Step 14: Update sender account properties
                        UpdateSenderAccountProperties(senderAccount, senderTransactionEntity, transferLimit, request.Amount);

                        // Step 15: Update teller account balances
                        UpdateTellerAccountBalances(tellerAccount, senderTransactionEntity, tellerProvision.UserName);

                        // Step 16: Create and process receiver transaction
                        var receiverTransactionEntity = await CreateReceiverTransactionEntity(request, receiverAccount, transferLimit, senderAccount, senderTransactionEntity);

                        // Step 17: Update receiver account properties
                        UpdateReceiverAccountProperties(receiverAccount, receiverTransactionEntity, request);
                        var transactionDto = CurrencyNotesMapper.MapTransactionToDto(senderTransactionEntity, null, _userInfoToken);


                        var tellerOperation = CreateTellerOperation(amount: request.Amount, operationType: OperationType.Credit, tellerAccount: tellerAccount, transactionEntityEntryFee: senderTransactionEntity, eventType: Events.ChargeOfTransfer.ToString(), userID: _userInfoToken.Email);


                        // Step 18: Save changes to the database
                        await _uow.SaveAsync();
                        //Sending SMS
                        await SendTransferSMSToSenderAndReceiver(senderAccount, receiverAccount, senderTransactionEntity, senderCustomer, receiverCustomer);
                        //var apiRequest = MakeAccountingPosting(request.Amount, receiverAccount, senderAccount, senderTransactionEntity, reciverProductName, reciverProductName, isInternalTransfer);
                        //var result = await _mediator.Send(apiRequest);
                        //if (result.StatusCode == 200)
                        //{
                        //    return ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto);
                        //}
                        return ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto, "");



                    }
                    return ServiceResponse<TransactionDto>.Return403($"Customer membership is not approved, Current Status: {senderCustomer.MembershipApprovalStatus}");
                }
                return ServiceResponse<TransactionDto>.Return403(serviceResponse.Message);



            }
            catch (Exception e)
            {
                // Step 21: Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while processing Transfer Transaction: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TransactionDto>.Return500(e);
            }
        }
        private async Task SendTransferSMSToSenderAndReceiver(Account senderAccount, Account receiverAccount, Transaction transaction, CustomerDto senderCustomer, CustomerDto receiverCustomer)
        {
            // Prepare sender SMS
            string senderMsg = $"Dear {senderCustomer.FirstName} {senderCustomer.LastName}, Your transfer of {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} to account number {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)} was successfully processed. Transaction Reference: {transaction.TransactionReference}. Date and Time: {transaction.CreatedDate}. A charge of {BaseUtilities.FormatCurrency(transaction.Fee)} was deducted from your account {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Your current balance is now {BaseUtilities.FormatCurrency(senderAccount.Balance)}. Thank you for banking with us.";
            if (senderCustomer.Language.ToLower() == "french")
            {
                senderMsg = $"Cher {senderCustomer.FirstName} {senderCustomer.LastName}, Votre transfert de {BaseUtilities.FormatCurrency(transaction.OriginalDepositAmount)} sur le compte numéro {BaseUtilities.PartiallyEncryptAccountNumber(receiverAccount.AccountNumber)} a été traité avec succès. Référence de transaction : {transaction.TransactionReference}. Date et heure : {transaction.CreatedDate}. Des frais de {BaseUtilities.FormatCurrency(transaction.Fee)} ont été déduits de votre compte {BaseUtilities.PartiallyEncryptAccountNumber(senderAccount.AccountNumber)}. Votre solde actuel est maintenant de {BaseUtilities.FormatCurrency(senderAccount.Balance)}. Merci de votre confiance en notre banque.";

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

        //private AddTransferPostingCommand MakeAccountingPosting(decimal Amount, Account reciverAccount, Account senderAccount, Transaction senderTransaction, string sendderProductName, string reciverProductName, bool IsInterBranch)
        //{
        //    var addAccountingPostingCommand = new AddTransferPostingCommand
        //    {
        //        FromAccountHolder = senderAccount.AccountName,
        //        FromAccountNumber = senderAccount.AccountNumber,
        //        FromProductId = senderAccount.ProductId,
        //        FromProductName = sendderProductName,
        //        ToAccountHolder = reciverAccount.AccountName,
        //        ToAccountNumber = reciverAccount.AccountNumber,
        //        ToProductId = reciverAccount.ProductId,
        //        ToProductName = reciverProductName,
        //        Naration = senderTransaction.Note,
        //        TransactionReferenceId = senderTransaction.TransactionReference,
        //        ExternalBranchId = senderTransaction.DestinationBrachId,
        //        IsInterBranchTransaction = IsInterBranch,
        //        AmountCollection = new List<TransferAmountCollection>(),

        //    };
        //    if (IsInterBranch)
        //    {
        //        addAccountingPostingCommand.AmountCollection.Add(new TransferAmountCollection
        //        {
        //            Amount = Amount,
        //            IsPrincipal = true,
        //            DirectionOfTransfter = "None",
        //            EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
        //        });
        //        addAccountingPostingCommand.AmountCollection.Add(new TransferAmountCollection
        //        {
        //            Amount = senderTransaction.SourceBranchCommission,
        //            IsPrincipal = false,
        //            DirectionOfTransfter = "Source",
        //            EventAttributeName = OperationEventRubbriqueName.Commission_Account.ToString(),
        //        });
        //        addAccountingPostingCommand.AmountCollection.Add(new TransferAmountCollection
        //        {
        //            Amount = senderTransaction.DestinationBranchCommission,
        //            IsPrincipal = false,
        //            EventAttributeName = OperationEventRubbriqueName.Commission_Account.ToString(),
        //        });
        //    }
        //    else
        //    {
        //        // For regular branch transactions
        //        addAccountingPostingCommand.AmountCollection.Add(new TransferAmountCollection
        //        {
        //            Amount = Amount,
        //            IsPrincipal = true,
        //            EventAttributeName = OperationEventRubbriqueName.Principal_Saving_Account.ToString(),
        //        });
        //        addAccountingPostingCommand.AmountCollection.Add(new TransferAmountCollection
        //        {
        //            Amount = senderTransaction.Fee,
        //            IsPrincipal = false,
        //            EventAttributeName = OperationEventRubbriqueName.Transfer_Fee_Account.ToString(),
        //        });
        //    }

        //    return addAccountingPostingCommand;

        //}
        // Helper methods can be placed here...
        // Helper method to retrieve teller provisioning information
       
        // Helper method to retrieve teller information
        private async Task<Teller> GetTeller(string tellerId)
        {
            return await _tellerRepository
                .FindBy(t => t.Id == tellerId && !t.IsPrimary)
                .FirstOrDefaultAsync();
        }

        // Helper method to retrieve teller account
        private async Task<Account> GetTellerAccount(string tellerId)
        {
            return await _AccountRepository
                .FindBy(t => t.TellerId == tellerId)
                .FirstOrDefaultAsync();
        }

        // Helper method to check if year and day are open
        private async Task<bool> AreYearAndDayOpen()
        {
            var configNames = new List<string> { "IsDayOpen", "IsYearOpen" };
            var configs = await _ConfigRepository.FindBy(a => configNames.Contains(a.Name)).ToListAsync();

            foreach (var config in configs)
            {
                if (config.Name.Equals("IsDayOpen") && !config.Value.Equals("True"))
                {
                    return false;
                }

                if (config.Name.Equals("IsYearOpen") && !config.Value.Equals("True"))
                {
                    return false;
                }
            }

            return true;
        }

        // Helper method to retrieve sender account details
        private async Task<Account> GetSenderAccount(string accountNumber)
        {
            return await _AccountRepository
                .FindBy(a => a.AccountNumber == accountNumber)
                .Include(a => a.Product)
                    .ThenInclude(t => t.TransferParameters)
                .AsNoTracking() // Apply no tracking here
                .FirstOrDefaultAsync();
        }

        private async Task<Account> GetReceiverAccount(string accountNumber)
        {
            return await _AccountRepository
                .FindBy(a => a.AccountNumber == accountNumber)
                .Include(a => a.Product)
                    .ThenInclude(t => t.TransferParameters)
                .AsNoTracking() // Apply no tracking here
                .FirstOrDefaultAsync();
        }


        // Helper method to verify sender account balance integrity
        private bool VerifySenderBalanceIntegrity(Account senderAccount)
        {
            string senderAccountBalance = senderAccount.Balance.ToString();
            return BalanceEncryption.VerifyBalanceIntegrity(senderAccountBalance, senderAccount.EncryptedBalance, senderAccount.AccountNumber);
        }

        // Helper method to get transfer limit based on source details
        private TransferParameter GetTransferLimit(IEnumerable<TransferParameter> transferLimits, string transferType)
        {
            return transferLimits.FirstOrDefault(w => w.TransferType == transferType);
        }

        // Helper method to check if there are enough funds for transfer
        private bool HasEnoughFundsForTransfer(Account account, TransferParameter transferLimit, decimal amount)

        {
            var closeFee = XAFWallet.CalculateCustomerCharges(
                transferLimit.TransferFeeRate,
0, 0, amount);

            decimal totalAmountToTransferWithCharges = amount + TransferCharges(transferLimit, amount);

            return account.Balance - totalAmountToTransferWithCharges >= closeFee;
        }

        private decimal TransferCharges(TransferParameter transferLimit, decimal amount)
        {
            return XAFWallet.CalculateCustomerCharges(
                transferLimit?.TransferFeeRate ?? 0,
0, 0, amount);
        }

        private decimal TotalAmountToTransferWithCharges(TransferParameter transferLimit, decimal amount)
        {
            return amount + TransferCharges(transferLimit, amount);
        }


        // Helper method to retrieve receiver account details


        // Helper method to verify receiver account balance integrity
        private bool VerifyReceiverBalanceIntegrity(Account receiverAccount)
        {
            string receiverAccountBalance = receiverAccount.Balance.ToString();
            return BalanceEncryption.VerifyBalanceIntegrity(receiverAccountBalance, receiverAccount.EncryptedBalance, receiverAccount.AccountNumber);
        }

        // Helper method to check if the transfer amount lies within limits
        private bool IsTransferAmountWithinLimits(decimal amount, TransferParameter transferLimit)
        {
            return DecimalComparer.IsGreaterThanOrEqual(amount, transferLimit.MinAmount) && DecimalComparer.IsLessThanOrEqual(amount, transferLimit.MaxAmount);
        }

        // Helper method to create sender transaction entity
        private async Task<Transaction> CreateSenderTransactionEntity(TransferTransactionCommand request, Account senderAccount, TransferParameter transferLimit, Account receiverAccount, decimal Charges, string currencyid, string sourceBranchId, string destinationBranchId, bool isInternalTransfer, string tellerId)
        {
            decimal totalAmounToTransfer = TotalAmountToTransferWithCharges(transferLimit, request.Amount);
            var senderTransactionEntity = new Transaction();
            senderTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            senderTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            senderTransactionEntity.TransactionReference = isInternalTransfer ? BaseUtilities.GenerateInsuranceUniqueNumber(20, "IBT") : BaseUtilities.GenerateInsuranceUniqueNumber(20, "LT");
            senderTransactionEntity.TransactionType = transferLimit.TransferType;
            senderTransactionEntity.Operation = TransactionType.TRANSFER.ToString();
            senderTransactionEntity.PreviousBalance = senderAccount.Balance;
            senderTransactionEntity.Balance = senderAccount.Balance - totalAmounToTransfer;
            senderTransactionEntity.AccountId = senderAccount.Id;
            senderTransactionEntity.AccountNumber = senderAccount.AccountNumber;
            senderTransactionEntity.SenderAccountId = senderAccount.Id;
            senderTransactionEntity.ReceiverAccountId = receiverAccount.Id;
            senderTransactionEntity.Status = "COMPLETED";
            senderTransactionEntity.ProductId = senderAccount.ProductId;
            senderTransactionEntity.BankId = senderAccount.BankId;
            senderTransactionEntity.BranchId = senderAccount.BranchId;
            senderTransactionEntity.OriginalDepositAmount = request.Amount;
            senderTransactionEntity.Fee = Charges;
            senderTransactionEntity.Tax = 0;
            senderTransactionEntity.Credit = 0;
            senderTransactionEntity.Debit = (request.Amount + Charges);
            senderTransactionEntity.Amount = -(request.Amount + Charges);
            //senderTransactionEntity.CurrencyNotesId = currencyid;
            senderTransactionEntity.OperationType = OperationType.Debit.ToString();
            senderTransactionEntity.FeeType = Events.ChargeOfTransfer.ToString();
            senderTransactionEntity.TellerId = tellerId;
            senderTransactionEntity.SourceType = "BackOffice";
            senderTransactionEntity.SourceBrachId = sourceBranchId;
            senderTransactionEntity.IsInterBrachOperation = isInternalTransfer;
            senderTransactionEntity.DestinationBrachId = destinationBranchId;
            senderTransactionEntity.DestinationBranchCommission = isInternalTransfer ? XAFWallet.CalculateCommission(transferLimit.DestinationBranchOfficeShare, Charges) : 0;
            senderTransactionEntity.SourceBranchCommission = isInternalTransfer ? XAFWallet.CalculateCommission(transferLimit.SourceBrachOfficeShare, Charges) : Charges;
            senderTransactionEntity.Note ??= $"{TransactionType.TRANSFER.ToString()} the sum of {BaseUtilities.FormatCurrency(request.Amount)} to {receiverAccount.AccountNumber}";
            _TransactionRepository.Add(senderTransactionEntity);
            return senderTransactionEntity;
        }

        // Helper method to update sender account properties
        private void UpdateSenderAccountProperties(Account senderAccount, Transaction senderTransactionEntity, TransferParameter transferLimit, decimal amount)
        {
            // Update Account entity properties with values from the request
            senderAccount.PreviousBalance = senderAccount.Balance;
            senderAccount.Balance -= TotalAmountToTransferWithCharges(transferLimit, amount);
            senderAccount.EncryptedBalance = BalanceEncryption.Encrypt(senderAccount.Balance.ToString(), senderAccount.AccountNumber);
            senderAccount.Product = null;
            // Use the repository to update the existing Account entity
            _AccountRepository.Update(senderAccount);
        }

        // Helper method to create receiver transaction entity
        private async Task<Transaction> CreateReceiverTransactionEntity(TransferTransactionCommand request, Account receiverAccount, TransferParameter transferLimit, Account senderAccount, Transaction senderTransaction)
        {
            var receiverTransactionEntity = new Transaction();
            // Convert UTC to local time and set it in the entity
            receiverTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
            receiverTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
            receiverTransactionEntity.TransactionReference = senderTransaction.TransactionReference;
            receiverTransactionEntity.TransactionType = transferLimit.TransferType;
            receiverTransactionEntity.Operation = TransactionType.TRANSFER.ToString();
            receiverTransactionEntity.PreviousBalance = receiverAccount.Balance;
            receiverTransactionEntity.Balance = receiverAccount.Balance + request.Amount;
            receiverTransactionEntity.AccountId = receiverAccount.Id;
            receiverTransactionEntity.SenderAccountId = senderAccount.Id;
            receiverTransactionEntity.AccountNumber = receiverAccount.AccountNumber;
            receiverTransactionEntity.ReceiverAccountId = receiverAccount.Id;
            receiverTransactionEntity.ProductId = receiverAccount.ProductId;
            receiverTransactionEntity.Status = "COMPLETED";
            receiverTransactionEntity.Note ??= $"Recieves a transfer of {BaseUtilities.FormatCurrency(request.Amount)} from Account No: {senderAccount.AccountNumber}";
            receiverTransactionEntity.BankId = receiverAccount.BankId;
            receiverTransactionEntity.BranchId = receiverAccount.BranchId;
            receiverTransactionEntity.OriginalDepositAmount = request.Amount;
            receiverTransactionEntity.Fee = 0;
            receiverTransactionEntity.Tax = 0;
            receiverTransactionEntity.Amount = request.Amount;
            //receiverTransactionEntity.CurrencyNotesId = senderTransaction.CurrencyNotesId;
            receiverTransactionEntity.OperationType = OperationType.Credit.ToString();
            receiverTransactionEntity.FeeType = Events.None.ToString();
            receiverTransactionEntity.TellerId = senderTransaction.TellerId;
            receiverTransactionEntity.Debit = 0;
            receiverTransactionEntity.Credit = request.Amount;
            receiverTransactionEntity.SourceType = senderTransaction.SourceType;
            receiverTransactionEntity.IsInterBrachOperation = senderTransaction.IsInterBrachOperation;
            receiverTransactionEntity.SourceBrachId = senderTransaction.BranchId;
            receiverTransactionEntity.DestinationBrachId = senderTransaction.DestinationBrachId;
            receiverTransactionEntity.DestinationBranchCommission = senderTransaction.DestinationBranchCommission;
            receiverTransactionEntity.SourceBranchCommission = senderTransaction.SourceBranchCommission;
            _TransactionRepository.Add(receiverTransactionEntity);
            return receiverTransactionEntity;
        }

        // Helper method to update receiver account properties
        private void UpdateReceiverAccountProperties(Account receiverAccount, Transaction receiverTransactionEntity, TransferTransactionCommand request)
        {
            // Update Account entity properties with values from the request
            receiverAccount.PreviousBalance = receiverAccount.Balance;
            receiverAccount.Balance += request.Amount;
            receiverAccount.EncryptedBalance = BalanceEncryption.Encrypt(receiverAccount.Balance.ToString(), receiverAccount.AccountNumber);
            receiverAccount.Product = null;
            // Use the repository to update the existing Account entity
            _AccountRepository.Update(receiverAccount);
        }

        // Helper method to log error and audit
        private async Task LogErrorAndAudit(string userEmail, string logAction, TransferTransactionCommand request, string errorMessage, string logLevel, int statusCode, string userToken)
        {
            _logger.LogError(errorMessage);
            await APICallHelper.AuditLogger(userEmail, logAction, request, errorMessage, logLevel, statusCode, userToken);
        }

        private decimal CalculateWithdrawalCharges(decimal feePercentage, decimal amount)
        {
            // Ensure depositPercentage is within the valid range [0, 100]
            feePercentage = Math.Max(0, Math.Min(100, feePercentage));

            // Calculate the deposit charges based on the percentage
            decimal charges = (feePercentage / 100) * amount;

            return charges;
        }



        private decimal CalculateDepositCharges(decimal depositPercentage, decimal amount)
        {
            // Ensure depositPercentage is within the valid range [0, 100]
            depositPercentage = Math.Max(0, Math.Min(100, depositPercentage));

            // Calculate the deposit charges based on the percentage
            decimal depositCharges = (depositPercentage / 100) * amount;

            return depositCharges;
        }




        private void UpdateTellerAccountBalances(Account tellerAccount, Transaction transaction, string userID)
        {

            if (transaction.Fee > 0)
            {
                tellerAccount.PreviousBalance = tellerAccount.Balance;
                tellerAccount.Balance += transaction.Fee;
                var tellerOperationWithFeeCharges = CreateTellerOperation(transaction.Fee, OperationType.Credit, tellerAccount, transaction, Events.ChargeOfTransfer.ToString(), userID);
                _tellerOperationRepository.Add(tellerOperationWithFeeCharges);
                _AccountRepository.Update(tellerAccount);
            }

        }

        private TellerOperation CreateTellerOperation(decimal amount, OperationType operationType, Account tellerAccount, Transaction transactionEntityEntryFee, string eventType, string userID)
        {
            return new TellerOperation
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                AccountID = tellerAccount.Id,
                AccountNumber = tellerAccount.AccountNumber,
                Amount = amount,
                Description = $"{transactionEntityEntryFee.TransactionType} of {BaseUtilities.FormatCurrency(amount)}, Account Number: {tellerAccount.AccountNumber}",
                OperationType = operationType.ToString(),
                BranchId = tellerAccount.BranchId,
                BankId = tellerAccount.BankId,
                CurrentBalance = tellerAccount.Balance,
                Date = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow),
                PreviousBalance = tellerAccount.PreviousBalance,
                TellerID = tellerAccount.TellerId,
                TransactionReference = transactionEntityEntryFee.TransactionReference,
                TransactionType = transactionEntityEntryFee.TransactionType,
                UserID = userID,
                EventName = eventType,
                DestinationBrachId = transactionEntityEntryFee.DestinationBrachId,
                SourceBranchId = transactionEntityEntryFee.SourceBrachId,
                IsInterBranch = transactionEntityEntryFee.IsInterBrachOperation,
                DestinationBranchCommission = transactionEntityEntryFee.DestinationBranchCommission,
                SourceBranchCommission = transactionEntityEntryFee.SourceBranchCommission
            };
        }
    }
}