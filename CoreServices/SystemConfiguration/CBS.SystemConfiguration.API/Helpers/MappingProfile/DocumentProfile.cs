using AutoMapper;
using CBS.SystemConfiguration.Data;

using CBS.SystemConfiguration.MediatR.Commands;

namespace CBS.SystemConfiguration.API
{


    public class CountryProfile : Profile
    {
        public CountryProfile()
        {
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<UpdateCountryCommand, CountryDto>();
            CreateMap<AddCountryCommand, CountryDto>();
            CreateMap<AddCountryCommand, Country>();
        }
    }

    public class DivisionProfile : Profile
    {
        public DivisionProfile()
        {
            CreateMap<Division, DivisionDto>().ReverseMap();
            CreateMap<UpdateDivisionCommand, DivisionDto>();
            CreateMap<AddDivisionCommand, DivisionDto>();
            CreateMap<AddDivisionCommand, Division>();
            //CreateMap<UpdateTrialBalanceReferenceCommand, TrialBalanceReferenceDto>();
        }
    }

    public class SubdivisionProfile : Profile
    {
        public SubdivisionProfile()
        {
            CreateMap<Subdivision, SubdivisionDto>().ReverseMap();
            CreateMap<UpdateSubdivisionCommand, SubdivisionDto>();
            CreateMap<AddSubdivisionCommand, Subdivision>();
            CreateMap<AddSubdivisionCommand, SubdivisionDto>();
 
        }
    }
    public class RegionProfile : Profile
    {
        public RegionProfile()
        {
            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<UpdateRegionCommand, RegionDto>();
            CreateMap<AddRegionCommand, Region>();
            CreateMap<AddRegionCommand, RegionDto>();

        }
    }
    public class TownProfile : Profile
    {
        public TownProfile()
        {
            CreateMap<Town, TownDto>().ReverseMap();
            CreateMap<UpdateTownCommand, TownDto>();
            CreateMap<AddTownCommand, Town>();
            CreateMap<AddTownCommand, TownDto>();
          
        }
    }


}