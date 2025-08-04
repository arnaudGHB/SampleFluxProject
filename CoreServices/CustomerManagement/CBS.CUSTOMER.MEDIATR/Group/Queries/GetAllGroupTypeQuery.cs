using CBS.CUSTOMER.DATA.Entity.Groups;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    public class GetAllGroupTypeQuery : IRequest<ServiceResponse<List<CUSTOMER.DATA.Entity.GroupType>>>
    {
    }
}
