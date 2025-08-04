using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.AccountingEntry.Handlers
{
    public class Get6ColumnTrialBalanceQueryHandler : IRequestHandler<Get6ColumnTrialBalanceQuery, ServiceResponse<TrialBalance6ColumnDto>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IIncomeExpenseStatementRepository _incomeExpenseStatementRepository;
        private readonly IChartOfAccountRepository _chartofaccountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Get6ColumnTrialBalanceQueryHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public Get6ColumnTrialBalanceQueryHandler(
            IAccountRepository accountRepository,
            IChartOfAccountRepository chartofaccountRepository,
            IAccountingEntryRepository accountingEntryRepository,
            IIncomeExpenseStatementRepository incomeExpenseStatementRepository,
            IMapper mapper,
            ILogger<Get6ColumnTrialBalanceQueryHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _accountingEntryRepository = accountingEntryRepository;
            _chartofaccountRepository = chartofaccountRepository;
            _accountRepository = accountRepository;

            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        public async Task<ServiceResponse<TrialBalance6ColumnDto>> Handle(Get6ColumnTrialBalanceQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var listAccount = _accountRepository.All.Where(x => x.AccountHolder.Equals("")).ToList();
                var listAccountEntries = _accountingEntryRepository.All.Where(x => x.TransactionDate >= query.FromDate && x.TransactionDate <= query.ToDate).ToList();
                // Retrieve the accounting rule based on the provided AccountingRuleId
                var generatorOFinancialStatementGenerator = new TrialBalanceGenerator(listAccount, listAccountEntries);
                // Iterate through each accounting rule entry and perform the posting
                var model = generatorOFinancialStatementGenerator.GenerateTrialBalance6Column(query);
                var mappedObject = _mapper.Map<TrialBalance6ColumnDto>(model);
                _logger.LogInformation($"Generating 6 column TrailBalance for the period of {query.FromDate} to {query.ToDate} for {query.EntityType} with Id {query.EntityId} has been produce successfully");
                // You can return a list of AccountingEntryDto as needed
                return ServiceResponse<TrialBalance6ColumnDto>.ReturnResultWith200(mappedObject);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while Generating 6 column  TrailBalance  for the period of {query.FromDate} to {query.ToDate} for {query.EntityType} with Id {query.EntityId} : {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TrialBalance6ColumnDto>.Return500(e);
            }
        }


    }
}
