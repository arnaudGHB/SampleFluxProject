using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.Transaction
{
    public class ThirdPartyCashOut
    {
        public string TransactionReference { get; set; }
        public string OTP { get; set; }
    }
   
}
