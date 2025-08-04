using AutoMapper;

namespace CBS.AccountManagement.API
{
    public class AccountTypeDetailProfile : Profile
    {
        public AccountTypeDetailProfile()
        {
            //AccountTypeDto
            CreateMap<MediatR.Commands.ProductAccountBookDetail, Data.AccountTypeDetail>().ReverseMap();
        }
    }
}