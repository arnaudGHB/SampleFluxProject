using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR 
{
    public class UploadChartOfAccountQuery : IRequest<ServiceResponse<List<ChartOfAccountDto>>>
    {
        public List<ChartOfAccountDto> ChartOfAccounts { get; set; }
        public bool LanguageConvert { get; set; } 
    }
}
