using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class EconomicActivityProfile: Profile
    {
        public EconomicActivityProfile()
        {
            CreateMap<EconomicActivity, EconomicActivityDto>().ReverseMap();
            CreateMap<AddEconomicActivityCommand, EconomicActivity>();
            CreateMap<UpdateEconomicActivityCommand, EconomicActivity>();
        }
    }
}
