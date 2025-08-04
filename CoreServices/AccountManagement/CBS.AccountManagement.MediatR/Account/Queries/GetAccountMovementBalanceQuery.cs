using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Account.Queries
{
    public class GetAccountMovementBalanceQuery : IRequest<ServiceResponse<double>>
    {
 
        public string TellerSource { get; set; }
        public string BranchId { get;   set; }
    }
}
