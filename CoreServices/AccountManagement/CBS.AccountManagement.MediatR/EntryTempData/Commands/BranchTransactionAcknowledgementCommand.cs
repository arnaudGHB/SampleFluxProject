using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class BranchTransactionAcknowledgementCommand : IRequest<ServiceResponse<bool>>
    {
        public bool IsDoubleValidationNeeded { get;   set; }
        public List<EntryTempDataCommand> EntryTempDatas { get;   set; }
        public string AccountingEventRuleId { get;   set; }
        public List<string> ListOfBranchIds { get;   set; }
        public bool IsSystem { get;   set; }
        public string BranchId { get;   set; }
    }
}
