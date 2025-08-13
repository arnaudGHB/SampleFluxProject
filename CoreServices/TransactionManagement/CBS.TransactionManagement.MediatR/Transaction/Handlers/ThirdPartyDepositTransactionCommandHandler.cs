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
using Microsoft.IdentityModel.Tokens;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new Transaction.
    /// </summary>
    public class ThirdPartyDepositTransactionCommandHandler : IRequestHandler<ThirdPartyDepositTransactionCommand, ServiceResponse<TransactionDto>>
    {
        private readonly ICurrencyNotesRepository _CurrencyNotesRepository;
        private readonly IAccountRepository _AccountRepository;
        private readonly ITransactionRepository _TransactionRepository; // Repository for accessing Transaction data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<ThirdPartyDepositTransactionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the ThirdPartyDepositTransactionCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Transaction data access.</param>
        /// <param name="TransactionRepository">Repository for Transaction data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public ThirdPartyDepositTransactionCommandHandler(
            ICurrencyNotesRepository CurrencyNotesRepository,
            IAccountRepository AccountRepository,
            ITransactionRepository TransactionRepository,
            IMapper mapper,
            ILogger<ThirdPartyDepositTransactionCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow)
        {
            _AccountRepository = AccountRepository;
            _CurrencyNotesRepository = CurrencyNotesRepository;
            _TransactionRepository = TransactionRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the ThirdPartyDepositTransactionCommand to add a new Transaction.
        /// </summary>
        /// <param name="request">The ThirdPartyDepositTransactionCommand containing Transaction data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TransactionDto>> Handle(ThirdPartyDepositTransactionCommand request, CancellationToken cancellationToken)
        {
            Dictionary<XAFDenomination, int> mappedDictionary = CurrencyNotesMapper.MapCurrencyNotesToDictionary(request.CurrencyNotes);

            try
            {
                // Retrieve the account details along with associated product, deposit limits, and withdrawal limits
                var account = await _AccountRepository
                    .FindBy(a => a.AccountNumber == request.AccountNumber)
                    .Include(a => a.Product)
                        .ThenInclude(d => d.DepositLimits)
                    .FirstAsync();

                // Check if the account exists
                if (account == null)
                {
                    var errorMessage = $"Error occurred while initiating Transaction, unknown customer account {request.AccountNumber}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return404(errorMessage);
                }

                // Check if the account is active
                if (!account.Status.Equals("ACTIVE"))
                {
                    var errorMessage = $"Error occurred while initiating Transaction, Account {request.AccountNumber} is not active";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }

                //check that the depositor has provided the required information
                if (request.DepositorIDExpiryDate == null || request.DepositorIDIssueDate == null || request.DepositorIDNumber == null || request.DepositorIDNumber == null)
                {
                    var errorMessage = $"Error occurred while initiating Transaction, please fill in the depositor Id, name, expiry and issue dates";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }

                // Verify the integrity of the account balance
                string accountBalance = account.Balance.ToString();
                if (!BalanceEncryption.VerifyBalanceIntegrity(accountBalance, account.EncryptedBalance, account.AccountNumber))
                {
                    var errorMessage = $"Error occurred while initiating Transaction, Account balance {account.Balance} has been tampered with";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }

                // Initialize UpdateAccountBalanceCommand
                UpdateAccountBalanceCommand UpdateAccountBalanceCommand = null;


                var deposits = account.Product.DepositLimits.ToList();
                var deposit = deposits.FirstOrDefault(dl => dl.DepositType == request.DepositType);

                if (deposit == null)
                {
                    var errorMessage = $"Error occurred while initiating Transaction: undefined deposit type {request.DepositType}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }

                var CurrencyNotesId = "";
                if (request.DepositType.Equals(DepositType.CASH.ToString()))
                {
                    if (mappedDictionary.IsNullOrEmpty())
                    {
                        var errorMessage = $"Error occurred while initiating Transaction, no notes supplied for cash deposit";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<TransactionDto>.Return400(errorMessage);
                    }
                    XAFWallet xafWallet = new XAFWallet();

                    xafWallet.CollectDenominations(mappedDictionary);
                    var totalCashDeposited = xafWallet.CalculateTotalValue();

                    if (request.Amount != totalCashDeposited)
                    {
                        var errorMessage = $"Error occurred while initiating Transaction, collected notes don't correspond to entered amount ";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<TransactionDto>.Return400(errorMessage);
                    }

                    // Map the AddCurrencyNotesCommand to a CurrencyNotes entity
                    var CurrencyNotesEntity = _mapper.Map<CurrencyNotes>(request.CurrencyNotes);
                    // Convert UTC to local time and set it in the entity
                    CurrencyNotesEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                    CurrencyNotesEntity.Id = BaseUtilities.GenerateUniqueNumber();
                    CurrencyNotesId = CurrencyNotesEntity.Id;
                    // Add the Currency notes entity to the repository
                    _CurrencyNotesRepository.Add(CurrencyNotesEntity);
                }

                // Validate the deposit amount against limits
                if (DecimalComparer.IsLessThan(request.Amount, deposit.MinAmount) && DecimalComparer.IsGreaterThan(request.Amount, deposit.MaxAmount))
                {
                    var errorMessage = $"Error occurred while initiating deposit with amount: {request.Amount}. Deposit amount must be greater than {deposit.MaxAmount} and less than {deposit.MinAmount} respectively";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TransactionDto>.Return400(errorMessage);
                }


                // Map the ThirdPartyDepositTransactionCommand to a Transaction entity
                var TransactionEntity = _mapper.Map<Transaction>(request);

                // Convert UTC to local time and set it in the entity
                TransactionEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                TransactionEntity.TransactionType = TransactionType.DEPOSIT.ToString();
                TransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
                TransactionEntity.AccountId = account.Id;
                TransactionEntity.PreviousBalance = account.Balance;
                TransactionEntity.BalanceBroughtForward = account.Balance + request.Amount;
                TransactionEntity.AccountNumber = account.AccountNumber;
                TransactionEntity.Status = "COMPLETED";
                TransactionEntity.SenderAccountId = account.Id;
                TransactionEntity.ReceiverAccountId = account.Id;
                TransactionEntity.ProductId = account.ProductId;
                TransactionEntity.CurrencyNotesId = CurrencyNotesId;
                TransactionEntity.TransactionRef = BaseUtilities.GenerateUniqueNumber();
                TransactionEntity.Note ??= $"Deposit of {BaseUtilities.FormatCurrency(request.Amount)} via {TransactionType.DEPOSIT.ToString()}";

                // Add the new Transaction entity to the repository
                _TransactionRepository.Add(TransactionEntity);
                var updateAmount = request.Amount;
                //calculate charges if any
                if (deposit.FeePercentage != 0)
                {
                    //write a fee transaction deduction a % of the deposited amount
                    var FeeTransactionEntity = TransactionEntity;

                    FeeTransactionEntity.TransactionType = TransactionType.DEPOSIT_FEE.ToString();
                    FeeTransactionEntity.Id = BaseUtilities.GenerateUniqueNumber();
                    FeeTransactionEntity.AccountId = account.Id;
                    FeeTransactionEntity.PreviousBalance = account.Balance;
                    var fee = request.Amount * deposit.FeePercentage;
                    FeeTransactionEntity.BalanceBroughtForward = account.Balance - fee;
                    FeeTransactionEntity.AccountNumber = account.AccountNumber;
                    FeeTransactionEntity.Status = "COMPLETED";
                    FeeTransactionEntity.SenderAccountId = account.Id;
                    FeeTransactionEntity.ReceiverAccountId = account.Id;
                    FeeTransactionEntity.ProductId = account.ProductId;
                    FeeTransactionEntity.TransactionRef = BaseUtilities.GenerateUniqueNumber();
                    FeeTransactionEntity.Note ??= $"Fee of {BaseUtilities.FormatCurrency(fee)} applied to {deposit.DepositType.ToString()} deposit of {BaseUtilities.FormatCurrency(request.Amount)}";

                    _TransactionRepository.Add(FeeTransactionEntity);
                    updateAmount = updateAmount - fee;
                }

                account.PreviousBalance = account.Balance;
                account.Balance = account.Balance + updateAmount;
                account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber);

                // Use the repository to update the existing Account entity
                _AccountRepository.Update(account);

                // Save changes to the database
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<TransactionDto>.Return500();
                }

                // Map the Transaction entity to TransactionDto and return it with a success response
                var TransactionDto = _mapper.Map<TransactionDto>(TransactionEntity);
                return ServiceResponse<TransactionDto>.ReturnResultWith200(TransactionDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Transaction: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TransactionDto>.Return500(e);
            }
        }
    }
}
