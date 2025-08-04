using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Queries
{
    /// <summary>
    /// Query for retrieving C-MONEY member activations based on filters.
    /// </summary>
    public class GetCMoneyMemberActivationsQuery : IRequest<ServiceResponse<List<CMoneyMembersActivationAccountDto>>>
    {
        /// <summary>
        /// Determines if the query should filter by branch.
        /// </summary>
        public bool ByBranch { get; set; }

        /// <summary>
        /// Determines if the query should filter by user.
        /// </summary>
        public bool ByUser { get; set; }

        /// <summary>
        /// Determines if the query should filter by date.
        /// </summary>
        public bool ByDate { get; set; }

        /// <summary>
        /// Filter by start date of activation.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Filter by end date of activation.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// A parameter string to represent either branch ID or user ID.
        /// </summary>
        public string ParameterString { get; set; }

        /// <summary>
        /// Determines if the query should filter by active members.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Determines if the query should filter by deactivated members.
        /// </summary>
        public bool IsDeactivated { get; set; }
    }
}
