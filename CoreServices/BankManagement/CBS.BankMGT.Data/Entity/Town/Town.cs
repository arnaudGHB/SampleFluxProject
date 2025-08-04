using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class Town : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SubdivisionId { get; set; } // Foreign key
        public virtual Subdivision Subdivision { get; set; }
        public virtual ICollection<ThirdPartyBranche> CorrespondingBankBranchs { get; set; } = new HashSet<ThirdPartyBranche>();
    }
}
