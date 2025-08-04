using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.ExceptionHandler;
using CBS.AccountManagement.Repository;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.AccountingEntry.Handlers
{
    public class ReverseAccountingEntryCommandHandler : IRequestHandler<ReverseAccountingEntryCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingRuleRepository _accountingRuleRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountingEntriesServices _accountingEntriesServices;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReverseAccountingEntryCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IAccountTypeRepository _accountTypeRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;

        public ReverseAccountingEntryCommandHandler(
            IAccountRepository accountRepository,
            IAccountTypeRepository accountTypeRepository,
            IAccountingRuleRepository accountingRuleRepository,
            IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
            IAccountingEntryRepository accountEntryRepository,
            IConfiguration configuration,
            ILogger<ReverseAccountingEntryCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IAccountingEntriesServices? accountingService)
        {
            _accountRepository = accountRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            _accountingRuleRepository = accountingRuleRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _accountingEntriesServices = accountingService;
            _accountingEntryRepository = accountEntryRepository;
            _accountTypeRepository = accountTypeRepository;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
      
        }

        public async Task<ServiceResponse<bool>> Handle(ReverseAccountingEntryCommand command, CancellationToken cancellationToken)
        {
            List<Data.AccountingEntry> results = new List<Data.AccountingEntry>();
            string errorMessage = "";
            try
            {

                var accountingEntries = _accountingEntryRepository.FindBy(x => x.ReferenceID == command.ReferenceId);
                if (!accountingEntries.Any())
                {
                    errorMessage = $"There is no accounting entry posted with this transaction Ref:{command.ReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.NotFound, LogAction.ReverseAccountingEntryCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                else
                {

                    foreach (var item in accountingEntries)
                    {
                        var accountDataCr = _accountRepository.Find(item.CrAccountId);
                        var accountDataDr = _accountRepository.Find(item.DrAccountId);
                        var accountcr = await _accountingEntriesServices.UpdateAccountAsync(accountDataCr, GetAmount(item), GetAccountBasedOnEntry(item));
                        var accountdr = await _accountingEntriesServices.UpdateAccountAsync(accountDataDr, GetAmount(item), GetAccountBasedOnEntry(item));
                        item.Status = PostingStatus.Posted.ToString();
                        item.IsDeleted = true;
                        item.CreatedBy = _userInfoToken.Id;
                        item.CreatedDate = BaseUtilities.UtcNowToDoualaTime();
                        item.EntryDate = BaseUtilities.UtcNowToDoualaTime();
                        SwapDrCrAmount(item);
                        item.EntryType = item.EntryType.ToString()== AccountOperationType.CREDIT.ToString()?AccountOperationType.DEBIT.ToString() : AccountOperationType.CREDIT.ToString();                   
                        item.Amount = item.Amount > 0 ? -1 * item.Amount : item.Amount;
                        item.Naration = "Reversal Operation done";
                        //item.CrCurrentBalance= accountdd.CurrentBalance;
                        item.Id ="R-"+ item.Id;
                        item.ReferenceID = item.ReferenceID+ "-R";
                        results.Add(item);

                    }
                    _accountingEntryRepository.AddRange(results);
                    await _uow.SaveAsync();

                    errorMessage = $"The reversal of  transaction Ref:{command.ReferenceId} was done successfully.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.ReverseAccountingEntryCommand, LogLevelInfo.Information);
          
                    return ServiceResponse<bool>.ReturnResultWith200(true);
                }

                // Retrieve the accounting rule based on the provided AccountingRuleId


                // Validate the existence of the accounting rule

            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $"Error occurred while performing account postings: {e.Message}";
                _logger.LogError(errorMessage);

                await APICallHelper.AuditLogger(_userInfoToken.Email, "ReverseAccountingEntryCommand", command, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<bool>.Return500(e);
            }
        }

        private   void SwapDrCrAmount(Data.AccountingEntry item)
        {

            var temp = item.CrAmount;
            item.CrAmount = item.DrAmount;
            item.DrAmount = temp;
            var temp1 = item.DrAccountNumber;
            item.DrAccountNumber = item.CrAccountNumber;
            item.CrAccountNumber = temp1;

        }

        private AccountOperationType GetAccountBasedOnEntry(Data.AccountingEntry item)
        {
            return item.CrAmount > item.DrAmount ? AccountOperationType.CREDIT : AccountOperationType.DEBIT;
        }

        private decimal GetAmount(Data.AccountingEntry item)
        {
            return item.CrAmount > item.DrAmount ? item.CrAmount : item.DrAmount;
        }

        private AccountOperations GetAccountBasedOnOperationType(Data.AccountingEntry accountingEntry)
        {

            if (accountingEntry.DrAmount > accountingEntry.CrAmount)
            {
                return new AccountOperations
                {
                    accountId = accountingEntry.AccountId,
                    IsDebit = true
                };
            }
            else
            {
                return new AccountOperations
                {
                    accountId = accountingEntry.AccountId,
                    IsDebit = false
                };
            }
        }

        private class AccountOperations
        {
            public string accountId { get; set; }
            public bool IsDebit { get; set; }
        }
    }

}

