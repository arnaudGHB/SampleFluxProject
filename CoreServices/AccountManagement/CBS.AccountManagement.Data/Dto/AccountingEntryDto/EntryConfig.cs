using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data.Dto.AccountingEntryDto
{
    public class EntryConfig
    {
        public string? OperationEventAttributeId { get; set; } 
        public decimal Amount { get; set; }
        public bool IsPrincipal { get; set; }// Indicate if OperationEventAttributeId is the entry is principal
    }
}
