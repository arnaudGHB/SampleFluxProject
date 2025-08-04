using System;
using System.Collections.Generic;

namespace CBS.BudgetManagement.Data
{
    public class ProjectBudgetDto
    {
        public string Id { get; set; }
        public string ProjectID { get; set; }
        public string FiscalYearID { get; set; }
        public decimal BudgetAmount { get; set; }
    }
}
