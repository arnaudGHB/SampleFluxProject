using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity.Currency;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.ExceptionHandler;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CBS.AccountManagement.MediatR.AccountingEntry.Handlers
{
//    public class ManualPostingEventCommandHandler : IRequestHandler<ManualPostingEventCommand, ServiceResponse<bool>>
//    {
//        private readonly IAccountingRuleRepository _accountingRuleRepository;
//        private readonly IAccountRepository _accountRepository;
//        private readonly IChartOfAccountRepository _chartOfAccountRepository;
//        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
//        private readonly IAccountingEntryRepository _accountingEntryRepository;
//        private readonly IAccountingEntriesServices _accountingEntriesServices;
//        private readonly IMapper _mapper;
//        private readonly ILogger<ManualPostingEventCommandHandler> _logger;
//        private readonly IUnitOfWork<POSContext> _uow;
//        private readonly IAccountTypeRepository _accountTypeRepository;
//        private readonly UserInfoToken _userInfoToken;
//        private readonly IConfiguration _configuration;

//        public ManualPostingEventCommandHandler(
//            IAccountRepository accountRepository,
//            IChartOfAccountRepository chartOfAccountRepository,
//IAccountingRuleRepository accountingRuleRepository,
//            IAccountingRuleEntryRepository accountingRuleEntryRepository,
//            IAccountingEntryRepository accountingEntryRepository,
//            IAccountTypeRepository accountTypeRepository,
//            IAccountingEntriesServices accountingEntriesServices,
//IMapper mapper,
//ILogger<ManualPostingEventCommandHandler> logger,
//IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IConfiguration configuration

//            )
//        {
//            _accountingEntryRepository = accountingEntryRepository;
//            _accountRepository = accountRepository;
//            _accountTypeRepository = accountTypeRepository;
//            _mapper = mapper;
//            _logger = logger;
//            _uow = uow;
//            _userInfoToken = userInfoToken;
//            _configuration = configuration;
//            _accountingRuleEntryRepository = accountingRuleEntryRepository;
//            _accountingRuleRepository = accountingRuleRepository;
//            _chartOfAccountRepository = chartOfAccountRepository;
//            _accountingEntriesServices = accountingEntriesServices;
//        }
//        public async Task<ServiceResponse<bool>> Handle(ManualPostingEventCommand command, CancellationToken cancellationToken)
//        {
//            try
//            {
//                string errorMessage = string.Empty;

//                string debitAccountId = command.DebitAccountId;
//                string creditAccountId = command.CreditAccountId;


//                var transactions = _accountingEntryRepository.FindBy(x => x.ReferenceID == command.TransactionReferenceId && x.IsDeleted == false).ToList();
//                if (transactions.Any())
//                {
//                    errorMessage = $"An entry has already been posted with this transaction";
//                    await APICallHelper.AuditLogger(_userInfoToken.Email, "ManualPostingEventCommand", command, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
//                    return ServiceResponse<bool>.Return409(errorMessage);


//                }
//                List<AccountingEntryDto> listResult = new List<AccountingEntryDto>();
//                var DebitAccount = await _accountingEntriesServices.UpdateAccountBalanceAsync(accountOwner: _userInfoToken.BranchId, accountId: debitAccountId, amount: command.Amount, operationType: AccountOperationType.DEBIT, "ManualPosting");
//                var CreditAccount = await _accountingEntriesServices.UpdateAccountBalanceAsync(accountOwner: _userInfoToken.BranchId, accountId: creditAccountId, amount: command.Amount, operationType: AccountOperationType.CREDIT, "ManualPosting");

//                Booking bookingdr = new Booking(command.Amount, 0, command.TransactionReferenceId, command.Naration, "ManualPosting");
//                Booking bookingCr = new Booking(command.Amount, 0, command.TransactionReferenceId, command.Naration, "ManualPosting");
//                AccountingEntryDto creditEntry = _accountingEntriesServices.CreateCreditEntry(bookingCr, CreditAccount, DebitAccount);
//                AccountingEntryDto debitEntry = _accountingEntriesServices.CreateDebitEntry(bookingdr, CreditAccount, DebitAccount);
//                listResult.Add(debitEntry);
//                listResult.Add(creditEntry);

//                List<Data.AccountingEntry> entris = new List<Data.AccountingEntry>();
//                entris = _mapper.Map(listResult, entris);
//                // Iterate through each accounting rule entry and perform the posting
//                _accountingEntryRepository.AddRange(entris);

//                await _uow.SaveAsync();
//                _logger.LogInformation("Account postings completed successfully.");
//                // You can return a list of AccountingEntryDto as needed
//                return ServiceResponse<bool>.ReturnResultWith200(true);
//            }
//            catch (Exception ex)
//            {

//                return ServiceResponse<bool>.Return500(ex.Message);
//            }
//        }



//    }

}
