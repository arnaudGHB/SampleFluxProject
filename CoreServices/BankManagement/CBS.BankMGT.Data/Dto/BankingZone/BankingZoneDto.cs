using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data 
{
    public class BankingZoneDto
    {
        public string Id { get; set; } // Unique identifier for the banking zone
        public string Code { get; set; } // Unique code representing the zone
        public string Name { get; set; } // Name of the banking zone
        public string LocationType { get; set; } // Type of location (e.g., Urban, Rural)
        public string LocationId { get; set; } // Identifier for the location related to this zone
    }


}
