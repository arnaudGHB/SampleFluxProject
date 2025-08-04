using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
//using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class UploadAccountingEntryRulesHandler : IRequestHandler<UploadAccountingEntryRules, ServiceResponse<ConfigurationsX>>
    {
        private readonly IAccountingRuleEntryRepository _AccountingRuleEntryRepository; // Repository for accessing Account data.
        private readonly ILogger<UploadAccountingEntryRulesHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IOperationEventRepository _operationEventRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly ICashMovementTrackingConfigurationRepository _chartOfAccountRepository;
        /// <summary>
        /// Constructor for initializing the UpdateAccountCommandHandler.
        /// </summary>
        /// <param name="AccountingRuleEntryRepository">Repository for Account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UploadAccountingEntryRulesHandler(
            IOperationEventRepository operationEventRepository,
            IOperationEventAttributeRepository operationEventAttributeRepository,
            ICashMovementTrackingConfigurationRepository chartOfAccountRepository,
            IAccountingRuleEntryRepository AccountingRuleEntryRepository,
            ILogger<UploadAccountingEntryRulesHandler> logger,
            IMapper mapper,
            UserInfoToken userInfoToken, IUnitOfWork<POSContext> uow = null)
        {
            _AccountingRuleEntryRepository = AccountingRuleEntryRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _operationEventRepository = operationEventRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
           // _uow.Context.Database.ConfigureAwait(false);
            _userInfoToken = userInfoToken;
        }
        //   context.Database.;


        public async Task<ServiceResponse<ConfigurationsX>> Handle(UploadAccountingEntryRules request, CancellationToken cancellationToken)
        {
            try
            {
                var operationEvents = new List<OperationEvent>();
                var operationEventAttributes = new List<OperationEventAttributes>();
                var AccountingRuleEntries = new List<Data.AccountingRuleEntry>();
                var chartOfAccounts = new List<Data.CashMovementTracker>();

                foreach (var item in request.OperationEvents)
                {
                    try
                    {
                        operationEvents.Add(new OperationEvent
                        {
                            OperationEventName = item.OperationEventName,
                            Description = item.Description,
                            CreatedBy = _userInfoToken.Id,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = _userInfoToken.Id,
                            IsDeleted = false,
                            DeletedBy = _userInfoToken.Id,
                            DeletedDate = DateTime.Now,
                            Id = item.Id.Trim()



                        }) ;
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                foreach (var item in request.OperationEventAttributes)
                {
                    try
                    {
                        operationEventAttributes.Add(new OperationEventAttributes
                        {
                            OperationEventId = item.OperationEventId.Trim(),
                            Name = item.Name,
                            CreatedBy = _userInfoToken.Id,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = _userInfoToken.Id,
                            IsDeleted = false,
                            Id = item.Id.Trim()



                        });
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                foreach (var item in request.AccountingRuleEntries)
                {
                    try
                    {
                        AccountingRuleEntries.Add(new Data.AccountingRuleEntry
                        {
                            //OperationEventId = item.OperationEventId.Trim(),
                            AccountingRuleEntryName = getAccountingRuleEntryName(item.OperationEventId, operationEvents),
                            //CreditAccountId = await getAccountIdAsync(item.CrAccountId, _chartOfAccountRepository),
                            //DebitAccountId = await getAccountIdAsync(item.DrAccountId, _chartOfAccountRepository),
                            OperationEventAttributeId = item.OperationEventAttributeId.Trim(),
                            IsPrimaryEntry = item.IsPrimaryEntry,
                            EventCode= item.EventCode,
                            CreatedBy = _userInfoToken.Id,
                            CreatedDate = DateTime.Now,
                            ModifiedDate = DateTime.Now,
                            ModifiedBy = _userInfoToken.Id,
                            IsDeleted = false,
                            BookingDirection = "xxxxxxxxxxxxxxxxxxxxxx",
                            Id = Guid.NewGuid().ToString()
                        }) ;
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                _operationEventRepository.AddRange(operationEvents);
                _operationEventAttributeRepository.AddRange(operationEventAttributes);  
                _AccountingRuleEntryRepository.AddRange(AccountingRuleEntries);
              

               await _uow.SaveAsync();
                var models = new ConfigurationsX
                {
                    OperationEvents = request.OperationEvents,
                    OperationEventAttributes = request.OperationEventAttributes,
                    AccountingRuleEntries = request.AccountingRuleEntries
                };
                return ServiceResponse<ConfigurationsX>.ReturnResultWith200(models);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task<string> getAccountIdAsync(string accountNumber, IChartOfAccountRepository chartOfAccounts)
        {
            var model = chartOfAccounts.All.Where(f=>f.AccountNumber.Trim().Equals(accountNumber)).ToList();
            if (model.Count()==0)
            {
                var newAccount = new Data.ChartOfAccount
                {
                    AccountNumber = accountNumber,
                    LabelEn = "",
                    LabelFr = "",
                    CreatedBy = _userInfoToken.Id,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = _userInfoToken.Id,
                    ModifiedDate = DateTime.Now,
                    IsDeleted = false,
                    DeletedBy = _userInfoToken.Id,
                    DeletedDate = DateTime.Now,
                    Id = Guid.NewGuid().ToString(),
                    ParentAccountNumber = accountNumber.Substring(0, accountNumber.Length - 1),
                    ParentAccountId = await getAccountIdAsync(accountNumber.Substring(0, accountNumber.Length - 1), chartOfAccounts)
                };          
                chartOfAccounts.Add(newAccount);
                await _uow.SaveAsync();
                return newAccount.Id;
            }
            else
            {
                return model.FirstOrDefault().Id;
            }


        }

        private string getAccountingRuleEntryName(string operationEventId, List<OperationEvent> operationEvents)
        {
            var model = operationEvents.Find(x => x.Id.Equals(operationEventId));
            if (model == null)
                return "no accounting_entry_name";
            return model.OperationEventName;
        }
    }
}
