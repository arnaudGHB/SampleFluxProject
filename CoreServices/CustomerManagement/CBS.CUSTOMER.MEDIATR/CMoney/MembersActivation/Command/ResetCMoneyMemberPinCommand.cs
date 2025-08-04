using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Command
{
    /// <summary>
    /// Command for resetting a C-MONEY member's PIN.
    /// </summary>
    public class ResetCMoneyMemberPinCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// The customer ID of the member whose PIN is to be reset.
        /// </summary>
        public string CustomerId { get; set; }
    }
}
