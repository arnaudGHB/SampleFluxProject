using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity.Document
{
    public class CustomerDocument:BaseEntity
    {
        [Key]
        public string DocumentId { get; set; }
        public string CustomerId { get; set; }
        public string? UrlPath { get; set; }
        public string? DocumentName { get; set; }
        public string? Extension { get; set; }
        public string? BaseUrl { get; set; }
        public string? DocumentType { get; set; }
    }
}
