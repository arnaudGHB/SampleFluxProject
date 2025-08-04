using AutoMapper;
using CBS.AccountingManagement.MediatR.Commands;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.API.Helpers
{
    public class UsersNotificationProfiles : Profile
    {
        public UsersNotificationProfiles()
        {
            CreateMap<UsersNotification, UsersNotificationDto>().ReverseMap();
            CreateMap<AddUserNotificationCommand, UsersNotification>();
            CreateMap<UpdateUserNotificationCommand, UsersNotification>();
        }
    }
}