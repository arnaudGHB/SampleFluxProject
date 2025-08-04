using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class IncomeExpenseStatement
    {
        public string? Id { get; set; }
        public string? EntityId { get; set; }
        public string? EntityType { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get { return TotalRevenue - TotalExpenses; } }
        public List<Account>? RevenueAccounts { get; set; }
        public List<Account>? ExpenseAccounts { get; set; }
        public string? AccountName { get; set; }
    }
}
