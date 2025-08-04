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

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transfer Transaction.
    /// </summary>
    public class TellerTransferTransactionCommandHandler : IRequestHandler<TellerTransferTransactionCommand, ServiceResponse<TransactionDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<TellerTransferTransactionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the TellerTransferTransactionCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public TellerTransferTransactionCommandHandler(
            IAccountRepository AccountRepository,
            ITransactionRepository TransactionRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<TellerTransferTransactionCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow)
        {
            _AccountRepository = AccountRepository;
            _TransactionRepository = TransactionRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the TellerTransferTransactionCommand to add a new Transfer Transaction.
        /// </summary>
        /// <param name="request">The TellerTransferTransactionCommand containing Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TransactionDto>> Handle(TellerTransferTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the sender account details along with associated product, deposit limits, and withdrawal limits
                var senderAccount = request.primaryTellerAccount;

                // Verify the integrity of the sender account balance
                string senderAccountBalance = senderAccount.Balance.ToString();
                if (!BalanceEncryption.VerifyBalanceIntegrity(senderAccountBalance, senderAccount.EncryptedBalance, senderAccount.AccountNumber))
                {
                    var errorMessage = $"Error occurred while initiating Transaction on sender, Account balance {senderAccount.Balance} has been tampered with";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }

                //check that there is enough to be withdrawn
                if (request.Amount > senderAccount.Balance)
                {
                    var errorMessage = $"Error occurred while initiating a withdrawal with amount: {request.Amount}. Withdrawal amount is greater than account balance";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }

                // Retrieve the receiver account details along with associated product, deposit limits, and withdrawal limits
                var receiverAccount = request.subTellerAccount;

                // Verify the integrity of the receiver account balance
                string receiverAccountBalance = receiverAccount.Balance.ToString();
                if (!BalanceEncryption.VerifyBalanceIntegrity(receiverAccountBalance, receiverAccount.EncryptedBalance, receiverAccount.AccountNumber))
                {
                    var errorMessage = $"Error occurred while initiating Transaction on receiver, Account balance {receiverAccount.Balance} has been tampered with";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }

                // Map the TellerTransferTransactionCommand to a Transaction entity
                var senderTransactionEntity = _mapper.Map<Transaction>(request);

                // Convert UTC to local time and set it in the entity
                senderTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                senderTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
                senderTransactionEntity.TransactionReference = BaseUtilities.GenerateUniqueNumber();
                senderTransactionEntity.TransactionType = TransactionType.TRANSFER.ToString();
                senderTransactionEntity.PreviousBalance = senderAccount.Balance;
                senderTransactionEntity.Balance = senderAccount.Balance - request.Amount;
                senderTransactionEntity.AccountId = senderAccount.Id;
                senderTransactionEntity.AccountNumber = senderAccount.AccountNumber;
                senderTransactionEntity.SenderAccountId = senderAccount.Id;
                senderTransactionEntity.ReceiverAccountId = receiverAccount.Id;
                senderTransactionEntity.Status = TransactionStatus.COMPLETED.ToString();
                senderTransactionEntity.ProductId = senderAccount.ProductId;
                //senderTransactionEntity.CurrencyNotesId = request.CurrencyNotesId;
                senderTransactionEntity.Note ??= $"{TransactionType.TRANSFER.ToString()} transfer of {BaseUtilities.FormatCurrency(request.Amount)} to {receiverAccount.AccountNumber}";

                // Add the new Transaction entity to the repository
                _TransactionRepository.Add(senderTransactionEntity);

                // Initialize UpdateReceiverAccountBalanceCommand
                //UpdateLoanAccountBalanceCommand updateReceiverAccountBalanceCommand = new UpdateLoanAccountBalanceCommand(receiverAccount.Id, request.Amount);

                // Map the TellerTransferTransactionCommand to a Transaction entity
                var receiverTransactionEntity = _mapper.Map<Transaction>(request);

                // Convert UTC to local time and set it in the entity
                receiverTransactionEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                receiverTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
                receiverTransactionEntity.TransactionReference = BaseUtilities.GenerateUniqueNumber();
                receiverTransactionEntity.TransactionType = TransactionType.DEPOSIT.ToString();
                receiverTransactionEntity.PreviousBalance = receiverAccount.Balance;
                receiverTransactionEntity.Balance = receiverAccount.Balance + request.Amount;
                receiverTransactionEntity.AccountId = receiverAccount.Id;
                receiverTransactionEntity.SenderAccountId = senderAccount.Id;
                receiverTransactionEntity.AccountNumber = receiverAccount.AccountNumber;
                receiverTransactionEntity.ReceiverAccountId = receiverAccount.Id;
                receiverTransactionEntity.ProductId = receiverAccount.ProductId;
                //senderTransactionEntity.CurrencyNotesId = request.CurrencyNotesId;
                receiverTransactionEntity.Status = TransactionStatus.COMPLETED.ToString();
                receiverTransactionEntity.Note ??= $"{TransactionType.TRANSFER.ToString()} transfer of {BaseUtilities.FormatCurrency(request.Amount)} to {senderAccount.AccountNumber}";

                // Add the new Transaction entity to the repository
                _TransactionRepository.Add(receiverTransactionEntity);

                senderAccount.PreviousBalance = senderAccount.Balance;
                senderAccount.Balance = senderAccount.Balance - request.Amount;
                senderAccount.EncryptedBalance = BalanceEncryption.Encrypt(senderAccount.Balance.ToString(), senderAccount.AccountNumber);

                // Use the repository to update the existing Account entity
                _AccountRepository.Update(senderAccount);

                // Update the receiver account balance
                receiverAccount.PreviousBalance = receiverAccount.Balance;
                receiverAccount.Balance = receiverAccount.Balance + request.Amount;
                receiverAccount.EncryptedBalance = BalanceEncryption.Encrypt(receiverAccount.Balance.ToString(), receiverAccount.AccountNumber);
                // Use the repository to update the existing Account entity
                _AccountRepository.Update(receiverAccount);

                // Save changes to the database
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<TransactionDto>.Return500();
                }

                // Map the Transaction entity to TransactionDto and return it with a success response
                var transactionDto = _mapper.Map<TransactionDto>(receiverTransactionEntity);
                // Return the successful response with the transaction details
                return ServiceResponse<TransactionDto>.ReturnResultWith200(transactionDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while processing Transfer Transaction: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TransactionDto>.Return500(e);
            }
        }
    }
}
