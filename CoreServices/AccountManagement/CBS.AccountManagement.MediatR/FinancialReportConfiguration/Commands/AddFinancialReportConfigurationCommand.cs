using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.MediatR.Queries;
using MediatR;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Data;
namespace CBS.AccountManagement.MediatR.Commands
{
    public class AddFinancialReportConfigurationCommand : IRequest<ServiceResponse<bool>>
    {
        public FinancialReportConfigurations FinancialReportConfigurations { get; set; }
    }
}
