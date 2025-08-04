using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class CorrespondingMapping : BaseEntity
    {
        public string Id { get; set; }
 
        public string DocumentReferenceCodeId { get; set; }
        public string AccountNumber { get; set; }
        public string ChartOfAccountId { get; set; }
        public string Cartegory { get; set; }
      //  public string BalanceType { get; set; }
        public bool IsActive { get; set; }
 
        public virtual DocumentReferenceCode DocumentReferenceCode { get; set; }
    
        public virtual ChartOfAccount ChartOfAccount { get; set; }


    }
}
