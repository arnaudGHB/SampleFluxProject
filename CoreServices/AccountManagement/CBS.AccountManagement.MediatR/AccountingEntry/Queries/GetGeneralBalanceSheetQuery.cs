using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR
{
    public class GetGeneralBalanceSheetQuery:IRequest<ServiceResponse<BalanceSheetDto>>
    {
        public string? BranchId { get; set; }
        public DateTime Date { get; set; }

        public string? DocumentId { get; set; }
    }
}
