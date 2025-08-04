using CBS.Communication.Data.Dto;
using CBS.Communication.Helper.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Communication.MediatR.Queries
{
    public class GetAllNotificationsByCustomerReferenceQuery : IRequest<ServiceResponse<List<NotificationDto>>>
    {
        public string MemberReference { get; set; }
        public string NotificationType { get; set; }
    }
}
