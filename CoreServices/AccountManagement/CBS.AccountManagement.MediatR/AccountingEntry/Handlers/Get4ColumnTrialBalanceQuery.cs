using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Handlers;
using CBS.AccountManagement.Repository;
using CBS.AccountManagement.Repository;
using Microsoft.Extensions.Logging;
using CBS.AccountManagement.MediatR.Queries;

namespace CBS.AccountManagement.MediatR
{

    public class Get4ColumnTrialBalanceQueryHandler : IRequestHandler<Get4ColumnTrialBalanceQuery, ServiceResponse<TrialBalance4ColumnDto>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IIncomeExpenseStatementRepository _incomeExpenseStatementRepository;
        private readonly IChartOfAccountRepository _chartofaccountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<Get4ColumnTrialBalanceQueryHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public Get4ColumnTrialBalanceQueryHandler(
            IAccountRepository accountRepository,
            IChartOfAccountRepository chartofaccountRepository,
            IAccountingEntryRepository accountingEntryRepository,
            IIncomeExpenseStatementRepository incomeExpenseStatementRepository,
            IMapper mapper,
            ILogger<Get4ColumnTrialBalanceQueryHandler> logger,
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

        public async Task<ServiceResponse<TrialBalance4ColumnDto>> Handle(Get4ColumnTrialBalanceQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var listAccount = _accountRepository.All.Where(x => x.AccountNumber.Equals("")).ToList();
                var listAccountEntries = _accountingEntryRepository.All.Where(x => x.TransactionDate>=query.FromDate&&x.TransactionDate<=query.ToDate).ToList();
                // Retrieve the accounting rule based on the provided AccountingRuleId
                var generatorOFinancialStatementGenerator  = new TrialBalanceGenerator(listAccount, listAccountEntries);
                // Iterate through each accounting rule entry and perform the posting
                var model = generatorOFinancialStatementGenerator.GenerateTrialBalance4Column(query);
                var mappedObject = _mapper.Map<TrialBalance4ColumnDto>(model);
                _logger.LogInformation($"TrailBalance for the period of {query.FromDate} to {query.ToDate} for {query.EntityType} with Id {query.EntityId} has been produce successfully");
                // You can return a list of AccountingEntryDto as needed
                return ServiceResponse<TrialBalance4ColumnDto>.ReturnResultWith200(mappedObject);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while generating TrailBalance for the period of {query.FromDate} to {query.ToDate} for {query.EntityType} with Id {query.EntityId} : {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TrialBalance4ColumnDto>.Return500(e);
            }
        }


    }

}
