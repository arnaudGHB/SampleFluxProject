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
    /// Command to approve the phone number for a C-MONEY member.
    /// </summary>
    public class ApprovePhoneNumberRequestCommand : IRequest<ServiceResponse<bool>>
    {
        public string PhoneNumberChangeHistoryId { get; set; }
        public string ApprovalComment { get; set; }
        public bool Approved { get; set; }

    }

}
