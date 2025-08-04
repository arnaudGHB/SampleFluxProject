using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using CBS.AccountManagement.Data.Dto.AccountingEntryDto;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.ExceptionHandler;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Data;
using CBS.AccountManagement.MediatR.Queries;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class MakeRemittanceCommandHandler : IRequestHandler<MakeRemittanceCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MakeRemittanceCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;

        //  Commissions on treasury or InterBankOperations
        public CustomerInfo _customerInfo { get; set; } = new CustomerInfo();
        public MakeRemittanceCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<MakeRemittanceCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            ICashMovementTrackingConfigurationRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, IMediator mediator)
        {
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
            _mediator = mediator;
            _accountingService = accountingEntriesServices;
        }

        // Initialize the required accounts for a transaction
        private async Task<TransactionAccounts> InitializeAccounts(MakeRemittanceCommand command)
        {
            var tellerAccount = await _accountingService.GetTellerAccount(command);
            var productEvent = command.AmountCollection.FirstOrDefault(x => x.AmountType=="MAIN_AMOUNT");
            var ProductAccountBO = await GetProductAccount(command, productEvent,false);
            var ProductAccountHO = await GetProductAccount(command, productEvent, true);

            return new TransactionAccounts(tellerAccount, ProductAccountBO, ProductAccountHO);
        }
 
        public async Task<ServiceResponse<bool>> Handle(MakeRemittanceCommand command, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            List<Data.AccountingEntry> AccountingEntries = new List<Data.AccountingEntry>();
              List<Data.AccountingEntryDto> AccountingEntriesDto = new List<Data.AccountingEntryDto>();

            try
            {
                // Check if transaction reference already exists
                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                    errorMessage = $"An entry has already been posted with this transaction Ref: {command.TransactionReferenceId}.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.AccountingPosting, LogLevelInfo.Information);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                // Retrieve accounts
                TransactionAccounts transactionAccounts = await InitializeAccounts(command);

                if (OperationEventAttributeTypes.deposit.ToString() == command.OperationType.ToLower().ToString())
                {
                    errorMessage = "MakeRemittanceCommand CASH IN operation.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.MakeRemittanceCommandCASHIN, LogLevelInfo.Information);
                    //  Handle Branch entries
                    var BOPrincipal = command.GetPrincipalAmount("MAIN_AMOUNT", LEVEL_OF_Execution.BRANCH_OFFICE.ToString());
                    var BOCommision = command.GetPrincipalAmount("COMMISSION_AMOUNT", LEVEL_OF_Execution.BRANCH_OFFICE.ToString());
                    var ThirdParty = BOPrincipal; //command.GetPrincipalAmount("TRANSFER_AMOUNT", LEVEL_OF_Execution.BRANCH_OFFICE.ToString());

                    var BOCommisionAccount = await _accountingService.GetAccount(BOCommision.GetOperationEventCode(command.ProductId), _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    //Cash movement from teller to transit
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(BOPrincipal.Naration, command.MemberReference, command.TransactionDate,
                    transactionAccounts.TellerAccount, transactionAccounts.ProductAccountBO, BOPrincipal.Amount, "MakeRemittanceCommand", command.TransactionReferenceId, _userInfoToken.BranchId, true, _userInfoToken.BranchId));
                    //Cash movement from teller to commission
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(BOCommision.Naration, command.MemberReference, command.TransactionDate,
                    transactionAccounts.TellerAccount, BOCommisionAccount, BOCommision.Amount, "MakeRemittanceCommand", command.TransactionReferenceId, _userInfoToken.BranchId, true, _userInfoToken.BranchId));
                    //Cash movement from Transit A to transit HO
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ThirdParty.Naration, command.MemberReference, command.TransactionDate,
                    transactionAccounts.ProductAccountBO, transactionAccounts.ProductAccountHO, ThirdParty.Amount, "MakeRemittanceCommand", command.TransactionReferenceId, _userInfoToken.BranchId, true, command.HeadOfficeBranchId));
                    //  Handle Head Office entries
                    var HOPrincipal = command.GetPrincipalAmount("MAIN_AMOUNT", LEVEL_OF_Execution.HEAD_OFFICE.ToString());
                    var HOCommision = command.GetPrincipalAmount("COMMISSION_AMOUNT", LEVEL_OF_Execution.HEAD_OFFICE.ToString());
                    //var ThirdPartyHO = command.GetPrincipalAmount("TRANSFER_AMOUNT", LEVEL_OF_Execution.BRANCH_OFFICE.ToString());
                    var HOCommisionAccount = await _accountingService.GetAccount(HOCommision.GetOperationEventCode(command.ProductId), command.HeadOfficeBranchId, command.HeadOfficeBranchCode);
                    //Cash movement from Transit HO to transit A
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(HOPrincipal.Naration, command.MemberReference, command.TransactionDate,
                    transactionAccounts.ProductAccountHO, transactionAccounts.ProductAccountBO, HOPrincipal.Amount, "MakeRemittanceCommand", command.TransactionReferenceId, command.HeadOfficeBranchId, true, command.HeadOfficeBranchId));
                    //Cash movement from Transit A to Commision HO
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(HOCommision.Naration, command.MemberReference, command.TransactionDate,
                    transactionAccounts.ProductAccountHO, HOCommisionAccount, HOCommision.Amount, "MakeRemittanceCommand", command.TransactionReferenceId, command.HeadOfficeBranchId, false ,_userInfoToken.BranchId));
              
                }
                else if (OperationEventAttributeTypes.withdrawal.ToString() == command.OperationType.ToLower().ToString())
                {
                    errorMessage = "MakeRemittanceCommand CASH OUT operation.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.MakeRemittanceCommandCASHOUT, LogLevelInfo.Information);
                    // Handle RECIEVING Branch Office entries
                    //  Handle Head Office entries
                    var HOPrincipal = command.GetPrincipalAmount("MAIN_AMOUNT", LEVEL_OF_Execution.HEAD_OFFICE.ToString());

                    var BOPrincipal = command.GetPrincipalAmount("MAIN_AMOUNT", LEVEL_OF_Execution.BRANCH_OFFICE.ToString());
                    var BOCommision = command.GetPrincipalAmount("COMMISSION_AMOUNT", LEVEL_OF_Execution.BRANCH_OFFICE.ToString());

                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(HOPrincipal.Naration, command.MemberReference, command.TransactionDate,
                    transactionAccounts.ProductAccountHO, transactionAccounts.ProductAccountBO, HOPrincipal.Amount, "MakeRemittanceCommand", command.TransactionReferenceId, command.HeadOfficeBranchId, true, command.HeadOfficeBranchId));
          
                    var ThirdParty = BOPrincipal; //command.GetPrincipalAmount("TRANSFER_AMOUNT", LEVEL_OF_Execution.BRANCH_OFFICE.ToString());

                    var BOCommisionAccount = await _accountingService.GetAccount(BOCommision.GetOperationEventCode(command.ProductId), _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    //Cash movement from Transit A to transit HO
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(ThirdParty.Naration, command.MemberReference, command.TransactionDate,
                 transactionAccounts.ProductAccountBO,transactionAccounts.TellerAccount,  ThirdParty.Amount, "MakeRemittanceCommand", command.TransactionReferenceId, _userInfoToken.BranchId, true, command.HeadOfficeBranchId));

                    //Cash movement from teller to transit
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(BOPrincipal.Naration, command.MemberReference, command.TransactionDate,
                    transactionAccounts.ProductAccountBO, transactionAccounts.TellerAccount, BOPrincipal.Amount, "MakeRemittanceCommand", command.TransactionReferenceId, _userInfoToken.BranchId, true, _userInfoToken.BranchId));
                    //Cash movement from teller to commission
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(BOCommision.Naration, command.MemberReference, command.TransactionDate,
                     transactionAccounts.ProductAccountBO, BOCommisionAccount, BOCommision.Amount, "MakeRemittanceCommand", command.TransactionReferenceId, _userInfoToken.BranchId, true, _userInfoToken.BranchId));

                }
                else
                {
                    errorMessage = "MakeRemittanceCommand REVERSAL operation.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.MakeRemittanceCommandCASHOUT, LogLevelInfo.Information);
                    // Handle RECIEVING Branch Office entries
                    //  Handle Head Office entries
              

                    var BOPrincipal = command.GetPrincipalAmount("MAIN_AMOUNT", LEVEL_OF_Execution.BRANCH_OFFICE.ToString());
                    var HOPrincipal = command.GetPrincipalAmount("MAIN_AMOUNT", LEVEL_OF_Execution.HEAD_OFFICE.ToString());
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(BOPrincipal.Naration, command.MemberReference, command.TransactionDate,
                   transactionAccounts.ProductAccountHO, transactionAccounts.ProductAccountBO, BOPrincipal.Amount, "MakeRemittanceCommand", command.TransactionReferenceId, command.HeadOfficeBranchId, true, command.HeadOfficeBranchId));


                    //Cash movement from teller to transit
                    AccountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(BOPrincipal.Naration, command.MemberReference, command.TransactionDate,
                    transactionAccounts.ProductAccountBO, transactionAccounts.TellerAccount, BOPrincipal.Amount, "MakeRemittanceCommand", command.TransactionReferenceId, _userInfoToken.BranchId, true, _userInfoToken.BranchId));
         
                }

                AccountingEntries = _mapper.Map<List<Data.AccountingEntry>>(AccountingEntriesDto);
                _accountingEntryRepository.AddRange(AccountingEntries);
                // Validate double-entry rule
                if (_accountingService.EvaluateDoubleEntryRule(AccountingEntries))
                {
                    await _uow.SaveAsyncWithOutAffectingBranchId();
                }
                else
                {
                    errorMessage = "Accounting double entry rule not validated. Contact administrator.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.MakeRemittanceCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return422(errorMessage);
                }

                errorMessage = "Transaction Completed Successfully";
                _logger.LogInformation(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.MakeRemittanceCommand, LogLevelInfo.Information);

                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = $"Transaction failed: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.MakeRemittanceCommand, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return403(false, errorMessage);
            }
        }

        private async Task<Data.Account> GetProductAccount(MakeRemittanceCommand command, CollectionAmount productEvent, bool IsHeadOffice)
        {
            var (branchId, branchCode) = IsHeadOffice
                ? (command.HeadOfficeBranchId, command.HeadOfficeBranchCode)
                : (_userInfoToken.BranchId, _userInfoToken.BranchCode);

            return await _accountingService.GetProductAccount(productEvent, command.ProductId, branchId, branchCode);
        }

        #region Nested Types
        // Record structure to group transaction accounts
        public record TransactionAccounts(Data.Account TellerAccount, Data.Account ProductAccountBO, Data.Account ProductAccountHO);
        #endregion



    }

}