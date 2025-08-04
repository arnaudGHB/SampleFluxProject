using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.OldLoanConfiguration
{
    public class OldLoanAccountingMapingDto
    {
        public string Id { get; set; }
        public string LoanTypeName { get; set; }
        public string ChartOfAccountIdForVAT { get; set; }
        public string ChartOfAccountIdForInterest { get; set; }
        public string ChartOfAccountIdForCapital { get; set; }
        public string ChartOfAccountIdForProvisionMoreThanOneYear { get; set; }
        public string ChartOfAccountIdForProvisionMoreThanTwoYear { get; set; }
        public string ChartOfAccountIdForProvisionMoreThanThreeYear { get; set; }
        public string ChartOfAccountIdForProvisionMoreThanFourYear { get; set; }

    }
}
