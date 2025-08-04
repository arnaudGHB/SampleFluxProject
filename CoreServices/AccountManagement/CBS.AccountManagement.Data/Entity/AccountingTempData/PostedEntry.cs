using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class PostedEntry :  BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string status { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public bool HasValidated { get; set; }
        public DateTime ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }

        public string PostingSource { get; set; } // 
        public JToken EntryDetail { get; set; }
    }
}
