using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using CBS.APICaller.Helper.LoginModel.Authenthication;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace CBS.AccountManagement.MediatR.Handlers
{


    public class CashClearingTransferFromBankDepositCommandHandler : IRequestHandler<CashClearingTransferFromBankDepositCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IDepositNotifcationRepository _cashReplenishmentRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<CashClearingTransferFromBankDepositCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IUserNotificationRepository _userNotificationRepository;

        public CashClearingTransferFromBankDepositCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
            IMediator mediator,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<CashClearingTransferFromBankDepositCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            IChartOfAccountRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, IDepositNotifcationRepository? cashReplenishmentRepository, IUserNotificationRepository? userNotificationRepository)
        {
            _accountRepository = accountRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _cashReplenishmentRepository = cashReplenishmentRepository;
            _uow = uow;
            _userNotificationRepository = userNotificationRepository;
            _transactionDataRepository = transactionDataRepository;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            //_chartOfAccountRepository = chartOfAccountRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            ILogger<AccountingEntriesServices> _serviceLogger = new LoggerFactory().CreateLogger<AccountingEntriesServices>();
            _accountingService = accountingEntriesServices;
            _mediator = mediator;
        }

        public async Task<ServiceResponse<bool>> Handle(CashClearingTransferFromBankDepositCommand command, CancellationToken cancellationToken)
        {
            Data.Account destinationAccount;
            Data.Account sourceAccount;
            List<AccountingEntryDto> accountingEntries = new List<AccountingEntryDto>();
            string errorMessage = "";

            try
            {
                // 🚀 Step 1: Validate if transaction already exists
                var transactions = _accountingEntryRepository.FindBy(x => x.ReferenceID == command.ReferenceId && x.IsDeleted == false).ToList();

                // 🚀 Step 2: Fetch the cash replenishment request
                var cashReplenishmentRequest = await _cashReplenishmentRepository.FindAsync(command.ReferenceId);
                if (cashReplenishmentRequest == null)
                {
                    errorMessage = $"No cash replenishment request found with reference: {command.ReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.CashClearingTransferFromBankDepositCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                // 🚀 Step 3: Fetch source and destination accounts
                sourceAccount = await _accountRepository.FindAsync(command.FromAccountId);
                destinationAccount = await _accountRepository.FindAsync(command.ToAccountId);

                // 🚀 Step 4: Generate accounting entries for cash movement
                var entriesDto = await _accountingService.CashMovementAsync(
                    GenerateNaration(sourceAccount, destinationAccount, command),
                    $"{_userInfoToken.FullName} {_userInfoToken.BranchCode}",
                    BaseUtilities.UtcToLocal(),
                    sourceAccount,
                    destinationAccount,
                    command.Amount,
                    "CashClearingCommand",
                    command.ReferenceId,
                    _userInfoToken.BranchId
                );

                accountingEntries.AddRange(entriesDto);

                // 🚀 Step 5: Save entries to repository
                var entries = _mapper.Map<List<Data.AccountingEntry>>(accountingEntries);
                _accountingEntryRepository.AddRange(entries);
                transactions[0].TagedAsPosted(transactions);
                _accountingEntryRepository.UpdateRange(transactions);

                // 🚀 Step 6: Update cash replenishment request status
                cashReplenishmentRequest.Status = CashReplishmentRequestStatus.Completed.ToString();
                _cashReplenishmentRepository.Update(cashReplenishmentRequest);

                // 🚀 Step 7: Remove user notification if it exists
                var userModel = _userNotificationRepository.FindBy(xx => xx.ActionId.Equals(cashReplenishmentRequest.Id)).FirstOrDefault();
                if (userModel == null)
                {
                    errorMessage = $"This cash clearing operation with ReferenceId:{cashReplenishmentRequest.Id} has already been completed.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.CashClearingTransferFromBankDepositCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                _userNotificationRepository.Remove(userModel);

                // 🚀 Step 8: Evaluate double-entry rule
                if (_accountingService.EvaluateDoubleEntryRule(entries) && entries.Count > 1)
                {
                    await _uow.SaveAsync();

                    // 🚀 Step 9: Initiate vault-to-vault transfer
                    var model = new VaultTransferCommand
                    {
                        Reference = command.ReferenceId,
                        FromBranchId = sourceAccount.BranchId,
                        ToBranchId = destinationAccount.BranchId,
                        CurrencyNotesRequest = command.CurrencyNotesRequest,
                        Amount = command.Amount
                    };

                    await _mediator.Send(model);
                    await BaseUtilities.LogAndAuditAsync("Vault-to-Vault cash transfer initiated successfully.", command, HttpStatusCodeEnum.OK, LogAction.CashClearingTransferFromBankDepositCommand, LogLevelInfo.Information);
                }
                else
                {
                    errorMessage = "Accounting double-entry rule not validated. Contact the administrator.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.CashClearingTransferFromBankDepositCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return422(errorMessage);
                }

                // ✅ Step 10: Return success response
                string successMessage = "Cash clearing transfer completed successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, command, HttpStatusCodeEnum.OK, LogAction.CashClearingTransferFromBankDepositCommand, LogLevelInfo.Information);
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // 🚨 Handle unexpected errors
                errorMessage = $"Error occurred during cash clearing transfer: {ex.Message}.";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.CashClearingTransferFromBankDepositCommand, LogLevelInfo.Critical);
                _logger.LogError(ex, errorMessage);
                return ServiceResponse<bool>.Return500(ex, errorMessage);
            }
        }

        //public async Task<ServiceResponse<bool>> Handle(CashClearingTransferFromBankDepositCommand command, CancellationToken cancellationToken)
        //{
        //    Data.Account destinationAccount;
        //    Data.Account sourceAccount;
        //    Data.Account LiaisonAccount;
        //    List<AccountingEntryDto> accountingEntrieses = new List<AccountingEntryDto>();

        //    string errorMessage = "";
        //    try
        //    {
        //        var transactions = _accountingEntryRepository.FindBy(x => x.ReferenceID == command.ReferenceId && x.IsDeleted == false).ToList();

        //        var cashReplenishmentRequest = await _cashReplenishmentRepository.FindAsync(command.ReferenceId);
        //        if (cashReplenishmentRequest ==null)
        //        {
        //            errorMessage = $"There is no cashreplenishement request with reference :{command.ReferenceId} currently pending";
        //            await APICallHelper.AuditLogger(_userInfoToken.Email, "CashClearingTransferFromBankDepositCommand", command, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
        //            return ServiceResponse<bool>.Return409(errorMessage);


        //        }

        //        sourceAccount = await _accountRepository.FindAsync(command.FromAccountId);
        //        destinationAccount = await _accountRepository.FindAsync(command.ToAccountId);

        //     var entriesDto =  await _accountingService.CashMovementAsync(GenerateNaration(sourceAccount, destinationAccount, command), (_userInfoToken.FullName+" "+_userInfoToken.BranchCode), BaseUtilities.UtcToLocal(), sourceAccount, destinationAccount, command.Amount, "CashClearingCommand", command.ReferenceId, _userInfoToken.BranchId);

        //        accountingEntrieses.AddRange(entriesDto);

        //        var entries = _mapper.Map<List<Data.AccountingEntry>>(accountingEntrieses);
        //        _accountingEntryRepository.AddRange(entries);

        //        _accountingEntryRepository.UpdateRange(transactions);
        //        cashReplenishmentRequest.Status = CashReplishmentRequestStatus.Completed.ToString();
        //        _cashReplenishmentRepository.Update(cashReplenishmentRequest);
        //        var userModel = _userNotificationRepository.FindBy(xx => xx.ActionId.Equals(cashReplenishmentRequest.Id)).FirstOrDefault();
        //        if (userModel == null)
        //        {
        //            return ServiceResponse<bool>.Return409($"This cash clearing operations with referenceId:{cashReplenishmentRequest.Id} has already been done ");
        //        }
        //        _userNotificationRepository.Remove(userModel);

        //        if (_accountingService.EvaluateDoubleEntryRule(entries) && entries.Count() > 1)
        //        {
        //            await _uow.SaveAsync();
        //            var model = new VaultTransferCommand
        //            {
        //                Reference = command.ReferenceId,
        //                FromBranchId = sourceAccount.BranchId,
        //                ToBranchId = destinationAccount.BranchId,
        //                CurrencyNotesRequest = command.CurrencyNotesRequest,
        //                Amount = command.Amount
        //            };
        //            var response = _mediator.Send(model);
        //            await APICallHelper.AuditLogger(_userInfoToken.Email, "CashTransferVaultToVault",
        //  command, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
        //        }
        //        else
        //        {
        //            return ServiceResponse<bool>.Return422("Accounting double entry rule not validated contact administrator");
        //        }
        //        return ServiceResponse<bool>.ReturnResultWith200(true, "Transaction Completed Successfully");
        //    }
        //    catch (Exception ex)
        //    {

        //        // Log error and return 500 Internal Server Error response with error message
        //        errorMessage = $" {ex.Message}";
        //        await APICallHelper.AuditLogger(_userInfoToken.Email, "CashClearingTransferCommandFromCashReplenishment",
        //            command, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

        //        _logger.LogError(errorMessage);
        //        return ServiceResponse<bool>.Return500(ex, errorMessage);
        //    }
        //}

        private string GenerateNaration(Data.Account sourceAccount, Data.Account destinationAccount, CashClearingTransferFromBankDepositCommand command)
        {
            return $"BRANCH CASH CLEARING of XAF {command.Amount} from {sourceAccount.AccountNumberCU}-{sourceAccount.AccountName} to " +
              $"{destinationAccount.AccountNumberCU}-{destinationAccount.AccountName} Ref:{command.ReferenceId}";

        }
 
    }
}
