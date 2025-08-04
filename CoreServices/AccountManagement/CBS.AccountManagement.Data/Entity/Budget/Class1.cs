using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    // Class for representing a budget period
 

    // Class for representing organizational units
    //public class OrganizationalUnit
    //{
    //    public string Id { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public string ParentUnitId { get; set; }

    //    //public int Level { get; set; }
    //    //public bool IsActive { get; set; }
    //    //public string Location { get; set; }
    //    //public string Manager { get; set; }
    //    //public string Email { get; set; }
    //    //public string Phone { get; set; }
    //    //public string Website { get; set; }
    //    //public virtual OrganizationalUnit? ParentUnit  { get; set; }
    //}



    // Class for representing a budget
    //public class Budget
    //{
    //    public int BudgetId { get; set; }
    //    public string BudgetPeriodId { get; set; }
    //    public string ChartOfAccountId { get; set; }
    //    public string UnitId { get; set; }
    //    public virtual OrganizationalUnit Unit { get; set; }
    //    public virtual BudgetPeriod Period { get; set; }
    //    public virtual ChartOfAccount Account { get; set; }
    //    public decimal Amount { get; set; }
    //    // Other properties as needed
    //}

    // Class for representing budget approval workflow
    public class BudgetApprovalWorkflow
    {
        public int WorkflowId { get; set; }
        public string Name { get; set; }
        // Other properties as needed
    }

    // Class for representing budget approval process
    public class BudgetApprovalProcess
    {
        public int ProcessId { get; set; }
        public Budget Budget { get; set; }
        public BudgetApprovalWorkflow Workflow { get; set; }
        public List<User> Reviewers { get; set; }
        public List<User> Approvers { get; set; }
        // Other properties as needed
    }

    // Class for representing users
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        // Other properties as needed
    }

    // Class for representing budget transactions
    public class BudgetTransaction
    {
        public int TransactionId { get; set; }
        public Budget Budget { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        // Other properties as needed
    }

    // Class for representing budget adjustments
    public class BudgetAdjustment
    {
        public int AdjustmentId { get; set; }
        public Budget Budget { get; set; }
        public decimal OldAmount { get; set; }
        public decimal NewAmount { get; set; }
        public DateTime AdjustmentDate { get; set; }
        // Other properties as needed
    }

    // Class for representing budget forecasts
    public class BudgetForecast
    {
        public int ForecastId { get; set; }
        public Budget Budget { get; set; }
        public decimal ForecastedAmount { get; set; }
        public DateTime ForecastDate { get; set; }
        // Other properties as needed
    }

    // Class for representing budget policies
    public class BudgetPolicy
    {
        public int PolicyId { get; set; }
        public string Name { get; set; }
        // Other properties as needed
    }

    // Class for representing budget policy exceptions
    public class BudgetPolicyException
    {
        public int ExceptionId { get; set; }
        public BudgetPolicy Policy { get; set; }
        public Budget Budget { get; set; }
        public string Reason { get; set; }
        // Other properties as needed
    }

    // Class for representing external systems for integration
    public class ExternalSystem
    {
        public int SystemId { get; set; }
        public string Name { get; set; }
        // Other properties as needed
    }

    // Class for representing budget data import/export
    public class BudgetDataTransfer
    {
        public int TransferId { get; set; }
        public ExternalSystem SourceSystem { get; set; }
        public ExternalSystem DestinationSystem { get; set; }
        public DateTime TransferDate { get; set; }
        // Other properties as needed
    }

    // Class for representing access control and security
    public class AccessControl
    {
        public int ControlId { get; set; }
        public User User { get; set; }
        public string Role { get; set; }
        // Other properties as needed
    }

    // Class for representing budget reporting
    public class BudgetReport
    {
        public int ReportId { get; set; }
        public string Name { get; set; }
        // Other properties as needed
    }

    // Class for representing budget analytics
    public class BudgetAnalytics
    {
        public int AnalyticsId { get; set; }
        public string Name { get; set; }
        // Other properties as needed
    }

    // Class for representing integration with core banking systems
    public class CoreBankingIntegration
    {
        public int IntegrationId { get; set; }
        public string Name { get; set; }
        // Other properties as needed
    }

}
