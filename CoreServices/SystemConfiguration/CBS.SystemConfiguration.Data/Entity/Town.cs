using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.SystemConfiguration.Data
{
    public class Town : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SubdivisionId { get; set; }
        public string DivisionId { get; set; }
        public string? RegionId { get; set; }
        public virtual Subdivision Subdivision { get; set; }
        public virtual Division Division { get; set; }
        public virtual Region Region { get; set; }

    }

}
