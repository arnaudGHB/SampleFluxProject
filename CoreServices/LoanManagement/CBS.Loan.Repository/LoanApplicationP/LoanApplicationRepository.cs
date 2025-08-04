using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data;
using CBS.NLoan.Data.Dto.CustomerAccountDto;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.Repository.LoanProductP;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.Repository.LoanApplicationP
{
    public class LoanApplicationRepository : GenericRepository<LoanApplication, LoanContext>, ILoanApplicationRepository
    {
        private readonly ILogger<LoanApplicationRepository> _logger;
        private readonly ILoanProductRepository _loanProductRepository;
        public LoanApplicationRepository(IUnitOfWork<LoanContext> unitOfWork, ILogger<LoanApplicationRepository> logger = null, ILoanProductRepository loanProductRepository = null)
            : base(unitOfWork)
        {
            _logger = logger;
            _loanProductRepository = loanProductRepository;
        }

        public bool LoanApplicationValidator(LoanApplication loanApplication, LoanProduct loanProduct, List<LoanGuarantor> loanGuarantors, List<LoanApplicationCollateral> productCollaterals, List<DocumentAttachedToLoan> documentAttachedToLoans)
        {
            if (loanApplication.IsThereGuarantor && !loanGuarantors.Any())
            {
                LogAndThrow("A guarantor was not found. Operation aborted.");
            }
            if (loanApplication.IsThereCollateral && !productCollaterals.Any())
            {
                LogAndThrow("No collateral was found. Operation aborted.");
            }
            if (!documentAttachedToLoans.Any())
            {
                LogAndThrow("Please attach corresponding loan processed documents.");
            }

            if (loanApplication.IsThereCollateral)
            {
                ValidateCollateral(loanApplication, productCollaterals);
            }

            if (loanApplication.RequiredDownPaymentCoverageRate)
            {
                ValidateDownPaymentCoverage(loanApplication, loanProduct);
            }

            return true;
        }



        private async void ValidateCollateral(LoanApplication loanApplication, List<LoanApplicationCollateral> loanApplicationCollaterals)
        {

            try
            {
               // var LP = await _loanProductRepository.FindAsync(loanApplication.LoanProductId);
                var customerCollateralRate = loanApplicationCollaterals.Sum(x => x.Amount);
                var collateralRate = loanApplication.LoanProduct.MinimumCollateralPercentage;
                if (customerCollateralRate < collateralRate)
                {
                    var error = $"The collateral rate is not high enough. Total coverage: {customerCollateralRate}%, Required: {collateralRate}%.";
                    LogAndThrow(error);
                }
            }
            catch (Exception ex)
            {

                LogAndThrow($"Error: {ex.Message}");
            }
        }

        private void ValidateDownPaymentCoverage(LoanApplication loanApplication, LoanProduct loanProduct)
        {
            if (loanProduct == null)
            {
                LogAndThrow("Loan product is required for down payment coverage validation.");
            }

            var requiredCoverageAmount = (loanProduct.MinimumDownPaymentPercentage / 100) * loanApplication.Amount;
            if (requiredCoverageAmount > loanApplication.DownPaymentCoverageAmountProvided)
            {
                var error = $"The down payment coverage amount is not sufficient. Required: {requiredCoverageAmount}, Provided: {loanApplication.DownPaymentCoverageAmountProvided}.";
                LogAndThrow(error);
            }
        }

        private void LogAndThrow(string message)
        {
            _logger?.LogError(message);
            throw new InvalidOperationException(message);
        }

        public async Task<bool> ValidateMembersAccounts(LoanApplication loanApplication, CustomerDto customer, List<AccountDto> accounts, LoanProduct loanProduct = null)
        {
            try
            {
                ValidateAccountCoverage(loanApplication, customer, accounts);

                if (loanApplication.RequiredDownPaymentCoverageRate && loanProduct != null)
                {
                    ValidateDownPaymentCoverage(loanApplication, loanProduct);
                }

                return true;
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Error validating member's accounts.");
                throw;
            }
        }
       

        private void ValidateAccountCoverage(LoanApplication loanApplication, CustomerDto customer, List<AccountDto> accounts)
        {
            var accountTypes = new Dictionary<string, Action<AccountDto, decimal, string>>
            {
                { "termdepost", (account, requiredAmount, msg) => ValidateAccountBalance(account, requiredAmount, $"{customer.firstName} {customer.lastName} has insufficient balance in Term Deposit Account. {msg}") },
                { "preferenceshare", (account, requiredAmount, msg) => ValidateAccountBalance(account, requiredAmount, $"{customer.firstName} {customer.lastName} has insufficient balance in Preference Share Account. {msg}") },
                { "deposit", (account, requiredAmount, msg) => ValidateAccountBalance(account, requiredAmount, $"{customer.firstName} {customer.lastName} has insufficient balance in Deposit Account. {msg}") },
                { "saving", (account, requiredAmount, msg) => ValidateAccountCoverage(account, loanApplication.SavingAccountCoverageRate, account, $"{customer.firstName} {customer.lastName} saving account does not cover the loan. {msg}") },
                { "membershare", (account, requiredAmount, msg) => ValidateAccountCoverage(account, loanApplication.ShareAccountCoverageAmount, account, $"{customer.firstName} {customer.lastName} share account does not cover the loan. {msg}") },
                { "salary", (account, requiredAmount, msg) => ValidateAccountCoverage(account, loanApplication.SalaryAccountCoverageRate, account, $"{customer.firstName} {customer.lastName} salary account does not cover the loan. {msg}") }
            };

            foreach (var accountType in accountTypes)
            {
                var account = accounts.FirstOrDefault(x => x.Product.AccountType.Equals(accountType.Key, StringComparison.OrdinalIgnoreCase));
                if (account != null)
                {
                    var requiredAmount = GetRequiredAmount(loanApplication, accountType.Key);
                    accountType.Value(account, requiredAmount, "Recharge account or adjust the option.");
                }
            }
        }

        private void ValidateAccountBalance(AccountDto account, decimal requiredAmount, string errorMessage)
        {
            if (Math.Abs(account.Balance) < requiredAmount)
            {
                LogAndThrow(errorMessage);
            }
        }

        private void ValidateAccountCoverage(AccountDto account, decimal coverageRate, AccountDto loanAccount, string errorMessage)
        {
            if (account.AccountType.ToLower() != "saving")
            {

                if (account.Balance < coverageRate)
                {
                    LogAndThrow(errorMessage);
                }
            }
            else
            {
                var coverageAmount = (coverageRate / 100) * loanAccount.Balance;
                if (coverageAmount > account.Balance)
                {
                    LogAndThrow(errorMessage);
                }
            }

        }

        private decimal GetRequiredAmount(LoanApplication loanApplication, string accountType)
        {
            var data = accountType switch
            {
                "termdepost" => loanApplication.TermDeposiAccountCoverageAmount,
                "preferenceshare" => loanApplication.PreferenceShareAccountCoverageAmount,
                "deposit" => loanApplication.DepositAccountCoverageAmount,
                "" => (loanApplication.SavingAccountCoverageRate / 100) * loanApplication.Amount,
                "membershare" => loanApplication.ShareAccountCoverageAmount,
                "salary" => (loanApplication.SalaryAccountCoverageRate / 100) * loanApplication.Amount,
                _ => 0
            };
            return data;
        }
        /// <summary>
        /// Calculates the total coverage amount provided by the customer.
        /// </summary>
        /// <param name="loanApplication">The loan application with coverage details.</param>
        /// <param name="savingBalance">The balance in the saving account.</param>
        /// <returns>The total coverage amount.</returns>
        public decimal DownPaymentProvision(LoanApplication loanApplication, decimal savingBalance)
        {
           
            return loanApplication.ShareAccountCoverageAmount
                 + loanApplication.DepositAccountCoverageAmount
                 + loanApplication.TermDeposiAccountCoverageAmount
                 + (loanApplication.SavingAccountCoverageRate / 100 * savingBalance)
                 + loanApplication.PreferenceShareAccountCoverageAmount;
        }
    }
}
