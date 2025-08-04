using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class Organization : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CountryId { get; set; } // Foreign key
        public virtual Country Country { get; set; }
        public virtual ICollection<Bank> Banks { get; set; } = new HashSet<Bank>();
    }
}
