using AutoMapper;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.MediatR;


namespace CBS.UserServiceManagement.API
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
