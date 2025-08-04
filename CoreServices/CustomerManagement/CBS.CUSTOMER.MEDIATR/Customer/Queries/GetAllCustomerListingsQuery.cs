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
    public class GetAllCustomerListingsQuery : IRequest<ServiceResponse<List<CustomerListingDto>>>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? QueryParameter { get; set; }
        public string? BranchId { get; set; }
        public string? MembersStatusType { get; set; } // None Members, Members, Both
        public string? LegalFormStatus { get; set; } // Moral_Person, Physical_Person, Both
        public int PageSize { get; set; } = 100;
        public int PageNumber { get; set; } = 1;
    }

}
