using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Communication.Helper.Helper
{
    public class SendPushNotificationResponse
    {
        [JsonProperty("name")]
        public string? ProjectName { get; set; }
        public string? DeviceNotificationToken { get; set; }
        public string? NotificationTitle { get; set; }
        public string? NotificationBody { get; set; }
        public string? NotificationImage { get; set; }
    }
}
