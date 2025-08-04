using CBS.AccountManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CBS.AccountManagement.MediatR.AccountingEntry
{
    /// <summary>
    /// The class is a utility for generating a balance sheet for a specific financial entity (branch or bank)
    /// based on provided accounts and accounting entries, with the ability to filter by entity level.
    /// The balance sheet includes information about total assets, total liabilities, and total equity.
    /// </summary>
    public class BalanceSheetGenerator
    {
        // Private fields to store the list of accounts and accounting entries
        private readonly List<Data.ChartOfAccount> accounts;
        private readonly List<Data.AccountingEntry> accountingEntries;

        // Constructor to initialize the BalanceSheetGenerator with accounts and accounting entries
        public BalanceSheetGenerator(List<Data.ChartOfAccount> accounts, List<Data.AccountingEntry> accountingEntries)
        {
            // Validate and assign the provided lists
            this.accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
            this.accountingEntries = accountingEntries ?? throw new ArgumentNullException(nameof(accountingEntries));
        }
        /// <summary>
        /// Method to generate a balance sheet based on specified parameters
        /// </summary>
        /// <param name="entityId">Id of the Branch or Bank</param>
        /// <param name="entityLevel"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        // 
        public GeneralBalanceSheet GenerateBalanceSheet(string entityId, string entityLevel, DateTime fromDate, DateTime toDate)
        {
            // Get accounts filtered by entity and category
            var accountsByEntity = GetAccountsByEntity(entityId, entityLevel);
            var assetsAccounts = FilterAccountsByCategory(accountsByEntity, "ASSETS");
            var liabilitiesAccounts = FilterAccountsByCategory(accountsByEntity, "LIABILITIES");

            // Calculate total assets, total liabilities, and total equity
            var totalAssets = CalculateTotal(DebitSelector);
            var totalLiabilities = CalculateTotal(CreditSelector);
            var totalEquity = totalAssets - totalLiabilities;

            // Create a GeneralBalanceSheet object with calculated values
            return new GeneralBalanceSheet
            {
                EntityId = entityId,
                EntityType = entityLevel,
                TotalAssets = totalAssets,
                TotalLiabilities = totalLiabilities,
                TotalEquity = totalEquity
            };
        }

        // Method to filter accounts based on the entity level
        private List<Data.ChartOfAccount> GetAccountsByEntity(string entityId, string entityLevel)
        {
            return entityLevel switch
            {
                //"BRANCH" => accounts.FindAll(x => x.BranchId == entityId),
                //"BANK" => accounts.FindAll(x => x.BankId == entityId),
                _ => new List<Data.ChartOfAccount>() // Return an empty list for unknown entity levels
            };
        }

        // Method to filter accounts based on the account category
        private List<Data.ChartOfAccount> FilterAccountsByCategory(List<Data.ChartOfAccount> accountsToFilter, string category)
        {
            return accountsToFilter.FindAll(x => x.AccountNumber.Equals(category));
        }

        // Method to calculate the total based on a specified selector function
        private decimal CalculateTotal(Func<Data.AccountingEntry, bool> selector)
        {
            // Filter accounting entries based on the selector and calculate the sum of amounts
            var entityAccountingEntries = accountingEntries.FindAll(new Predicate<Data.AccountingEntry>(selector));
            return entityAccountingEntries.Sum(a => a.Amount);
        }

        // Selector function to identify entries with a null debit account number
        private bool DebitSelector(Data.AccountingEntry entry) => entry.DebitAccountNumber == null;

        // Selector function to identify entries with a null credit account number
        private bool CreditSelector(Data.AccountingEntry entry) => entry.CreditAccountNumber == null;
    }

}
