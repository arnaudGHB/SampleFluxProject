// Ignore Spelling: Sms Api

using CBS.Communication.Helper.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Communication.Helper
{
    public  class SendMultipleSmsRequest
    {

        [JsonProperty("sender")]
        public string? Sender { get; set; }

        [JsonProperty("messages")]
        public List<SendSingleSmsSpecificationRequest>? BodyMessages { get; set; }
    }
}
