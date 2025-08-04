using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.BankingOperation.Commands;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.Data.Enum;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using CBS.AccountManagement.MediatR.Command;
namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountCategory.
    /// </summary>
    public class UpdateDepositNotificationCommandHandler : IRequestHandler<UpdateDepositNotificationCommand, ServiceResponse<bool>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDepositNotifcationRepository _depositNotificationRepository; // Repository for accessing AccountCategory data.
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UpdateDepositNotificationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IBankTransactionRepository _bankTransactionRepository;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;
        private readonly IChartOfAccountRepository _chartOfAccountRepository;
        private readonly IMediator _mediator;
        private readonly PathHelper _pathHelper;
        private readonly string EventCode = "Transit_To_Vault";
        private readonly IMongoUnitOfWork _mongoUnitOfWork;
        /// <summary>
        /// Constructor for initializing the UpdateCashInfusionCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public UpdateDepositNotificationCommandHandler(IDepositNotifcationRepository cashReplenishmentRepository, IAccountClassRepository AccountClassRepository,
            ILogger<UpdateDepositNotificationCommandHandler> logger,
           UserInfoToken userInfoToken,
            IMapper mapper, IUnitOfWork<POSContext> work, IAccountRepository accountRepository, IBankTransactionRepository? bankTransactionRepository, IAccountingEntriesServices? accountingService, IUserNotificationRepository? userNotificationRepository, IAccountingEntryRepository? accountingEntryRepository, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository, IChartOfAccountRepository? chartOfAccountRepository, PathHelper? pathHelper, IMongoUnitOfWork? mongoUnitOfWork, IMediator? mediator)
        {
            _depositNotificationRepository = cashReplenishmentRepository;
            _accountRepository = accountRepository;
            _bankTransactionRepository = bankTransactionRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
            _uow = work;
            _pathHelper = pathHelper;
            _accountingService = accountingService;
            _userNotificationRepository = userNotificationRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _mongoUnitOfWork = mongoUnitOfWork;
        }

        /// <summary>
        /// Handles the AddAccountClassCommand to add a new AccountClass.
        /// </summary>
        /// <param name="request">The AddAccountClassCommand containing AccountClass data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(UpdateDepositNotificationCommand request, CancellationToken cancellationToken)
        {
            List<AccountingEntryDto> accountingEntrieses = new List<AccountingEntryDto>();

            string errorMessage = string.Empty;
            try
            {
                var Entity =await _depositNotificationRepository.FindAsync(request.Id);
                if (Entity==null)
                {

                    errorMessage=($"{Entity.Id} does not exist in the current context");
                 
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateDepositNotificationCommand",

                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 403, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return403(errorMessage);
                }
                else

                {
                    if (Entity.IsDeleted == true)
                    {
                        errorMessage = ($"{Entity.Id} is already deleted");
                   
                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateDepositNotificationCommand",

                            JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 403, _userInfoToken.Token);

                        return ServiceResponse<bool>.Return403(errorMessage);
                    }
                    else
                    {
                        var accounts = await _accountingService.GetCashMovementAccountByEventCode(EventCode, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                        
                        var fromAccount = await _accountRepository.FindAsync(Entity.BankAccountId);
                        BankTransaction bankTransaction = new BankTransaction();
                        bankTransaction.Id = BaseUtilities.GenerateInsuranceUniqueNumber(16, "BTR");
                        bankTransaction.Amount = Entity.Amount;
                        bankTransaction.FromAccountId = Entity.BankAccountId;
                        bankTransaction.BranchId = Entity.BranchId;
                        bankTransaction.BankTransactionReference = request.BankTransactionReference;
                        bankTransaction.Balance = fromAccount.LastBalance.ToString();
                        bankTransaction.ReferenceId = Entity.Id;
                        bankTransaction.ValueDate = request.ValueDate;
                        bankTransaction.TransactionType = "DEPOSIT";
                        bankTransaction.Description = request.Comment;
                        bankTransaction.FileUpload = request.FilePath;
                        bankTransaction.ToAccountId = accounts.BalancingAccount.Id;
                        string reference = $"TTV-{_userInfoToken.BranchCode}-{Entity.Id.Substring(Entity.Id.Length - 6)}";
                        string Naration = $"Cash movement of XFA {Entity.Amount} from {fromAccount.AccountNumberCU}-{fromAccount.AccountName} to {accounts.BalancingAccount.AccountNumberCU}-{accounts.BalancingAccount.AccountName} of|Reference:{reference}  | Event: [Vault_To_Transit]";

                        accountingEntrieses = await _accountingService.CashMovementAsync(Naration, "HeadOffice-" + _userInfoToken.FullName, BaseUtilities.UtcToDoualaTime(DateTime.Now), fromAccount, accounts.BalancingAccount, bankTransaction.Amount,
                            EventCode, reference, Entity.BranchId, true, Entity.BranchId);
                        var entries = _mapper.Map<List<Data.AccountingEntry>>(accountingEntrieses);
                        _accountingEntryRepository.AddRange(entries);
                        _bankTransactionRepository.Add(bankTransaction);
                        Entity.DepositDate = Convert.ToDateTime(request.ValueDate);
                        Entity.Status = CashReplishmentRequestStatus.Completed.ToString();
                        _depositNotificationRepository.Update(Entity);
                        var userModel = _userNotificationRepository.FindBy(xx => xx.ActionId.Equals(Entity.Id)).FirstOrDefault();
                        if (userModel == null)
                        {
                            return ServiceResponse<bool>.Return409($"This BankDeposit opearations with referenceId:{Entity.Id} has already been done ");
                        }
                        _userNotificationRepository.Remove(userModel);

                        if (_accountingService.EvaluateDoubleEntryRule(entries) && entries.Count() > 1)
                        {
                            await _uow.SaveAsync();
                            // Get the MongoDB repository for TransactionTracker
                            var _transactionRecordRepository = _mongoUnitOfWork.GetRepository<BranchToBranchTransferDto>();
                            var denominations = await _transactionRecordRepository.GetByIdAsync(Entity.Id); 
                            var model = new CashMovementDenominationCommand
                            {
                                Reference = bankTransaction.ReferenceId,
                                Amount = bankTransaction.Amount,
                                CurrencyNotesRequest = denominations.CurrencyNotesRequest,
                                BranchId = _userInfoToken.BranchId,
                                OperationType = "CashOut"
                            };
                            await _mediator.Send(model);
                        }
                        else
                        {
                            return ServiceResponse<bool>.Return422("Accounting double entry rule not validated contact administrator");
                        }
                        return ServiceResponse<bool>.ReturnResultWith200(true, "Transaction Completed Successfully");

                    }
                  

                }
               
           

             
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                 errorMessage = $" {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e + errorMessage);
            }
        }

        private string GenerateNaration(Data.Account sourceAccount, Data.Account destinationAccount,  decimal Amount, string ReferenceId)
        {
            return $"BANK CASH OUT of XAF { Amount.ToString("N")} from {sourceAccount.AccountNumberCU}-{sourceAccount.AccountName} to " +
                $"{destinationAccount.AccountNumberCU}-{destinationAccount.AccountName} Ref:{ ReferenceId}";

        }
    }
}