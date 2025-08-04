using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.User
{
    public class OTPDto
    {
        public string OTP { get; set; }
        public DateTime ExpireDate { get; set; }
        public string Url { get; set; }
    }
}
