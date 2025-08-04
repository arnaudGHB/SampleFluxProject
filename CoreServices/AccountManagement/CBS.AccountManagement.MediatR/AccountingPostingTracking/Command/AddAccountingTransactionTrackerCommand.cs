using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Command
{
    public class AddAccountingTransactionTrackerCommand : IRequest<ServiceResponse<bool>>
    {
        public string CommandDataType { get; set; }
        public string CommandJsonObject { get; set; }
        public string TransactionReferenceId { get; set; }
        public DateTimeOffset TransactionDate { get; set; }
        public string DestinationUrl { get; set; }
        public string SourceUrl { get; set; }
        public string Id { get; set; }
        public bool HasPassed { get; set; }
        public int NumberOfRetry { get; set; }
        public DateTimeOffset DatePassed { get; set; }
    }
}
