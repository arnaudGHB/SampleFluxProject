using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServiceLayer.Objects.SmsObject
{
    public class SubSmsRequestDto
    {

        [JsonProperty("msisdn")]
        public string? Msisdn { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        public string?  Token { get; set; }
    }
}
