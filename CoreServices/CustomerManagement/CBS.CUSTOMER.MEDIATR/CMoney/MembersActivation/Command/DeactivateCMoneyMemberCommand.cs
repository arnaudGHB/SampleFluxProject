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
    /// Command for deactivating a C-MONEY member.
    /// </summary>
    public class DeactivateCMoneyMemberCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// The customer ID of the member to deactivate.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// The reason for deactivation or blocking the member.
        /// </summary>
        public string Reason { get; set; }
    }
}
