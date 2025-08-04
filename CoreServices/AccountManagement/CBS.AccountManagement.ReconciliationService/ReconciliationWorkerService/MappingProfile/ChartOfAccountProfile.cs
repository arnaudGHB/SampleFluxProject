using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class ChartOfAccountProfile : Profile
    {
        public ChartOfAccountProfile()
        {
            CreateMap<Data.ChartOfAccount, ChartOfAccountDto>().ReverseMap();
            CreateMap<AddChartOfAccountCommand, Data.ChartOfAccount>();
            CreateMap<UpdateChartOfAccountCommand, Data.ChartOfAccount>();
        }
    }
}