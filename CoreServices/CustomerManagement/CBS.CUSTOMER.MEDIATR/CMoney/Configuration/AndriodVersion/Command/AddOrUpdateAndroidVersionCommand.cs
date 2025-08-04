using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Command
{
    public class AddOrUpdateAndroidVersionCommand : IRequest<ServiceResponse<AndriodVersionConfigurationDto>>
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string VersionStatus { get; set; }
        public string ApkUrl { get; set; }
        public string AppName { get; set; }
        public string AppSecretcode { get; set; }
        public string AppCode { get; set; }
        public string Environment { get; set; }
    }
}
