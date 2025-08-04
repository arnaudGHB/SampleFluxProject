using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using System.Reflection.Metadata;
using MediatR;
using CBS.TransactionManagement.Data.Dto.Resource;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using DocumentFormat.OpenXml.InkML;
using CBS.TransactionManagement.Data.Entity.SavingProductFeeP;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Data.LoanQueries;
using CBS.TransactionManagement.Repository.WithdrawalNotificationP;
using CBS.TransactionManagement.Data.Dto.Transaction;

namespace CBS.TransactionManagement.Repository
{


    public class AccountRepository : GenericRepository<Account, TransactionContext>, IAccountRepository
    {
        private readonly ILogger<AccountRepository> _logger;
        private readonly string _fileStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MembersAccountExport");
        private readonly UserInfoToken _userInfoToken;
        private readonly ISavingProductRepository _savingProductRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly IMediator _mediator;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ISavingProductFeeRepository _savingProductFeeRepository;
        private readonly IWithdrawalNotificationRepository _withdrawalNotificationRepository;
        
        public AccountRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<AccountRepository> logger = null, UserInfoToken userInfoToken = null, ISavingProductRepository savingProductRepository = null, ITellerRepository tellerRepository = null, IMediator mediator = null, IPropertyMappingService propertyMappingService = null, ISavingProductFeeRepository savingProductFeeRepository = null, IWithdrawalNotificationRepository withdrawalNotificationRepository = null) : base(unitOfWork)
        {
            _logger = logger;
            _userInfoToken = userInfoToken;
            _savingProductRepository = savingProductRepository;
            _tellerRepository = tellerRepository;
            _mediator = mediator;
            _propertyMappingService = propertyMappingService;
            _savingProductFeeRepository = savingProductFeeRepository;
            _withdrawalNotificationRepository=withdrawalNotificationRepository;
        }
        public async Task<List<MembersAccountSummaryDto>> GetMembersAccountSummaries(string branchName, string tel, string address, string branchcode)
        {
            var customerAccountSummaries = await All.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .GroupBy(a => new { a.CustomerId, a.CustomerName, a.BranchCode })
                .Select(g => new MembersAccountSummaryDto
                {
                    MemberName = g.Key.CustomerName,
                    MemberReference = g.Key.CustomerId,
                    BranchCode = g.Key.BranchCode,
                    Saving = g.Sum(a => a.AccountType.ToLower() == AccountType.Saving.ToString().ToLower() ? a.Balance : 0),
                    PreferenceShare = g.Sum(a => a.AccountType.ToLower() == AccountType.PreferenceShare.ToString().ToLower() ? a.Balance : 0),
                    Share = g.Sum(a => a.AccountType.ToLower() == AccountType.MemberShare.ToString().ToLower() ? a.Balance : 0),
                    Deposit = g.Sum(a => a.AccountType.ToLower() == AccountType.Deposit.ToString().ToLower() ? a.Balance : 0),
                    Loan = g.Sum(a => a.AccountType.ToLower() == AccountType.Loan.ToString().ToLower() ? a.Balance : 0),
                    Gav = g.Sum(a => a.AccountType.ToLower() == AccountType.Gav.ToString().ToLower() ? a.Balance : 0),
                    DailyCollection = g.Sum(a => a.AccountType.ToLower() == AccountType.DailyCollection.ToString().ToLower() ? a.Balance : 0),
                    TotalBalance = g.Sum(a => a.Balance),
                    NetBalance = g.Sum(a => a.Balance) - g.Sum(a => a.AccountType.ToLower() == AccountType.Loan.ToString().ToLower() ? a.Balance : 0)  // Excluding Loan
                })
                .ToListAsync();

            return customerAccountSummaries;
        }


