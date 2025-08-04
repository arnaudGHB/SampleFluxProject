using AutoMapper;
using CBS.Communication.Data.Dto;
using CBS.Communication.Data.Entity;
using CBS.Communication.MediatR.Sms.Commands;

namespace CBS.Communication.API.Helpers.MappingProfile
{
    public class CommunicationProfiles : Profile
    {
        public CommunicationProfiles()
        {
            CreateMap<Notification, NotificationDto>().ReverseMap();
            CreateMap<SendSinglePushNotificationCommand, Notification>();
            CreateMap<SendSingleSmsCommand, Notification>();
            CreateMap<SendSingleSmsHalfCommand, Notification>();
        }
    }
}
