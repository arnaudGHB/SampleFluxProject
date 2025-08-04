using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class DocumentRef
    {
        public string BalanceSheetId { get; set; }
        public string AssetId { get; set; }
        public string LiabilityId { get; set; }
        public string ProfitAndLossId { get; set; }
        public string IncomeId { get; set; }
        public string ExpenseId { get; set; }
    }
}
