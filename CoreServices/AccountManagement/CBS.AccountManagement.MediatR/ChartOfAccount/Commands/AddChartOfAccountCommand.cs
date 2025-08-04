using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new ChartOfAccount.
    /// </summary>
    public class AddChartOfAccountCommand : IRequest<ServiceResponse<ChartOfAccountDto>>
    {

        public string RootParentId { get; set; }
        public string AccountNumber { get; set; }
        public string LabelEn { get; set; }
        public string LabelFr { get; set; }
        public bool IsBalanceAccount { get; set; }
        public bool CanBeNegative { get; set; }
        public bool IsDebit { get; set; }
        public string AccountCartegoryId { get; set; }

    }
}