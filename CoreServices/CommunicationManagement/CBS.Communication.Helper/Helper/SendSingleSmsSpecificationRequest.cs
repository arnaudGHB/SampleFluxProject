// Ignore Spelling: Sms Api

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Communication.Helper
{
    public  class SendSingleSmsSpecificationRequest
    {
        public string? Sender { get; set; }
        public string? Recipient { get; set; }
        public string? MessageBody { get; set; }
       
    }
}
