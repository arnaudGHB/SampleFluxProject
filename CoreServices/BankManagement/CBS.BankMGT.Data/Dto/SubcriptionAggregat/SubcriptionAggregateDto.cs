using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Dto
{
    public class SubcriptionAggregateDto
    {
        public SubcriptionAggregateDto(List<EconomicActivityDto> economicActivities, List<OrganizationDto> organizations, List<BankDto> banks, List<BranchDto> branches, List<CountryDto> countries, List<RegionDto> regions, List<SubdivisionDto> subdivisions, List<DivisionDto> divisions, List<TownDto> towns, List<SubcriptionPackageDto> subcriptionPackages)
        {
            EconomicActivities = economicActivities;
            Organizations = organizations;
            Banks = banks;
            Branches = branches;
            Countries = countries;
            Regions = regions;
            Subdivisions = subdivisions;
            Divisions = divisions;
            Towns = towns;
            SubcriptionPackages = subcriptionPackages;
        }

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
