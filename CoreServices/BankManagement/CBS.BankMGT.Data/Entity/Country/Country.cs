using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class Country:BaseEntity
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Region> Regions { get; set; } = new HashSet<Region>();
        public virtual ICollection<Currency> Currencies { get; set; } = new HashSet<Currency>();

    }

}
