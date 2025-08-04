using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class AddTransactionTrackerCommand : IRequest<ServiceResponse<bool>>
    {
        public string CommandDataType { get; set; }
        public string CommandJsonObject { get; set; }
        public string TransactionReference { get; set; }
        public bool HasPassed { get; set; }
        public int NumberOfRetry { get; set; }
        public string Id { get;  set; }
    }
}
