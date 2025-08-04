using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    public class GetAllCustomerByBranchIdQuery : IRequest<ServiceResponse<List<GetAllCustomers>>>
    {

        public string? BranchId { get; set; }


    }
}
