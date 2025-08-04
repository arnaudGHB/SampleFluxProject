using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public   class TrialBalanceReferenceDto
    {
        public string Id { get; set; }
        public string? ChartOfAccountId { get; set; }

        public string? OperationSide { get; set; }

        public string? OperationSideAmortization { get; set; }
        public string? OperationSideGross { get; set; }

        public string? AmortizationChartOfAccountId { get; set; }
        public string StatementModelId { get; set; }
        public string? GrossChartOfAccountId { get; set; }
    }
}
