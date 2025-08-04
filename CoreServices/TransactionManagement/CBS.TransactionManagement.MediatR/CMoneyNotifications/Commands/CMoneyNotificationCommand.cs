using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.CMoneyNotifications.Commands
{
  
    public class CMoneyNotificationCommand:IRequest<ServiceResponse<CMoneyNotificationDto>>
    {
        public string NotificationTitle { get; set; }
        public string NotificationBody { get; set; }
        public string MemberReference { get; set; }
        public string NotificationImage { get; set; }
        public CMoneyNotificationCommand(string notificationTitle, string notificationBody, string memberReference)
        {
            NotificationTitle=notificationTitle;
            NotificationBody=notificationBody;
            MemberReference=memberReference;
        }
    }
    public class CMoneyNotificationDto
    {
        public string Id { get; set; }
        public string NotificationType { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string MemberReference { get; set; }
        public string CustomerName { get; set; }
        public string BankID { get; set; }
        public string BranchID { get; set; }
        public string BranchName { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
