using CBS.AccountManagement.Data.Dto.AccountingRuleEntryDto;
using CBS.AccountManagement.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountingEntryRulePositionDto
    {
        public string AccountingRuleId { get; set; }
        public List<AccountingEntryRulePostingOrderDto> AccountingRuleEntries { get; set; }
    }
}
