using CBS.CUSTOMER.DATA.Entity.ChangePhoneNumber;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CMoney.ChangePhoneNumber.Queries
{
    /// <summary>
    /// Query to get the phone number history  for a C-MONEY member.
    /// </summary>
    public class GetBranchPhoneNumberChangeHistoriesByStatusRequestQuery : IRequest<ServiceResponse<List<PhoneNumberChangeHistory>>>
    {
        public string Status { get; set; }

    }

}
