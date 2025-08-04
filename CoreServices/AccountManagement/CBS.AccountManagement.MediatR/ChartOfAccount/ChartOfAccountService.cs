
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.ChartOfAccount
{
    public class ChartOfAccountService 
    {
        

        public async Task<List<Data.ChartOfAccount>> CreateChartOfAccountsHierarchy(string accountNumber)
        {
            if (accountNumber.Length != 6 || !int.TryParse(accountNumber, out _))
            {
                return null;
            }
            else
            {
                var createdAccounts = new List<Data.ChartOfAccount>();
                var accountLevels = new[] { 3, 4, 5, 6 };

                Data.ChartOfAccount parentAccount = null;

                //await _unitOfWork.BeginTransactionAsync();

                try
                {
                    foreach (var level in accountLevels)
                    {
                        var currentAccountNumber = accountNumber.Substring(0, level);
                        var existingAccount = createdAccounts.FindAll(x => x.AccountNumber == currentAccountNumber);

                        if (!existingAccount.Any())
                        {
                            var newAccount = new Data.ChartOfAccount
                            {
                                Id = Guid.NewGuid().ToString(),
                                AccountNumber = currentAccountNumber,
                                LabelFr = $"Compte {currentAccountNumber}", // Placeholder
                                LabelEn = $"Account {currentAccountNumber}", // Placeholder

                                ParentAccountNumber = parentAccount?.AccountNumber,
                                ParentAccountId = parentAccount?.Id
                            };

                            //_chartOfAccountRepository.Add(newAccount);
                            //await _unitOfWork.SaveAsync();

                            createdAccounts.Add(newAccount);
                            parentAccount = newAccount;
                        }
                        else
                        {
                            parentAccount = existingAccount.FirstOrDefault();
                        }
                    }


                }
                catch (Exception ex)
                {
                
                    throw(ex);
                }

                return createdAccounts;
            }

           
        }
    }
}
