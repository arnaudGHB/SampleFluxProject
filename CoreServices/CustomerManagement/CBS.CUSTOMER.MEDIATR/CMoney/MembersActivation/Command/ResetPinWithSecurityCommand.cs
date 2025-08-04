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
    /// Command for resetting a PIN using the security question and answer for a C-MONEY member.
    /// </summary>
    public class ResetPinWithSecurityCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// The login ID of the member.
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// The secret question provided by the member.
        /// </summary>
        public string SecretQuestion { get; set; }

        /// <summary>
        /// The secret answer provided by the member.
        /// </summary>
        public string SecretAnswer { get; set; }
    }

}
