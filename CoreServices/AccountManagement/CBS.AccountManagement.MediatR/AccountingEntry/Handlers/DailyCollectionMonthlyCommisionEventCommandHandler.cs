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

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class DailyCollectionMonthlyCommisionEventCommandHandler : IRequestHandler<DailyCollectionMonthlyCommisionEventCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DailyCollectionMonthlyCommisionEventCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;


        public DailyCollectionMonthlyCommisionEventCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<DailyCollectionMonthlyCommisionEventCommandHandler> logger,
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

        public async Task<ServiceResponse<bool>> Handle(DailyCollectionMonthlyCommisionEventCommand command, CancellationToken cancellationToken)
        {
            List<Data.AccountingEntry> accountingEntries = new List<Data.AccountingEntry>();
            List<Data.AccountingEntryDto> accountingEntriesDto = new List<Data.AccountingEntryDto>();
            string errorMessage = "";

            try
            {
                // 🚀 Step 1: Check if the transaction reference already exists
                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                    errorMessage = $"An entry has already been posted with this transaction Ref: {command.TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.DailyCollectionMonthlyCommisionEventCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                // 🚀 Step 2: Retrieve the Product Account
                var productAccount = await _accountingService.GetAccount(command.ProductId + "@Principal_Saving_Account", _userInfoToken.BranchId, _userInfoToken.BranchCode);
                if (productAccount == null)
                {
                    errorMessage = $"Product account not found for ProductId: {command.ProductId}.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.DailyCollectionMonthlyCommisionEventCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // 🚀 Step 3: Retrieve the Commission Account
                var commissionAccount = await _accountingService.GetAccount(command.ProductId + "@Commission_Account", _userInfoToken.BranchId, _userInfoToken.BranchCode);
                if (commissionAccount == null)
                {
                    errorMessage = $"Commission account not found for ProductId: {command.ProductId}.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.DailyCollectionMonthlyCommisionEventCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // 🚀 Step 4: Generate Accounting Entries
                accountingEntriesDto.AddRange(await _accountingService.CashMovementAsync(
                    command.Naration,
                    command.MemberReference,
                    command.TransactionDate,
                    productAccount,
                    commissionAccount,
                    Convert.ToDecimal(command.Amount),
                    "DailyCollectionMonthlyCommisionEventCommand",
                    command.TransactionReferenceId,
                    _userInfoToken.BranchId
                ));

                accountingEntries = _mapper.Map(accountingEntriesDto, accountingEntries);
                _accountingEntryRepository.AddRange(accountingEntries);

                // 🚀 Step 5: Validate and Persist Double-Entry Rule
                if (_accountingService.EvaluateDoubleEntryRule(accountingEntries) && accountingEntries.Count > 1)
                {
                    await _uow.SaveAsync();
                }
                else
                {
                    errorMessage = "Accounting double entry rule not validated. Contact the administrator.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.DailyCollectionMonthlyCommisionEventCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return422(errorMessage);
                }

                // ✅ Step 6: Return success response
                string successMessage = "Daily collection monthly commission transaction completed successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, command, HttpStatusCodeEnum.OK, LogAction.DailyCollectionMonthlyCommisionEventCommand, LogLevelInfo.Information);
                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // 🚨 Handle unexpected errors
                errorMessage = $"Error during daily collection monthly commission transaction: {ex.Message}.";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.DailyCollectionMonthlyCommisionEventCommand, LogLevelInfo.Critical);
                return ServiceResponse<bool>.Return500(ex, errorMessage);
            }
        }

    }


}