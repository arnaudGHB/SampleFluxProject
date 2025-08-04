using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class SMSDto
    {
        public string id { get; set; }
        public string to { get; set; }
        public double cost { get; set; }
        public double parts { get; set; }
        public string status { get; set; }
    }
}
