using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountingEntryRecordSequence
    {

        public string SequenceName { get; set; } = string.Empty; // Name of the sequence
        public string BranchCode { get; set; } = string.Empty;  // Branch identifier
        public DateTime SequenceDate { get; set; }             // Date of the sequence
        public string CurrentValue { get; set; } = string.Empty; // Current string value
        public bool IsUsed { get; set; }=false
            ;
    }
}
