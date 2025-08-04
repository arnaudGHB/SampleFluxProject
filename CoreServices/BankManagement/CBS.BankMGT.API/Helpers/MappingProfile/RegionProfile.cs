using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class RegionProfile: Profile
    {
        public RegionProfile()
        {
            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<AddRegionCommand, Region>();
            CreateMap<UpdateRegionCommand, Region>();
        }
    }
}
