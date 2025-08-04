using CBS.CUSTOMER.DATA.Dto.CMoney;
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
    /// Command for logging in a C-MONEY member.
    /// </summary>
    public class LoginCMoneyMemberCommand : IRequest<ServiceResponse<LoginResponseDto>>
    {
        /// <summary>
        /// The login ID of the member.
        /// </summary>
        public string LoginId { get; set; }
        public string NotificationToken { get; set; }
        public string? AppCode { get; set; }
        public string? AppSecret { get; set; }
        /// <summary>
        /// The PIN for authentication.
        /// </summary>
        public string PIN { get; set; }
        public string? AndroidVersion { get; set; }
        public GeoLocationResponse? GeoLocation { get; set; }
    }
    public class GeoLocationResponse
    {
        public string Ip { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Loc { get; set; }
        public string Org { get; set; }
        public string Timezone { get; set; }
        public string Readme { get; set; }
    }
}
