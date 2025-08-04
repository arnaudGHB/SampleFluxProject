// Ignore Spelling: Sms

using CBS.Communication.Data.Dto;
using CBS.Communication.Helper.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Communication.MediatR.Sms.Commands
{
    public class SendSinglePushNotificationCommand : IRequest<ServiceResponse<NotificationDto>>
    {
        public string? NotificationTitle { get; set; }
        public string? NotificationBody { get; set; }
        public string? MemberReference { get; set; }
        public string? NotificationImage { get; set; }
    }
}
