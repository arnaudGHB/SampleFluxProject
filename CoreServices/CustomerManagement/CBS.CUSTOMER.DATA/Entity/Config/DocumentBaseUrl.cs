using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity.Config
{
    public class DocumentBaseUrl
    {
        [Key]
        public string id { get; set; }
        public string baseURL{ get; set; }
    }
}
