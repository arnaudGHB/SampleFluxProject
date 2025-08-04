using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class TownProfile: Profile
    {
        public TownProfile()
        {
            CreateMap<Town, TownDto>().ReverseMap();
            CreateMap<AddTownCommand, Town>();
            CreateMap<UpdateTownCommand, Town>();
        }
    }
}
