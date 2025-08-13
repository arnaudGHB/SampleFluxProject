using AutoMapper;
using CBS.UserServiceMGT.Data;
using CBS.UserServiceMGT.MediatR;


namespace CBS.UserServiceMGT.API
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
                     CreateMap<User, UserDto>().ReverseMap();
           // CreateMap<UpdateUserCommand, User>().ReverseMap();
            CreateMap<AddUserCommand, User>().ReverseMap();

        }
    }
}
