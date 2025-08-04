using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class UploadAccountingEntryRules : IRequest<ServiceResponse<ConfigurationsX>>
    {
      
            public List<OperationEventDto> OperationEvents { get; set; } = new List<OperationEventDto> { };
            public List<OperationEventAttributesDto> OperationEventAttributes { get; set; } = new List<OperationEventAttributesDto>();
            public List<AccountingRuleEntryDtos> AccountingRuleEntries { get; set; } = new List<AccountingRuleEntryDtos>();
    
    }
}
