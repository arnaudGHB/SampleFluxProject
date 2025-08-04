using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CBS.AccountManagement.MediatR.Commands.AddTransferEventCommand;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
 

namespace CBS.AccountManagement.MediatR.Handlers
{


    public class AddTransferToNonMemberEventCommandHandler : IRequestHandler<AddTransferToNonMemberEventCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IAccountingRuleRepository _accountingRuleRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly PathHelper _pathHelper;
        private readonly IMediator _mediator;
        private readonly ILogger<AddTransferToNonMemberEventCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private const string productEventCodeSuffix = "@Principal_Saving_Account";


        public AddTransferToNonMemberEventCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
            IMediator mediator,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<AddTransferToNonMemberEventCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            ICashMovementTrackingConfigurationRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, IAccountingRuleRepository? accountingRuleRepository, PathHelper? pathHelper)
        {
            _accountingRuleRepository = accountingRuleRepository;
            _accountRepository = accountRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _transactionDataRepository = transactionDataRepository;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _chartOfAccountRepository = chartOfAccountRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            ILogger<AccountingEntriesServices> _serviceLogger = new LoggerFactory().CreateLogger<AccountingEntriesServices>();
            _accountingService = accountingEntriesServices;
            _mediator = mediator;
            _pathHelper = pathHelper;
        }

        public async Task<ServiceResponse<bool>> Handle(AddTransferToNonMemberEventCommand command, CancellationToken cancellationToken)
        {
         
            List<AccountingEntryDto> accountingEntrieses = new List<AccountingEntryDto>();
                       string errorMessage = "";
            try
            {
                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                      errorMessage = $"An entry has already been posted with this transaction Ref:{command.TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync( errorMessage,command,HttpStatusCodeEnum.Conflict,LogAction.AddTransferToNonMemberEventCommand, LogLevelInfo.Information);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                TransferComponent transferComponent= await GetTransferComponent(command);

                if (transferComponent == null) 
                {
                    errorMessage = "Transfer component not validated contact administrator";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AddTransferToNonMemberEventCommand, LogLevelInfo.Information);

                    return ServiceResponse<bool>.Return422(errorMessage);
                }
                var sourceAccount = transferComponent.CashOnHand.Account;// await _accountingService.UpdateAccountBalanceAsync(command.MemberReference,transferComponent.CashOnHand.Account, transferComponent.CashOnHand.Amount, AccountOperationType.DEBIT, "AddTransferToNonMemberEventCommand");
                var destinationAccount = transferComponent.Transit.Account;// await _accountingService.UpdateAccountBalanceAsync(ProductAccount, ProductEventCode.Amount, AccountOperationType.DEBIT, "MakeAccountPostingCommand");

                accountingEntrieses.AddRange(await _accountingService.CashMovementAsync(transferComponent.CashOnHand.Naration, command.MemberReference,
                     command.TransactionDate, sourceAccount, destinationAccount, transferComponent.CashOnHand.Amount, "AddTransferToNonMemberEventCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
             

                var destinationAccount1 = transferComponent.Transit.Account;// await _accountingService.UpdateAccountBalanceAsync(command.MemberReference, transferComponent.Transit.Account, transferComponent.Transit.Amount, AccountOperationType.CREDIT, "AddTransferToNonMemberEventCommand");
                var sourceAccount1 = transferComponent.CashOnHand.Account;// await _accountingService.UpdateAccountBalanceAsync(ProductAccount, ProductEventCode.Amount, AccountOperationType.DEBIT, "MakeAccountPostingCommand");
                accountingEntrieses.AddRange(await _accountingService.CashMovementAsync(transferComponent.CashOnHand.Naration, command.MemberReference,
                                  command.TransactionDate, sourceAccount1, destinationAccount1, transferComponent.Transit.Amount, "AddTransferToNonMemberEventCommand", command.TransactionReferenceId, _userInfoToken.BranchId));

                var destinationAccount2 = transferComponent.Revenue.Account;//await _accountingService.UpdateAccountBalanceAsync(command.MemberReference, transferComponent.Revenue.Account, transferComponent.Revenue.Amount, AccountOperationType.CREDIT, "AddTransferToNonMemberEventCommand");
                var sourceAccount2 = transferComponent.CashOnHand.Account;// await _accountingService.UpdateAccountBalanceAsync(ProductAccount, ProductEventCode.Amount, AccountOperationType.DEBIT, "MakeAccountPostingCommand");
                accountingEntrieses.AddRange(await _accountingService.CashMovementAsync(transferComponent.CashOnHand.Naration, command.MemberReference,
                                 command.TransactionDate, sourceAccount1, destinationAccount1, transferComponent.Revenue.Amount, "AddTransferToNonMemberEventCommand", command.TransactionReferenceId, _userInfoToken.BranchId));



                var entries = _mapper.Map<List<Data.AccountingEntry>>(accountingEntrieses);
                _accountingEntryRepository.AddRange(entries);

                if (_accountingService.EvaluateDoubleEntryRule(entries)&&entries.Count()>1)
                {
                    await _uow.SaveAsyncWithOutAffectingBranchId();
                }
                else
                {
                    errorMessage = "Accounting double entry rule not validated contact administrator";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.AddTransferToNonMemberEventCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return422(errorMessage);
                }
                errorMessage = $"AddTransferToNonMemberEventCommand Transaction Completed Successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.AddTransferToNonMemberEventCommand, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);
            }
            catch (Exception ex)
            {

                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $" {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.AddTransferToNonMemberEventCommand, LogLevelInfo.Error);


                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(ex, errorMessage);
            }
        }

        private AccountOperationType getbookingdirection(string bookingDirection)
        {
           if (bookingDirection.ToLower() == AccountOperationType.CREDIT.ToString().ToLower()) 
            {
                return AccountOperationType.CREDIT;
            }
            else if (bookingDirection.ToLower() == AccountOperationType.DEBIT.ToString().ToLower())
            {
                return AccountOperationType.DEBIT;
            }
            else
            {
                return AccountOperationType.None;
            }
        }

        private async Task<TransferComponent> GetTransferComponent(AddTransferToNonMemberEventCommand command)
        {
            try
            {
                TransferComponent transferComponent = new TransferComponent();
                var accountingRoles = await _accountingRuleRepository.FindBy(c => c.IsDeleted == false && c.RuleName == command.EventCode).ToListAsync();

                if (command.TransferSource == "teller")
                {
                    // Revenue
                    var entryRuleFromMemberRevenue = await _accountingRuleEntryRepository.FindAsync((command.TransferCollection.Find(x => x.AccountType.ToLower() == "revenue").EventAttribute));
                    transferComponent.Revenue.Account = await _accountingService.GetAccountBasedOnChartOfAccountID(entryRuleFromMemberRevenue.DeterminationAccountId,_userInfoToken.BranchId,_userInfoToken.BranchCode);
                    transferComponent.Revenue.BookingDirection = command.TransferCollection.Find(x => x.AccountType.ToLower() == "revenue").BookingDirection;
                    transferComponent.Revenue.Amount = command.TransferCollection.Find(x => x.AccountType.ToLower() == "revenue").Amount;
                    transferComponent.Revenue.Naration = command.TransferCollection.Find(x => x.AccountType.ToLower() == "revenue").Naration;

                    // Transit
                    var entryRuleFromMemberTransit = await _accountingRuleEntryRepository.FindAsync((command.TransferCollection.Find(x => x.AccountType.ToLower() == "transit").EventAttribute));

                    transferComponent.Transit.Account = await _accountingService.GetAccountBasedOnChartOfAccountID(entryRuleFromMemberTransit.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    transferComponent.Transit.BookingDirection = command.TransferCollection.Find(x => x.AccountType.ToLower() == "transit").BookingDirection;
                    transferComponent.Transit.Amount = command.TransferCollection.Find(x => x.AccountType.ToLower() == "transit").Amount;
                    transferComponent.Transit.Naration = command.TransferCollection.Find(x => x.AccountType.ToLower() == "transit").Naration;

                    // SourceAccount
                    var entryRuleFromMemberTeller = await _accountingRuleEntryRepository.FindAsync((command.TransferCollection.Find(x => x.AccountType.ToLower() == "teller").EventAttribute));
                    transferComponent.CashOnHand.Account = await _accountingService.GetAccountBasedOnChartOfAccountID(entryRuleFromMemberTeller.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    transferComponent.CashOnHand.BookingDirection = command.TransferCollection.Find(x => x.AccountType.ToLower() == "teller").BookingDirection;
                    transferComponent.CashOnHand.Amount = command.TransferCollection.Find(x => x.AccountType.ToLower() == "teller").Amount;
                    transferComponent.CashOnHand.Naration = command.TransferCollection.Find(x => x.AccountType.ToLower() == "teller").Naration;

                }
                else if (command.TransferSource == "member")
                {
                    // Revenue
                    var entryRuleFromMemberRevenue = await _accountingRuleEntryRepository.FindAsync((command.TransferCollection.Find(x => x.AccountType.ToLower() == "revenue").EventAttribute));
                    transferComponent.Revenue.Account = await _accountingService.GetAccountBasedOnChartOfAccountID(entryRuleFromMemberRevenue.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    transferComponent.Revenue.BookingDirection = command.TransferCollection.Find(x => x.AccountType.ToLower() == "revenue").BookingDirection;
                    transferComponent.Revenue.Amount = command.TransferCollection.Find(x => x.AccountType.ToLower() == "revenue").Amount;
                    transferComponent.Revenue.Naration = command.TransferCollection.Find(x => x.AccountType.ToLower() == "revenue").Naration;
                    // Transit
                    var entryRuleFromMemberTransit = await _accountingRuleEntryRepository.FindAsync((command.TransferCollection.Find(x => x.AccountType.ToLower() == "transit").EventAttribute));

                    transferComponent.Transit.Account = await _accountingService.GetAccountBasedOnChartOfAccountID(entryRuleFromMemberTransit.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    transferComponent.Transit.BookingDirection = command.TransferCollection.Find(x => x.AccountType.ToLower() == "transit").BookingDirection;
                    transferComponent.Transit.Amount = command.TransferCollection.Find(x => x.AccountType.ToLower() == "transit").Amount;
                    transferComponent.Transit.Naration = command.TransferCollection.Find(x => x.AccountType.ToLower() == "transit").Naration;

                    // SourceAccount  Member Deposit Account@Principal_Saving_Account
                    var entryRuleFromMemberCashOnHands = _accountingService.GetAccountEntryRuleByProductID(command.ProductId + productEventCodeSuffix);
                    transferComponent.CashOnHand.Account = await _accountingService.GetAccountBasedOnChartOfAccountID(entryRuleFromMemberCashOnHands.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    transferComponent.CashOnHand.BookingDirection = command.TransferCollection.Find(x => x.AccountType.ToLower() == "member").BookingDirection;
                    transferComponent.CashOnHand.Amount = command.TransferCollection.Find(x => x.AccountType.ToLower() == "member").Amount;
                    transferComponent.CashOnHand.Naration = command.TransferCollection.Find(x => x.AccountType.ToLower() == "member").Naration;

                }


                // FromMemberAccountToNonMember



                // FromTellerToNonMember

                //// WithdrawalTransferFund et teller
                //var WithdrawalFromTellerTransit = await _accountingRuleEntryRepository.FindAsync(_pathHelper.WithdrawalTransferFundToNonMemberViaTeller_EventAttributIdForCashInHand);
                //var WithdrawalFromTellerRevenue = await _accountingRuleEntryRepository.FindAsync(_pathHelper.TransferFromMemberToNonMember_EventAttributIdForTransit);



                return transferComponent;
            }
            catch (Exception EX)
            {

                throw(EX);
            }
        }

        private string GetAccountType(string AccountType, TransferCollection? transferCollection)
        {
            throw new NotImplementedException();
        }

        private async Task LogError(string message, Exception ex, AddTransferEventCommand command)
        {
            // Common logic for logging errors and auditing
            _logger.LogError(ex, message);
            // Assuming APICallHelper.AuditLogger is a static utility method for auditing
            await APICallHelper.AuditLogger(_userInfoToken.Email, nameof(AddTransferEventCommand), command, message, "Error", 500, _userInfoToken.Token);
        }

    }

    public class TransferComponent
    {
        public TransferAccountComponent Transit { get; set; } = new TransferAccountComponent();
        public TransferAccountComponent CashOnHand { get; set; } = new TransferAccountComponent();
        public TransferAccountComponent Revenue { get; set; } = new TransferAccountComponent();
    }

    public class TransferAccountComponent
    {
        public Data.Account Account { get; set; }= new Data.Account();  
        public string BookingDirection { get; set; }
        public decimal Amount { get; set; }
        public string Naration { get; set; }
    }
}
