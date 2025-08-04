using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class CustomerApiRequest
    {
        public string id { get; set; }
        public string urlPath { get; set; }
        public string documentName { get; set; }
        public string extension { get; set; }
        public string baseUrl { get; set; }
        public string documentType { get; set; }
    }
}
