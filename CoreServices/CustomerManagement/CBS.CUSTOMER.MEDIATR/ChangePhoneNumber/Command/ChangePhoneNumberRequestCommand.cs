using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CMoney.ChangePhoneNumber.Command
{
    /// <summary>
    /// Command to change the phone number for a C-MONEY member.
    /// </summary>
    public class ChangePhoneNumberRequestCommand : IRequest<ServiceResponse<bool>>
    {
        public string CustomerId { get; set; }
        public string OldPhoneNumber { get; set; }
        public string NewPhoneNumber { get; set; }
        public string RequestComment { get; set; }
        public int OtpCode { get; set; }


    }

}
