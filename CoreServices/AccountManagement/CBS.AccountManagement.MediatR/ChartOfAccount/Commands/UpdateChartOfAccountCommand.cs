using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Account.
    /// </summary>
    public class UpdateChartOfAccountCommand : IRequest<ServiceResponse<ChartOfAccountDto>>
    {
        public string Id { get; set; }
        // Unique account number 
        public string AccountNumber { get; set; }
        // Descriptive label for the account
        public string LabelEn { get; set; }
        // Indicates if this is a debit account
        public string LabelFr { get; set; }
 
        public bool IsBalanceAccount { get; set; }
        public bool CanBeNegative { get; set; }
        public bool IsDebit { get; set; }
        public string AccountCartegoryId { get; set; }
        // Specifies if account balance can go negative
        public string ParentAccountId { get; set; }

        // Account number of parent account, if hierarchical
        public string ParentAccountNumber { get; set; }

 
    }
}