using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class TransactionReversalRequestData: BaseEntity
    {
        public string Id { get; set; }
        public string? ReversalRequest { get; set; }
        public string? DataBeforeReversal { get; set; }
        public string? DataAfterReversal { get; set; }
    }
}
