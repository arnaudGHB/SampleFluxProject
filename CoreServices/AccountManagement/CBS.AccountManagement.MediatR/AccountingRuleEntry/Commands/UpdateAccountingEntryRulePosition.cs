using CBS.AccountManagement.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto.AccountingRuleEntryDto;
using CBS.AccountManagement.Data.Entity;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class UpdateAccountingEntryRulePositionCommand : IRequest<ServiceResponse<AccountingEntryRulePositionDto>>
    {
        public string AccountingRuleId { get; set; }
        public List<AccountingEntryRulePostingOrderDto> AccountingRuleEntries { get; set; }
    }
}
