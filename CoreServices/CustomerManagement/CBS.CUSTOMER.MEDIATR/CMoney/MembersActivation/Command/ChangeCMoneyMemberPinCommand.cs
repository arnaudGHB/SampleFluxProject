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
    /// Command for changing a C-MONEY member's PIN.
    /// </summary>
    public class ChangeCMoneyMemberPinCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// The customer ID of the member whose PIN is to be changed.
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// The old PIN to be verified before changing.
        /// </summary>
        public string OldPin { get; set; }

        /// <summary>
        /// The new PIN to be set.
        /// </summary>
        public string NewPin { get; set; }
    }
}
