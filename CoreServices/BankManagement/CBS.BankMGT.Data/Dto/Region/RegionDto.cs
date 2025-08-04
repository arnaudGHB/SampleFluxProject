using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Dto
{
    public class RegionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CountryId { get; set; } // Foreign key
        public CountryDto Country { get; set; }
        public List<DivisionDto> Divisions { get; set; }
    }
}
