using AutoMapper;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.API
{
    public class SubcriptionAggregationProfile : Profile
    {
        public SubcriptionAggregationProfile()
        {
            CreateMap<SubcriptionAggregateDto, TownDto>().ReverseMap();
           
        }
    }
}
