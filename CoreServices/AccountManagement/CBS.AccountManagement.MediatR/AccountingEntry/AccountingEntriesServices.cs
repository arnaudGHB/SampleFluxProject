using AutoMapper;
using AutoMapper.Configuration.Conventions;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Data.Entity.Currency;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.AccountingEntry;
using CBS.AccountManagement.MediatR.AccountingEntry.Commands;
using CBS.AccountManagement.MediatR.AccountingEntry.Handlers;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.ExceptionHandler;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CBS.AccountManagement.MediatR.Commands.AddTransferEventCommand;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace CBS.AccountManagement.MediatR
{


    public class AccountingEntriesServices : IAccountingEntriesServices
    {

        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountRepository _chartOfaccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountingRuleRepository _accountingRuleRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IOperationEventRepository _operationEventRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;
        ILogger<AccountingEntriesServices> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IAccountCategoryRepository _accountCategoryRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        private readonly PathHelper _pathHelper;
        private readonly IMongoUnitOfWork _mongoUnitOfWork;

        public const string LOAN_Product_EVENTCODE = "@Loan_Principal_Account";//    public string GetLoanEventCode()\r\n        {\r\n            return this.LoanProductId + \"\";\r\n\r\n        }";
        public const string SAVING_Product_EVENTCODE = "@Principal_Saving_Account";

        public AccountingEntriesServices(
            IAccountRepository accountRepository,
            IAccountCategoryRepository accountCategoryRepository,
            IChartOfAccountRepository chartOfaccountRepository,
            IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IAccountingEntryRepository accountingEntryRepository,
            IConfiguration configuration,
            IMediator mediator,
          IAccountingRuleRepository accountingRuleRepository,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            IOperationEventAttributeRepository? operationEventAttributeRepository,
            IOperationEventRepository? operationEventRepository, ILogger<AccountingEntriesServices> logger, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository, IMongoUnitOfWork? mongoUnitOfWork)
        {
            _accountRepository = accountRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _operationEventRepository = operationEventRepository;
            _chartOfaccountRepository = chartOfaccountRepository;
            _logger = logger;
            _uow = uow;
            _accountingRuleRepository = accountingRuleRepository;
            _accountCategoryRepository = accountCategoryRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _mediator = mediator;
            _pathHelper = new PathHelper(configuration);
            _mongoUnitOfWork = mongoUnitOfWork;
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
        }
        //  var tellerAccount = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == chart0.Id && x.LiaisonId == BranchId&&x.AccountOwnerId==_userInfoToken.BranchId).AsNoTracking();
        public async Task<(bool, string)> CheckDuplicateEntries(string errorMessage, string referenceId)
        {
            var transactions = await _accountingEntryRepository.FindBy(x => x.ReferenceID == referenceId && x.IsDeleted == false).ToListAsync();
            if (transactions.Any())
            {
                errorMessage = $"An entry has already been posted with this transaction";

                return (true, errorMessage);


            }
            return (false, errorMessage);
        }
        public async Task<bool> TransactionExists(string transactionReferenceId)
        {
            var existingTransactions = _accountingEntryRepository
                .FindBy(x => x.ReferenceID == transactionReferenceId && !x.IsDeleted)
                .ToList();

            return existingTransactions.Any();
        }
        public async Task<Data.Account> CreateAccountForBranchByChartOfAccountIdAsync(string determinationAccountId, string branchId, string branchCode)
        {
            IQueryable<Data.Account> ProductFromAccount = null;
            var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(determinationAccountId);
            var account = _accountRepository.FindBy(x => x.AccountOwnerId == branchId && x.ChartOfAccountManagementPositionId == determinationAccountId).AsNoTracking();
            if (account.Any())
            {
                return account.FirstOrDefault();
            }
            else
            {
                chartOfAccount.ChartOfAccount = await _chartOfaccountRepository.FindAsync(chartOfAccount.ChartOfAccountId);
                //var branch = await GetBranchCodeAsync(branchId);
                var model = new Commands.AddAccountCommand
                {
                    AccountOwnerId = branchId,
                    AccountName = chartOfAccount.Description + " " + branchCode,
                    OwnerBranchCode = branchCode,
                    AccountNumberManagementPosition = chartOfAccount.PositionNumber.PadRight(3, '0'),
                    AccountNumber = chartOfAccount.ChartOfAccount.AccountNumber,
                    AccountNumberNetwok = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _pathHelper.BankManagement_BankCode + chartOfAccount.PositionNumber.PadRight(3, '0')).PadRight(12, '0') + branchCode,
                    AccountNumberCU = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + chartOfAccount.PositionNumber.PadRight(3, '0')).PadRight(9, '0') + branchCode,
                    ChartOfAccountManagementPositionId = chartOfAccount.Id,
                    AccountCategoryId = chartOfAccount.ChartOfAccount.AccountCartegoryId,
                    //BookingDirection = (chartOfAccount.IsDebit == false) ? "" : ""
                };

                var response = await _mediator.Send(model);
                if (response.Success.Equals(false))
                {
                    throw new Exception($"System was unable to create account: {model.AccountNumberCU} for {model.OwnerBranchCode}");
                }
                ProductFromAccount = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == chartOfAccount.Id && x.AccountOwnerId == branchId).AsNoTracking();
                return ProductFromAccount.FirstOrDefault();
            }


        }

        public async Task<Data.Account> CreateLiaisonAccountForBranchByChartOfAccountIdAsync(string determinationAccountId, string branchId, string branchCode, bool remittanceStatus)
        {
            IQueryable<Data.Account> ProductFromAccount = null;
            var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(determinationAccountId);
            var account = _accountRepository.FindBy(x => x.AccountOwnerId == _userInfoToken.BranchId && x.LiaisonId == branchId && x.ChartOfAccountManagementPositionId == determinationAccountId).AsNoTracking();
            if (account.Any())
            {
                return account.FirstOrDefault();
            }
            else
            {
                chartOfAccount.ChartOfAccount = await _chartOfaccountRepository.FindAsync(chartOfAccount.ChartOfAccountId);
                //var branch = await GetBranchCodeAsync(branchId);
                var model = new Commands.AddAccountCommand
                {
                    AccountOwnerId = _userInfoToken.BranchId,
                    AccountName = chartOfAccount.Description + " " + branchCode,
                    OwnerBranchCode = branchCode,
                    AccountNumberManagementPosition = chartOfAccount.PositionNumber.PadRight(3, '0'),
                    AccountNumber = chartOfAccount.ChartOfAccount.AccountNumber,
                    AccountNumberNetwok = chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _pathHelper.BankManagement_BankCode + _userInfoToken.BranchCode + branchCode,
                    AccountNumberCU = chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BranchCode + branchCode,
                    ChartOfAccountManagementPositionId = chartOfAccount.Id,
                    AccountTypeId = branchId,
                    AccountCategoryId = chartOfAccount.ChartOfAccount.AccountCartegoryId,
                    LiaisonBranchCode = branchCode
                    //BookingDirection = (chartOfAccount.IsDebit == false) ? "" : ""
                };
                var response = await _mediator.Send(model);
                if (response.Success.Equals(false))
                {
                    throw new Exception($"System was unable to create account: {model.AccountNumberCU} for {model.OwnerBranchCode}");
                }
                if (remittanceStatus)
                {
                    ProductFromAccount = _accountRepository.FindBy(x => x.AccountOwnerId == _userInfoToken.BranchId && x.ChartOfAccountManagementPositionId == determinationAccountId).AsNoTracking();

                }
                else
                {
                    ProductFromAccount = _accountRepository.FindBy(x => x.AccountOwnerId == _userInfoToken.BranchId && x.LiaisonId == branchId && x.ChartOfAccountManagementPositionId == determinationAccountId).AsNoTracking();
                }
                return ProductFromAccount.FirstOrDefault();
            }


        }

        public ChartOfAccountManagementPosition GetAccountNumberManagementPosition(Data.CashMovementTracker chartOfAccount)
        {

            var models = _chartOfAccountManagementPositionRepository.FindBy(f => f.ChartOfAccountId == chartOfAccount.Id);
            return models.FirstOrDefault();

        }


        public async Task<Data.Account> UpdateAccountBalanceAsync(Data.Account Account, decimal amount, AccountOperationType operationType, string PostingEvent)
        {
            string errorMessage = string.Empty;
            string commandObject = $"accountOwner: {Account.AccountOwnerId}, accountId:{Account.Id} , amount :{amount} ";
            List<DataAccount> dataAccounts = new List<DataAccount>();
            Data.Account account = (await ChackIfOperationIsValid(AccountOperationType.CREDIT == operationType, Account, amount, operationType)) == true ? await UpdateAccountAsync(Account, amount, operationType) : throw new ArgumentException($"The account balance of {Account.AccountNumberCU}-{Account.AccountName} is {Account.CurrentBalance}XFA does not permit you to processes this transaction");
            _accountRepository.UpdateInCasecade(account);
            return account;
        }

        public async Task<Data.Account> UpdateAccountBalanceForCashMovementClass5Async(Data.Account Account, decimal amount, AccountOperationType operationType, string PostingEvent)
        {
            string errorMessage = string.Empty;
            string commandObject = $"accountOwner: {Account.AccountOwnerId}, accountId:{Account.Id} , amount :{amount} ";
            List<DataAccount> dataAccounts = new List<DataAccount>();
            Data.Account account = await UpdateAccountAsync(Account, amount, operationType);
            _accountRepository.UpdateInCasecade(account);
            return account;
        }
        public async Task<Data.Account> UpdateAccountDepositeAsync(bool IsDeposit, Data.Account Account, decimal amount, AccountOperationType operationType, string PostingEvent)
        {
            string errorMessage = string.Empty;
            string commandObject = $"accountOwner: {Account.AccountOwnerId}, accountId:{Account.Id} , amount :{amount} ";
            List<DataAccount> dataAccounts = new List<DataAccount>();
            Data.Account account = (await ChackIfOperationIsValid(IsDeposit, Account, amount, operationType)) == true ? await UpdateAccountAsync(Account, amount, operationType) : throw new ArgumentException($"The account balance of {Account.AccountNumberCU}-{Account.AccountName} is {Account.CurrentBalance}XFA does not permit you to processes this transaction");

            _accountRepository.Update(account);
            return account;
        }
        public async Task<Data.Account> UpdateAccountBalanceAsync(string memeberRefence, Data.Account Account, decimal amount, AccountOperationType operationType, string PostingEvent)
        {
            string errorMessage = string.Empty;
            string commandObject = $"accountOwner: {Account.AccountOwnerId}, accountId:{Account.Id} , amount :{amount} ";
            if (CheckIfAccountIsRootAccount(Account))
            {
                throw new Exception("Root Account in used, Hence Applicable but not configured!!! contact system administrator");
            }
            Data.Account account = (await ChackIfOperationIsValid(AccountOperationType.CREDIT == operationType, Account, amount, operationType)) == true ? await UpdateAccountAsync(Account, amount, operationType) : throw new ArgumentException($"The account balance of {Account.AccountNumberCU}-{Account.AccountName} is {Account.CurrentBalance}XFA does not permit you to processes this transaction");

            _accountRepository.UpdateInCasecade(account);


            return account;
        }

        private bool CheckIfAccountIsRootAccount(Data.Account account)
        {
            var model = _chartOfAccountManagementPositionRepository.FindBy(x => x.Description == "ROOT ACCOUNT").FirstOrDefault();
            return account.Id == model.Id;
        }
        public async Task<bool> ChackIfOperationIsValid(bool IsDeposit, Data.Account account, decimal amount, AccountOperationType operationType)
        {
            // Store last balance before any changes

            return true;
        }


        public async Task<Data.Account> CreateLiaisonAccountForAwayBranchWithChartOfAccountIdAsync(string determinationAccountId, string branchId, string branchCode)
        {
            IQueryable<Data.Account> ProductFromAccount = null;
            var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(determinationAccountId);
            var account = _accountRepository.FindBy(x => x.AccountOwnerId == branchId && x.LiaisonId == _userInfoToken.BranchId && x.ChartOfAccountManagementPositionId == determinationAccountId).AsNoTracking();
            if (account.Any())
            {
                return account.FirstOrDefault();
            }
            else
            {
                chartOfAccount.ChartOfAccount = await _chartOfaccountRepository.FindAsync(chartOfAccount.ChartOfAccountId);
                //var branch = await GetBranchCodeAsync(branchId);
                var model = new Commands.AddAccountCommand
                {
                    AccountOwnerId = branchId,
                    AccountName = chartOfAccount.Description + " " + branchCode,
                    OwnerBranchCode = branchCode,
                    AccountNumberManagementPosition = chartOfAccount.PositionNumber.PadRight(3, '0'),
                    AccountNumber = chartOfAccount.ChartOfAccount.AccountNumber,
                    AccountNumberNetwok = chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _pathHelper.BankManagement_BankCode + branchCode + _userInfoToken.BranchCode,
                    AccountNumberCU = chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + branchCode + _userInfoToken.BranchCode,
                    ChartOfAccountManagementPositionId = chartOfAccount.Id,
                    AccountTypeId = _userInfoToken.BranchId,
                    AccountCategoryId = chartOfAccount.ChartOfAccount.AccountCartegoryId,
                    //BookingDirection = (chartOfAccount.IsDebit == false) ? "" : ""
                };
                var response = await _mediator.Send(model);
                if (response.Success.Equals(false))
                {
                    throw new Exception($"System was unable to create account: {model.AccountNumberCU} for {model.OwnerBranchCode}");
                }

                ProductFromAccount = _accountRepository.FindBy(x => x.AccountOwnerId == branchId && x.LiaisonId == _userInfoToken.BranchId && x.ChartOfAccountManagementPositionId == determinationAccountId).AsNoTracking();

                return ProductFromAccount.FirstOrDefault();
            }


        }



        public async Task<Data.Account> CreateLiaisonAccountForAwayBranchWithChartOfAccountIdAsync(string determinationAccountId, string OwnerBranchId, string LiasonBranchId, string OwnerbranchCode, string LiasonBranchBranchCode)
        {
            IQueryable<Data.Account> ProductFromAccount = null;
            var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(determinationAccountId);
            var account = _accountRepository.FindBy(x => x.AccountOwnerId == OwnerBranchId && x.LiaisonId == LiasonBranchId && x.ChartOfAccountManagementPositionId == determinationAccountId).AsNoTracking();
            if (account.Any())
            {
                return account.FirstOrDefault();
            }
            else
            {
                chartOfAccount.ChartOfAccount = await _chartOfaccountRepository.FindAsync(chartOfAccount.ChartOfAccountId);
                //var branch = await GetBranchCodeAsync(branchId);
                var model = new Commands.AddAccountCommand
                {
                    AccountOwnerId = OwnerBranchId,
                    AccountName = chartOfAccount.Description + " " + OwnerbranchCode,
                    OwnerBranchCode = OwnerbranchCode,
                    LiaisonBranchCode = LiasonBranchBranchCode,
                    AccountNumberManagementPosition = chartOfAccount.PositionNumber.PadRight(3, '0'),
                    AccountNumber = chartOfAccount.ChartOfAccount.AccountNumber,
                    AccountNumberNetwok = chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _pathHelper.BankManagement_BankCode + OwnerbranchCode + LiasonBranchBranchCode,
                    AccountNumberCU = chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + OwnerbranchCode + LiasonBranchBranchCode,
                    ChartOfAccountManagementPositionId = chartOfAccount.Id,
                    AccountTypeId = LiasonBranchId,
                    AccountCategoryId = chartOfAccount.ChartOfAccount.AccountCartegoryId,
      
                };
                var response = await _mediator.Send(model);
                if (response.Success.Equals(false))
                {
                    throw new Exception($"System was unable to create account: {model.AccountNumberCU} for {model.OwnerBranchCode}");
                }

                ProductFromAccount = _accountRepository.FindBy(x => x.AccountOwnerId == OwnerBranchId && x.LiaisonId == LiasonBranchId && x.ChartOfAccountManagementPositionId == determinationAccountId).AsNoTracking();

                return ProductFromAccount.FirstOrDefault();
            }


        }


        /// <summary>
        /// Updates account balances according to OHADA accounting rules and validates final balance
        /// </summary>
        /// <returns>Returns true if the resulting balance is non-negative, false otherwise</returns>
        public async Task<bool> ChackIfOperationIsValidCorrect(bool IsDeposit, Data.Account account, decimal amount, AccountOperationType operationType)
        {
            // Store last balance before any changes
            account.LastBalance = account.CurrentBalance;
            // Determine account behavior based on OHADA rules
            bool isDebitNormal = account.AccountNumber.StartsWith("2") || // Fixed Assets
                                account.AccountNumber.StartsWith("3") || // Inventory
                                account.AccountNumber.StartsWith("5") || // Financial
                                account.AccountNumber.StartsWith("6");   // Expenses
            bool isCreditNormal = account.AccountNumber.StartsWith("1") || // Capital
                                account.AccountNumber.StartsWith("7");    // Income
                                                                          // Handle class 4 accounts separately
            bool isClass4 = account.AccountNumber.StartsWith("4");
            bool isReceivable = isClass4 && await CheckIfAccountIsReceivableAsync(account);

            if (operationType == AccountOperationType.DEBIT)
            {
                if (isDebitNormal || (isClass4 && !isReceivable))
                {
                    // For debit-normal accounts, debit increases the balance
                    account.DebitBalance += amount;
                }
                else if (isCreditNormal || (isClass4 && isReceivable))
                {
                    // For credit-normal accounts, debit decreases the balance
                    account.DebitBalance += amount;
                }
            }
            else // CREDIT
            {
                if (isDebitNormal || (isClass4 && !isReceivable))
                {
                    // For debit-normal accounts, credit decreases the balance
                    account.CreditBalance += amount;
                }
                else if (isCreditNormal || (isClass4 && isReceivable))
                {
                    // For credit-normal accounts, credit increases the balance
                    account.CreditBalance += amount;
                }
            }

            // Calculate current balance based on account type
            if (isDebitNormal || (isClass4 && !isReceivable))
            {
                account.CurrentBalance = account.CreditBalance - account.DebitBalance;
            }
            else
            {
                account.CurrentBalance = account.DebitBalance - account.CreditBalance;

            }

            // Update audit fields
            account.ModifiedDate = DateTime.UtcNow;
            account.ModifiedBy = _userInfoToken.Id;

            // Return false if the current balance is negative, true otherwise
            return account.CurrentBalance >= 0;
        }
        //public async Task<bool> ChackIfOperationIsValid(bool IsDeposit, Data.Account account, decimal amount, AccountOperationType operationType)
        //{
        //    Data.Account temp_account = account;
        //    // Store last balance before any changes


        //    // Determine account behavior based on OHADA rules
        //    bool isDebitNormal = temp_account.AccountNumber.StartsWith("2") || // Fixed Assets
        //                        temp_account.AccountNumber.StartsWith("3") || // Inventory
        //                        temp_account.AccountNumber.StartsWith("5") || // Financial
        //                        temp_account.AccountNumber.StartsWith("6");   // Expenses

        //    bool isCreditNormal = temp_account.AccountNumber.StartsWith("1") || // Capital
        //                        temp_account.AccountNumber.StartsWith("7");    // Income

        //    // Handle class 4 accounts separately
        //    bool isClass4 = temp_account.AccountNumber.StartsWith("4");
        //    bool isReceivable = isClass4 && await CheckIfAccountIsReceivableAsync(temp_account);

        //    if (operationType == AccountOperationType.DEBIT)
        //    {
        //        if (isDebitNormal || (isClass4 && !isReceivable))
        //        {
        //            // For debit-normal accounts, debit increases the balance
        //            temp_account.DebitBalance += amount;
        //        }
        //        else if (isCreditNormal || (isClass4 && isReceivable))
        //        {
        //            // For credit-normal accounts, debit decreases the balance
        //            temp_account.DebitBalance += amount;
        //        }
        //    }
        //    else // CREDIT
        //    {
        //        if (isDebitNormal || (isClass4 && !isReceivable))
        //        {
        //            // For debit-normal accounts, credit decreases the balance
        //            temp_account.CreditBalance += amount;
        //        }
        //        else if (isCreditNormal || (isClass4 && isReceivable))
        //        {
        //            // For credit-normal accounts, credit increases the balance
        //            temp_account.CreditBalance += amount;
        //        }
        //    }

        //    // Calculate current balance based on account type
        //    if (isDebitNormal || (isClass4 && !isReceivable))
        //    {
        //        temp_account.CurrentBalance = temp_account.DebitBalance - temp_account.CreditBalance;
        //    }
        //    else
        //    {
        //        temp_account.CurrentBalance = temp_account.CreditBalance - temp_account.DebitBalance;
        //    }

        //    // Update audit fields
        //    temp_account.ModifiedDate = DateTime.UtcNow;
        //    temp_account.ModifiedBy = _userInfoToken.Id;
        //    if ((isDebitNormal || temp_account.AccountNumber.StartsWith("451")) && AccountOperationType.CREDIT.Equals(operationType))
        //    {
        //        return temp_account.CurrentBalance > 0;
        //    }
        //    if ((isCreditNormal || temp_account.AccountNumber.StartsWith("451")) && AccountOperationType.DEBIT.Equals(operationType))
        //    {
        //        return temp_account.CurrentBalance > 0;
        //    }
        //    return temp_account.CurrentBalance > 0;
        //}

        /// <summary>
        /// Updates account balances according to OHADA accounting rules
        /// </summary>
        public async Task<Data.Account> UpdateAccountAsync(Data.Account account, decimal amount, AccountOperationType operationType)
        {
            try
            {
                if (account.AccountNumber.StartsWith("000") )
                {
                    string message = $"There is a ROOT ACCOUNT EXCEPTION, Please contact admin";
                    throw new CustomBusinessException(message, new Exception("DeterminantAccount:\n " + JsonConvert.SerializeObject(account) ));
                }
                // Store last balance before any cha00k0es
                account.LastBalance = account.CurrentBalance;

                // Determine account behavior based on OHADA rules
                bool isDebitNormal = account.AccountNumber.StartsWith("2") || // Fixed Assets
                                                                              //   || // Inventory
                                    account.AccountNumber.StartsWith("5") || // Financial
                                    account.AccountNumber.StartsWith("6");    // Expenses

                bool isCreditNormal = account.AccountNumber.StartsWith("1") || // Capital
                                    account.AccountNumber.StartsWith("7") ||    // Income
                                       account.AccountNumber.StartsWith("3");
                // Handle class 4 accounts separatelyio
                bool isClass4Receivable = account.AccountNumber.StartsWith("41") || account.AccountNumber.StartsWith("42") || account.AccountNumber.StartsWith("46");
                bool isClass4Payables = account.AccountNumber.StartsWith("40") || account.AccountNumber.StartsWith("43") || account.AccountNumber.StartsWith("48") || account.AccountNumber.StartsWith("49") ||
                    account.AccountNumber.StartsWith("44") || account.AccountNumber.StartsWith("45") || account.AccountNumber.StartsWith("47");

                if (operationType == AccountOperationType.DEBIT)
                {

                    // For debit-normal accounts, debit increases the balance
                    account.DebitBalance += amount;

                }
                else // CREDIT
                {
                    account.CreditBalance += amount;
                    //account.CurrentBalance = account.CurrentBalance - amount;
                }
                if (isDebitNormal || isClass4Receivable)
                {
                    account.CurrentBalance = account.DebitBalance - account.CreditBalance;
                }
                else
                {
                    account.CurrentBalance = account.CreditBalance - account.DebitBalance;
                }

                // Update audit fields
                account.ModifiedDate = DateTime.UtcNow;
                account.ModifiedBy = _userInfoToken.Id;
                //if (account.CurrentBalance>=0)
                //{
                    return account;
                //}
                //else
                //{
                //    throw new  InvalidDataException($"The  operation will turn the account balance of {account.AccountNumberCU}-{account.AccountName} negative");
                //}
            
            }
            catch (Exception ex)
            {

                throw(ex);
            }
        }
        private async Task<bool> CheckIfAccountIsReceivableAsync(Data.Account account)
        {
            var model = await _accountCategoryRepository.FindAsync(account.AccountCategoryId);
            return model.Name.ToLower() == "revenue";
        }



        /// <summary>
        /// Create the account using ChartOfAccountId
        /// </summary>
        /// <param name="chartOfAccountManagementPositionId"></param>
        /// <returns></returns>
        public async Task CreateAccountByChartOfAccountManagementPositionId(string chartOfAccountManagementPositionId, string branchCode, string BranchId)
        {
            Data.Account data = new Data.Account();
            var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(chartOfAccountManagementPositionId);

            if (chartOfAccount != null)
            {
                //var bankInfo = await APICallHelper.GetBankInfo(new PathHelper(_configuration), _userInfoToken);
                var accountId = _accountRepository.All.Where(c => c.ChartOfAccountManagementPositionId == chartOfAccountManagementPositionId && c.AccountOwnerId == BranchId);

                if (accountId.Any())
                {
                    data = accountId.FirstOrDefault();
                }
                else
                {

                    var AccountManagementPosition = chartOfAccount.PositionNumber.PadRight(3, '0');
                    chartOfAccount.ChartOfAccount = await _chartOfaccountRepository.FindAsync(chartOfAccount.ChartOfAccountId);

                    AddAccountCommand command = new AddAccountCommand
                    {
                        AccountName = chartOfAccount.Description + " " + branchCode,
                        AccountNumber = chartOfAccount.ChartOfAccount.AccountNumber,
                        AccountNumberManagementPosition = AccountManagementPosition,
                        AccountNumberNetwok = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _pathHelper.BankManagement_BankCode + branchCode).PadRight(12, '0') + AccountManagementPosition,
                        AccountNumberCU = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + branchCode).PadRight(9, '0') + AccountManagementPosition,
                        AccountOwnerId = BranchId,
                        AccountTypeId = "",
                        ChartOfAccountManagementPositionId = chartOfAccountManagementPositionId,
                        OwnerBranchCode = branchCode,
                        AccountCategoryId = chartOfAccount.ChartOfAccount.AccountCartegoryId,
                    };
                    await _mediator.Send(command);




                }
            }

        }

        public bool EvaluateDoubleEntryRule(List<Data.AccountingEntry> listResult)
        {
            var sumCredit = listResult.Where(x => x.EntryType.ToLower() == AccountOperationType.CREDIT.ToString().ToLower()).Sum(x => x.DrAmount);
            var sumDebit = listResult.Where(x => x.EntryType.ToLower() == AccountOperationType.DEBIT.ToString().ToLower()).Sum(x => x.CrAmount);
            return sumCredit == sumDebit || sumCredit == 0;
        }
        public AccountingEntryDto CreateDebitEntry(Booking book, Data.Account CrAccount, Data.Account DrAccount, DateTime TransactionDate)
        {
            AccountingEntryDto model = new();

            model.BankId = _userInfoToken.BankId;
            model.BranchId = CrAccount.BranchId;
            model.ReferenceID = book.TransactionReferenceId;
            model.EntryDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
            model.EntryType = AccountOperationType.DEBIT.ToString();
            model.CurrentBalance = book.Amount;
            model.CrAccountId = CrAccount.Id;
            model.AccountId = DrAccount.Id;
            model.DrAccountId = DrAccount.Id;
            model.AccountId = DrAccount.Id;
            model.CrAccountNumber = CrAccount.AccountNumberCU;
            model.DrAccountNumber = DrAccount.AccountNumberCU;
            model.AccountNumber = DrAccount.AccountNumber;
            model.AccountNumberReference = DrAccount.AccountNumberReference;
            model.DrAmount = book.Amount;
            model.CrAmount = 0;
            model.IsAuxilaryEntry = book.IsAuxilaryTransaction;
            model.Amount = -1 * book.Amount;
            model.CrCurrentBalance = CrAccount.CurrentBalance;
            model.DrCurrentBalance = DrAccount.CurrentBalance;
            model.DrBalanceBroughtForward = DrAccount.LastBalance;
            model.CrBalanceBroughtForward = CrAccount.LastBalance;
            model.Status = PostingStatus.Posted.ToString();
            model.ValueDate = TransactionDate;
            model.Source = "SYSTEM";
            model.ExternalBranchId = "NOT SET";
            model.ReviewedBy = "NOT SET";
            model.Representative = book.MemberReference == null ? "N/A" : book.MemberReference;
            model.Naration = book.Naration == null ? "N/A" : book.Naration;
            model.AccountCartegory = DrAccount.AccountCategoryId;
            model.ExternalBranchId = book.ExternalBranchId;
            model.Description = book.Naration == null ? "N/A" : book.Naration;
            model.OperationType = book.OperationType;
            model.EventCode = "";
            model.Currency = Currency.GetCurrency();
            model.Id = BaseUtilities.GenerateInsuranceUniqueNumber(7, _userInfoToken.BankCode + _userInfoToken.BranchCode);
            model.InitiatorId = "NOT SET";
            return model;
        }
        public AccountingEntryDto CreateCreditEntry(Booking book, Data.Account CrAccount, Data.Account DrAccount, DateTime TransactionDate)
        {
            AccountingEntryDto model = new();
            model.EventCode = "";
            model.DrAmount = 0;
            model.CurrentBalance = book.Amount;
            model.CrAmount = book.Amount;
            model.IsAuxilaryEntry = book.IsAuxilaryTransaction;
            model.BankId = _userInfoToken.BankId;
            model.BranchId = DrAccount.BranchId;
            model.ReferenceID = book.TransactionReferenceId;
            model.EntryDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
            model.EntryType = AccountOperationType.CREDIT.ToString();
            model.CrAccountId = CrAccount.Id;
            model.DrAccountId = DrAccount.Id;
            model.AccountId = CrAccount.Id;
            model.CrAccountNumber = CrAccount.AccountNumberCU;
            model.DrAccountNumber = DrAccount.AccountNumberCU;
            model.AccountNumber = CrAccount.AccountNumber;
            model.AccountNumberReference = CrAccount.AccountNumberReference;
            model.Amount = book.Amount;
            model.CrCurrentBalance = CrAccount.CurrentBalance;
            model.DrCurrentBalance = DrAccount.CurrentBalance;
            model.DrBalanceBroughtForward = DrAccount.LastBalance;
            model.CrBalanceBroughtForward = CrAccount.LastBalance;
            model.Status = PostingStatus.Posted.ToString();
            model.ValueDate = TransactionDate;
            model.Source = "SYSTEM";
            model.ReviewedBy = "NOT SET";
            
            model.InitiatorId = "NOT SET";
            model.Representative = book.MemberReference == null ? "N/A" : book.MemberReference;
            model.Naration = book.Naration == null ? "N/A" : book.Naration;
            model.AccountCartegory = DrAccount.AccountCategoryId;
            model.ExternalBranchId = book.ExternalBranchId;
            //model.BranchId=
            model.Description = book.Naration == null ? "N/A" : book.Naration;
            model.OperationType = book.OperationType;
            model.Currency = Currency.GetCurrency();
            model.AccountCartegory = CrAccount.AccountCategoryId;
            model.Id = BaseUtilities.GenerateInsuranceUniqueNumber(7, _userInfoToken.BankCode + _userInfoToken.BranchCode);
            return model;


        }

        private async Task<CashMovementAccount> GetAccountAndOperationTypeAsync(AccountingRuleEntry item, string BranchId, string BranchCode)
        {
            CashMovementAccount accountAndOperation = new CashMovementAccount();
            var chartOfAccontPosition = await _chartOfAccountManagementPositionRepository.FindAsync(item.DeterminationAccountId);
            var chartOfAccontPositionCredit = await _chartOfAccountManagementPositionRepository.FindAsync(item.BalancingAccountId);

            var ChartOfAccountDetermination = await _chartOfaccountRepository.FindAsync(chartOfAccontPosition.ChartOfAccountId);
            var ChartOfAccountBalancing = await _chartOfaccountRepository.FindAsync(chartOfAccontPositionCredit.ChartOfAccountId);


            if (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper())
            {
                accountAndOperation.BookingDirection = "DEBIT";
                var modelDetermination = _accountRepository.FindBy(c => c.AccountNumber == ChartOfAccountDetermination.AccountNumber && c.AccountOwnerId == BranchId);
                var modelBalancing = _accountRepository.FindBy(c => c.AccountNumber == ChartOfAccountBalancing.AccountNumber && c.AccountOwnerId == BranchId);

                if (modelDetermination.Any())
                {
                    accountAndOperation.SourceAccount = modelDetermination.FirstOrDefault();
                    accountAndOperation.DestinationAccount = modelBalancing.FirstOrDefault();
                }
                else
                {
                    accountAndOperation.SourceAccount = await CreateAccountForBranchByChartOfAccountIdAsync(item.DeterminationAccountId, BranchId, BranchCode);
                    accountAndOperation.DestinationAccount = await CreateAccountForBranchByChartOfAccountIdAsync(item.BalancingAccountId, BranchId, BranchCode);

                }


            }
            else
            {

                accountAndOperation.BookingDirection = "CREDIT";


                var modelDetermination = _accountRepository.FindBy(c => c.AccountNumber == ChartOfAccountDetermination.AccountNumber && c.AccountOwnerId == BranchId);
                var modelBalancing = _accountRepository.FindBy(c => c.AccountNumber == ChartOfAccountBalancing.AccountNumber && c.AccountOwnerId == BranchId);

                if (modelDetermination.Any())
                {
                    accountAndOperation.SourceAccount = modelDetermination.FirstOrDefault();
                    accountAndOperation.DestinationAccount = modelBalancing.FirstOrDefault();
                }
                else
                {
                    accountAndOperation.SourceAccount = await CreateAccountForBranchByChartOfAccountIdAsync(item.DeterminationAccountId, BranchId, BranchCode);
                    accountAndOperation.DestinationAccount = await CreateAccountForBranchByChartOfAccountIdAsync(item.BalancingAccountId, BranchId, BranchCode);

                }

            }
            return accountAndOperation;
        }
        public string BuildServiceNaration(decimal amount, MakeAccountPostingCommand command, NarationType NarationType, Data.Account account, AccountOperationType operationType)
        {
            string message = string.Empty;
            if (operationType == AccountOperationType.DEBIT)
            {
                if (NarationType == NarationType.WithdrawalCharges)
                {
                    message = $"{NarationType.ToString()} of XAF{amount} has been debited from {command.AccountNumber} in favour of {account.AccountNumber}-{account.AccountName}";

                }
                else if (NarationType == NarationType.DepositCharges)
                {
                    message = $"{NarationType.ToString()} of XAF{amount} has been debited from {command.AccountNumber} in favour of {account.AccountNumber}-{account.AccountName}";

                }

            }
            else if (operationType == AccountOperationType.CREDIT)
            {

                if (NarationType == NarationType.WithdrawalCharges)
                {
                    message = $"{NarationType.ToString()} of XAF{amount} has been credited to {account.AccountNumber}-{account.AccountName} in the expense of  {command.AccountNumber} ";

                }
                else if (NarationType == NarationType.DepositCharges)
                {
                    message = $"{NarationType.ToString()} of XAF{amount} has been credited to {account.AccountNumber}-{account.AccountName} in the expense of  {command.AccountNumber} ";

                }
            }
            return message;
        }
        public TransactionData GenerateTransactionRecord(string accountNumber, string types, string Code, string transactionReferenceId, string naration, decimal amount)
        {
            return new TransactionData
            {
                Id = BaseUtilities.GenerateInsuranceUniqueNumber(20, Code),      // Account Number(s): The account(s) involved in the transaction.
                AccountNumber = accountNumber,
                TransactionType = types.ToString().ToUpper(),               // Amount: The monetary value of the transaction.
                Amount = amount,
                ReferenceNumber = transactionReferenceId,
                Status = TransactionStatus.POSTED.ToString(),
                Description = naration,

            };
        }
        public TransactionData GenerateTransactionRecord(string accountNumber, OperationEventAttributeTypes types, TransactionCode Code, string transactionReferenceId, string naration, decimal amount)
        {
            return new TransactionData
            {
                Id = BaseUtilities.GenerateInsuranceUniqueNumber(20, Code.ToString()),      // Account Number(s): The account(s) involved in the transaction.
                AccountNumber = accountNumber,
                TransactionType = types.ToString().ToUpper(),               // Amount: The monetary value of the transaction.
                Amount = amount,
                ReferenceNumber = transactionReferenceId,
                Status = TransactionStatus.POSTED.ToString(),
                Description = naration,

            };
        }
        public async Task<List<CashMovementAccount>> GetListAccountAndOperationTypesAsync(List<AccountingRuleEntry> ruleEntries, string EventCode, string BranchId, string BranchCode)
        {
            List<CashMovementAccount> accountAndOperationTypes = new();
            if (ruleEntries != null)
            {
                foreach (var item in ruleEntries)
                {
                    var model = await GetAccountAndOperationTypeAsync(item, BranchId, BranchCode);
                    accountAndOperationTypes.Add(model);
                }
            }
            else
            {
                string errorMessage = $"AccountingEntryRule with ID {EventCode} not found.";
                _logger.LogInformation(errorMessage);

            }
            return accountAndOperationTypes;
        }
        public List<AccountingRuleEntry>? GetAccountingEntryRule(string? eventCode)
        {
            List<AccountingRuleEntry> ruleEntries = new List<AccountingRuleEntry>();
            try
            {
                var entries = _accountingRuleEntryRepository.FindBy(x => x.EventCode == eventCode);
                if (entries.Any())
                {
                    ruleEntries = entries.ToList();
                }
                return ruleEntries;
            }
            catch (Exception ex)
            {

                throw new Exception($"The product event ruleId {eventCode} does not exist in this context");
            }
        }

        public AccountingRuleEntry GetAccountingEntryRuleByEventCode(string OpeningEvent)
        {
            AccountingRuleEntry entry = new AccountingRuleEntry();
            var entries = _accountingRuleEntryRepository.FindBy(x => x.EventCode == OpeningEvent);
            if (entries.Any())
            {
                entry = entries.FirstOrDefault();
            }
            else
            {
                throw new Exception($"The product event ruleId {OpeningEvent} does not exist in this context");
            }
            return entry;
        }
        public AccountingRuleEntry GetAccountingEntryRuleByEventCode(Data.BulkTransaction.MakeAccountPostingCommand command)
        {
            AccountingRuleEntry entry = new AccountingRuleEntry();
            string eventName = command.ProductType.ToLower().Contains("loan") ? LOAN_Product_EVENTCODE : SAVING_Product_EVENTCODE;
            string eventCode = $"{command.FromProductId}{eventName}";          
            var entries = _accountingRuleEntryRepository.FindBy(x => x.EventCode == eventCode);
            if (entries.Any())
            {
                entry = entries.FirstOrDefault();
            }
            else
            {
                throw new Exception($"The product event ruleId {eventCode} does not exist in this context");
            }
            return entry;
        }
        public AccountingRuleEntry GetAccountEntryRuleByProductID(string? eventCode)
        {
            AccountingRuleEntry entry = new AccountingRuleEntry();
            var entries = _accountingRuleEntryRepository.FindBy(x => x.ProductAccountingBookId == eventCode).AsNoTracking();
            if (entries.Any())
            {
                entry = entries.FirstOrDefault();
            }
            else
            {
                throw new Exception($"The product event ruleId {eventCode} does not exist in this context");
            }

            return entry;
        }



        public AccountingRuleEntry GetAccountEntryRuleByEventCode(string? eventCode)
        {
            AccountingRuleEntry entry = new AccountingRuleEntry();
            var entries = _accountingRuleEntryRepository.FindBy(x => x.EventCode == eventCode).AsNoTracking();
            if (entries.Any())
            {
                entry = entries.FirstOrDefault();
            }
            else
            {
                throw new Exception($"The product event ruleId {eventCode} does not exist in this context");
            }

            return entry;
        }




        public async Task<List<AccountingEntryDto>> TransferFundBetweenBranch(string naration, string memberReference, DateTime TransactionDate, Data.Account fromAccount, Data.Account toAccount, Data.Account liaisonAccount, decimal amount, AddTransferEventCommand eventCommand)
        {
            List<Data.Account> listAccount = new List<Data.Account>();
            List<AccountingEntryDto> list = new List<AccountingEntryDto>();

            var models = await CashMovementAsync(naration, memberReference, eventCommand.TransactionDate, fromAccount, liaisonAccount, amount, "TransferFundBetweenBranch", eventCommand.TransactionReferenceId, _userInfoToken.BranchId);
            list.AddRange(models);

            var model0s = await CashMovementAsync(naration, memberReference, eventCommand.TransactionDate, liaisonAccount, toAccount, amount, "TransferFundBetweenBranch", eventCommand.TransactionReferenceId, _userInfoToken.BranchId);
            list.AddRange(model0s);
            return await Task.FromResult(list);

        }


        public static Dictionary<string, AccountingEntryDto> GenerateTrialBalance(List<AccountingEntryDto> entries, DateTime endDate)
        {
            var trialBalance = new Dictionary<string, AccountingEntryDto>();

            // Aggregate debit and credit amounts for each account
            foreach (var entry in entries)
            {
                if (entry.ValueDate >= SetDateToBeginningOfYear() && entry.ValueDate <= endDate)
                {
                    if (trialBalance.ContainsKey(entry.DrAccountNumber))
                        trialBalance[entry.DrAccountNumber].DrAmount += entry.DrAmount;
                    else
                        trialBalance[entry.DrAccountNumber].DrAmount = entry.DrAmount;

                    if (trialBalance.ContainsKey(entry.CrAccountNumber))
                        trialBalance[entry.CrAccountNumber].CrAmount = -entry.CrAmount;
                    else
                        trialBalance[entry.CrAccountNumber].CrAmount = -entry.CrAmount;
                }
            }

            // Add B/F balances
            foreach (var entry in entries)
            {
                if (!trialBalance.ContainsKey(entry.DrAccountNumber))
                    trialBalance[entry.DrAccountNumber].DrBalanceBroughtForward = entry.DrBalanceBroughtForward;

                if (!trialBalance.ContainsKey(entry.CrAccountNumber))
                    trialBalance[entry.CrAccountNumber].CrBalanceBroughtForward = entry.CrBalanceBroughtForward;
            }

            return trialBalance;
        }

        private static DateTime SetDateToBeginningOfYear()
        {
            return new DateTime(DateTime.Now.Year, 1, 1);
        }

        private static DateTime SetDateLastfYear()
        {
            return new DateTime(DateTime.Now.Year, 12, 31);
        }













        //public async Task<Data.Account> CreateAccountForBranch(string BranchId, string externalBranchCode, Data.ChartOfAccount chartOfAccount)
        //{
        //    Data.Account account1 = new Data.Account();
        //    var account = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId.Equals(chartOfAccount.Id) && x.AccountOwnerId.Equals(BranchId));
        //    if (account.Any())
        //    {
        //        account1 = account.FirstOrDefault();
        //    }
        //    var branch = await GetBranchCodeAsync(BranchId);
        //    var accountManagementPosition = GetAccountNumberManagementPosition(chartOfAccount);

        //    var model = new Commands.AddAccountCommand
        //    {
        //        AccountOwnerId = BranchId,
        //        BranchCodeX = branch.branchCode,
        //        AccountName = chartOfAccount.LabelEn + " " + branch.name,
        //        AccountNumber = chartOfAccount.AccountNumber,
        //        AccountNumberNetwok = (chartOfAccount.AccountNumber.PadRight(6, '0') + branch.bank.bankCode + externalBranchCode).PadRight(12, '0')+ accountManagementPosition,
        //        AccountNumberCU = (chartOfAccount.AccountNumber.PadRight(6, '0') + externalBranchCode).PadRight(9, '0')+ accountManagementPosition,
        //        ChartOfAccountId = chartOfAccount.Id,
        //        AccountTypeId = BranchId,
        //        AccountCategoryId = chartOfAccount.AccountCartegoryId
        //    };

        //    await _mediator.Send(model);

        //    account = _accountRepository.FindBy(x => x.ChartOfAccountId.Equals(chartOfAccount.Id) && x.BranchId.Equals(BranchId));
        //    if (account.Any())
        //    {
        //        account1 = account.FirstOrDefault();
        //    }
        //    return account1;
        //}


        public Data.AccountingEntry CreateEntry(Data.Account account, EntryTempData item, string InitiatorId, DateTime TransactionDate)
        {
            Data.AccountingEntry model = new();

            model.BankId = _userInfoToken.BankId;
            model.BranchId = item.BranchId;
            model.ReferenceID = item.Reference;
            model.EntryDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
            model.EntryType = (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? AccountOperationType.DEBIT.ToString() : AccountOperationType.CREDIT.ToString();
            model.CurrentBalance = item.Amount;
            model.AccountId = account.Id;
            model.CrAccountNumber = (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? null : account.AccountNumberCU;
            model.DrAccountNumber = (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? account.AccountNumberCU : null;
            model.DrAmount = (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? item.Amount : 0;
            model.CrAmount = (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? 0 : item.Amount;
            model.CrAccountId= (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? "" : item.AccountId;
            model.DrAccountId = (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? item.AccountId : "";
            model.IsAuxilaryEntry = false;
            model.Amount = (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? -1 * item.Amount : item.Amount;
            model.CrCurrentBalance = (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? 0 : account.CurrentBalance;
            model.DrCurrentBalance = (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? account.CurrentBalance : 0;
            model.DrBalanceBroughtForward = (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? account.LastBalance : 0;
            model.CrBalanceBroughtForward = (item.BookingDirection.ToUpper() == AccountOperationType.DEBIT.ToString().ToUpper()) ? 0 : account.LastBalance;
            model.Status = PostingStatus.Posted.ToString();
            model.AccountNumber = account.AccountNumberCU;
            model.AccountNumberReference = account.AccountNumberReference;
            model.ValueDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
            model.Source = "SYSTEM";
            model.Representative = "SYSTEM";
            model.InitiatorId = InitiatorId;
            model.AccountCartegory = account.AccountCategoryId;
            model.Naration = item.Description;

            model.OperationType = item.BookingDirection.ToUpper();
            model.EventCode = "ManualEntry";
            model.Currency = Currency.GetCurrency();
            model.Id = BaseUtilities.GenerateInsuranceUniqueNumber(7, _userInfoToken.BankCode + item.BranchId);
            model.ValueDate = TransactionDate;
            model.ExternalBranchId = item.ExternalBranchId;
            return model;
        }


        public async Task<Data.Account> GetAccountForProcessing(string ChartOfAccountId, AddTransferEventCommand command)
        {
            IQueryable<Data.Account> account = null;
            Data.Account accountResult = new Data.Account();
            var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(ChartOfAccountId);
            chartOfAccount.ChartOfAccount = await _chartOfaccountRepository.FindAsync(chartOfAccount.ChartOfAccountId);


            if (chartOfAccount != null)
            {
                if (command.IsInterBranchTransaction)
                {
                    account = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId.Equals(chartOfAccount.Id) && x.AccountOwnerId.Equals(command.ExternalBranchId));
                }
                else
                {
                    account = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId.Equals(chartOfAccount.Id) && x.AccountOwnerId.Equals(_userInfoToken.BranchId));
                }

                if (account.Any() == false)
                {
                    var accountNumber = (command.IsInterBranchTransaction) ? chartOfAccount.ChartOfAccount.AccountNumber + "" + _userInfoToken.BranchCode : chartOfAccount.ChartOfAccount.AccountNumber + "" + GetBranchCodeAsync(command.ExternalBranchId);

                    var model = new Commands.AddAccountCommand
                    {
                        AccountOwnerId = _userInfoToken.BranchId,
                        AccountNumberManagementPosition = chartOfAccount.PositionNumber.PadRight(3, '0'),
                        OwnerBranchCode = _userInfoToken.BranchCode,
                        AccountName = chartOfAccount.Description + " " + _userInfoToken.BranchName,
                        AccountNumber = chartOfAccount.ChartOfAccount.AccountNumber,
                        AccountNumberNetwok = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BankCode + _userInfoToken.BranchCode).PadRight(12, '0') + chartOfAccount.PositionNumber.PadRight(3, '0'),

                        AccountNumberCU = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BranchCode).PadRight(9, '0') + chartOfAccount.PositionNumber.PadRight(3, '0'),
                        ChartOfAccountManagementPositionId = chartOfAccount.Id,
                        AccountTypeId = _userInfoToken.BranchId,
                        AccountCategoryId = chartOfAccount.ChartOfAccount.AccountCartegoryId
                    };
                    await _mediator.Send(model);
                    var BranchId = (command.IsInterBranchTransaction) ? command.ExternalBranchId : _userInfoToken.BranchId;
                    accountResult = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId.Equals(chartOfAccount.Id) && x.BranchId.Equals(BranchId)).FirstOrDefault();

                }
                else
                {
                    accountResult = account.FirstOrDefault();
                }


            }
            return accountResult;
        }

 
        public async Task<Data.Account> GetHomeliaisonAwayAccount(MakeAccountPostingCommand command)
        {
            bool IsInterBranchProcessing = false;
            AmountCollection liaisonProperty = new AmountCollection();
            Data.Account TellerAccount; // Initialize account to null

            var entryRuleId = command.AmountCollection[0].GetLiaisonEventCode(command.ProductId);
            var Entries = GetAccountEntryRuleByProductID(entryRuleId);
            TellerAccount = await GetAccountFromAccountingEntryRuleLiaisonAsync(Entries, _userInfoToken.BranchId, command.ExternalBranchId, _userInfoToken.BranchCode, command.ExternalBranchCode);


            return TellerAccount;
        }
        public async Task<Data.Account> GetHomeliaisonAwayAccount(Data.BulkTransaction.MakeAccountPostingCommand command)
        {
            bool IsInterBranchProcessing = false;
            AmountCollection liaisonProperty = new AmountCollection();
            Data.Account TellerAccount; // Initialize account to null

            var entryRuleId = command.AmountCollection[0].GetLiaisonEventCode(command.ToProductId);
            var Entries = GetAccountEntryRuleByProductID(entryRuleId);
            TellerAccount = await GetAccountFromAccountingEntryRuleLiaisonAsync(Entries, _userInfoToken.BranchId, command.ExternalBranchId, _userInfoToken.BranchCode, command.ExternalBranchCode);


            return TellerAccount;
        }

        public async Task<Data.Account> GetAwayLiaisonAccountHome(MakeAccountPostingCommand command)
        {
            bool IsInterBranchProcessing = false;
            AmountCollection liaisonProperty = new AmountCollection();
            Data.Account TellerAccount; // Initialize account to null

            var entryRuleId = command.AmountCollection[0].GetLiaisonEventCode(command.ProductId);
            var Entries = GetAccountEntryRuleByProductID(entryRuleId);
            TellerAccount = await GetAccountFromAccountingEntryRuleLiaisonAsync(Entries, command.ExternalBranchId, _userInfoToken.BranchId, command.ExternalBranchCode, _userInfoToken.BranchCode);


            return TellerAccount;
        }
        public async Task<Data.Account> GetLiaisonAccount(AddTransferEventCommand command)
        {
            bool IsInterBranchProcessing = false;
            AmountCollection liaisonProperty = new AmountCollection();
            Data.Account TellerAccount; // Initialize account to null

            var entryRuleId = command.AmountCollection[0].GetLiaisonEventCode(command.FromProductId);
            var Entries = GetAccountEntryRuleByProductID(entryRuleId);
            TellerAccount = await GetAccountFromAccountingEntryRuleAsync(Entries, command.ExternalBranchId, command.ExternalBranchCode, command.TransactionReferenceId.Contains("BWRM"));

            return TellerAccount;
        }

        public async Task<Data.Account> GetAccount(string EventCode, string BranchId, string BranchCode, bool remittanceStatus = false)
        {

            Data.Account TellerAccount; // Initialize account to null
            Data.AccountingRuleEntry accountingRuleEntryAccount = GetAccountEntryRuleByEventCode(EventCode);


            TellerAccount = await GetAccountFromAccountingEntryRuleAsync(accountingRuleEntryAccount, BranchId, BranchCode, remittanceStatus);

            return TellerAccount;
        }
    

        private async Task<Data.Account> GetProductAccount(AccountingRuleEntry ruleEntry, string BranchId, string branchCode)
        {
            Data.Account TellerAccount = null;
            string errorMessage = "";
            if (ruleEntry != null) // Add null check for Entries and DeterminationAccountId
            {

                var chart = await _chartOfAccountManagementPositionRepository.FindAsync(ruleEntry.DeterminationAccountId);
                var tellerAccount = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == chart.Id && x.AccountOwnerId == BranchId).AsNoTracking();
                if (tellerAccount.Any())
                {
                    TellerAccount = tellerAccount.FirstOrDefault();

                }
                else
                {
                    TellerAccount = await CreateAccountForBranchByChartOfAccountIdAsync(ruleEntry.DeterminationAccountId, BranchId, branchCode);
                }



            }
            else
            {

                errorMessage = $"Product Account{ruleEntry.EventCode}  not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand.AccountingEntriesServices.GetAccountFromAccountingEntryRuleAsync",
                    ruleEntry, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            return TellerAccount;
        }


        public async Task<Data.Account> GetAccountFromAccountingEntryRuleLiaisonAsync(AccountingRuleEntry ruleEntry, string OwnerBranchId, string LiaisonBranchId, string OwnerBranchCode, string LiaisonBranchCode, bool RemitanceStatus = false)
        {
            Data.Account TellerAccount = null;
            string errorMessage = "";
            if (ruleEntry != null) // Add null check for Entries and DeterminationAccountId
            {

                var chart = await _chartOfAccountManagementPositionRepository.FindAsync(ruleEntry.DeterminationAccountId);
                var tellerAccount = _accountRepository.FindBy(x => x.AccountOwnerId == OwnerBranchId.Trim() && x.LiaisonId == LiaisonBranchId.Trim());
                if (tellerAccount.Any())
                {
                    TellerAccount = tellerAccount.FirstOrDefault();

                }
                else
                {
                    TellerAccount = await CreateLiaisonAccountForAwayBranchWithChartOfAccountIdAsync(ruleEntry.DeterminationAccountId, OwnerBranchId, LiaisonBranchId, OwnerBranchCode, LiaisonBranchCode);
                }
            }
            else
            {

                errorMessage = $"Product Account{ruleEntry.EventCode} not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand.AccountingEntriesServices.GetAccountFromAccountingEntryRuleAsync",
                    ruleEntry, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            return TellerAccount;
        }

        private async Task<Data.Account> GetAccountFromAccountingEntryRuleAsync(AccountingRuleEntry ruleEntry, string BranchId, string branchCode, bool RemitanceStatus = false)
        {
            Data.Account TellerAccount = null;
            string errorMessage = "";
            if (ruleEntry != null) // Add null check for Entries and DeterminationAccountId
            {

                var chart = await _chartOfAccountManagementPositionRepository.FindAsync(ruleEntry.DeterminationAccountId);
                var tellerAccount = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == chart.Id && x.AccountOwnerId == BranchId).AsNoTracking();
                if (tellerAccount.Any())
                {
                    TellerAccount = tellerAccount.FirstOrDefault();

                }
                else
                {
                    TellerAccount = await CreateAccountForBranchByChartOfAccountIdAsync(ruleEntry.DeterminationAccountId, BranchId, branchCode);
                }



            }
            else
            {

                errorMessage = $"Product Account{ruleEntry.EventCode} not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand.AccountingEntriesServices.GetAccountFromAccountingEntryRuleAsync",
                    ruleEntry, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            return TellerAccount;
        }

        private async Task<(Data.Account sourceAccount, Data.Account destinationAccount)> GetAccountVectorFromAccountingEntryRuleAsync(
        AccountingRuleEntry ruleEntry,
        string branchId,
        string branchCode,
        bool remitanceStatus = false)
        {
            if (ruleEntry == null)
            {
                string errorMessage = $"Product Account{ruleEntry?.EventCode} not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(
                    _userInfoToken.Email,
                    "MakeAccountPostingCommand.AccountingEntriesServices.GetAccountVectorFromAccountingEntryRuleAsync",
                    ruleEntry,
                    errorMessage,
                    LogLevelInfo.Information.ToString(),
                    422,
                    _userInfoToken.Token);
                throw new Exception(errorMessage);
            }

            Data.Account sourceAccount = null;
            Data.Account destinationAccount = null;

            if (branchId == _userInfoToken.BranchId)
            {
                // Handle same branch scenario
                var chartSource = await _chartOfAccountManagementPositionRepository.FindAsync(ruleEntry.DeterminationAccountId);
                var chartDestination = await _chartOfAccountManagementPositionRepository.FindAsync(ruleEntry.BalancingAccountId);

                var sourceAccounts = _accountRepository
                    .FindBy(x => x.ChartOfAccountManagementPositionId == chartSource.Id &&
                                x.AccountOwnerId == branchId)
                    .AsNoTracking();

                var destinationAccounts = _accountRepository
                    .FindBy(x => x.ChartOfAccountManagementPositionId == chartDestination.Id &&
                                x.AccountOwnerId == branchId)
                    .AsNoTracking();

                sourceAccount = sourceAccounts.FirstOrDefault() ??
                    await CreateAccountForBranchByChartOfAccountIdAsync(ruleEntry.DeterminationAccountId, branchId, branchCode);

                destinationAccount = destinationAccounts.FirstOrDefault() ??
                    await CreateAccountForBranchByChartOfAccountIdAsync(ruleEntry.BalancingAccountId, branchId, branchCode);
            }
            else
            {
                // Handle different branch scenario
                var chartSource = await _chartOfAccountManagementPositionRepository.FindAsync(ruleEntry.DeterminationAccountId);

                var sourceAccounts = _accountRepository
                    .FindBy(x => x.ChartOfAccountManagementPositionId == chartSource.Id &&
                                x.LiaisonId == branchId &&
                                x.AccountOwnerId == _userInfoToken.BranchId)
                    .AsNoTracking();

                sourceAccount = sourceAccounts.FirstOrDefault();

                if (sourceAccount == null)
                {
                    if (ruleEntry.IsLiaison())
                    {
                        sourceAccount = await CreateLiaisonAccountForBranchByChartOfAccountIdAsync(
                            ruleEntry.DeterminationAccountId,
                            branchId,
                            branchCode,
                            remitanceStatus);

                        destinationAccount = await CreateLiaisonAccountForBranchByChartOfAccountIdAsync(
                            ruleEntry.BalancingAccountId,
                            branchId,
                            branchCode,
                            remitanceStatus);
                    }
                    else
                    {
                        sourceAccount = await CreateAccountForBranchByChartOfAccountIdAsync(
                            ruleEntry.DeterminationAccountId,
                            branchId,
                            branchCode);

                        destinationAccount = await CreateAccountForBranchByChartOfAccountIdAsync(
                            ruleEntry.BalancingAccountId,
                            branchId,
                            branchCode);
                    }
                }
            }

            return (sourceAccount, destinationAccount);
        }


        public async Task<(Data.Account DeterminantAccount, Data.Account BalancingAccount)> GetCashMovementAccountByEventCode(string eventCode, string branchID, string branchCode)
        {
            Data.Account sourceAccount = null;
            Data.Account destinationAccount = null;
            string errorMessage = "";
            var ruleEntry = this.GetAccountingEntryRuleByEventCode(eventCode);
            if (ruleEntry == null)
            {
                errorMessage = $"EventCode:{eventCode} AccountingEntryRule ID  not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                    "GetAccountingEntryRuleByEventCode", errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            if (ruleEntry.DeterminationAccountId == null)
            {
                errorMessage = $"EventCode Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                     "GetAccountingEntryRuleByEventCode", errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            var model = await GetAccountVectorFromAccountingEntryRuleAsync(ruleEntry, branchID, branchCode);
            return model;
        }

        public async Task<(Data.Account DeterminantAccount, Data.Account BalancingAccount, AccountOperationType BookingDirection)> GetCashMovementAccountWithBookingDirectionByEventCode(string eventCode, string branchID, string branchCode)
        {
            Data.Account sourceAccount = null;
            Data.Account destinationAccount = null;
            string errorMessage = "";
            var ruleEntry = this.GetAccountingEntryRuleByEventCode(eventCode);
            if (ruleEntry == null)
            {
                errorMessage = $"EventCode:{eventCode} AccountingEntryRule ID  not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                    "GetCashMovementAccountWithBookingDirectionByEventCode", errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            if (ruleEntry.DeterminationAccountId == null)
            {
                errorMessage = $"EventCode Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                     "GetCashMovementAccountWithBookingDirectionByEventCode", errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            var model = await GetAccountVectorFromAccountingEntryRuleAsync(ruleEntry, branchID, branchCode);
            return ConvertWithApproriateBookingDirection(ruleEntry, model);
        }

        private (Data.Account DeterminantAccount, Data.Account BalancingAccount, AccountOperationType BookingDirection) ConvertWithApproriateBookingDirection(AccountingRuleEntry ruleEntry, (Data.Account sourceAccount, Data.Account destinationAccount) model)
        {
            var bookingDirection = AccountOperationType.DEBIT.ToString().ToLower().Equals(ruleEntry.BookingDirection.ToLower()) ? AccountOperationType.DEBIT : AccountOperationType.CREDIT;
            return (model.sourceAccount, model.destinationAccount, bookingDirection);
        }
        public async Task<Data.Account> GetTellerAccount(MakeAccountPostingCommand command)
        {
            Data.Account TellerAccount; // Initialize account to null
            string errorMessage = "";
            var ruleEntry = this.GetAccountingEntryRuleByEventCode(command.Source);
            if (ruleEntry == null)
            {
                errorMessage = $"Primary Teller AccountingEntryRule ID  not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            if (ruleEntry.DeterminationAccountId == null)
            {
                errorMessage = $"Primary Teller Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }


            if (ruleEntry != null && ruleEntry.DeterminationAccountId != null) // Add null check for Entries and DeterminationAccountId
            {
                var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(ruleEntry.DeterminationAccountId);
                var tellerAccount = _accountRepository.FindBy(x => x.AccountOwnerId == _userInfoToken.BranchId && x.ChartOfAccountManagementPositionId == chartOfAccount.Id).AsNoTracking();
                if (tellerAccount.Any())
                {
                    TellerAccount = tellerAccount.FirstOrDefault();

                }
                else
                {
                    TellerAccount = await CreateAccountForBranchByChartOfAccountIdAsync(ruleEntry.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                }

            }
            else
            {

                errorMessage = $"Primary Teller Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            return TellerAccount;
        }

        public async Task<Data.Account> GetTellerAccount(MakeRemittanceCommand command)
        {
            Data.Account TellerAccount; // Initialize account to null
            string errorMessage = "";
            var ruleEntry = this.GetAccountingEntryRuleByEventCode(command.Source);
            if (ruleEntry == null)
            {
                errorMessage = $"Primary Teller AccountingEntryRule ID  not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeRemittanceDepositCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            if (ruleEntry.DeterminationAccountId == null)
            {
                errorMessage = $"Primary Teller Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeRemittanceDepositCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }


            if (ruleEntry != null && ruleEntry.DeterminationAccountId != null) // Add null check for Entries and DeterminationAccountId
            {
                var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(ruleEntry.DeterminationAccountId);
                var tellerAccount = _accountRepository.FindBy(x => x.AccountOwnerId == _userInfoToken.BranchId && x.ChartOfAccountManagementPositionId == chartOfAccount.Id).AsNoTracking();
                if (tellerAccount.Any())
                {
                    TellerAccount = tellerAccount.FirstOrDefault();

                }
                else
                {
                    TellerAccount = await CreateAccountForBranchByChartOfAccountIdAsync(ruleEntry.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                }

            }
            else
            {

                errorMessage = $"Primary Teller Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeRemittanceDepositCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            return TellerAccount;
        }

        public async Task<Data.Account> GetTellerAccount(Data.BulkTransaction.MakeAccountPostingCommand command)
        {
            Data.Account TellerAccount; // Initialize account to null
            string errorMessage = "";
            var ruleEntry = this.GetAccountingEntryRuleByEventCode(command);
            if (ruleEntry == null)
            {
                errorMessage = $"Primary Teller AccountingEntryRule ID  not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            if (ruleEntry.DeterminationAccountId == null)
            {
                errorMessage = $"Primary Teller Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }


            if (ruleEntry != null && ruleEntry.DeterminationAccountId != null) // Add null check for Entries and DeterminationAccountId
            {
                var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(ruleEntry.DeterminationAccountId);
                var tellerAccount = _accountRepository.FindBy(x => x.AccountOwnerId == _userInfoToken.BranchId && x.ChartOfAccountManagementPositionId == chartOfAccount.Id).AsNoTracking();
                if (tellerAccount.Any())
                {
                    TellerAccount = tellerAccount.FirstOrDefault();

                }
                else
                {
                    TellerAccount = await CreateAccountForBranchByChartOfAccountIdAsync(ruleEntry.DeterminationAccountId, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                }

            }
            else
            {

                errorMessage = $"Primary Teller Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            return TellerAccount;
        }

        public async Task<Data.Account> GetAccountByEventCode(Commands.AmountEventCollection command, string branchID, string BranchCode)
        {
            Data.Account TellerAccount; // Initialize account to null
            string errorMessage = "";
            var ruleEntry = this.GetAccountingEntryRuleByEventCode(command.EventCode);
            if (ruleEntry == null)
            {
                errorMessage = $"EventCode AccountingEntryRule ID  not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            if (ruleEntry.DeterminationAccountId == null)
            {
                errorMessage = $"EventCode Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }


            if (ruleEntry != null && ruleEntry.DeterminationAccountId != null) // Add null check for Entries and DeterminationAccountId
            {
                var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(ruleEntry.DeterminationAccountId);
                var tellerAccount = _accountRepository.FindBy(x => x.AccountOwnerId == branchID && x.ChartOfAccountManagementPositionId == chartOfAccount.Id).AsNoTracking();
                if (tellerAccount.Any())
                {
                    TellerAccount = tellerAccount.FirstOrDefault();

                }
                else
                {
                    TellerAccount = await CreateAccountForBranchByChartOfAccountIdAsync(ruleEntry.DeterminationAccountId, branchID, BranchCode);
                }

            }
            else
            {

                errorMessage = $"EventCode Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            return TellerAccount;
        }

        public async Task<Data.Account> GetAccountByEventCode(Data.BulkTransaction.AmountEventCollection command, string branchID, string BranchCode)
        {
            Data.Account TellerAccount; // Initialize account to null
            string errorMessage = "";
            var ruleEntry = this.GetAccountingEntryRuleByEventCode(command.EventCode);
            if (ruleEntry == null)
            {
                errorMessage = $"EventCode AccountingEntryRule ID  not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            if (ruleEntry.DeterminationAccountId == null)
            {
                errorMessage = $"EventCode Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }


            if (ruleEntry != null && ruleEntry.DeterminationAccountId != null) // Add null check for Entries and DeterminationAccountId
            {
                var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(ruleEntry.DeterminationAccountId);
                var tellerAccount = _accountRepository.FindBy(x => x.AccountOwnerId == branchID && x.ChartOfAccountManagementPositionId == chartOfAccount.Id).AsNoTracking();
                if (tellerAccount.Any())
                {
                    TellerAccount = tellerAccount.FirstOrDefault();

                }
                else
                {
                    TellerAccount = await CreateAccountForBranchByChartOfAccountIdAsync(ruleEntry.DeterminationAccountId, branchID, BranchCode);
                }

            }
            else
            {

                errorMessage = $"EventCode Account not set contact your administrator for support.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            return TellerAccount;
        }

        public async Task<Branch> GetBranchCodeAsync(string externalBranchId)
        {
            var BranchInfo = await APICallHelper.GetBankInfos(_pathHelper, _userInfoToken, externalBranchId, _userInfoToken.Token);
            return (BranchInfo == null) ? throw new Exception($"Retrieving BranchCodeX for branchId {externalBranchId} failed please contact system administrator") : BranchInfo;
        }

        public async Task<Data.Account> GetAccountBasedOnChartOfAccountID(string? balancingAccountId, string BranchId, string BranchCode)
        {

            var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(balancingAccountId);
            chartOfAccount.ChartOfAccount = await _chartOfaccountRepository.FindAsync(chartOfAccount.ChartOfAccountId);
            var account = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId.Equals(chartOfAccount.Id) && x.AccountOwnerId == (BranchId));
            if (account.Any())
            {
                return await account.FirstOrDefaultAsync();
            }
            var model = new Commands.AddAccountCommand();
            if (chartOfAccount.ChartOfAccount.AccountNumber.Contains("451"))
            {
                model = new Commands.AddAccountCommand
                {
                    AccountOwnerId = BranchId,
                    OwnerBranchCode = BranchCode,
                    AccountNumberManagementPosition = chartOfAccount.PositionNumber.PadRight(3, '0'),
                    AccountName = chartOfAccount.Description + " " + _userInfoToken.BranchName,
                    AccountNumber = chartOfAccount.ChartOfAccount.AccountNumber,
                    AccountNumberNetwok = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BankCode + BranchCode).PadRight(12, '0') + chartOfAccount.PositionNumber.PadRight(3, '0'),
                    AccountNumberCU = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + BranchCode).PadRight(9, '0') + chartOfAccount.PositionNumber.PadRight(3, '0'),
                    ChartOfAccountManagementPositionId = chartOfAccount.Id,
                    AccountCategoryId = chartOfAccount.ChartOfAccount.AccountCartegoryId
                };
            }
            else
            {
                  model = new Commands.AddAccountCommand
                {
                    AccountOwnerId = BranchId,
                    OwnerBranchCode = BranchCode,
                    AccountNumberManagementPosition = chartOfAccount.PositionNumber.PadRight(3, '0'),
                    AccountName = chartOfAccount.Description + " " + _userInfoToken.BranchName,
                    AccountNumber = chartOfAccount.ChartOfAccount.AccountNumber,
                    AccountNumberNetwok = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BankCode + BranchCode).PadRight(12, '0') + chartOfAccount.PositionNumber.PadRight(3, '0'),
                    AccountNumberCU = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + BranchCode).PadRight(9, '0') + chartOfAccount.PositionNumber.PadRight(3, '0'),
                    ChartOfAccountManagementPositionId = chartOfAccount.Id,
                    AccountCategoryId = chartOfAccount.ChartOfAccount.AccountCartegoryId
                };
            }

         

            await _mediator.Send(model);
            account = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId.Equals(chartOfAccount.Id) && x.AccountOwnerId.Equals(BranchId));
            if (account.Any())
            {
                return await account.FirstOrDefaultAsync();
            }
            else
            {
                throw new Exception($"Creation of account failed for branchId:{_userInfoToken.BranchCode} object: {JsonConvert.SerializeObject(model)}");
            }
        }


        public async Task<Data.Account> GetProductAccount(Data.BulkTransaction.AmountCollection productEventCode, string productName, string BranchId, string branchCode)
        {
            var productEntryRuleId = productEventCode.GetOperationEventCode(productName);
            var productEntryRule = GetAccountEntryRuleByProductID(productEntryRuleId);

            if (productEntryRule == null)
            {
                throw new NullReferenceException($"The productEntryRuleId:{productEntryRuleId} does not exist in the configured context.");
            }
            return await GetProductAccount(productEntryRule, BranchId, branchCode);

        }


        public async Task<Data.Account> GetProductAccount(AmountCollection? productEventCode, string productName, string BranchId, string branchCode)
        {
            var productEntryRuleId = productEventCode.GetOperationEventCode(productName);
            var productEntryRule = GetAccountEntryRuleByProductID(productEntryRuleId);

            if (productEntryRule == null)
            {
                throw new NullReferenceException($"The productEntryRuleId:{productEntryRuleId} does not exist in the configured context.");
            }
            return await GetProductAccount(productEntryRule, BranchId, branchCode);

        }

        public async Task<Data.Account> GetProductAccount(CollectionAmount? productEventCode, string productName, string BranchId, string branchCode)
        {
            var productEntryRuleId = productEventCode.GetOperationEventCode(productName);
            var productEntryRule = GetAccountEntryRuleByProductID(productEntryRuleId);

            if (productEntryRule == null)
            {
                throw new NullReferenceException($"The productEntryRuleId:{productEntryRuleId} does not exist in the configured context.");
            }
            return await GetProductAccount(productEntryRule, BranchId, branchCode);

        }

        public async Task<Data.Account> GetCommissionAccount(AmountCollection? productEventCode, string productName, string BranchId, string branchCode, bool IsRemittance = false)
        {
            string key = "";

            var productEntryRuleId = productEventCode.GetOperationEventCode(productName);
            if (productEntryRuleId.Contains("SourceBrachCommission_Account") || productEntryRuleId.Contains("SourceBrachCommission_Account"))
            {
                if (productEntryRuleId.Contains("SourceBrachCommission_Account"))
                {
                    productEntryRuleId = productEntryRuleId.Replace("SourceBrachCommission_Account", "Commission_Account");
                }
                else
                {
                    productEntryRuleId = productEntryRuleId.Replace("DestinationBranchCommission_Account", "Commission_Account");
                }
            }



            var productEntryRule = GetAccountEntryRuleByProductID(productEntryRuleId);

            return await GetAccountFromAccountingEntryRuleAsync(productEntryRule, BranchId, branchCode, IsRemittance);
        }
        public async Task<Data.Account> GetCommissionAccount(TransferAmountCollection productEventCode, string productName, string BranchId, string branchCode)
        {
            var productEntryRuleId = productEventCode.GetOperationEventCode(productName);
            var productEntryRule = GetAccountEntryRuleByProductID(productEntryRuleId);
            BranchId = productEventCode.IsDestination() ? BranchId : _userInfoToken.BranchId;
            return await GetAccountFromAccountingEntryRuleAsync(productEntryRule, BranchId, branchCode);
        }
        public async Task<List<AccountingEntryDto>> CashMovementAsync(string naration, string MemberReference, DateTime TransactionDate, Data.Account debitAccount, Data.Account creditAccount, decimal Amount, string PostingEventCommand, string TransactionReferenceId, string branchId, bool IsInterBranchTransaction = false, string externalBranchId = "NOT_SET")
        {
            try
            {
                if (debitAccount.AccountNumber.StartsWith("000")|| creditAccount.AccountNumber.StartsWith("000"))
                {
                    string message = $"There is a ROOT ACCOUNT EXCEPTION  makes the accounting posting impossible, Please contact admin";
                    throw new CustomBusinessException(message,  new Exception("DeterminantAccount:\n " + JsonConvert.SerializeObject(debitAccount) + "\n BalancingAccount:" + JsonConvert.SerializeObject(creditAccount)));
                }
                List<AccountingEntryDto> accountingEntryDtos = new List<AccountingEntryDto>();
                debitAccount = await UpdateAccountBalanceAsync(debitAccount, Amount, AccountOperationType.DEBIT, PostingEventCommand);
                creditAccount = await UpdateAccountBalanceAsync(creditAccount, Amount, AccountOperationType.CREDIT, PostingEventCommand);

                Booking crBooking = new Booking(naration, MemberReference, TransactionReferenceId, AccountOperationType.CREDIT, debitAccount, creditAccount, Amount, branchId, IsInterBranchTransaction, IsInterBranchTransaction, externalBranchId);
                Booking drBooking = new Booking(naration, MemberReference, TransactionReferenceId, AccountOperationType.DEBIT, debitAccount, creditAccount, Amount, branchId, IsInterBranchTransaction, IsInterBranchTransaction, externalBranchId);
                accountingEntryDtos.Add(CreateCreditEntry(crBooking, creditAccount, debitAccount, TransactionDate));
                accountingEntryDtos.Add(CreateDebitEntry(drBooking, creditAccount, debitAccount, TransactionDate));
                return accountingEntryDtos;
            }
            catch (CustomBusinessException ex)
            {
                ExceptionLogger.LogException(ex, PostingEventCommand, GetLogAction(PostingEventCommand));
                throw(ex);
            }
        }

        private LogAction GetLogAction(string postingEventCommand)
        {
            if (string.IsNullOrEmpty(postingEventCommand))
            {
                return LogAction.NothingHappened;
            }

            // Try to parse directly if the string matches an enum name
            if (Enum.TryParse<LogAction>(postingEventCommand, out LogAction result))
            {
                return result;
            }

            // Handle special cases with different naming conventions
            switch (postingEventCommand)
            {
                // Core banking operations
                case string s when s.Contains("Transfer", StringComparison.OrdinalIgnoreCase):
                    return s.Contains("Failed", StringComparison.OrdinalIgnoreCase)
                        ? LogAction.TransferFailed
                        : LogAction.TransferFunds;

                case string s when s.Contains("Deposit", StringComparison.OrdinalIgnoreCase):
                    return s.Contains("Failed", StringComparison.OrdinalIgnoreCase)
                        ? LogAction.CashDepositFailed
                        : LogAction.DepositMade;

                case string s when s.Contains("Withdrawal", StringComparison.OrdinalIgnoreCase):
                    return s.Contains("Failed", StringComparison.OrdinalIgnoreCase)
                        ? LogAction.CashWithdrawalFailed
                        : LogAction.WithdrawalRequested;

                case string s when s.Contains("Loan", StringComparison.OrdinalIgnoreCase):
                    if (s.Contains("Disbursement", StringComparison.OrdinalIgnoreCase))
                        return LogAction.LoanDisbursement;
                    else if (s.Contains("Approval", StringComparison.OrdinalIgnoreCase))
                        return LogAction.LoanApproved;
                    else if (s.Contains("Refinancing", StringComparison.OrdinalIgnoreCase))
                        return LogAction.LoanRescheduled;
                    else if (s.Contains("Refund", StringComparison.OrdinalIgnoreCase))
                        return LogAction.LoanRepayment;
                    else
                        return LogAction.LoanInquiryRequested;

                // Accounting operations
                case string s when s.Contains("Accounting", StringComparison.OrdinalIgnoreCase):
                    return LogAction.AccountingPosting;

                case string s when s.Contains("Posting", StringComparison.OrdinalIgnoreCase):
                    return LogAction.AccountingPosting;

                case string s when s.Contains("TrialBalance", StringComparison.OrdinalIgnoreCase):
                    return LogAction.Read;

                case string s when s.Contains("Query", StringComparison.OrdinalIgnoreCase):
                    return LogAction.Read;

                case string s when s.Contains("Get", StringComparison.OrdinalIgnoreCase):
                    return LogAction.Read;

                case string s when s.Contains("Command", StringComparison.OrdinalIgnoreCase):
                    if (s.Contains("Add", StringComparison.OrdinalIgnoreCase))
                        return LogAction.Create;
                    else if (s.Contains("Update", StringComparison.OrdinalIgnoreCase))
                        return LogAction.Update;
                    else if (s.Contains("Delete", StringComparison.OrdinalIgnoreCase))
                        return LogAction.Delete;
                    else
                        return LogAction.TransactionTrackerAccounting;

                // Cash operations
                case string s when s.Contains("Cash", StringComparison.OrdinalIgnoreCase):
                    if (s.Contains("Replenishment", StringComparison.OrdinalIgnoreCase))
                        return LogAction.CashClearingTransferFromCashReplenishmentCommand;
                    else if (s.Contains("Infusion", StringComparison.OrdinalIgnoreCase))
                        return LogAction.AddCashInfusionCommand;
                    else
                        return LogAction.TransactionTrackerAccounting;

                // Vault/Branch operations
                case string s when s.Contains("Vault", StringComparison.OrdinalIgnoreCase):
                    return LogAction.VaultInitilization;

                case string s when s.Contains("Branch", StringComparison.OrdinalIgnoreCase):
                    return LogAction.BranchToBranchTransferCashOut;

                // Remittance
                case string s when s.Contains("Remittance", StringComparison.OrdinalIgnoreCase):
                    if (s.Contains("CASHIN", StringComparison.OrdinalIgnoreCase))
                        return LogAction.MakeRemittanceCommandCASHIN;
                    else if (s.Contains("CASHOUT", StringComparison.OrdinalIgnoreCase))
                        return LogAction.MakeRemittanceCommandCASHOUT;
                    else
                        return LogAction.MakeRemittanceCommand;

                // HTTP/API
                case string s when s.Contains("HTTP", StringComparison.OrdinalIgnoreCase):
                    return LogAction.HttpRequestReception;

                case string s when s.Contains("API", StringComparison.OrdinalIgnoreCase):
                    return LogAction.APICallHelper;

                // Default case
                default:
                    // Try to find a close match by removing spaces, underscores, etc.
                    string normalizedCommand = Regex.Replace(postingEventCommand, @"[^a-zA-Z0-9]", "");

                    foreach (LogAction action in Enum.GetValues(typeof(LogAction)))
                    {
                        string normalizedAction = Regex.Replace(action.ToString(), @"[^a-zA-Z0-9]", "");

                        if (normalizedCommand.Contains(normalizedAction, StringComparison.OrdinalIgnoreCase) ||
                            normalizedAction.Contains(normalizedCommand, StringComparison.OrdinalIgnoreCase))
                        {
                            return action;
                        }
                    }

                    // If nothing matches, return a safe default
                    return LogAction.TransactionTrackerAccounting;
            }
        }

        public async Task<Data.Account> GetMFI_ChartOfAccount(string MFI_ChartOfAccountId, string BranchId, string branchCode)
        {
            Data.Account TellerAccount = null;
            string errorMessage = "";

            if (BranchId == _userInfoToken.BranchId)
            {
                var chart = await _chartOfAccountManagementPositionRepository.FindAsync(MFI_ChartOfAccountId);
                if (chart == null)
                {
                    errorMessage = $"microfinance chart of accountid not found";

                    throw new NullReferenceException(errorMessage);
                }
                var tellerAccount = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == chart.Id && x.AccountOwnerId == BranchId).AsNoTracking();
                if (tellerAccount.Any())
                {
                    TellerAccount = tellerAccount.FirstOrDefault();

                }
                else
                {
                    TellerAccount = await CreateAccountForBranchByChartOfAccountIdAsync(MFI_ChartOfAccountId, BranchId, branchCode);
                }
            }
            else
            {
                var chart0 = await _chartOfAccountManagementPositionRepository.FindAsync(MFI_ChartOfAccountId);
                var tellerAccount = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == chart0.Id && x.AccountOwnerId == BranchId).AsNoTracking();
                if (tellerAccount.Any())
                {
                    TellerAccount = tellerAccount.FirstOrDefault();

                }
                else
                {
                    TellerAccount = await CreateAccountForBranchByChartOfAccountIdAsync(MFI_ChartOfAccountId, BranchId, branchCode);
                }
            }



            return TellerAccount;
        }

        public Data.Account? GetAccountByAccountNumber(string accountNumber, string branchId)
        {
            var xAccount = _accountRepository.All.Where(a => a.BranchId == branchId)
                                                 .AsEnumerable() // Switch to LINQ-to-Objects
                                                 .Where(a => a.AccountNumber.PadRight(6, '0') == accountNumber)
                                                 .ToList();
            if (xAccount.Any())
            {
                return xAccount.FirstOrDefault();
            }
            else
            {
                throw new Exception("There is no accountnumber:" + accountNumber + " for this branch " + branchId);
            }
        }

        public async Task<Data.Account> GetTransferCommissionAccount(Data.BulkTransaction.AmountCollection? productEventCode, string productName, string BranchId, string branchCode, bool IsRemittance = false)
        {
            string key = "";

            var productEntryRuleId = productEventCode.GetOperationEventCode(productName);
            

            var productEntryRule = GetAccountEntryRuleByProductID(productEntryRuleId);

            return await GetAccountFromAccountingEntryRuleAsync(productEntryRule, BranchId, branchCode, IsRemittance);
        }

        public async Task<Data.Account> GetCommissionAccount(OperationEventAttributeTypes operationType, AmountCollection? productEventCode, string productName, string BranchId, string branchCode, bool IsRemittance = false)
        {
            string key = "";

            var productEntryRuleId = productEventCode.GetOperationEventCode(productName);
            if (productEntryRuleId.Contains("SourceBrachCommission_Account") || productEntryRuleId.Contains("DestinationBranchCommission_Account"))
            {
                if (productEntryRuleId.Contains("SourceBrachCommission_Account"))
                {
                    productEntryRuleId = productEntryRuleId.Replace("SourceBrachCommission_Account", OperationEventAttributeTypes.deposit.Equals(operationType) ? "CashIn_Commission_Account" : "CashOut_Commission_Account");
                }
                else
                {
                    productEntryRuleId = productEntryRuleId.Replace("DestinationBranchCommission_Account", OperationEventAttributeTypes.deposit.Equals(operationType) ? "CashIn_Commission_Account" : "CashOut_Commission_Account");
                }
            }



            var productEntryRule = GetAccountEntryRuleByProductID(productEntryRuleId);

            return await GetAccountFromAccountingEntryRuleAsync(productEntryRule, BranchId, branchCode, IsRemittance);
        }

        public async Task<Data.Account?> GetAccountByAccountNumberInsteadHavingId(string chartOfAccountId, string branchId, string branchCode)
        {
            var xAccount = _accountRepository.All.Where(a => a.BranchId == branchId && a.ChartOfAccountManagementPositionId.Equals(chartOfAccountId));
            if (xAccount.Any())
            {
                return await xAccount.FirstOrDefaultAsync();
            }
            else
            {
                return await GetAccountBasedOnChartOfAccountID(chartOfAccountId, branchId, branchCode);
            }
        }

        public async Task<Data.Account> GetAccountUsingMFIChart(string? ChartOfAccountId, string toBranchId, string toBranchCode)
        {

            Data.Account Account; // Initialize account to null
            string errorMessage = "";

            Account = await CreateAccountForBranchByChartOfAccountIdAsync(ChartOfAccountId, toBranchId, toBranchCode);

            return Account;

        }
        public async Task<Data.Account> GetAccountUsingMFIChartForliason(string? ChartOfAccountId, string toBranchId, string toBranchCode)
        {

            Data.Account Account; // Initialize account to null
            string errorMessage = "";

            Account = await CreateLiaisonAccountForBranchByChartOfAccountIdAsync(ChartOfAccountId, toBranchId, toBranchCode,false);

            return Account;

        }

        public async Task<Data.Account> GetHomeLiaisonAccount(MakeAccountPostingCommand command)
        {
            bool IsInterBranchProcessing = false;
            AmountCollection liaisonProperty = new AmountCollection();
            Data.Account TellerAccount; // Initialize account to null

            var entryRuleId = command.AmountCollection[0].GetLiaisonEventCode(command.ProductId);
            var Entries = GetAccountEntryRuleByProductID(entryRuleId);
            TellerAccount = await GetAccountFromAccountingEntryRuleAsync(Entries, command.ExternalBranchId, command.ExternalBranchCode, command.TransactionReferenceId.Contains("BWRM"));


            return TellerAccount;
        }
        /// <summary>
        /// Control if doubble entry validation has been activated for a user accounting event.
        /// </summary>
        /// <param name="systemId"></param>
        /// <returns></returns>
        
        //public async Task<bool> CheckIfDoubbleVaidationIsRequired(string systemId)
        //{
        //    var AccountingRule = await _accountingRuleRepository.FindBy(x => x.System_Id == systemId).FirstOrDefaultAsync();
        //    return AccountingRule.IsDoubleValidationNeeded.HasValue;
        //}
        public async Task<bool> CheckIfDoubbleVaidationIsRequired(string systemId)
        {
            var auditTrailRepository = _mongoUnitOfWork.GetRepository<AccountingEventRule>();
            // Retrieve the BudgetName entity with the specified ID from the repository
            var entity = await auditTrailRepository.GetByIdAsync(systemId);
            return entity.IsDoubleValidationNeeded;

        }
        public async Task<bool> IsBranchEligibility(string systemId,string branchId)
        {
            var auditTrailRepository = _mongoUnitOfWork.GetRepository<AccountingEventRule>();
            // Retrieve the BudgetName entity with the specified ID from the repository
            var entity = await auditTrailRepository.GetByIdAsync(branchId);
            return entity.ListOfEligibleBranchId.Contains(branchId);

        }


        public async Task<Data.Account> GetAccountForProcessing(string ChartOfAccountId, MakeNonCashAccountAdjustmentCommand command)
        {
            IQueryable<Data.Account> account = null;
            Data.Account accountResult = new Data.Account();
            var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(ChartOfAccountId);
            chartOfAccount.ChartOfAccount = await _chartOfaccountRepository.FindAsync(chartOfAccount.ChartOfAccountId);


            if (chartOfAccount != null)
            {
            
                    account = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId.Equals(chartOfAccount.Id) && x.AccountOwnerId.Equals(_userInfoToken.BranchId));
                

                if (account.Any() == false)
                {
                    var accountNumber = chartOfAccount.ChartOfAccount.AccountNumber + "" + _userInfoToken.BranchCode;
                    var model = new Commands.AddAccountCommand
                    {
                        AccountOwnerId = _userInfoToken.BranchId,
                        AccountNumberManagementPosition = chartOfAccount.PositionNumber.PadRight(3, '0'),
                        OwnerBranchCode = _userInfoToken.BranchCode,
                        AccountName = chartOfAccount.Description + " " + _userInfoToken.BranchName,
                        AccountNumber = chartOfAccount.ChartOfAccount.AccountNumber,
                        AccountNumberNetwok = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BankCode + _userInfoToken.BranchCode).PadRight(12, '0') + chartOfAccount.PositionNumber.PadRight(3, '0'),

                        AccountNumberCU = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BranchCode).PadRight(9, '0') + chartOfAccount.PositionNumber.PadRight(3, '0'),
                        ChartOfAccountManagementPositionId = chartOfAccount.Id,
                        AccountTypeId = _userInfoToken.BranchId,
                        AccountCategoryId = chartOfAccount.ChartOfAccount.AccountCartegoryId
                    };
                    await _mediator.Send(model);
                    var BranchId =  _userInfoToken.BranchId;
                    accountResult = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId.Equals(chartOfAccount.Id) && x.BranchId.Equals(BranchId)).FirstOrDefault();

                }
                else
                {
                    accountResult = account.FirstOrDefault();
                }


            }
            return accountResult;
        }
        public async Task<Data.Account> GetTransferCommissionAccount(OperationEventAttributeTypes operationType, Data.BulkTransaction.AmountCollection? productEventCode, string productName, string BranchId, string branchCode, bool IsRemittance = false)
        {
            string key = "";

            var productEntryRuleId = productEventCode.GetOperationEventCode(productName);
            if (productEntryRuleId.Contains("SourceBrachCommission_Account") || productEntryRuleId.Contains("DestinationBranchCommission_Account"))
            {
                if (productEntryRuleId.Contains("SourceBrachCommission_Account"))
                {
                    productEntryRuleId = productEntryRuleId.Replace("SourceBrachCommission_Account", OperationEventAttributeTypes.deposit.Equals(operationType) ? "CashIn_Commission_Account" : "CashOut_Commission_Account");
                }
                else
                {
                    productEntryRuleId = productEntryRuleId.Replace("DestinationBranchCommission_Account", OperationEventAttributeTypes.deposit.Equals(operationType) ? "CashIn_Commission_Account" : "CashOut_Commission_Account");
                }
            }



            var productEntryRule = GetAccountEntryRuleByProductID(productEntryRuleId);

            return await GetAccountFromAccountingEntryRuleAsync(productEntryRule, BranchId, branchCode, IsRemittance);
        }
       
        public async Task<Data.Account> GetAwayLiaisonAccountHome(Data.BulkTransaction.MakeAccountPostingCommand command)
        {
            bool IsInterBranchProcessing = false;
            AmountCollection liaisonProperty = new AmountCollection();
            Data.Account TellerAccount; // Initialize account to null

            var entryRuleId = command.AmountCollection[0].GetLiaisonEventCode(command.FromProductId);
            var Entries = GetAccountEntryRuleByProductID(entryRuleId);
            TellerAccount = await GetAccountFromAccountingEntryRuleLiaisonAsync(Entries, command.ExternalBranchId, _userInfoToken.BranchId, command.ExternalBranchCode, _userInfoToken.BranchCode);


            return TellerAccount;
        }

        public async Task<Data.Account> GetAccountByProductID(string fromProductId,string productType, string branchId, string branchCode)
        {

            Data.Account TellerAccount; // Initialize account to null
            string EventCode = $"{fromProductId}{(productType.ToLower().Contains("loan") ? LOAN_Product_EVENTCODE : SAVING_Product_EVENTCODE)}";
            
            Data.AccountingRuleEntry accountingRuleEntryAccount = GetAccountEntryRuleByEventCode(EventCode);


            TellerAccount = await GetAccountFromAccountingEntryRuleAsync(accountingRuleEntryAccount, branchId, branchCode, false);

            return TellerAccount;
        }
    }


}
