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
    /// Command for reactivating a C-MONEY member.
    /// </summary>
    public class ReactivateCMoneyMemberCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// The customer ID of the member to reactivate.
        /// </summary>
        public string CustomerId { get; set; }
    }
}
