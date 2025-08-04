using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class Subdivision : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DivisionId { get; set; } // Foreign key
        public virtual Division Division { get; set; }
        public virtual ICollection<Town> Towns { get; set; } = new HashSet<Town>();

   
    }
}
