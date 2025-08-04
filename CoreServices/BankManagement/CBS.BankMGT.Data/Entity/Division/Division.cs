using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class Division : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RegionId { get; set; } // Foreign key
        public virtual Region Region { get; set; }
        public virtual ICollection<Subdivision> Subdivisions { get; set; } = new HashSet<Subdivision>();
  }
}
