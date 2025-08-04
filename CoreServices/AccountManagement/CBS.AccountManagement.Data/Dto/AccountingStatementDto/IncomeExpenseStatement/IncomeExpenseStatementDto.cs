using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class IncomeExpenseStatementDto
    {
        public string Id { get; set; }
        public string? EntityType { get; set; }
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get { return TotalRevenue - TotalExpenses; } }
        public string? BranchId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public decimal CurrentBalance { get; set; }
        public string Cartegory { get; set; }

    }
}
