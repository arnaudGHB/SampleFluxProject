using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Dto
{
    public class SubcriptionDto
    {
        public List<EconomicActivityDto> EconomicActivities { get; set; }
        public List<OrganizationDto> Organizations { get; set; }
        public List<BankDto> Banks { get; set; }
        public List<BranchDto> Branches { get; set; }
        public List<CountryDto> Countries { get; set; }
        public List<RegionDto> Regions { get; set; }
        public List<SubdivisionDto> Subdivisions { get; set; }
        public List<DivisionDto> Divisions { get; set; }
        public List<TownDto> Towns { get; set; }
        public List<SubcriptionPackageDto> SubcriptionPackages { get; set; }

    }
}
