// Ignore Spelling: Sms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Communication.Helper.Helper
{
    public class SendSingleSmsResponse
    {
        public string? id { get; set; }
        //public string? @from { get; set; }
        public string? to { get; set; }
        public double cost { get; set; }
        public int parts { get; set; }
        public string? status { get; set; }
    }
}
