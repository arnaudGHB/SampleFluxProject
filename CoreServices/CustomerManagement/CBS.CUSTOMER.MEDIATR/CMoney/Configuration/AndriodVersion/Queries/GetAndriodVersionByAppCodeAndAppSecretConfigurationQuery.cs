using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Queries
{
    public class GetAndriodVersionByAppCodeAndAppSecretConfigurationQuery : IRequest<ServiceResponse<AndriodVersionConfigurationDto>>
    {

        public string AppCode { get; set; }
        public string AppSecret { get; set; }



    }
}
