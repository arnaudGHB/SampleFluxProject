using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
   
    public class TransactionReversalRequestDto 
    {    public string RequestMessage { get; set; }
        public string Id { get; set; }
        public string ReferenceId { get; set; } // opening balance reference id
        public string IssuedBy { get; set; }
        public DateTime IssuedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedMessage { get; set; }
        public string Status { get; set; }
    }

}
