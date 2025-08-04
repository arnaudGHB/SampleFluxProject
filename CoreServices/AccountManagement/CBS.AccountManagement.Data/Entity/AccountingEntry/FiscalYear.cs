namespace CBS.AccountManagement.Data
{
    public class FiscalYear : BaseEntity
    {
        public string Id { get; set; } // Unique identifier for the fiscal year
        public string Name { get; set; } // Name or label for the fiscal year (e.g., "FY2023")
        public DateTime StartDate { get; set; } // Start date of the fiscal year
        public DateTime EndDate { get; set; } // End date of the fiscal year
        public bool IsClosed { get; set; } // Flag indicating whether the fiscal year is closed
        public decimal TotalRevenue { get; set; } // Total revenue for the fiscal year
        public decimal TotalExpenses { get; set; } // Total expenses for the fiscal year
        public decimal NetIncome { get; set; } // Net income for the fiscal year
        public List<string> AssociatedDepartments { get; set; } // List of department names associated with the fiscal year
        public Dictionary<string, decimal> BudgetAllocation { get; set; } // Budget allocation for different categories or departments
        public List<string> Events { get; set; } // List of significant events or milestones during the fiscal year
        public string RegulatoryComplianceStatus { get; set; } // Status of regulatory compliance for the fiscal year
        public Dictionary<string, decimal> TaxLiabilities { get; set; } // Tax liabilities for different tax categories
        public string FinancialReportUrl { get; set; } // URL to the financial report document for the fiscal year
        public string FinancialPerformanceSummary { get; set; } // Summary or analysis of the fiscal year's financial performance
        public List<string> KeyPerformanceIndicators { get; set; } // List of key performance indicators for the fiscal year
        public string AuditOpinion { get; set; } // Opinion from an external audit regarding financial statements
        public List<string> ComplianceIssues { get; set; } // List of compliance issues or concerns during the fiscal year
        public Dictionary<string, decimal> ReserveFunds { get; set; } // Allocation and status of reserve funds
        public List<string> Stakeholders { get; set; } // List of stakeholders involved in the fiscal year planning and analysis
        public Dictionary<string, decimal> ReservedBudgets { get; set; } // Allocation and utilization of reserved budgets
        public List<string> MajorProjects { get; set; } // List of major projects or initiatives during the fiscal year
        public string FinancialStrategy { get; set; } // Brief description of the financial strategy adopted for the fiscal year
        public string RiskAssessmentSummary { get; set; } // Summary of the risk assessment related to financial activities
        public string FinancialProjectionSummary { get; set; } // Summary of financial projections for the fiscal year
        public List<string> ExternalContracts { get; set; } // List of external contracts or agreements impacting finances
        public Dictionary<string, decimal> ContingencyFunds { get; set; } // Allocation and utilization of contingency funds
        public List<string> FinancialTargets { get; set; } // List of financial targets set for the fiscal year
        public string CapitalExpenditurePlan { get; set; } // Plan detailing capital expenditures for the fiscal year
        public string FinancialNarrative { get; set; } // Narrative or commentary on the fiscal year's financial performance
        public List<string> RegulatoryChanges { get; set; } // List of significant regulatory changes affecting finances
        public Dictionary<string, decimal> ProjectedCashFlow { get; set; } // Projection of cash flow for different periods within the fiscal year
        public string FinancialPolicyUrl { get; set; } // URL to the organization's financial policy document
        public string FinancialTrainingProgram { get; set; } // Details about financial training programs conducted during the fiscal year

    }


}