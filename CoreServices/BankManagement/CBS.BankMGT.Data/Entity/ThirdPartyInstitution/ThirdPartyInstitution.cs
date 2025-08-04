using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class ThirdPartyInstitution:BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string InstitionType { get; set; }
        public string HeadOfficeTelephone { get; set; }
        public string HeadOfficeLocation { get; set; }
        public string? Email { get; set; }
        public ICollection<ThirdPartyBranche> ThirdPartyBranches { get; set; }

    }

}
