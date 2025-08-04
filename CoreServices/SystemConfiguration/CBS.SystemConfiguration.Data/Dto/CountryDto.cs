using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.SystemConfiguration.Data
{
    public class CountryDto
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

       
    }

    public class LocationDto
    {
       
        public CountryDto Country { get; set; }
        public List<RegionDto> Regions { get; set; }

        public List<DivisionDto> Divisions { get; set; }
        public List<SubdivisionDto> Subdivisions { get; set; }
        public List<TownDto> Towns { get; set; }
    }
}
