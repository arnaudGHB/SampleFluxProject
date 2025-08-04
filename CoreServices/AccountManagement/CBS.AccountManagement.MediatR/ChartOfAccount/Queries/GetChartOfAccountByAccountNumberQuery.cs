using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.ChartOfAccount.Queries
{
    public class GetChartOfAccountByAccountNumberQuery : IRequest<ServiceResponse<ChartOfAccountDto>>
    {
        /// <summary>
        /// Gets or sets the AccountNumber of the ChartOfAccount to be retrieved.
        /// </summary>
        public string AccountNumber { get; set; }
    }
}
