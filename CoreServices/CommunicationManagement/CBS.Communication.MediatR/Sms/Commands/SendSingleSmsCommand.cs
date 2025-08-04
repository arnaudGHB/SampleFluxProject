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
    public class SendSingleSmsCommand : IRequest<ServiceResponse<NotificationDto>>
    {
        public string? SenderService { get; set; }
        public string? Title { get; set; }
        public string? CustomerName { get; set; }
        public string? Recipient { get; set; }
        public string? MessageBody { get; set; }
        public string? MemberReference { get; set; }
    }
}