        public async Task<List<MembersAccountSummaryDto>> GetMembersAccountSummaries(bool IsByBranch, string branchId = null, string branchcode = null)
        {
            try
            {
                // Fetch accounts from the database first
                var accounts = await FindBy(x => !x.IsDeleted && !x.IsTellerAccount && x.BranchId == branchId)
                                          .AsNoTracking()
                                          .ToListAsync();

                // Create customer account summaries list without loans
                var customerAccountSummaries = accounts
                    .GroupBy(a => a.CustomerId)  // Group by CustomerId only
                    .OrderBy(g => g.FirstOrDefault().CustomerName)  // Order by the first account's CustomerName in each group
                    .Select(g => new MembersAccountSummaryDto
                    {
                        MemberName = g.FirstOrDefault().CustomerName,  // Use the CustomerName of the first account in the group
                        MemberReference = g.Key,  // CustomerId is the group key
                        BranchCode = branchcode,
                        Saving = g.Sum(a => a.AccountType.ToLower() == AccountType.Saving.ToString().ToLower() ? a.Balance : 0),
                        PreferenceShare = g.Sum(a => a.AccountType.ToLower() == AccountType.PreferenceShare.ToString().ToLower() ? a.Balance : 0),
                        Share = g.Sum(a => a.AccountType.ToLower() == AccountType.MemberShare.ToString().ToLower() ? a.Balance : 0),
                        Deposit = g.Sum(a => a.AccountType.ToLower() == AccountType.Deposit.ToString().ToLower() ? a.Balance : 0),
                        Gav = g.Sum(a => a.AccountType.ToLower() == AccountType.Gav.ToString().ToLower() ? a.Balance : 0),
                        DailyCollection = g.Sum(a => a.AccountType.ToLower() == AccountType.DailyCollection.ToString().ToLower() ? a.Balance : 0),
                        TotalBalance = g.Sum(a => a.Balance),
                        NetBalance = g.Sum(a => a.Balance)  // Initially excluding Loan
                    })
                    .ToList();

                // Fetch loans via mediator
                var getAllLoanQueries = new GetAllLoanQueries { BranchId = branchId, IsByBranch = true, QueryParam = "Open" };
                var response = await _mediator.Send(getAllLoanQueries);

                List<Loan> loans = new List<Loan>();
                if (response.StatusCode == 200)
                {
                    loans = response.Data;
                }

                // Now loop through the customerAccountSummaries to update Loan details
                foreach (var customerAccount in customerAccountSummaries)
                {
                    var customerLoans = loans.Where(l => l.CustomerId == customerAccount.MemberReference).ToList();
                    customerAccount.Loan = customerLoans.Sum(l => l.Balance);  // Sum the DueAmount for the customer's loans

                    // Update NetBalance by deducting the loan amounts
                    customerAccount.NetBalance = customerAccount.TotalBalance - customerAccount.Loan;
                }

                return customerAccountSummaries;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        public async Task<MemberAccountSituationListing> GetMembersAccountSummaries(AccountResource accountResource)
        {
            // Define the base query for accounts
            IQueryable<Account> query = FindBy(x => !x.IsDeleted).AsNoTracking();

            // Apply branch filtering if IsByBranch is true
            if (accountResource.IsByBranch)
            {
                query = query.Where(x => x.BranchId == accountResource.BranchId);
            }

            // Apply sorting based on the provided order by property
            query = query.ApplySort(accountResource.OrderBy, _propertyMappingService.GetPropertyMapping<MembersAccountSummaryDto, MembersAccountSummaryDto>())
                         .OrderBy(x => x.CustomerName);

            // Filter accounts based on SearchQuery if provided
            if (!string.IsNullOrWhiteSpace(accountResource.SearchQuery) && accountResource.SearchQuery.ToLower() != "all")
            {
                string searchQueryLower = accountResource.SearchQuery.ToLower();
                query = query.Where(c =>
                    (c.CustomerName != null && c.CustomerName.ToLower().Contains(searchQueryLower)) ||
                    (c.CustomerId != null && c.CustomerId.ToLower().Contains(searchQueryLower)) ||
                    (c.BranchCode != null && c.BranchCode.ToLower().Contains(searchQueryLower)));
            }

            // Create and return the paginated list of accounts
            var membersAccountSummaries = new MemberAccountSituationListing();
            var membersAccounts = await membersAccountSummaries.Create(query, accountResource.Skip, accountResource.PageSize);
            return membersAccounts;
        }



        public async Task<MemberAccountSituationListing> GetTellerAccounts(AccountResource accountResource)
        {
            // Define the base query for accounts
            IQueryable<Account> query = FindBy(x => x.BranchId == accountResource.BranchId && !x.IsDeleted).AsNoTracking();

            // Apply sorting based on the provided order by property
            query = query.ApplySort(accountResource.OrderBy, _propertyMappingService.GetPropertyMapping<MembersAccountSummaryDto, MembersAccountSummaryDto>())
                         .OrderBy(x => x.CustomerName);

            // Filter accounts based on SearchQuery if provided
            if (!string.IsNullOrWhiteSpace(accountResource.SearchQuery) && accountResource.SearchQuery.ToLower() != "all")
            {
                string searchQueryLower = accountResource.SearchQuery.ToLower();
                query = query.Where(c =>
                    (c.CustomerName != null && c.CustomerName.ToLower().Contains(searchQueryLower)) ||
                    (c.CustomerId != null && c.CustomerId.ToLower().Contains(searchQueryLower)) ||
                    (c.BranchCode != null && c.BranchCode.ToLower().Contains(searchQueryLower)));
            }

            // Create and return the paginated list of accounts
            var memberAccountSituationListing = new MemberAccountSituationListingx();
            return await memberAccountSituationListing.Create(query, accountResource.Skip, accountResource.PageSize);
        }



        // Method to retrieve sub teller account
        public async Task<Account> RetrieveTellerAccount(Teller teller)
        {
            var tellerAccount = await FindBy(t => t.TellerId == teller.Id && t.IsDeleted == false)
                .FirstOrDefaultAsync(); // Find teller account.

            // Check if teller account exists
            if (tellerAccount == null)
            {


                var errorMessage = $"Teller {teller.Name} does not have an account";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            //string accountBalance = tellerAccount.Balance.ToString();
            //if (!BalanceEncryption.VerifyBalanceIntegrity(accountBalance, tellerAccount.EncryptedBalance, tellerAccount.AccountNumber))
            //{
            //    var errorMessage = $"Unauthorized modification detected in the member's account balance ({BaseUtilities.FormatCurrency(tellerAccount.Balance)}). Please contact your system administrator immediately to investigate and resolve this issue.";
            //    _logger.LogError(errorMessage); // Log error message.
            //    throw new InvalidOperationException(errorMessage); // Throw exception.
            //}
            return tellerAccount; // Return teller account.
        }

        public async Task<Account> CheckTellerBalance(Teller teller, decimal Amount)
        {
            var tellerAccount = await FindBy(t => t.TellerId == teller.Id && t.IsDeleted == false)
                .FirstOrDefaultAsync(); // Find teller account.

            // Check if teller account exists
            if (tellerAccount == null)
            {
                var errorMessage = $"Teller {teller.Name} does not have an account";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            if ((tellerAccount.Balance - Amount) < 0)
            {
                var errorMessage = $"Teller {teller.Name}'s account has insufficient funds to complete this operation. Current Balance: {BaseUtilities.FormatCurrency(tellerAccount.Balance)}. To proceed, please make a cash requisition from the primary teller to ensure there are sufficient funds available.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            return tellerAccount; // Return teller account.
        }
        public async Task<Account> RetrieveNoneMemberMobileMoneyAccountByMemberReference(string MemberReference)
        {
            var tellerAccount = await FindBy(t => t.CustomerId == MemberReference && t.IsDeleted == false && t.AccountType.Contains("MobileMoney"))
                .FirstOrDefaultAsync(); // Find teller account.

            // Check if teller account exists
            if (tellerAccount == null)
            {
                var errorMessage = $"Account do not exit for member refernce {MemberReference}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            return tellerAccount; // Return teller account.
        }
        public async Task<Account> RetrieveMobileMoneyTellerAccount(string branchid, string tellerType)
        {
            var teller = await _tellerRepository.FindBy(x => x.BranchId == branchid && x.TellerType == tellerType && x.IsDeleted == false).FirstOrDefaultAsync();
            // Check if teller account exists
            if (teller == null)
            {
                var errorMessage = $"Teller of type {tellerType} does not exist.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            var tellerAccount = await RetrieveTellerAccount(teller); // Find teller account.
            return tellerAccount; // Return teller account.
        }
        public async Task<Account> RetrieveMobileMoneyTellerAccountByTellerCodeAndBranchId(string branchid, string tellerCode)
        {
            var teller = await _tellerRepository.FindBy(x => x.BranchId == branchid && x.Code == tellerCode && x.IsDeleted == false).FirstOrDefaultAsync();
            // Check if teller account exists
            if (teller == null)
            {
                var errorMessage = $"Teller of type {tellerCode} does not exist.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            var tellerAccount = await RetrieveTellerAccount(teller); // Find teller account.
            return tellerAccount; // Return teller account.
        }
        public async Task<Account> RetrieveMobileMoneyTellerAccountAndCheckBalance(string branchid, string tellerId, decimal amount)
        {
            var teller = await _tellerRepository.FindBy(x => x.BranchId == branchid && x.Id == tellerId && x.IsDeleted == false).FirstOrDefaultAsync();
            // Check if teller account exists
            if (teller == null)
            {
                var errorMessage = $"Teller of type {tellerId} does not exist.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }


            var tellerAccount = await RetrieveTellerAccount(teller); // Find teller account.

            string accountBalance = tellerAccount.Balance.ToString();
            if (tellerAccount.Balance - amount < 0)
            {
                var errorMessage = $"Insufficient fund.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }


            return tellerAccount; // Return teller account.
        }
        // Method to retrieve sub teller account
        public async Task OpenDay(string Reference, Account account, DateTime accountingDay)
        {
            if (account.OpenningOfDayDate?.Date == accountingDay.Date)
            {
                var errorMessage = $"The accounting day is open with reference {account.OpenningOfDayReference} on date [{account.OpenningOfDayDate}].";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            if (account.OpenningOfDayStatus == CloseOfDayStatus.OOD.ToString())
            {
                var errorMessage = $"The operational day on {account.OpenningOfDayDate} is still open. Please close the previous day before opening a new one.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }


            account.OpenningOfDayStatus = CloseOfDayStatus.OOD.ToString();
            account.OpenningOfDayReference = Reference;
            account.OpenningOfDayDate = accountingDay;
            UpdateInCasecade(account);
        }


        public async Task ClosseDay(Account account)
        {
            if (account.OpenningOfDayStatus == CloseOfDayStatus.CLOSED.ToString())
            {
                var errorMessage = $"The operation day: {account.OpenningOfDayDate} is already clossed for the operation date: {account.OpenningOfDayDate}.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            account.OpenningOfDayStatus = CloseOfDayStatus.CLOSED.ToString();
            UpdateInCasecade(account);
        }


       
        public void ResetAccountBalance(Account account, decimal amount)
        {
            account.PreviousBalance = account.Balance;
            account.Balance = amount;
            account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance
            UpdateInCasecade(account);
        }
        public void ResetAccountBalance(Account account)
        {
            account.PreviousBalance = account.Balance;
            account.Balance = 0;
            account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance
            UpdateInCasecade(account);
        }
        public void DeleteAccount(Account account)
        {
            account.IsDeleted = true;
            UpdateInCasecade(account);
        }
        public async void UpdateAccountNumber(Account account)
        {
            var product = await _savingProductRepository.FindAsync(account.ProductId);
            string accountNumber = product.AccountNuber == null ? $"{product.Code}" : product.AccountNuber;
            account.AccountNumber = BaseUtilities.GenerateAccountNumber(account.CustomerId, accountNumber);
            UpdateInCasecade(account);
            await _uow.SaveAsync();
        }
        public async void UpdateAccountNumber(List<Account> accounts)
        {
            foreach (var account in accounts)
            {
                var product = await _savingProductRepository.FindAsync(account.ProductId);
                string accountNumber = product.AccountNuber == null ? $"{product.Code}" : product.AccountNuber;
                account.AccountNumber = BaseUtilities.GenerateAccountNumber(account.CustomerId, accountNumber);
                UpdateInCasecade(account);
                await _uow.SaveAsync();

            }
        }
        public void CreditAccount(Account account, decimal amount,string lastOperationType)
        {
            account.PreviousBalance = account.Balance;
            account.Balance += amount;
            account.Product = null;
            account.LastOperationAmount=amount;
            account.LastOperation=lastOperationType;
            account.DateOfLastOperation=BaseUtilities.UtcNowToDoualaTime();
            account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance
            UpdateInCasecade(account);
        }
        public Account CreditAccountBalanceReturned(Account account, decimal amount, string lastOperationType)
        {
            account.PreviousBalance = account.Balance;
            account.Balance += amount;
            account.Product = null;
            account.LastOperationAmount=amount;
            account.LastOperation=lastOperationType;
            account.DateOfLastOperation=BaseUtilities.UtcNowToDoualaTime();
            account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance
            UpdateInCasecade(account);
            return account;
        }
        public Account DebitAccountBalanceReturned(Account account, decimal amount, string lastOperationType)
        {
            account.PreviousBalance = account.Balance;
            account.Balance -= amount;
            account.LastOperationAmount=amount;
            account.LastOperation=lastOperationType;
            account.DateOfLastOperation=BaseUtilities.UtcNowToDoualaTime();
            account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance
            UpdateInCasecade(account);
            return account;
        }
        public void DebitAccount(Account account, decimal amount, string lastOperationType)
        {
            account.PreviousBalance = account.Balance;
            account.Balance -= amount;
            account.LastOperationAmount=amount;
            account.LastOperation=lastOperationType;
            account.DateOfLastOperation=BaseUtilities.UtcNowToDoualaTime();
            account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber); // Encrypt balance
            UpdateInCasecade(account);
        }
        public void CreditAndDebitAccount(Account account, decimal creditAmount, decimal debitAmount)
        {
            // Validate the input amounts
            if (creditAmount < 0)
            {
                throw new ArgumentException("The credit amount cannot be negative.", nameof(creditAmount));
            }

            if (debitAmount < 0)
            {
                throw new ArgumentException("The debit amount cannot be negative.", nameof(debitAmount));
            }

            // Store the current balance as the previous balance
            account.PreviousBalance = account.Balance;

            // Calculate the net change by applying credit and debit
            decimal netChange = creditAmount - debitAmount;

            // Apply the net change to the account balance
            account.Balance += netChange;

            // Nullify the product reference for serialization safety
            account.Product = null;

            // Encrypt the updated balance
            account.EncryptedBalance = BalanceEncryption.Encrypt(account.Balance.ToString(), account.AccountNumber);

            // Log the transaction details
            _logger.LogInformation(
                $"Account {account.AccountNumber} updated: Credited {creditAmount}, Debited {debitAmount}, " +
                $"New Balance: {account.Balance}, Previous Balance: {account.PreviousBalance}."
            );

            // Persist the changes to the database
            UpdateInCasecade(account);
        }

        public bool VerifyBalanceIntegrity(Account account)
        {
            string receiverAccountBalance = account.Balance.ToString();
            bool checkedIntegrity = BalanceEncryption.VerifyBalanceIntegrity(receiverAccountBalance, account.EncryptedBalance, account.AccountNumber);
            if (!checkedIntegrity)
            {
                var errorMessage = $"Sorry, but it seems that the balance of the account [{account.AccountNumber}-{account.AccountName}] has been tampered with, making it impossible to verify its integrity. Please reach out to customer support for further investigation.";
                throw new InvalidOperationException(errorMessage); // Throw exception.

            }
            return true;
        }
        // Method to retrieve account information based on the providd account number
        public async Task<Account> GetAccount(string accountNumber, string operationType)
        {
            if (operationType.ToLower() == TransactionType.DEPOSIT.ToString().ToLower())
            {
                var account = await
                          FindBy(a => a.AccountNumber == accountNumber && a.IsDeleted == false)
                          .Include(a => a.Product)
                          .Include(a => a.Product.CashDepositParameters)
                          .FirstOrDefaultAsync();

                // Check if the account type is a loan account
                if (account.AccountType == AccountType.Loan.ToString())
                {
                    var errorMessage = $"Sorry, you can't cash into a loan account directly.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }
                if (!account.Product.CashDepositParameters.Any())
                {
                    var errorMessage = $"Sorry, cash limits is not configuration for members account type: {account.AccountType}. Contact your system administrator.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }
                // Verify the integrity of the account balance
                //string accountBalance = account.Balance.ToString();
                //if (!BalanceEncryption.VerifyBalanceIntegrity(accountBalance, account.EncryptedBalance, account.AccountNumber))
                //{
                //    var errorMessage = $"Unauthorized modification detected in the member's account balance ({BaseUtilities.FormatCurrency(account.Balance)}). Please contact your system administrator immediately to investigate and resolve this issue.";
                //    _logger.LogError(errorMessage); // Log error message.
                //    throw new InvalidOperationException(errorMessage); // Throw exception.
                //}
                return account; // Return account.
            }
            else if (operationType.ToLower() == TransactionType.WITHDRAWAL.ToString().ToLower())
            {
                var account = await
                          FindBy(a => a.AccountNumber == accountNumber && a.IsDeleted == false)
                          .Include(a => a.Product)
                          .Include(a => a.Product.WithdrawalParameters)
                          .FirstOrDefaultAsync();

                // Check if the account type is a loan account
                if (account.AccountType == AccountType.Loan.ToString())
                {
                    var errorMessage = $"Sorry, you can't cash out from a loan account directly.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }
                if (!account.Product.WithdrawalParameters.Any())
                {
                    var errorMessage = $"Sorry, cash out limits is not configuration for members account type: {account.AccountType}. Contact your system administrator.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }
                // Verify the integrity of the account balance
                string accountBalance = account.Balance.ToString();
                if (!BalanceEncryption.VerifyBalanceIntegrity(accountBalance, account.EncryptedBalance, account.AccountNumber))
                {
                    var errorMessage = $"Unauthorized modification detected in the member's account balance ({BaseUtilities.FormatCurrency(account.Balance)}). Please contact your system administrator immediately to investigate and resolve this issue.";
                    _logger.LogError(errorMessage); // Log error message.
                    //throw new InvalidOperationException(errorMessage); // Throw exception.
                }

                return account; // Return account.
            }
            else if (operationType.ToLower() == TransactionType.TRANSFER.ToString().ToLower())
            {
                var account = await
                          FindBy(a => a.AccountNumber == accountNumber && a.IsDeleted == false)
                          .Include(a => a.Product)
                          .Include(a => a.Product.TransferParameters)
                          .FirstOrDefaultAsync();

                //// Check if the account type is a loan account
                //if (account.AccountType == AccountType.Loan.ToString())
                //{
                //    var errorMessage = $"Sorry, you can't cash into a loan account directly.";
                //    throw new InvalidOperationException(errorMessage); // Throw exception.
                //}
                if (!account.Product.TransferParameters.Any())
                {
                    var errorMessage = $"Sorry, transfer limits is not configuration for members account type: {account.AccountType}. Contact your system administrator.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }
                // Verify the integrity of the account balance
                string accountBalance = account.Balance.ToString();
                if (!BalanceEncryption.VerifyBalanceIntegrity(accountBalance, account.EncryptedBalance, account.AccountNumber))
                {
                    var errorMessage = $"Unauthorized modification detected in the member's account balance ({BaseUtilities.FormatCurrency(account.Balance)}). Please contact your system administrator immediately to investigate and resolve this issue.";
                    _logger.LogError(errorMessage); // Log error message.
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }

                return account; // Return account.
            }
            else if (operationType.ToLower() == TransactionType.TTP_TRANSFER.ToString().ToLower())
            {
                var account = await
                          FindBy(a => a.AccountNumber == accountNumber && a.IsDeleted == false)
                          .Include(a => a.Product)
                          .Include(a => a.Product.TransferParameters)
                          .FirstOrDefaultAsync();

                // Check if the account type is a loan account
                if (account == null)
                {
                    var errorMessage = $"Sorry, Account number {accountNumber} does not exist.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }
                if (account.AccountType == AccountType.Loan.ToString())
                {
                    var errorMessage = $"Sorry, you can't peform any operation into a loan account directly.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }
                if (!account.Product.TransferParameters.Any())
                {
                    var errorMessage = $"TTP Operation: Sorry, transfer limits is not configuration for members account type: {account.AccountType}. Contact your system administrator.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }
                // Verify the integrity of the account balance
                string accountBalance = account.Balance.ToString();
                if (!BalanceEncryption.VerifyBalanceIntegrity(accountBalance, account.EncryptedBalance, account.AccountNumber))
                {
                    var errorMessage = $"TTP Operation: Unauthorized modification detected in the member's account balance ({BaseUtilities.FormatCurrency(account.Balance)}). Please contact your system administrator immediately to investigate and resolve this issue.";
                    _logger.LogError(errorMessage); // Log error message.
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }

                return account; // Return account.
            }
            else if (operationType.ToLower() == TransactionType.CASHIN_LOAN_REPAYMENT.ToString().ToLower())
            {
                var account = await
                        FindBy(a => a.AccountNumber == accountNumber && a.IsDeleted == false)
                        .Include(a => a.Product)
                        .ThenInclude(a => a.CashDepositParameters)
                        .FirstOrDefaultAsync();

                if (!account.Product.CashDepositParameters.Any())
                {
                    var errorMessage = $"Sorry, cash in limits is not configuration for members account type: {account.AccountType}. Contact your system administrator.";
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }
                // Verify the integrity of the account balance
                string accountBalance = account.Balance.ToString();
                if (!BalanceEncryption.VerifyBalanceIntegrity(accountBalance, account.EncryptedBalance, account.AccountNumber))
                {
                    var errorMessage = $"Unauthorized modification detected in the member's account balance ({BaseUtilities.FormatCurrency(account.Balance)}). Please contact your system administrator immediately to investigate and resolve this issue.";
                    _logger.LogError(errorMessage); // Log error message.
                    throw new InvalidOperationException(errorMessage); // Throw exception.
                }

                return account; // Return account.
            }
            _logger.LogError("Invalid options selected."); // Log error message.
            throw new InvalidOperationException("Invalid options selected."); // Throw exception.
        }


        public async Task<Account> GetRemittanceAccount(string accountNumber, string accountType, string operationType)
        {
            // Validate if the accountType matches the RemittanceTypes enum
            if (!Enum.TryParse(accountType, true, out RemittanceTypes remittanceService))
            {
                var errorMessage = $"Invalid account type. Allowed types are: {string.Join(", ", Enum.GetNames(typeof(RemittanceTypes)))}.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            if (operationType.ToLower() == TransactionType.DEPOSIT.ToString().ToLower())
            {
                var account = await FindBy(a => a.AccountNumber == accountNumber && a.IsDeleted == false)
                    .Include(a => a.Product)
                    .Include(a => a.Product.CashDepositParameters)
                    .FirstOrDefaultAsync();

                if (!account.Product.CashDepositParameters.Any())
                {
                    var errorMessage = $"Sorry, cash limits are not configured for the account type: {accountType}. Contact your system administrator.";
                    throw new InvalidOperationException(errorMessage);
                }

                return account;
            }
            else if (operationType.ToLower() == TransactionType.WITHDRAWAL.ToString().ToLower())
            {
                var account = await FindBy(a => a.AccountNumber == accountNumber && a.IsDeleted == false)
                    .Include(a => a.Product)
                    .Include(a => a.Product.WithdrawalParameters)
                    .FirstOrDefaultAsync();

                if (!account.Product.WithdrawalParameters.Any())
                {
                    var errorMessage = $"Sorry, withdrawal limits are not configured for the account type: {accountType}. Contact your system administrator.";
                    throw new InvalidOperationException(errorMessage);
                }

                return account;
            }
            else if (operationType.ToLower() == TransactionType.TRANSFER.ToString().ToLower())
            {
                var account = await FindBy(a => a.AccountNumber == accountNumber && a.IsDeleted == false)
                    .Include(a => a.Product)
                    .Include(a => a.Product.TransferParameters)
                    .FirstOrDefaultAsync();

                if (!account.Product.TransferParameters.Any())
                {
                    var errorMessage = $"Sorry, transfer limits are not configured for the account type: {accountType}. Contact your system administrator.";
                    throw new InvalidOperationException(errorMessage);
                }

                return account;
            }
            else
            {
                var errorMessage = "Invalid operation type selected.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }

        public async Task<Account> GetGavAccount(string accountNumber)
        {
            var account = await
                          FindBy(a => a.AccountNumber == accountNumber && a.AccountType.ToLower() == AccountType.Gav.ToString().ToLower() && a.IsDeleted == false)
                          .Include(a => a.Product)
                          .Include(a => a.Product.TransferParameters)
                          .FirstOrDefaultAsync();
            if (account == null)
            {
                var errorMessage = $"Operation failed: The specified account number {accountNumber} does not exist as GAV account. Please verify the account details and try again.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage); // Throw exception.

            }
            // Check if account is activated for third-party transactions

            if (!account.Product.ActivateFor3PPApp)
            {
                var errorMessage = $"Operation failed: The account, {account.AccountType} is not eligible for third-party transactions. Please verify the account details and ensure it is registered as a GAV account before proceeding.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            // Check if the account type is a loan account
            if (account.AccountType.ToLower() != AccountType.Gav.ToString().ToLower())
            {
                var errorMessage = $"Sorry, operations cannot be performed directly on a {account.AccountType} account. Please choose a different account type.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            if (!account.Product.TransferParameters.Any())
            {
                var errorMessage = $"TTP Operation: Sorry, transfer limits are not configured for the account type: {account.AccountType}. Please contact your system administrator.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Verify the integrity of the account balance
            string accountBalance = account.Balance.ToString();
            if (!BalanceEncryption.VerifyBalanceIntegrity(accountBalance, account.EncryptedBalance, account.AccountNumber))
            {
                var errorMessage = $"TTP Operation: Unauthorized modification detected in the member's account balance ({BaseUtilities.FormatCurrency(account.Balance)}). Please contact your system administrator immediately to investigate and resolve this issue.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }

            return account; // Return account.
        }


        /// <summary>
        /// Validates whether a specified account meets the necessary constraints for a given operation type on either 
        /// a Mobile or Third-Party app, and for a specified service type.
        /// </summary>
        /// <param name="account">The account object containing details and permissions of the product.</param>
        /// <param name="operationType">The type of transaction operation being performed (Transfer, CashIn, CashOut).</param>
        /// <param name="appType">Specifies whether the operation is on the Mobile App or a Third-Party App.</param>
        /// <param name="serviceType">The service for which the operation is being performed (GAV, CMONEY).</param>
        /// <exception cref="InvalidOperationException">Thrown if the account is not activated for the app or if 
        /// the operation type is not permitted for the account.</exception>
        public void CheckAppConstraints(Account account, TransactionType operationType, AppType appType, ServiceType serviceType)
        {
            // Check if the account is activated for the specified app type (MobileApp or ThirdPartyApp)
            bool isActivated = appType switch
            {
                // If the app type is MobileApp, check if 'ActivateForMobileApp' is true on the product.
                AppType.MobileApp => account.Product.ActivateForMobileApp,

                // If the app type is ThirdPartyApp, check if 'ActivateFor3PPApp' is true on the product.
                AppType.ThirdPartyApp => account.Product.ActivateFor3PPApp,

                // Default case to return false if app type does not match any known values.
                _ => false
            };

            // If the account is not activated for the specified app type, throw an exception with an error message.
            if (!isActivated)
            {
                // Log an error message describing the failure reason.
                var errorMessage = $"Operation failed: The specified {account.AccountType} account is not activated for {operationType} operations on {appType}.";
                _logger.LogError(errorMessage);

                // Throw an exception to halt further processing of this transaction due to the constraint.
                throw new InvalidOperationException(errorMessage);
            }

            // Determine if the operation type (Transfer, CashIn, CashOut) is permitted based on the app type.
            bool canPerformOperation = operationType switch
            {
                // Check if Transfer operation is permitted based on the app type.
                TransactionType.TRANSFER => appType == AppType.MobileApp
                    ? account.Product.CanPeformTransferMobileApp
                    : account.Product.CanPeformTransfer3PP,

                // Check if CashIn operation is permitted based on the app type.
                TransactionType.CASH_IN => appType == AppType.MobileApp
                    ? account.Product.CanPeformCashinMobileApp
                    : account.Product.CanPeformCashin3PP,

                // Check if CashOut operation is permitted based on the app type.
                TransactionType.CASH_WITHDRAWAL => appType == AppType.MobileApp
                    ? account.Product.CanPeformCashOutMobileApp
                    : account.Product.CanPeformCashOut3PP,

                // Default case to return false if the operation type does not match any known values.
                _ => false
            };

            // If the specified operation type is not permitted, throw an exception with an error message.
            if (!canPerformOperation)
            {
                // Generate the operation name by converting enum value to lowercase and replacing underscores with spaces.
                var operationName = operationType.ToString().Replace('_', ' ').ToLower();

                // Log an error message detailing why the operation failed.
                var errorMessage = $"Operation failed: {operationName} operations are not permitted for this {operationType} account with {account.AccountType} on {appType}.";
                _logger.LogError(errorMessage);

                // Throw an exception to indicate operation failure due to constraints.
                throw new InvalidOperationException(errorMessage);
            }
        }

        public void CheckMobileAppConstraints(Account account, string accountType, TransactionType operationType)
        {
            if (!account.Product.ActivateForMobileApp)
            {
                var errorMessage = $"Operation failed: The specified {accountType} account is not activated for mobile app operations.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            switch (operationType)
            {
                case TransactionType.TRANSFER:
                    if (!account.Product.CanPeformTransferMobileApp)
                    {
                        var errorMessage = $"Operation failed: Transfers are not allowed for this {accountType} account on the mobile app.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }
                    break;
                case TransactionType.CASH_IN:
                    if (!account.Product.CanPeformCashinMobileApp)
                    {
                        var errorMessage = $"Operation failed: Cash-in operations are not allowed for this {accountType} account on the mobile app.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }
                    break;
                case TransactionType.CASH_WITHDRAWAL:
                    if (!account.Product.CanPeformCashOutMobileApp)
                    {
                        var errorMessage = $"Operation failed: Cash-out operations are not allowed for this {accountType} account on the mobile app.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }
                    break;
            }
        }

        public void Check3PPAppConstraints(Account account, string accountType, TransactionType operationType)
        {
            if (!account.Product.ActivateFor3PPApp)
            {
                var errorMessage = $"Operation failed: The specified {accountType} account is not activated for third-party app operations.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            switch (operationType)
            {
                case TransactionType.TRANSFER:
                    if (!account.Product.CanPeformTransfer3PP)
                    {
                        var errorMessage = $"Operation failed: Transfers are not permitted for this {accountType} account with third-party applications.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }
                    break;
                case TransactionType.CASH_IN:
                    if (!account.Product.CanPeformCashin3PP)
                    {
                        var errorMessage = $"Operation failed: Cash-in operations are not permitted for this {accountType} account with third-party applications.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }
                    break;
                case TransactionType.CASH_WITHDRAWAL:
                    if (!account.Product.CanPeformCashOut3PP)
                    {
                        var errorMessage = $"Operation failed: Cash-out operations are not permitted for this {accountType} account with third-party applications.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }
                    break;
            }
        }

        public async Task<Account> Get3PPDepositAccount(string customerId)
        {
            var account = await
                          FindBy(a => a.CustomerId == customerId && a.AccountType.ToLower() == "deposit" && a.IsDeleted == false)
                          .Include(a => a.Product)
                          .Include(a => a.Product.CashDepositParameters)
                          .FirstOrDefaultAsync();
            if (account == null)
            {



                var errorMessage = $"Member do not have a deposit account. Contact member service to add deposit account.";
                throw new InvalidOperationException(errorMessage); // Throw exception.

            }
            return account;
        }
        public async Task<Account> GetMemberLoanAccount(string customerId)
        {
            var account = await
                          FindBy(a => a.CustomerId == customerId && a.AccountType.ToLower() == "loan" && a.IsDeleted == false)
                          .Include(a => a.Product)
                          .Include(a => a.Product.CashDepositParameters)
                          .FirstOrDefaultAsync();
            if (account == null)
            {




                var errorMessage = $"Member do not have a loan account. Contact member service to add loan account.";
                throw new InvalidOperationException(errorMessage); // Throw exception.

            }
            return account;
        }

        public async Task<bool> CheckIfMemberHaveLoanAccount(string customerId)
        {
            var account = await FindBy(a => a.CustomerId == customerId && a.AccountType.ToLower() == "loan" && a.IsDeleted == false).FirstOrDefaultAsync();
            if (account == null)
            {

                return false;
            }
            return true; ;
        }
        public async Task<Account> GetAccountByAccountNumber(string accountNumber)
        {
            var account = await
                          FindBy(a => a.AccountNumber == accountNumber && a.IsDeleted == false).Include(x => x.Product).ThenInclude(x => x.TransferParameters)
                          .FirstOrDefaultAsync();

            // Check if account is null, meaning it either doesn't exist or has been marked as deleted
            if (account == null)
            {
                var errorMessage = $"Operation failed: No active account found with the specified account number '{accountNumber}'. " +
                                   "Please verify the account details and ensure the account is not marked as deleted.";
                throw new InvalidOperationException(errorMessage); // Throw exception with more descriptive error.
            }
            return account;
        }

        public DetermineTransferTypeDto DetermineTransferType(Account sourceAccount, Account destinationAccount)
        {
            var transferType = new DetermineTransferTypeDto();

            if (sourceAccount.CustomerId == destinationAccount.CustomerId)
            {
                // Self-transfer within the same customer's accounts
                transferType.TransferType = TransferType.Self.ToString();
                transferType.IsInterBranch = false;
            }
            else if (sourceAccount.BranchId != destinationAccount.BranchId)
            {
                // Inter-branch transfer between different branches
                transferType.TransferType = TransferType.Inter_Branch.ToString();
                transferType.IsInterBranch = true;
            }
            else
            {
                // Local transfer within the same branch and different customers
                transferType.TransferType = TransferType.Local.ToString();
                transferType.IsInterBranch = false;
            }

            return transferType;
        }

        /// <summary>
        /// Calculates the transfer charges for a given amount based on the transfer parameters and fee configuration.
        /// </summary>
        /// <param name="amount">The amount for which the transfer charges need to be calculated.</param>
        /// <param name="productId">The product ID associated with the transfer.</param>
        /// <param name="feeOperationType">The type of fee operation (e.g., Membership, Operation, etc.).</param>
        /// <param name="senderBranchId">The ID of the sender's branch.</param>
        /// <param name="transferType">The type of transfer (e.g., Local, Inter-Branch).</param>
        /// <param name="isSavingAccount">Indicates if the operation is related to a savings account.</param>
        /// <param name="isWithdrawalNotified">Indicates if the withdrawal notification is enabled.</param>
        /// <param name="formFee">The form fee associated with the transaction.</param>
        /// <returns>A TransferCharges object containing the calculated service charge and total charges.</returns>
        public async Task<TransferCharges> CalculateTransferCharges(
            decimal amount,
            string productId,
            FeeOperationType feeOperationType,
            string senderBranchId,
            string transferType,
            bool isSavingAccount = false,
            bool isWithdrawalNotified = false,
            decimal formFee = 0, bool isMinorAccount = false)
        {
            var transferCharges = new TransferCharges(); // Initialize transfer charges object.

            if (!isSavingAccount)
            {
                // Query fee configuration for the specified product and transfer type.
                var feeQuery = BuildFeeQuery(productId, transferType, "transfer", feeOperationType);

                // Fetch applicable fees.
                var fees = await feeQuery.ToListAsync();

                if (!fees.Any())
                {
                    var errorMessage = $"Transfer charges for {feeOperationType} are not configured for the transfer type {transferType} on the selected account. Contact your administrator.";
                    _logger.LogError(errorMessage); // Log configuration error.
                    throw new InvalidOperationException(errorMessage); // Throw exception for missing configuration.
                }

                // Calculate charges based on rate and range.
                var rateCharge = CalculateRateCharge(fees, senderBranchId, amount);
                var rangeCharge = CalculateRangeCharge(fees, senderBranchId, amount);

                // Determine the applicable service charge.
                transferCharges.ServiceCharge = rateCharge > 0 ? rateCharge : rangeCharge;

                if (transferCharges.ServiceCharge <= 0)
                {
                    var errorMessage = $"No valid service charge configuration found for the amount {amount}.";
                    _logger.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Calculate total charges (including additional fees if any).
                transferCharges.TotalCharges = XAFWallet.CalculateCustomerWithdrawalCharges(transferCharges.ServiceCharge, 0, isMinorAccount);
            }
            else
            {
                if (isWithdrawalNotified)
                {
                    transferCharges.ServiceCharge = 0; // No service charge for notified withdrawals.
                    transferCharges.TotalCharges = XAFWallet.CalculateCustomerWithdrawalCharges(transferCharges.ServiceCharge, 0, isMinorAccount);
                }
                else
                {
                    // Query fee configuration for savings account-related transfer type.
                    var feeQuery = BuildFeeQuery(productId, transferType, "cmoney_swn", feeOperationType);

                    // Fetch applicable fees.
                    var fees = await feeQuery.ToListAsync();

                    if (!fees.Any())
                    {
                        var errorMessage = $"Transfer charges for {feeOperationType} are not configured for the transfer type {transferType} on the selected account. Contact your administrator.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }

                    // Calculate charges based on rate and range.
                    var rateCharge = CalculateRateCharge(fees, senderBranchId, amount);
                    var rangeCharge = CalculateRangeCharge(fees, senderBranchId, amount);

                    // Determine the applicable service charge.
                    transferCharges.ServiceCharge = rateCharge > 0 ? rateCharge : rangeCharge;

                    if (transferCharges.ServiceCharge <= 0)
                    {
                        var errorMessage = $"No valid service charge configuration found for the amount {amount}.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }

                    // Calculate total charges (including additional fees if any).
                    transferCharges.TotalCharges = XAFWallet.CalculateCustomerWithdrawalCharges(transferCharges.ServiceCharge, formFee, isMinorAccount);
                    transferCharges.TransfeFormCharge=formFee;
                }
            }

            return transferCharges; // Return the calculated transfer charges.
        }

        /// <summary>
        /// Builds a query for fetching fee configurations based on the provided parameters.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <param name="transferType">The type of transfer.</param>
        /// <param name="feeType">The type of fee (e.g., "transfer").</param>
        /// <param name="feeOperationType">The fee operation type.</param>
        /// <returns>An IQueryable object for fetching fee configurations.</returns>
        private IQueryable<SavingProductFee> BuildFeeQuery(string productId, string transferType, string feeType, FeeOperationType feeOperationType)
        {
            IQueryable<SavingProductFee> feeQuery = _savingProductFeeRepository
             .FindBy(x => x.SavingProductId == productId
                          && x.FeePolicyType.ToLower() == transferType.ToLower()
                          && x.FeeType.ToLower() == feeType
                          && !x.IsDeleted)
             .Include(x => x.Fee)
             .ThenInclude(f => f.FeePolicies);

            // Step 2: Filter fees based on the FeeOperationType.
            feeQuery = feeOperationType switch
            {
                FeeOperationType.MemberShip => feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.MemberShip.ToString()),
                FeeOperationType.Operation => feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.Operation.ToString()),
                FeeOperationType.CMoney => feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.CMoney.ToString()),
                FeeOperationType.Gav => feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.Gav.ToString()),
                FeeOperationType.MobileMoney => feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.MobileMoney.ToString()),
                FeeOperationType.OrangeMoney => feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.OrangeMoney.ToString()),
                FeeOperationType.CivilServants => feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.CivilServants.ToString()),
                FeeOperationType.Internal => feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.Internal.ToString()),
                FeeOperationType.PrivateInstitutions => feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.PrivateInstitutions.ToString()),
                FeeOperationType.DailyServers => feeQuery.Where(x => x.Fee.OperationFeeType == FeeOperationType.DailyServers.ToString()),
                _ => throw new InvalidOperationException($"Unsupported fee operation type: {feeOperationType}")
            };

            return feeQuery; // Return the query.
        }

        /// <summary>
        /// Calculates rate-based charges for a transaction.
        /// </summary>
        /// <param name="fees">The list of applicable fees.</param>
        /// <param name="branchId">The sender's branch ID.</param>
        /// <param name="amount">The transaction amount.</param>
        /// <returns>The calculated rate-based charge.</returns>
        private decimal CalculateRateCharge(IEnumerable<SavingProductFee> fees, string branchId, decimal amount)
        {
            var rateFee = fees.FirstOrDefault(x => x.Fee.FeeType.Equals("rate", StringComparison.OrdinalIgnoreCase));

            var rateCharge = rateFee?.Fee?.FeePolicies
                .Where(p => p.BranchId == branchId || p.IsCentralised)
                .Select(p => p.Value)
                .FirstOrDefault();

            return rateCharge.HasValue ? XAFWallet.ConvertPercentageToCharge(rateCharge.Value, amount) : 0;
        }

        /// <summary>
        /// Calculates range-based charges for a transaction.
        /// </summary>
        /// <param name="fees">The list of applicable fees.</param>
        /// <param name="branchId">The sender's branch ID.</param>
        /// <param name="amount">The transaction amount.</param>
        /// <returns>The calculated range-based charge.</returns>
        private decimal CalculateRangeCharge(IEnumerable<SavingProductFee> fees, string branchId, decimal amount)
        {
            var rangeFee = fees.FirstOrDefault(x => x.Fee.FeeType.Equals("range", StringComparison.OrdinalIgnoreCase));

            var rangeCharges = rangeFee?.Fee?.FeePolicies
                .Where(p => p.BranchId == branchId || p.IsCentralised)
                .ToList();

            // If no valid range charges found, retry with IsCentralised = false
            if (rangeCharges == null || !rangeCharges.Any())
            {
                rangeCharges = rangeFee?.Fee?.FeePolicies
                    .Where(p => !p.IsCentralised)
                    .ToList();
            }

            return rangeCharges != null && rangeCharges.Any()
                ? CurrencyNotesMapper.GetFeePolicyRange(rangeCharges, amount).Charge
                : 0;
        }



        /// <summary>
        /// Calculates the transfer charges based on the amount, product ID, transfer type, and branch-specific or centralized fee policies.
        /// Handles both percentage-based charges (rate) and range-based charges (range).
        /// </summary>
        /// <param name="amount">The amount to be transferred.</param>
        /// <param name="productid">The ID of the product related to the transfer.</param>
        /// <param name="transferType">The type of remittance transfer (e.g., WesternUnion, Ria, MoneyGram).</param>
        /// <param name="senderBranchid">The ID of the sender's branch.</param>
        /// <param name="transferType_local_Inter_branch">The policy type for local or inter-branch transfers.</param>
        /// <returns>Returns a <see cref="RemittanceChargeDto"/> containing calculated transfer charges and details.</returns>
        public async Task<RemittanceChargeDto> CalculateRemittanceTransferCharges(decimal amount, string productid, RemittanceTypes transferType, string senderBranchid, string transfterType,bool isMinorAccount)
        {
            // Initialize transfer charges DTO to store the calculated details.
            RemittanceChargeDto remittanceCharge = new RemittanceChargeDto();
            // Fetch fees for the specific product ID and transfer type, excluding deleted entries.
            IQueryable<SavingProductFee> feeQuery = _savingProductFeeRepository
                .FindBy(x => x.SavingProductId == productid
                             && x.FeePolicyType.ToLower() == transfterType.ToLower()
                             && x.FeeType.ToLower() == "transfer"
                             && !x.IsDeleted)
                .Include(x => x.Fee)
                .ThenInclude(f => f.FeePolicies);

            // Filter fees based on the specified remittance type.
            switch (transferType)
            {
                case RemittanceTypes.Ria:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == RemittanceTypes.Ria.ToString());
                    break;
                case RemittanceTypes.MoneyGram:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == RemittanceTypes.MoneyGram.ToString());
                    break;
                case RemittanceTypes.MPesa:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == RemittanceTypes.MPesa.ToString());
                    break;
                case RemittanceTypes.OFX:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == RemittanceTypes.OFX.ToString());
                    break;
                case RemittanceTypes.Payoneer:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == RemittanceTypes.Payoneer.ToString());
                    break;
                case RemittanceTypes.TrustSoftCredit:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == RemittanceTypes.TrustSoftCredit.ToString());
                    break;
                case RemittanceTypes.WesternUnion:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == RemittanceTypes.WesternUnion.ToString());
                    break;
                case RemittanceTypes.WorldRemit:
                    feeQuery = feeQuery.Where(x => x.Fee.OperationFeeType == RemittanceTypes.WorldRemit.ToString());
                    break;
                default:
                    throw new InvalidOperationException("Unsupported transfer type.");
            }

            // Execute the query to retrieve applicable fees.
            var fees = await feeQuery.ToListAsync();

            // Log and throw an error if no fees are found for the specified configuration.
            if (!fees.Any())
            {
                string errorMessage = $"Transfer charges for {transferType.ToString()} are not configured for this type of transfer ({transfterType}). Contact your administrator.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Extract rate-based fee (percentage charge) and range-based fee (specific amount charge).
            var savingProductFeeForRate = fees.FirstOrDefault(x => x.Fee.FeeType.Equals("rate", StringComparison.OrdinalIgnoreCase));
            var savingProductFeeForRange = fees.FirstOrDefault(x => x.Fee.FeeType.Equals("range", StringComparison.OrdinalIgnoreCase));

            // Determine the applicable fee and status.
            string globality = "0";
            string GlobalStatus = "Not found";

            // Select the relevant fee policy (rate-based or range-based).
            var selectedFee = savingProductFeeForRate ?? savingProductFeeForRange;

            if (selectedFee != null)
            {
                // Determine globality for the selected fee type.
                globality = selectedFee.Fee.FeePolicies
                    .FirstOrDefault(x => x.BranchId == senderBranchid)?.Value.ToString()
                    ?? selectedFee.Fee.FeePolicies
                    .FirstOrDefault(x => x.IsCentralised)?.Value.ToString()
                    ?? "0";

                // Determine status for the selected fee type.
                string status = selectedFee.Fee.FeePolicies.Any(x => x.BranchId == senderBranchid)
                    ? "Branch-specific"
                    : selectedFee.Fee.FeePolicies.Any(x => x.IsCentralised)
                        ? "Centralised"
                        : "Not found";

                // Combine globality and status.
                GlobalStatus = $"Globality: {globality}, Status: {status}";
            }

            // Retrieve range-based fees specific to the sender branch or centralized ones if not found.
            var savingProductFeeForRanges = savingProductFeeForRange?.Fee?.FeePolicies .Where(x => x.BranchId == senderBranchid).ToList();

            savingProductFeeForRanges=!savingProductFeeForRanges.Any()? savingProductFeeForRange?.Fee?.FeePolicies.Where(x => x.IsCentralised).ToList(): savingProductFeeForRanges;

            // Calculate percentage-based charge if rate-based fees exist.
            decimal percentageChargedAmount = 0;
            if (savingProductFeeForRate?.Fee?.FeePolicies.Any() == true)
            {
                percentageChargedAmount = XAFWallet.ConvertPercentageToCharge(savingProductFeeForRate.Fee.FeePolicies.FirstOrDefault().Value, amount);
            }

            // Apply the calculated charge or fallback to range-based charge.
            if (percentageChargedAmount > 0)
            {
                remittanceCharge.ServiceCharge = percentageChargedAmount;
                remittanceCharge.PercentageValue = savingProductFeeForRate.Fee.FeePolicies.FirstOrDefault().Value;
            }
            else
            {
                // Calculate charge based on range if applicable.
                decimal rangeChargeCalculated = CurrencyNotesMapper.GetFeePolicyRange(savingProductFeeForRanges, amount).Charge;
                remittanceCharge.ServiceCharge = rangeChargeCalculated;
                remittanceCharge.PercentageValue = 0;
            }

            // Populate transfer charges DTO with calculated data.
            remittanceCharge.FeeType = selectedFee?.Fee?.FeeType;
            remittanceCharge.GlobalStatus = GlobalStatus;
            remittanceCharge.RemittanceType = transferType.ToString();
            remittanceCharge.FeeName = selectedFee?.Fee?.Name;

            // Calculate total charges including additional fees (if any).
            remittanceCharge.TotalCharges = XAFWallet.CalculateCustomerWithdrawalCharges(remittanceCharge.ServiceCharge, 0, isMinorAccount);
            remittanceCharge.Charge=remittanceCharge.ServiceCharge;
            remittanceCharge.Amount=amount;
            // Return the populated transfer charges DTO.
            return remittanceCharge;
        }

        public async Task<Account> GetAccountByAccountNumber(string accountNumber, string accountType)
        {
            var account = await
                          FindBy(a => a.AccountNumber == accountNumber && !a.IsDeleted)
                          .FirstOrDefaultAsync();

            if (account == null)
            {
                var errorMessage = $"Account with Account Number '{accountNumber}' does not exist.";
                throw new InvalidOperationException(errorMessage); // Throw exception with explicit message.
            }
            


            return account;
        }

        public async Task<Account> GetMemberAccount(string customerReference, string accountType)
        {
            var account = await
                          FindBy(a => a.CustomerId == customerReference && a.AccountType == accountType && a.IsDeleted == false)
                          .FirstOrDefaultAsync();

            if (account == null)
            {
                var errorMessage = $"Account with customer reference '{customerReference}' does not exist.";
                throw new InvalidOperationException(errorMessage); // Throw exception with explicit message.
            }
            if (account.AccountType != accountType)
            {
                var errorMessage = $"Account type '{accountType}' does not exist.";
                throw new InvalidOperationException(errorMessage); // Throw exception with explicit message.
            }
            return account;
        }
        public async Task<FileDownloadInfoDto> ExportMemberAccountSummaryAsync(BranchDto branch, bool isBranch)
        {
            var summaries = await GetMembersAccountSummaries(isBranch, branch.id, branch.branchCode);

            var path = $"{_fileStoragePath}\\{branch.branchCode}\\";
            // Ensure the directory exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = $"MemberAccount_{branch.branchCode}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var filePath = Path.Combine(path, fileName);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("AccountBalances-F8");
                // File title and header information merged into a single cell
                worksheet.Cell(1, 1).Value =
                    $"{branch.Bank.name?.ToUpper()}\n" +
                    $"{branch.name.ToUpper()}\n" +
                    $"BRANCH CODE: {branch.branchCode}, TEL: {branch.telephone}\n" +
                    $"LOCATION: {branch.address.ToUpper()}\n" +
                    $"INDIVIDUAL MEMBER'S ACCOUNT SITUATIONS\n" +
                    $"DATE PRINTED: {DateTime.Now:dd-MM-yyyy hh:mm:ss}".ToUpper() + $" BY {_userInfoToken.FullName.ToUpper()}";

                // Merging the first six rows into one single cell (1,1) to (6,12)
                var mergedRange = worksheet.Range(1, 1, 6, 12);
                mergedRange.Merge();

                // Set alignment
                mergedRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                mergedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                mergedRange.Style.Alignment.WrapText = true;

                // Apply font styling
                mergedRange.Style.Font.Bold = false;
                mergedRange.Style.Font.FontSize = 12; // Increase font size to 12
                mergedRange.Style.Font.FontName = "Bahnschrift Light"; // Set font family to Bahnschrift Light

                // Apply border styling (blue border and bold)
                mergedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                mergedRange.Style.Border.OutsideBorderColor = XLColor.Blue; // Set outside border color to blue

                // Adjust all widths automatically to fit content
                worksheet.Columns().AdjustToContents();

                worksheet.Range(8, 1, 8, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                worksheet.Range(8, 1, 8, 12).Style.Border.OutsideBorderColor = XLColor.Blue;

                // Adding table headers
                var currentRow = 8;
                worksheet.Cell(currentRow, 1).Value = "Names";
                worksheet.Cell(currentRow, 2).Value = "M.References";
                worksheet.Cell(currentRow, 3).Value = "B.CD";
                worksheet.Cell(currentRow, 4).Value = "Savings";
                worksheet.Cell(currentRow, 5).Value = "Deposits";
                worksheet.Cell(currentRow, 6).Value = "P.Shares";
                worksheet.Cell(currentRow, 7).Value = "O.Shares";
                worksheet.Cell(currentRow, 8).Value = "Loans";
                worksheet.Cell(currentRow, 9).Value = "GAV";
                worksheet.Cell(currentRow, 10).Value = "Daily Collection"; // New Daily Collection column
                worksheet.Cell(currentRow, 11).Value = "Total Balances";
                worksheet.Cell(currentRow, 12).Value = "Net Balances";

                // Applying style to headers
                worksheet.Range(currentRow, 1, currentRow, 12).Style.Font.Bold = true;
                worksheet.Range(currentRow, 1, currentRow, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                worksheet.Range(currentRow, 1, currentRow, 12).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(currentRow, 1, currentRow, 12).Style.Border.OutsideBorderColor = XLColor.Blue;

                // Inserting data
                foreach (var summary in summaries)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = summary.MemberName;
                    worksheet.Cell(currentRow, 2).Value = summary.MemberReference;
                    worksheet.Cell(currentRow, 3).Value = summary.BranchCode;
                    worksheet.Cell(currentRow, 4).Value = summary.Saving;
                    worksheet.Cell(currentRow, 5).Value = summary.Deposit;
                    worksheet.Cell(currentRow, 6).Value = summary.PreferenceShare;
                    worksheet.Cell(currentRow, 7).Value = summary.Share;
                    worksheet.Cell(currentRow, 8).Value = summary.Loan;
                    worksheet.Cell(currentRow, 9).Value = summary.Gav;
                    worksheet.Cell(currentRow, 10).Value = summary.DailyCollection; // Inserting Daily Collection data
                    worksheet.Cell(currentRow, 11).Value = summary.TotalBalance;
                    worksheet.Cell(currentRow, 12).Value = summary.NetBalance;
                }

                worksheet.Columns("D", "L").Style.NumberFormat.Format = "#,##0";

                // Adding borders to the data area
                worksheet.Range(8, 1, currentRow, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                worksheet.Range(8, 1, currentRow, 12).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(8, 1, currentRow, 12).Style.Border.OutsideBorderColor = XLColor.Blue;

                // Adding the summary row at L1 and M1
                worksheet.Cell(1, 14).Value = "ACCOUNT BALANCES SUMMARY";
                worksheet.Cell(1, 15).Value = "Values";
                worksheet.Range(1, 14, 1, 15).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(1, 14, 1, 15).Style.Font.Bold = true;
                worksheet.Range(1, 14, 1, 15).Style.Fill.BackgroundColor = XLColor.LightGray;
                worksheet.Range(1, 14, 1, 15).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(1, 14, 1, 15).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                worksheet.Range(1, 14, 1, 15).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                worksheet.Range(1, 14, 1, 15).Style.Border.OutsideBorderColor = XLColor.Blue;

                // Adding Number of Accounts
                currentRow = 2;
                worksheet.Cell(currentRow, 14).Value = "Number of Accounts";
                worksheet.Cell(currentRow, 15).Value = summaries.Count;

                currentRow++;
                worksheet.Cell(currentRow, 14).Value = "Total Savings";
                worksheet.Cell(currentRow, 15).Value = summaries.Sum(x => x.Saving);

                currentRow++;
                worksheet.Cell(currentRow, 14).Value = "Total Deposits";
                worksheet.Cell(currentRow, 15).Value = summaries.Sum(x => x.Deposit);

                currentRow++;
                worksheet.Cell(currentRow, 14).Value = "Total P.Shares";
                worksheet.Cell(currentRow, 15).Value = summaries.Sum(x => x.PreferenceShare);

                currentRow++;
                worksheet.Cell(currentRow, 14).Value = "Total O.Share";
                worksheet.Cell(currentRow, 15).Value = summaries.Sum(x => x.Share);

                currentRow++;
                worksheet.Cell(currentRow, 14).Value = "Total Loans";
                worksheet.Cell(currentRow, 15).Value = summaries.Sum(x => x.Loan);

                currentRow++;
                worksheet.Cell(currentRow, 14).Value = "Total GAV";
                worksheet.Cell(currentRow, 15).Value = summaries.Sum(x => x.Gav);

                currentRow++;
                worksheet.Cell(currentRow, 14).Value = "Total Daily Collection";
                worksheet.Cell(currentRow, 15).Value = summaries.Sum(x => x.DailyCollection); // Summarizing Daily Collection

                currentRow++;
                worksheet.Cell(currentRow, 14).Value = "Total Balances";
                worksheet.Cell(currentRow, 15).Value = summaries.Sum(x => x.TotalBalance);

                currentRow++;
                worksheet.Cell(currentRow, 14).Value = "Net Balances";
                worksheet.Cell(currentRow, 15).Value = summaries.Sum(x => x.NetBalance);

                // Formatting columns as currency with 0 decimal places
                worksheet.Columns("O").Style.NumberFormat.Format = "#,##0";

                // Adding borders to summary area
                worksheet.Range(2, 14, currentRow, 15).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                worksheet.Range(2, 14, currentRow, 15).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                worksheet.Range(2, 14, currentRow, 15).Style.Border.OutsideBorderColor = XLColor.Blue;

                // Apply the font to the entire worksheet
                worksheet.Cells().Style.Font.FontName = "Bahnschrift Light";

                // Automatically adjust column widths based on content
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(filePath);
            }

            // Return the download path
            var downloadPath = $"/MembersAccountExport/{branch.branchCode}/{fileName}";
            var fileDownloadInfo = CreateFileDownloadInfo(fileName, downloadPath, filePath);
            return fileDownloadInfo;
        }

        //public async Task<FileDownloadInfoDto> ExportMemberAccountSummaryAsync(BranchDto branch, bool isBranch)
        //{
        //    var summaries = await GetMembersAccountSummaries(isBranch, branch.id, branch.branchCode);

        //    var path = $"{_fileStoragePath}\\{branch.branchCode}\\";
        //    // Ensure the directory exists
        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }

        //    var fileName = $"MemberAccount_{branch.branchCode}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        //    var filePath = Path.Combine(path, fileName);

        //    using (var workbook = new XLWorkbook())
        //    {
        //        var worksheet = workbook.Worksheets.Add("AccountBalances-F8");
        //        // File title and header information merged into a single cell
        //        worksheet.Cell(1, 1).Value =
        //            $"{branch.name.ToUpper()}\n" +
        //            $"BRANCH CODE: {branch.branchCode}, TEL: {branch.telephone}\n" +
        //            $"LOCATION: {branch.address.ToUpper()}\n" +
        //            $"INDIVIDUAL MEMBER'S ACCOUNT SITUALTIONS\n" +
        //            $"DATE PRINTED: {DateTime.Now:dd-MM-yyyy hh:mm:ss}".ToUpper() + $" BY {_userInfoToken.FullName.ToUpper()}";

        //        // Merging the first six rows into one single cell (1,1) to (6,10)
        //        var mergedRange = worksheet.Range(1, 1, 6, 10);
        //        mergedRange.Merge();

        //        // Set alignment
        //        mergedRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        mergedRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        //        mergedRange.Style.Alignment.WrapText = true;

        //        // Apply font styling
        //        mergedRange.Style.Font.Bold = false;
        //        mergedRange.Style.Font.FontSize = 12; // Increase font size to 12
        //        mergedRange.Style.Font.FontName = "Bahnschrift Light"; // Set font family to Bahnschrift Light

        //        // Apply border styling (blue border and bold)
        //        mergedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        //        mergedRange.Style.Border.OutsideBorderColor = XLColor.Blue; // Set outside border color to blue

        //        // Adjust all widths automatically to fit content
        //        worksheet.Columns().AdjustToContents();

        //        worksheet.Range(8, 1, 8, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        //        worksheet.Range(8, 1, 8, 10).Style.Border.OutsideBorderColor = XLColor.Blue;

        //        // Adding table headers
        //        var currentRow = 8;
        //        worksheet.Cell(currentRow, 1).Value = "Names";
        //        worksheet.Cell(currentRow, 2).Value = "M.References";
        //        worksheet.Cell(currentRow, 3).Value = "B.CD";
        //        worksheet.Cell(currentRow, 4).Value = "Savings";
        //        worksheet.Cell(currentRow, 5).Value = "Deposits";
        //        worksheet.Cell(currentRow, 6).Value = "P.Shares";
        //        worksheet.Cell(currentRow, 7).Value = "O.Shares";
        //        worksheet.Cell(currentRow, 8).Value = "Loans";
        //        worksheet.Cell(currentRow, 9).Value = "Total Balances";
        //        worksheet.Cell(currentRow, 10).Value = "Net Balances";

        //        // Applying style to headers
        //        worksheet.Range(currentRow, 1, currentRow, 10).Style.Font.Bold = true;
        //        //worksheet.Range(currentRow, 1, currentRow, 10).Style.Fill.BackgroundColor = XLColor.SplashedWhite;
        //        worksheet.Range(currentRow, 1, currentRow, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        //        worksheet.Range(currentRow, 1, currentRow, 10).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Range(currentRow, 1, currentRow, 10).Style.Border.OutsideBorderColor = XLColor.Blue;

        //        // Inserting data
        //        foreach (var summary in summaries)
        //        {
        //            currentRow++;
        //            worksheet.Cell(currentRow, 1).Value = summary.MemberName;
        //            worksheet.Cell(currentRow, 2).Value = summary.MemberReference;
        //            worksheet.Cell(currentRow, 3).Value = summary.BranchCode;
        //            worksheet.Cell(currentRow, 4).Value = summary.Saving;
        //            worksheet.Cell(currentRow, 5).Value = summary.Deposit;
        //            worksheet.Cell(currentRow, 6).Value = summary.PreferenceShare;
        //            worksheet.Cell(currentRow, 7).Value = summary.Share;
        //            worksheet.Cell(currentRow, 8).Value = summary.Loan;
        //            worksheet.Cell(currentRow, 9).Value = summary.TotalBalance;
        //            worksheet.Cell(currentRow, 10).Value = summary.NetBalance;
        //        }

        //        worksheet.Columns("D", "J").Style.NumberFormat.Format = "#,##0";

        //        // Adding borders to the data area
        //        worksheet.Range(8, 1, currentRow, 10).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        //        worksheet.Range(8, 1, currentRow, 10).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Range(8, 1, currentRow, 10).Style.Border.OutsideBorderColor = XLColor.Blue;

        //        // Adding the summary row at L1 and M1
        //        worksheet.Cell(1, 12).Value = "ACCOUNT BALANCES SUMMARY";
        //        worksheet.Cell(1, 13).Value = "Values";
        //        worksheet.Range(1, 12, 1, 13).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //        worksheet.Range(1, 12, 1, 13).Style.Font.Bold = true;
        //        worksheet.Range(1, 12, 1, 13).Style.Fill.BackgroundColor = XLColor.LightGray;
        //        worksheet.Range(1, 12, 1, 13).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Range(1, 12, 1, 13).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        //        worksheet.Range(1, 12, 1, 13).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        //        worksheet.Range(1, 12, 1, 13).Style.Border.OutsideBorderColor = XLColor.Blue;

        //        // Adding Number of Accounts
        //        currentRow = 2;
        //        worksheet.Cell(currentRow, 12).Value = "Number of Accounts";
        //        worksheet.Cell(currentRow, 13).Value = summaries.Count;

        //        currentRow++;
        //        worksheet.Cell(currentRow, 12).Value = "Total Savings";
        //        worksheet.Cell(currentRow, 13).Value = summaries.Sum(x => x.Saving);

        //        currentRow++;
        //        worksheet.Cell(currentRow, 12).Value = "Total Deposits";
        //        worksheet.Cell(currentRow, 13).Value = summaries.Sum(x => x.Deposit);

        //        currentRow++;
        //        worksheet.Cell(currentRow, 12).Value = "Total P.Shares";
        //        worksheet.Cell(currentRow, 13).Value = summaries.Sum(x => x.PreferenceShare);

        //        currentRow++;
        //        worksheet.Cell(currentRow, 12).Value = "Total O.Share";
        //        worksheet.Cell(currentRow, 13).Value = summaries.Sum(x => x.Share);

        //        currentRow++;
        //        worksheet.Cell(currentRow, 12).Value = "Total Loans";
        //        worksheet.Cell(currentRow, 13).Value = summaries.Sum(x => x.Loan);

        //        currentRow++;
        //        worksheet.Cell(currentRow, 12).Value = "Total Balances";
        //        worksheet.Cell(currentRow, 13).Value = summaries.Sum(x => x.TotalBalance);

        //        currentRow++;
        //        worksheet.Cell(currentRow, 12).Value = "Net Balances";
        //        worksheet.Cell(currentRow, 13).Value = summaries.Sum(x => x.NetBalance);

        //        // Formatting columns as currency with 0 decimal places
        //        worksheet.Columns("M").Style.NumberFormat.Format = "#,##0";

        //        // Adding borders to summary area
        //        worksheet.Range(2, 12, currentRow, 13).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        //        worksheet.Range(2, 12, currentRow, 13).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //        worksheet.Range(2, 12, currentRow, 13).Style.Border.OutsideBorderColor = XLColor.Blue;

        //        // Apply the font to the entire worksheet
        //        worksheet.Cells().Style.Font.FontName = "Bahnschrift Light";

        //        // Automatically adjust column widths based on content
        //        worksheet.Columns().AdjustToContents();

        //        workbook.SaveAs(filePath);
        //    }

        //    // Return the download path
        //    var downloadPath = $"/MembersAccountExport/{branch.branchCode}/{fileName}";
        //    var fileDownloadInfo = CreateFileDownloadInfo(fileName, downloadPath, filePath);
        //    return fileDownloadInfo;
        //}

        /// <summary>
        /// Export customer balances asynchronously for all ordinary accounts.
        /// </summary>
        /// <returns>Information about the downloaded file</returns>
        public async Task<FileDownloadInfoDto> ExportCustomerBalancesAsync()
        {
            // Retrieve all accounts that are not deleted
            var accounts = await All.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync();

            // Generate a unique file name for the Excel file
            string fileName = $"All_IABalances_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string filePath = Path.Combine(_fileStoragePath, fileName);

            // Group accounts by customer and calculate total balances
            var customerGroups = GetCustomerGroups(accounts);

            // Create a new Excel workbook
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("All_MembersBalances");

                // Set headers and branch information
                List<string> accountTypes = await SetHeaderAndBranchInfo(worksheet, "AlL-Branches", _userInfoToken.BankCode);

                // Populate data in the worksheet
                PopulateData(worksheet, customerGroups, accountTypes);

                // Save the workbook to the file path
                SaveWorkbook(workbook, filePath);
            }

            // Create and return FileDownloadInfoDto
            return CreateFileDownloadInfo(fileName, $"/AccountExports/{fileName}", filePath);
        }

        /// <summary>
        /// Export customer balances asynchronously for all ordinary accounts by branch ID.
        /// </summary>
        /// <param name="branch">Branch information</param>
        /// <returns>Information about the downloaded file</returns>
        public async Task<FileDownloadInfoDto> ExportCustomerBalancesByBranchIdAsync(BranchDto branch)
        {
            // Retrieve accounts by branch ID that are not deleted
            var accounts = await FindBy(x => x.BranchId == branch.id && !x.IsDeleted).AsNoTracking().ToListAsync();

            // Generate a unique file name for the Excel file based on branch information
            string fileName = $"{branch.branchCode}_{branch.bankInitial}_IABalances_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string filePath = Path.Combine(_fileStoragePath, fileName);

            // Group accounts by customer and calculate total balances
            var customerGroups = GetCustomerGroups(accounts);

            // Create a new Excel workbook
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add($"{branch.branchCode}_MembersBalances");

                // Set headers and branch information
                List<string> accountTypes = await SetHeaderAndBranchInfo(worksheet, branch.name, branch.branchCode);

                // Populate data in the worksheet
                PopulateData(worksheet, customerGroups, accountTypes);

                // Save the workbook to the file path
                SaveWorkbook(workbook, filePath);
            }

            // Create and return FileDownloadInfoDto
            return CreateFileDownloadInfo(fileName, $"/AccountExports/{fileName}", filePath);
        }

        private FileDownloadInfoDto CreateFileDownloadInfo(string fileName, string downloadPath, string filePath)
        {
            return new FileDownloadInfoDto
            {
                Id = Guid.NewGuid().ToString(),
                FileName = Path.GetFileNameWithoutExtension(fileName),
                Extension = Path.GetExtension(fileName),
                DownloadPath = downloadPath,
                FullPath = filePath,
                FileType = "Excel",
                TransactionType = "Individual Account Summaries",
                Size = BaseUtilities.GetFileSize(filePath) // Implement GetFileSize method to get the file size
            };
        }

        /// <summary>
        /// Groups accounts by customer and calculates total balances.
        /// </summary>
        /// <param name="accounts">Queryable collection of accounts</param>
        /// <returns>List of customer groups</returns>
        private List<CustomerGroup> GetCustomerGroups(List<Account> accounts)
        {
            // Group by CustomerId
            var groupedAccounts = accounts
                .GroupBy(a => a.CustomerId)
                .Where(g => g.Key != null)
                .ToList(); // Materialize the query to ensure the subsequent LINQ operations are performed in memory

            // Project into CustomerGroup objects
            var customerGroups = groupedAccounts
                .Select(g => new CustomerGroup
                {
                    CustomerId = g.Key,
                    CustomerName = g.First().CustomerName,
                    BranchCode = g.First().BranchCode,
                    TotalBalance = g.Sum(a => a.Balance),
                    AccountBalances = g
                        .GroupBy(a => a.AccountType)
                        .ToDictionary(ag => ag.Key, ag => ag.Sum(a => a.Balance))
                })
                .ToList();

            return customerGroups;
        }


        /// <summary>
        /// Sets headers and branch information in the Excel worksheet.
        /// </summary>
        /// <param name="worksheet">Excel worksheet</param>
        /// <param name="branchName">Branch name</param>
        /// <param name="code">Branch code</param>
        /// <returns>List of account types</returns>
        private async Task<List<string>> SetHeaderAndBranchInfo(IXLWorksheet worksheet, string branchName, string code)
        {
            var savingProducts = await _savingProductRepository.All.ToListAsync();
            List<string> accountTypes = savingProducts.Select(sp => sp.AccountType).ToList();

            var headerColumns = 4 + accountTypes.Count;

            // Set header styles and merge cells
            worksheet.Range(1, 1, 1, headerColumns).Merge().Style
                     .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                     .Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);
            worksheet.Cell(1, 1).Value = $"Individual Account Balances As Of Date {DateTime.Now:dd-MMM-yyyy, hh:mm:ss}";

            // Set branch information and merge cells
            var branchNameRange = worksheet.Range(2, 1, 2, headerColumns);
            branchNameRange.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(2, 1).Value = $"BranchName: {branchName}";
            branchNameRange.Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            var codeRange = worksheet.Range(3, 1, 3, headerColumns);
            codeRange.Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Cell(3, 1).Value = $"Code: {code}";
            codeRange.Style.Font.SetBold().Font.SetFontSize(12).Font.SetFontColor(XLColor.Black);

            // Set column headers for account types
            worksheet.Cell(4, 1).Value = "Acc Number";
            worksheet.Cell(4, 2).Value = "Names";
            worksheet.Cell(4, 3).Value = "Code";

            // Apply styles to the header row and add borders
            var headerRow = worksheet.Range(4, 1, 4, headerColumns);
            headerRow.Style.Font.SetBold().Font.SetFontSize(12);
            headerRow.Style.Fill.SetBackgroundColor(XLColor.LightBlue);
            headerRow.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            headerRow.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            for (int i = 0; i < accountTypes.Count; i++)
            {
                worksheet.Cell(4, 4 + i).Value = accountTypes[i];
            }

            worksheet.Cell(4, 4 + accountTypes.Count).Value = "Total-Acc-Balances";

            // Apply border to the last cell in the header row
            var totalBalanceCell = worksheet.Cell(4, 4 + accountTypes.Count);
            totalBalanceCell.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            totalBalanceCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Set default column width for all columns to ensure they maintain size even if empty
            for (int col = 1; col <= headerColumns; col++)
            {
                worksheet.Column(col).Width = 15; // You can adjust the width value as needed
            }

            return accountTypes;
        }
        /// <summary>
        /// Populates the Excel worksheet with customer data, including customer ID, name, branch code, account balances, and total balance.
        /// </summary>
        /// <param name="worksheet">The Excel worksheet to populate.</param>
        /// <param name="customerGroups">The list of customer groups containing customer data.</param>
        /// <param name="accountTypes">The list of account types.</param>
        private void PopulateData(IXLWorksheet worksheet, List<CustomerGroup> customerGroups, List<string> accountTypes)
        {
            // Start from row 6 to leave space for header rows
            int currentRow = 6;

            // Initialize total balances
            var totalBalances = new Dictionary<string, decimal>();
            foreach (var accountType in accountTypes)
            {
                totalBalances[accountType] = 0;
            }
            decimal grandTotalBalance = 0;

            // Loop through each customer group
            foreach (var customer in customerGroups)
            {
                // Fill customer data into respective cells
                worksheet.Cell(currentRow, 1).Value = customer.CustomerId;
                worksheet.Cell(currentRow, 2).Value = customer.CustomerName;
                worksheet.Cell(currentRow, 3).Value = customer.BranchCode;

                // Fill account balances for each account type and update totals
                for (int i = 0; i < accountTypes.Count; i++)
                {
                    var accountType = accountTypes[i];
                    var balance = customer.AccountBalances.ContainsKey(accountType) ? customer.AccountBalances[accountType] : 0;
                    worksheet.Cell(currentRow, 4 + i).Value = balance;
                    worksheet.Cell(currentRow, 4 + i).Style.NumberFormat.Format = "#,##0.0"; // Format with 1 decimal place and add "XAF"
                    worksheet.Cell(currentRow, 4 + i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    totalBalances[accountType] += balance;
                }

                // Fill total balance for the customer and update grand total
                worksheet.Cell(currentRow, 4 + accountTypes.Count).Value = customer.TotalBalance;
                worksheet.Cell(currentRow, 4 + accountTypes.Count).Style.NumberFormat.Format = "#,##0.0"; // Format with 1 decimal place and add "XAF"
                worksheet.Cell(currentRow, 4 + accountTypes.Count).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                grandTotalBalance += customer.TotalBalance;

                // Add borders to the customer data cells
                worksheet.Cell(currentRow, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(currentRow, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                // Move to the next row
                currentRow++;
            }

            // Add the total row
            worksheet.Cell(currentRow, 1).Value = "Total";
            worksheet.Cell(currentRow, 1).Style.Font.SetBold().Font.SetFontSize(11.5).Fill.SetBackgroundColor(XLColor.LightBlue).Border.OutsideBorder = XLBorderStyleValues.Thin;
            for (int i = 0; i < accountTypes.Count; i++)
            {
                worksheet.Cell(currentRow, 4 + i).Value = totalBalances[accountTypes[i]];
                worksheet.Cell(currentRow, 4 + i).Style.Font.SetBold().Font.SetFontSize(11.5).Fill.SetBackgroundColor(XLColor.LightBlue).Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(currentRow, 4 + i).Style.NumberFormat.Format = "[$XAF] #,##0.0"; // Format with 1 decimal place and add "XAF"
            }
            worksheet.Cell(currentRow, 4 + accountTypes.Count).Value = grandTotalBalance;
            worksheet.Cell(currentRow, 4 + accountTypes.Count).Style.Font.SetBold().Font.SetFontSize(11.5).Fill.SetBackgroundColor(XLColor.LightBlue).Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 4 + accountTypes.Count).Style.NumberFormat.Format = "[$XAF] #,##0.0"; // Format with 1 decimal place and add "XAF"

            // Add summary footer
            currentRow += 2; // Add some space before the footer
            worksheet.Cell(currentRow, 1).Value = "Summary";
            worksheet.Cell(currentRow, 1).Style.Font.SetBold().Font.SetFontSize(12.5).Fill.SetBackgroundColor(XLColor.LightBlue);
            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range(currentRow, 1, currentRow, 2).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "Total Members";
            worksheet.Cell(currentRow, 2).Value = customerGroups.Count;
            worksheet.Cell(currentRow, 1).Style.Font.SetBold().Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            currentRow++;

            var totalLoan = totalBalances.ContainsKey("Loan") ? totalBalances["Loan"] : 0;
            worksheet.Cell(currentRow, 1).Value = "Total Loan";
            worksheet.Cell(currentRow, 2).Value = totalLoan;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
            worksheet.Cell(currentRow, 1).Style.Font.SetBold().Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            currentRow++;

            var totalPShare = totalBalances.ContainsKey("PreferenceShare") ? totalBalances["PreferenceShare"] : 0;
            worksheet.Cell(currentRow, 1).Value = "Total P Shares";
            worksheet.Cell(currentRow, 2).Value = totalPShare;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
            worksheet.Cell(currentRow, 1).Style.Font.SetBold().Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            currentRow++;

            var totalMemberShare = totalBalances.ContainsKey("MemberShare") ? totalBalances["MemberShare"] : 0;
            worksheet.Cell(currentRow, 1).Value = "Total M. Shares";
            worksheet.Cell(currentRow, 2).Value = totalMemberShare;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
            worksheet.Cell(currentRow, 1).Style.Font.SetBold().Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            currentRow++;

            var totalSavings = totalBalances.ContainsKey("Saving") ? totalBalances["Saving"] : 0;
            worksheet.Cell(currentRow, 1).Value = "Total Savings";
            worksheet.Cell(currentRow, 2).Value = totalSavings;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
            worksheet.Cell(currentRow, 1).Style.Font.SetBold().Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            currentRow++;

            var totalDeposit = totalBalances.ContainsKey("Deposit") ? totalBalances["Deposit"] : 0;
            worksheet.Cell(currentRow, 1).Value = "Total Deposits";
            worksheet.Cell(currentRow, 2).Value = totalDeposit;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
            worksheet.Cell(currentRow, 1).Style.Font.SetBold().Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            currentRow++;

            worksheet.Cell(currentRow, 1).Value = "G. Total Balances";
            worksheet.Cell(currentRow, 1).Style.Font.SetBold().Font.SetFontSize(11).Fill.SetBackgroundColor(XLColor.LightBlue);
            worksheet.Cell(currentRow, 2).Value = grandTotalBalance;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "[$XAF] #,##0.0";
            worksheet.Cell(currentRow, 2).Style.Font.SetBold().Font.SetFontSize(11).Fill.SetBackgroundColor(XLColor.LightBlue);
            worksheet.Cell(currentRow, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(currentRow, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Set column widths after populating the data
            SetColumnWidths(worksheet, 4 + accountTypes.Count);
        }

        //private void SetColumnWidths(IXLWorksheet worksheet, int headerColumns)
        //{
        //    for (int col = 1; col <= headerColumns; col++)
        //    {
        //        worksheet.Column(col).AdjustToContents();
        //    }
        //}




        /// </summary>
        /// <param name="workbook">The Excel workbook to save.</param>
        /// <param name="filePath">The file path where the workbook will be saved.</param>
        private void SaveWorkbook(XLWorkbook workbook, string filePath)
        {
            // Save the workbook to the specified file path
            // Save the workbook to the file
            workbook.SaveAs(filePath);
        }

        /// <summary>
        /// Adjusts the column widths in the Excel worksheet based on the content.
        /// </summary>
        /// <param name="worksheet">The Excel worksheet whose column widths need to be adjusted.</param>
        /// <param name="headerColumns">The total number of header columns in the worksheet.</param>
        private void SetColumnWidths(IXLWorksheet worksheet, int headerColumns)
        {
            for (int col = 1; col <= headerColumns; col++)
            {
                worksheet.Column(col).AdjustToContents();
            }
        }
        /// <summary>
        /// Validates if a transfer operation can proceed, considering the account balance, blocked amounts,
        /// transfer charges, minimum balance requirements, and transfer limits.
        /// </summary>
        /// <param name="amount">The requested transfer amount.</param>
        /// <param name="account">The account initiating the transfer.</param>
        /// <param name="transferParameter">The transfer parameters including min and max limits.</param>
        /// <param name="totalAmountWithCharges">The total amount including transfer charges.</param>
        /// <param name="memberType">The type of the member (Physical or Moral Person).</param>
        /// <returns>True if the transfer is allowed; throws an exception otherwise.</returns>
        public bool CheckBalanceForTransferCharges(
            decimal amount,
            Account account,
            TransferParameter transferParameter,
            decimal totalAmountWithCharges,
            string memberType)
        {
            // Calculate the total amount to withdraw including any blocked funds
            decimal totalWithBlockedAmount = account.BlockedAmount + totalAmountWithCharges;

            // Step 1: Validate balance without considering blocked amounts
            if ((account.Balance - totalAmountWithCharges) < account.Product.MinimumAccountBalanceMoralPerson)
            {
                var errorMessage = $"Your {account.AccountType} account requires a minimum balance of " +
                                   $"{BaseUtilities.FormatCurrency(account.Product.MinimumAccountBalanceMoralPerson)}. " +
                                   $"Your current balance ({BaseUtilities.FormatCurrency(account.Balance)}) is insufficient for this operation.";

                throw new InvalidOperationException(errorMessage);
            }

            // Step 2: Validate balance considering blocked amounts
            if ((account.Balance - totalWithBlockedAmount) < account.Product.MinimumAccountBalanceMoralPerson)
            {
                var errorMessage = $"Your {account.AccountType} account requires a minimum balance of " +
                                   $"{BaseUtilities.FormatCurrency(account.Product.MinimumAccountBalanceMoralPerson)}. " +
                                   $"Your current balance ({BaseUtilities.FormatCurrency(account.Balance)}) is insufficient due to a blocked amount of " +
                                   $"{BaseUtilities.FormatCurrency(account.BlockedAmount)} for the reason: {account.ReasonOfBlocked}.";

                throw new InvalidOperationException(errorMessage);
            }

            // Step 3: Validate balance requirements based on member type
            if (memberType == MemberType.Physical_Person.ToString())
            {
                // Ensure the balance meets the minimum requirement for physical persons
                if ((account.Balance - totalAmountWithCharges) < account.Product.MinimumAccountBalancePhysicalPerson)
                {
                    var errorMessage = $"Your {account.AccountType} account requires a minimum balance of " +
                                       $"{BaseUtilities.FormatCurrency(account.Product.MinimumAccountBalancePhysicalPerson)}. " +
                                       $"Your current balance ({BaseUtilities.FormatCurrency(account.Balance)}) is insufficient.";

                    throw new InvalidOperationException(errorMessage);
                }
            }
            else
            {
                // Ensure the balance meets the minimum requirement for moral persons
                if ((account.Balance - totalAmountWithCharges) < account.Product.MinimumAccountBalanceMoralPerson)
                {
                    var errorMessage = $"Your {account.AccountType} account requires a minimum balance of " +
                                       $"{BaseUtilities.FormatCurrency(account.Product.MinimumAccountBalanceMoralPerson)}. " +
                                       $"Your current balance ({BaseUtilities.FormatCurrency(account.Balance)}) is insufficient.";

                    throw new InvalidOperationException(errorMessage);
                }
            }

            // Step 4: Validate the requested transfer amount against transfer limits
            if (DecimalComparer.IsLessThan(amount, transferParameter.MinAmount) || DecimalComparer.IsGreaterThan(amount, transferParameter.MaxAmount))
            {
                var errorMessage = $"The requested transfer amount must be between " +
                                   $"{BaseUtilities.FormatCurrency(transferParameter.MinAmount)} and {BaseUtilities.FormatCurrency(transferParameter.MaxAmount)}.";

                throw new InvalidOperationException(errorMessage);
            }

            // All checks passed, transfer is allowed
            return true;
        }

        /// <summary>
        /// Checks if a withdrawal operation can proceed, considering the account balance, blocked amounts, withdrawal charges, 
        /// minimum balance requirements, and withdrawal limits.
        /// </summary>
        /// <param name="Amount">The requested amount to withdraw.</param>
        /// <param name="account">The account from which the withdrawal is being made.</param>
        /// <param name="withdrawalParameter">The withdrawal parameters including min and max limits.</param>
        /// <param name="TotalAmountToWithdrawWithCharges">The total amount including charges.</param>
        /// <param name="memberType">The type of the member (Physical or Moral Person).</param>
        /// <returns>True if the withdrawal is allowed; throws an exception otherwise.</returns>
        public bool CheckBalanceForWithdrawalWithCharges(
            decimal Amount,
            Account account,
            WithdrawalParameter withdrawalParameter,
            decimal TotalAmountToWithdrawWithCharges,
            string memberType)
        {
            // Calculate total to withdraw including blocked amount
            decimal TotalToWithdrawWithBlockedAmount = account.BlockedAmount + TotalAmountToWithdrawWithCharges;

            // Check if the balance without blocked amounts is sufficient
            if ((account.Balance - TotalAmountToWithdrawWithCharges) < account.Product.MinimumAccountBalanceMoralPerson)
            {
                var errorMessage = $"Your {account.AccountType} account requires a minimum balance of " +
                                   $"{BaseUtilities.FormatCurrency(account.Product.MinimumAccountBalanceMoralPerson)}. " +
                                   $"Your current balance ({BaseUtilities.FormatCurrency(account.Balance)}) is insufficient for this operation.";

             

                throw new InvalidOperationException(errorMessage);
            }

            // Check if the balance after blocked amounts is sufficient
            if ((account.Balance - TotalToWithdrawWithBlockedAmount) < account.Product.MinimumAccountBalanceMoralPerson)
            {
                var errorMessage = $"Your {account.AccountType} account requires a minimum balance of " +
                                   $"{BaseUtilities.FormatCurrency(account.Product.MinimumAccountBalanceMoralPerson)}. " +
                                   $"Your current balance ({BaseUtilities.FormatCurrency(account.Balance)}) is insufficient due to a blocked amount of " +
                                   $"{BaseUtilities.FormatCurrency(account.BlockedAmount)} for the reason: {account.ReasonOfBlocked}.";

             

                throw new InvalidOperationException(errorMessage);
            }

            // Check balance requirements based on member type
            if (memberType == MemberType.Physical_Person.ToString())
            {
                // For physical persons, ensure the balance meets the minimum requirement
                if ((account.Balance - TotalAmountToWithdrawWithCharges) < account.Product.MinimumAccountBalancePhysicalPerson)
                {
                    var errorMessage = $"Your {account.AccountType} account requires a minimum balance of " +
                                       $"{BaseUtilities.FormatCurrency(account.Product.MinimumAccountBalancePhysicalPerson)}. " +
                                       $"Your current balance ({BaseUtilities.FormatCurrency(account.Balance)}) is insufficient.";

                    throw new InvalidOperationException(errorMessage);
                }
            }
            else
            {
                // For moral persons, ensure the balance meets the minimum requirement
                if ((account.Balance - TotalAmountToWithdrawWithCharges) < account.Product.MinimumAccountBalanceMoralPerson)
                {
                    var errorMessage = $"Your {account.AccountType} account requires a minimum balance of " +
                                       $"{BaseUtilities.FormatCurrency(account.Product.MinimumAccountBalanceMoralPerson)}. " +
                                       $"Your current balance ({BaseUtilities.FormatCurrency(account.Balance)}) is insufficient.";

                   

                    throw new InvalidOperationException(errorMessage);
                }
            }

            // Check if the requested amount is within the withdrawal limits
            if (DecimalComparer.IsLessThan(Amount, withdrawalParameter.MinAmount) || DecimalComparer.IsGreaterThan(Amount, withdrawalParameter.MaxAmount))
            {
                var errorMessage = $"The requested amount must be between " +
                                   $"{BaseUtilities.FormatCurrency(withdrawalParameter.MinAmount)} and {BaseUtilities.FormatCurrency(withdrawalParameter.MaxAmount)}.";

             

                throw new InvalidOperationException(errorMessage);
            }

            // All checks passed, return true
            return true;
        }

        /// <summary>
        /// Checks if the withdrawal operation can proceed based on the account balance, 
        /// minimum balance requirements, and withdrawal limits.
        /// </summary>
        /// <param name="Amount">The amount to withdraw.</param>
        /// <param name="account">The account to perform the operation on.</param>
        /// <param name="memberType">The type of member (Physical or Moral Person).</param>
        /// <param name="interOperationType">The type of inter-operation for the withdrawal.</param>
        /// <returns>True if all checks pass, otherwise throws an exception.</returns>
        public async Task<bool> CheckBalanceForWithdrawalWithOutCharges(decimal Amount, Account account, string memberType, InterOperationType interOperationType)
        {
            var interOperationTypeString = interOperationType.ToString(); // Convert InterOperationType to string
            var savingProduct = await _savingProductRepository.FindAsync(account.ProductId);

            // Retrieve withdrawal parameter based on operation type
            var withdrawalParameter = savingProduct.WithdrawalParameters
                .FirstOrDefault(dl => dl.WithdrawalType.ToString() == interOperationTypeString);

            if (withdrawalParameter == null)
            {
                var errorMessage = "Withdrawal parameters are not configured for the given operation type.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Calculate total amount to withdraw including blocked funds
            decimal TotalToWithdrawWithBlockedAmount = account.BlockedAmount + Amount;

            // Determine the minimum required balance based on the member type
            decimal minimumBalance = memberType == MemberType.Physical_Person.ToString()
                ? account.Product.MinimumAccountBalancePhysicalPerson
                : account.Product.MinimumAccountBalanceMoralPerson;

            // Validate minimum balance after withdrawal
            if ((account.Balance - Amount) < minimumBalance)
            {
                var errorMessage = $"Your {account.AccountType} account requires a minimum balance of {BaseUtilities.FormatCurrency(minimumBalance)}. " +
                                   $"Your current balance ({BaseUtilities.FormatCurrency(account.Balance)}) is insufficient for this operation.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    withdrawalParameter,
                    HttpStatusCodeEnum.Forbidden,
                    LogAction.Read,
                    LogLevelInfo.Error);
                throw new InvalidOperationException(errorMessage);
            }

            // Validate balance including blocked amount for moral person accounts
            if (memberType != MemberType.Physical_Person.ToString() &&
                (account.Balance - TotalToWithdrawWithBlockedAmount) < account.Product.MinimumAccountBalanceMoralPerson)
            {
                var errorMessage = $"Your {account.AccountType} account requires a minimum balance of {BaseUtilities.FormatCurrency(account.Product.MinimumAccountBalanceMoralPerson)}. " +
                                   $"Your current balance ({BaseUtilities.FormatCurrency(account.Balance)}) is insufficient due to a blocked amount of " +
                                   $"{BaseUtilities.FormatCurrency(account.BlockedAmount)} for the reason: {account.ReasonOfBlocked}.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    withdrawalParameter,
                    HttpStatusCodeEnum.Forbidden,
                    LogAction.Read,
                    LogLevelInfo.Error);
                throw new InvalidOperationException(errorMessage);
            }

            // Validate withdrawal amount limits
            if (DecimalComparer.IsLessThan(Amount, withdrawalParameter.MinAmount) ||
                DecimalComparer.IsGreaterThan(Amount, withdrawalParameter.MaxAmount))
            {
                var errorMessage = $"The requested withdrawal amount must be between " +
                                   $"{BaseUtilities.FormatCurrency(withdrawalParameter.MinAmount)} and {BaseUtilities.FormatCurrency(withdrawalParameter.MaxAmount)}.";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    withdrawalParameter,
                    HttpStatusCodeEnum.Forbidden,
                    LogAction.Read,
                    LogLevelInfo.Error);
                throw new InvalidOperationException(errorMessage);
            }

            return true; // All checks passed
        }

    }



    //public class AccountRepository : GenericRepository<Account, TransactionContext>, IAccountRepository
    //{
    //    private readonly ILogger<AccountRepository> _logger; // Logger for logging handler actions and errors.

    //    public AccountRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<AccountRepository> logger = null) : base(unitOfWork)
    //    {
    //        _logger = logger;
    //    }


    //}

    // Define a custom class for CustomerGroup
    public class CustomerGroup
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string BranchCode { get; set; }
        public decimal TotalBalance { get; set; }
        public Dictionary<string, decimal> AccountBalances { get; set; }
    }
}
