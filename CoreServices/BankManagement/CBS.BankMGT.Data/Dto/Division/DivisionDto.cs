using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Dto
{
    public class DivisionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RegionId { get; set; } // Foreign key
        public RegionDto Region { get; set; }
        public List<SubdivisionDto> Subdivisions { get; set; }
    }
}
