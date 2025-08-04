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
using CBS.AccountManagement.MediatR.AccountingEntry.Commands;
using CBS.AccountManagement.MediatR.Queries;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class CleanAccountAndAccountingEntriesCommandHandler : IRequestHandler<CleanAccountAndAccountingEntriesCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CleanAccountAndAccountingEntriesCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;
        private const  string EventCode = "Vault_To_Cash";

        public CleanAccountAndAccountingEntriesCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<CleanAccountAndAccountingEntriesCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            IChartOfAccountManagementPositionRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, IMediator mediator)
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

public async Task<ServiceResponse<bool>> Handle(CleanAccountAndAccountingEntriesCommand command, CancellationToken cancellationToken)
{
    List<Data.AccountingEntry> listOfEntries = new List<Data.AccountingEntry>();
    List<Data.Account> listOfAccounts = new List<Data.Account>();
    string errorMessage = "";

    try
    {
        // 🚀 Step 1: Validate the input (Ensure branch IDs are provided)
        if (command.BranchIds == null || !command.BranchIds.Any())
        {
            errorMessage = "Invalid request: No branch IDs provided for account cleanup.";
            await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.BadRequest, LogAction.CleanAccountAndAccountingEntries, LogLevelInfo.Warning);
            return ServiceResponse<bool>.Return404(errorMessage);
        }

        // 🚀 Step 2: Fetch all related accounting entries and accounts for the provided branch IDs
        foreach (var branchId in command.BranchIds)
        {
            var entries = _accountingEntryRepository.FindBy(x => x.BranchId == branchId || x.ExternalBranchId == branchId);
            var accounts = _accountRepository.FindBy(x => x.AccountOwnerId == branchId || x.BranchId == branchId);
            listOfEntries.AddRange(entries);
            listOfAccounts.AddRange(accounts);
        }

        // 🚀 Step 3: Perform bulk deletion
        if (listOfEntries.Any())
        {
            await _accountingEntryRepository.RemoveRangeAsync(listOfEntries);
        }

        if (listOfAccounts.Any())
        {
            await _accountRepository.RemoveRangeAsync(listOfAccounts);
        }

        // ✅ Step 4: Successful cleanup operation
        string successMessage = "Successfully cleaned up accounts and accounting entries.";
        await BaseUtilities.LogAndAuditAsync(successMessage, command, HttpStatusCodeEnum.OK, LogAction.VaultInitilization, LogLevelInfo.Information);
        return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
    }
    catch (Exception ex)
    {
        // 🚨 Handle unexpected errors
        errorMessage = $"An error occurred while cleaning accounts and accounting entries: {ex.Message}.";
        await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.VaultInitilization, LogLevelInfo.Critical);
        _logger.LogError(ex, errorMessage);
        return ServiceResponse<bool>.Return500(ex, errorMessage);
    }
}


  

    }


}