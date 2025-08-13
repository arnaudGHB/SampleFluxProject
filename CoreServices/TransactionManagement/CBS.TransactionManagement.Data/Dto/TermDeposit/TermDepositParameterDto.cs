using CBS.TransactionManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class TermDepositParameterDto
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public int DurationInDay { get; set; }
        public string DurationInPeriod { get; set; }//Months, Years, Weeks, Days
        public decimal EarlyCloseFeeFlat { get; set; }
        public decimal EarlyCloseFeeRate { get; set; }
        public string? OperationEventAttributeId { get; set; }
        public bool? IsDebit { get; set; }
        public string ChartOfAccountIdPrincipalAccount { get; set; }
        public string ChartOfAccountIdInterestAccrualAccount { get; set; }
        public string ChartOfAccountIdInterestExpenseAccount { get; set; }
        public string ChartOfAccountIdInterestWriteOffAccount { get; set; }
        public string ChartOfAccountIdEarlyCloseFeeAccount { get; set; }

        // Virtual property for relationship
        public SavingProduct Product { get; set; }
    }
}
