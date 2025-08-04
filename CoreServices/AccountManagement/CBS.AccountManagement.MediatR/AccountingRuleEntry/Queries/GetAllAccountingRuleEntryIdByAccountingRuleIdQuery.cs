using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Data.Dto.AccountingRuleEntryDto;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllAccountingRuleEntryIdByAccountingRuleIdQuery:IRequest<ServiceResponse<List<AccountingEntryRuleIdsDto>>>
    {
 
        public string? AccountingRuleId { get; set; }

    }
}
