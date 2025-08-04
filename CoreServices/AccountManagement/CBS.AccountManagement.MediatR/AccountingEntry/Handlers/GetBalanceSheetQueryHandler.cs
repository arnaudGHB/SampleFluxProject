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
using CBS.AccountManagement.MediatR.AccountingEntry;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Handlers;
using CBS.AccountManagement.Repository;
using CBS.AccountManagement.Repository;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR 
{
   
    public class GetGeneralBalanceSheetQueryHandler : IRequestHandler<GetGeneralBalanceSheetQuery, ServiceResponse<GeneralBalanceSheetDto>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IGeneralBalanceSheetRepository _generalBalanceSheetRepository;
        private readonly IChartOfAccountRepository _chartofaccountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetGeneralBalanceSheetQueryHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        public GetGeneralBalanceSheetQueryHandler(
            IAccountRepository accountRepository,
            IChartOfAccountRepository chartofaccountRepository,
            IAccountingEntryRepository accountingEntryRepository,
            IGeneralBalanceSheetRepository generalBalanceSheetRepository,
            IMapper mapper,
            ILogger<GetGeneralBalanceSheetQueryHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _accountingEntryRepository = accountingEntryRepository;
            _chartofaccountRepository = chartofaccountRepository;
            _accountRepository = accountRepository;
            _generalBalanceSheetRepository = generalBalanceSheetRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        public async Task<ServiceResponse<GeneralBalanceSheetDto>> Handle(GetGeneralBalanceSheetQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var listAccount = _chartofaccountRepository.All.Where(x => x.AccountNumber.Equals("")).ToList();
                var listAccountEntries = _accountingEntryRepository.All.Where(x => x.TransactionDate>=query.FromDate&&x.TransactionDate<=query.ToDate).ToList();
                // Retrieve the accounting rule based on the provided AccountingRuleId
                var generatorGeneralBalanceSheet  = new BalanceSheetGenerator(listAccount, listAccountEntries);
                // Iterate through each accounting rule entry and perform the posting
                var model = generatorGeneralBalanceSheet.GenerateBalanceSheet(query.EntityId, query.EntityType,
                    query.FromDate, query.ToDate);
                var mappedObject = _mapper.Map<GeneralBalanceSheetDto>(model);
                _generalBalanceSheetRepository.Add(model);
                 await _uow.SaveAsync();
                _logger.LogInformation($"Generating 6 column GeneralBalanceSheet for the period of {query.FromDate} to {query.ToDate} for {query.EntityType} with Id {query.EntityId} has been produce successfully");
                // You can return a list of AccountingEntryDto as needed
                return ServiceResponse<GeneralBalanceSheetDto>.ReturnResultWith200(mappedObject);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while Generating 6 column GeneralBalanceSheet for the period of {query.FromDate} to {query.ToDate} for {query.EntityType} with Id {query.EntityId} : {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<GeneralBalanceSheetDto>.Return500(e);
            }
        }


    }

}
