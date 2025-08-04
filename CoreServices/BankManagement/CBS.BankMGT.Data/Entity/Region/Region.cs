using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class Region : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CountryId { get; set; } // Foreign key
        public virtual Country Country { get; set; }
        public virtual ICollection<Division> Divisions { get; set; } = new HashSet<Division>();
 }
}
