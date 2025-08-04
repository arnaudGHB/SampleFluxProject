using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAccountStatusQuery : IRequest<ServiceResponse<AccountDetails>>
    {
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
        public string? BranchId { get; set; }
    }
}
