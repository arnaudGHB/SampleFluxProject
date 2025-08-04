using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API
{
    public class SubAccountClassProfile : Profile
    {
        public SubAccountClassProfile()
        {
            CreateMap<AccountClass, AccountClassDto>().ReverseMap();
            CreateMap<AddAccountClassCommand, AccountClass>();
            CreateMap<UpdateAccountClassCommand, AccountClass>();
        }
    }
}