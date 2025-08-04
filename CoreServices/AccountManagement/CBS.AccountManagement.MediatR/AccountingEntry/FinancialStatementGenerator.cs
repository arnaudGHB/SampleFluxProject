using CBS.AccountManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR
{
    public class FinancialStatementGenerator
    {
        // Private fields to store the list of accounts and accounting entries
        private readonly List<Data.Account> accounts;
        private readonly List<Data.AccountingEntry> accountingEntries;

        // Constructor to initialize the FinancialStatementGenerator with accounts and accounting entries
        public FinancialStatementGenerator(List<Data.Account> accounts, List<Data.AccountingEntry> accountingEntries)
        {
            // Validate and assign the provided lists
            this.accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
            this.accountingEntries = accountingEntries ?? throw new ArgumentNullException(nameof(accountingEntries));
        }

        // Method to generate an Income and Expense Statement for a specific entity, entity level, and date range
        public IncomeExpenseStatement GenerateIncomeExpenseStatement(GetFinancialStatementQuery modelQuery)
        {
            // Filter accounting entries within the specified date range
            var filteredEntries = accountingEntries
                .Where(entry => entry.TransactionDate >= modelQuery.FromDate  && entry.TransactionDate <= modelQuery.ToDate)
                .ToList();

            // Get accounts relevant to the specified entity and entity level
            var accountsByEntity = GetAccountsByEntity(modelQuery.EntityId, modelQuery.EntityType);

            // Lists to store revenue and expense accounts
            var revenueAccounts = new List<Data.Account>();
            var expenseAccounts = new List<Data.Account>();

            // Variables to calculate total revenue and expenses
            decimal totalRevenue = 0;
            decimal totalExpenses = 0;

            // Iterate through accounts to categorize them as revenue or expense
            foreach (var account in accountsByEntity)
            {
                // Filter entries related to the current account
                var accountEntries = filteredEntries
                    .Where(entry => entry.DebitAccountNumber == account.AccountNumber || entry.CreditAccountNumber == account.AccountNumber)
                    .ToList();

                // Calculate the balance for the current account
                var accountBalance = CalculateAccountBalance(account.AccountNumber, accountEntries);

                // Categorize the account based on its balance
                if (accountBalance > 0)
                {
                    revenueAccounts.Add(account);
                    totalRevenue += accountBalance;
                }
                else if (accountBalance < 0)
                {
                    expenseAccounts.Add(account);
                    totalExpenses += Math.Abs(accountBalance);
                }
            }

            // Create and return an IncomeExpenseStatement
            return new IncomeExpenseStatement
            {
                EntityId = modelQuery.EntityId,
                EntityType = modelQuery.EntityType,
                FromDate = modelQuery .FromDate,
                ToDate = modelQuery.ToDate,
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                RevenueAccounts = revenueAccounts,
                ExpenseAccounts = expenseAccounts
            };
        }

        // Method to filter accounts based on the entity level
        private List<Data.Account> GetAccountsByEntity(string entityId, string entityLevel)
        {
            return entityLevel switch
            {
                "BRANCH" => accounts.FindAll(x => x.BranchId == entityId),
                "BANK" => accounts.FindAll(x => x.BankId == entityId),
                _ => new List<Data.Account>() // Return an empty list for unknown entity levels
            };
        }

        // Method to calculate the balance for a specific account based on its entries
        private decimal CalculateAccountBalance(string accountId, List<Data.AccountingEntry> entries)
        {
            // Calculate the total debit and credit amounts for the account
            var debitTotal = entries.Where(entry => entry.DebitAccountNumber == accountId).Sum(entry => entry.Amount);
            var creditTotal = entries.Where(entry => entry.CreditAccountNumber == accountId).Sum(entry => entry.Amount);

            // Calculate and return the account balance
            return debitTotal - creditTotal;
        }
    }


}
