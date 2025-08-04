using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public static class AccountMapper
    {
        public static AccountDto Map(Account account)
        {
            return new AccountDto
            {
                Id = account.Id,
            
                AccountNumber = account.AccountNumber,
                AccountNumberNetwork = account.AccountNumberNetwork,
                AccountNumberReference = account.AccountNumberReference,
                AccountNumberCU = account.AccountNumberCU,
                AccountName = account.AccountName,
                BeginningBalance = account.BeginningBalance,
                CurrentBalance = account.CurrentBalance,
                LastBalance = account.LastBalance,
                Status = account.Status,
                BranchId = account.BranchId,
                CanBeNegative = account.CanBeNegative,
                BankId = account.BankId,
                ChartOfAccountManagementPositionId = account.ChartOfAccountManagementPositionId,
                AccountOwnerId = account.AccountOwnerId,

                AccountCategoryId = account.AccountCategoryId,
                Account1 = account.Account1 ,
                Account2  = account.Account2 ,
                Account3  = account.Account3 ,
                Account4  = account.Account4 ,
                Account5  = account.Account5 ,
                Account6  = account.Account6 ,
             
               
            };
        }

        public static Account Map(AccountDto dto)
        {
            return new Account
            {
                Id = dto.Id,
                AccountNumber = dto.AccountNumber,
              
            };
        }

        public static List<AccountDto> Map(List<Data.Account> accounts)
        {
            return accounts.Select(x => Map(x)).ToList();
        }
    }
}
