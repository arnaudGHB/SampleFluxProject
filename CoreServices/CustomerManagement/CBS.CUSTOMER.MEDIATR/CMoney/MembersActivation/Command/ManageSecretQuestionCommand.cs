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
    /// Command for managing secret questions and answers for a C-MONEY member.
    /// </summary>
    public class ManageSecretQuestionCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// The login ID of the member.
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// The secret question to be set or validated.
        /// </summary>
        public string SecretQuestion { get; set; }

        /// <summary>
        /// The secret answer to be set or validated.
        /// </summary>
        public string SecretAnswer { get; set; }
    }

}
