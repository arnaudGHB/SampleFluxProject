using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class ThirdPartyBankLocations : BaseEntity
    {
        public  string Id { get; set; }
        public string BranchName { get; set; }
        public string RegionId { get; set; }
        public string DivisionId { get; set; }
        public string SubdivisionId { get; set; }
        public string Town { get; set; }
        public string Quater { get; set; }
        public string ThirdPartyBankId { get; set; }
        public virtual ThirdPartyBank ThirdPartyBank { get; set; }  
    }
}
