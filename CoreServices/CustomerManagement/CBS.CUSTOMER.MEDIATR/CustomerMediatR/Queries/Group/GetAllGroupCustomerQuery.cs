using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    public class GetAllGroupCustomerQuery : IRequest<ServiceResponse<List<DATA.Entity.GroupCustomer>>>
    {
    }
}
