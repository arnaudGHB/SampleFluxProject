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
    public class SendMultipleSmsCommand : IRequest<ServiceResponse<List<NotificationDto>>>
    {
        public string? SenderService { get; set; }
        public List<SmsMessages>? Messages { get; set; }
    }
}
