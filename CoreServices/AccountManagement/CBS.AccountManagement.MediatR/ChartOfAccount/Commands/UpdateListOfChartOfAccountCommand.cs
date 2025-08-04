using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.ChartOfAccount.Commands
{
  
    public class UpdateListOfChartOfAccountCommand : IRequest<ServiceResponse<List<ChartOfAccountDto>>>
    {
        public   List<ChartOfAccountDto> ChartOfAccounts { get; set; }
        public string AccountClassId { get; set; }
    }
}
