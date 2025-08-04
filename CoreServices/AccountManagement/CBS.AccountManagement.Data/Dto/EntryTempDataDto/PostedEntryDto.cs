using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class PostedEntryDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string PostingSource { get; set; }
 
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string BranchId{ get; set; }
        public JToken EntryDetail { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
    }
}
