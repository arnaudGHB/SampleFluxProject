using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class CorrespondingMappingException 
    {
        public string Id { get; set; }
 
        public string AccountNumber { get; set; }
        public string DocumentReferenceCodeId { get; set; }
        public string Category { get; set; }
        public string ChartOfAccountId { get; set; }
        public virtual ChartOfAccount ChartOfAccount { get; set; }
        public virtual DocumentReferenceCode DocumentReferenceCode { get; set; }
    }
}
