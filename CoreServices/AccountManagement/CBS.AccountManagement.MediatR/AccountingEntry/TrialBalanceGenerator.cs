using CBS.AccountManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.MediatR.Queries;

namespace CBS.AccountManagement.MediatR
{
    // Class representing a utility to generate trial balance based on accounts and accounting entries
    public class TrialBalanceGenerator
    {
        // Private fields to store the list of accounts and accounting entries
        private readonly List<Data.Account> accounts;
        private readonly List<Data.AccountingEntry> accountingEntries;

        // Constructor to initialize the BalanceSheetGenerator with accounts and accounting entries
        public TrialBalanceGenerator(List<Data.Account> accounts, List<Data.AccountingEntry> accountingEntries)
        {
            // Validate and assign the provided lists
            this.accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));
            this.accountingEntries = accountingEntries ?? throw new ArgumentNullException(nameof(accountingEntries));
        }

        // Method to generate a Trial Balance within two dates at the level of the Branch or Bank
        public TrialBalance4ColumnDto GenerateTrialBalance4Column(Get4ColumnTrialBalanceQuery modelQuery)
        {
            // Filter accounting entries within the specified date range
            var filteredEntries = accountingEntries
                .Where(entry => entry.TransactionDate >= modelQuery.FromDate && entry.TransactionDate <= modelQuery.ToDate)
                .ToList();

            // Filter accounts based on the entity and entity level
            var accountsByEntity = GetAccountsByEntity(modelQuery.EntityId, modelQuery.EntityType);

            // Initialize dictionaries to store debit and credit totals for each account
            var debitTotals = new Dictionary<string, decimal>();
            var creditTotals = new Dictionary<string, decimal>();

            // Calculate debit and credit totals for each account
            foreach (var entry in filteredEntries)
            {
                if (entry.DebitAccountNumber != null)
                {
                    if (!debitTotals.ContainsKey(entry.DebitAccountNumber))
                    {
                        debitTotals[entry.DebitAccountNumber] = 0;
                    }

                    debitTotals[entry.DebitAccountNumber] += entry.Amount;
                }

                if (entry.CreditAccountNumber != null)
                {
                    if (!creditTotals.ContainsKey(entry.CreditAccountNumber))
                    {
                        creditTotals[entry.CreditAccountNumber] = 0;
                    }

                    creditTotals[entry.CreditAccountNumber] += entry.Amount;
                }
            }
            // Combine debit and credit totals for each account to calculate balances
            var trialBalanceItems = new List<TrialBalanceItem>();
            var trialBalance6Items = new List<TrialBalance6Item>();

            foreach (var account in accountsByEntity)
            {
                var debitTotal = debitTotals.GetValueOrDefault(account.AccountNumber, 0);
                var creditTotal = creditTotals.GetValueOrDefault(account.AccountNumber, 0);
                var balance = debitTotal - creditTotal;
                trialBalanceItems.Add(new TrialBalanceItem
                {
                    AccountNumber = account.AccountNumber,
                    //AccountName = account.Description,
                    BeginningBalance = (account.BeginningBalance > 0)
                        ? account.BeginningBalance.ToString() + "C"
                        : account.BeginningBalance.ToString() + "D",
                    DebitBalance = debitTotal.ToString(),
                    CreditBalance = creditTotal.ToString(),
                    EndBalance = balance.ToString()
                });
            }
            // Class representing the Trial Balance
            return new TrialBalance4ColumnDto()
            {
                EntityId = modelQuery.EntityId,
                EntityType = modelQuery.EntityType,
                FromDate = modelQuery.FromDate,
                ToDate = modelQuery.ToDate,
                Items = trialBalanceItems
            };
         

        }

        public TrialBalance6ColumnDto GenerateTrialBalance6Column(Get6ColumnTrialBalanceQuery modelQuery )
        {
            // Filter accounting entries within the specified date range
            var filteredEntries = accountingEntries
                .Where(entry => entry.TransactionDate >= modelQuery.FromDate && entry.TransactionDate <= modelQuery.ToDate)
                .ToList();

            // Filter accounts based on the entity and entity level
            var accountsByEntity = GetAccountsByEntity(modelQuery.EntityId , modelQuery.EntityType);

            // Initialize dictionaries to store debit and credit totals for each account
            var debitTotals = new Dictionary<string, decimal>();
            var creditTotals = new Dictionary<string, decimal>();

            // Calculate debit and credit totals for each account
            foreach (var entry in filteredEntries)
            {
                if (entry.DebitAccountNumber != null)
                {
                    if (!debitTotals.ContainsKey(entry.DebitAccountNumber))
                    {
                        debitTotals[entry.DebitAccountNumber] = 0;
                    }

                    debitTotals[entry.DebitAccountNumber] += entry.Amount;
                }

                if (entry.CreditAccountNumber != null)
                {
                    if (!creditTotals.ContainsKey(entry.CreditAccountNumber))
                    {
                        creditTotals[entry.CreditAccountNumber] = 0;
                    }

                    creditTotals[entry.CreditAccountNumber] += entry.Amount;
                }
            }
            // Combine debit and credit totals for each account to calculate balances
    
            var trialBalance6Items = new List<TrialBalance6Item>();

            foreach (var account in accountsByEntity)
            {
                var debitTotal = debitTotals.GetValueOrDefault(account.AccountNumber, 0);
                var creditTotal = creditTotals.GetValueOrDefault(account.AccountNumber, 0);
                var balance = debitTotal - creditTotal;
                trialBalance6Items.Add(new TrialBalance6Item
                {
                    AccountNumber = account.AccountNumber,
                    AccountName = account.AccountNumber,
                    CreditBeginningBalance = (account.BeginningBalance > 0)? account.BeginningBalance.ToString():"0",
                    DebitBeginningBalance = (account.BeginningBalance < 0) ? account.BeginningBalance.ToString() : "0",
                    CreditBalance = creditTotal.ToString(),
                    DebitBalance = debitTotal.ToString(),
                    CreditEndBalance = (balance > 0) ? balance.ToString() : "0",
                    DebitEndBalance = (balance < 0) ? balance.ToString() : "0",
                });
            }
            // Class representing the Trial Balance
            return new TrialBalance6ColumnDto()
            {
                EntityId = modelQuery.EntityId,
                EntityType = modelQuery.EntityType,
                FromDate = modelQuery.FromDate,
                ToDate = modelQuery.ToDate,
                Items = trialBalance6Items
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
    }
}