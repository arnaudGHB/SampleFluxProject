using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto.Customers
{
    public class DownloadSuccessfullyUploadedCustomersDto
    {
        public string? FileName { get; set; }
        public Stream? Stream { get; set; }
    }
}
