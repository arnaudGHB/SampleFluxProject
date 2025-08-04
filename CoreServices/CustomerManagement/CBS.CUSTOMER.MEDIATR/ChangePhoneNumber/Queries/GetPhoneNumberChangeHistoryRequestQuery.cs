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
    /// Query to get the phone number history by  PhoneNumberChangeHistoryId for a C-MONEY member.
    /// </summary>
    public class GetPhoneNumberChangeHistoryRequestQuery : IRequest<ServiceResponse<PhoneNumberChangeHistory>>
    {
        public string PhoneNumberChangeHistoryId { get; set; }

    }

}
