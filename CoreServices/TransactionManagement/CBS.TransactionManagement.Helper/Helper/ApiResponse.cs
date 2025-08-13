using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Helper.Helper
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public string[] Errors { get; set; }
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}
