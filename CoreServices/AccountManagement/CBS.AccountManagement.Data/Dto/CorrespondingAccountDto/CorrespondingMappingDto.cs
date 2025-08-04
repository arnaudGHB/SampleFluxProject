using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class CorrespondingMappingDto
    {
        public string Id { get; set; }
        public string ChartOfAccountId { get; set; }
        public string ReferenceCode { get; set; }
        public string AccountNumber { get; set; }
        public string Cartegory { get; set; }
        public string EndingBalanceNature { get; set; }
    }
}
