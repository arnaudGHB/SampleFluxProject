using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.SystemConfiguration.Data
{
    public class Region : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CountryId { get; set; }

        public virtual Country Country { get; set; }
        public virtual ICollection<Division> Divisions { get; set; } = new HashSet<Division>();
        public virtual ICollection<Town> Towns { get; set; } = new HashSet<Town>();
    }
}
