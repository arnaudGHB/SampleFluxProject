using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
 
    public class InternalAutoPostingEventCommand : IRequest<ServiceResponse<bool>>
    {
        public string EventCode { get; set; }
        public decimal Amount { get; set; }
        public string TransactionReference { get; set; }
        public string Naration { get; set; }
 

    }
}
