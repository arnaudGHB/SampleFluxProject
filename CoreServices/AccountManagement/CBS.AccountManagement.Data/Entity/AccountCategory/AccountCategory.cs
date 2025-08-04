using System.Collections.Generic;

namespace CBS.AccountManagement.Data
{
    /// <summary>
    /// financial statement line items:
    /// BalanceSheetAsset
    /// BalanceSheetLiabilities
    /// ProfitAndLossIncome
    /// ProfitAndLossExpense
    /// </summary>
    public class AccountCategory : BaseEntity
    {
        public string Id { get; set; }
     
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Account> Account { get; set; }

     
    }
}