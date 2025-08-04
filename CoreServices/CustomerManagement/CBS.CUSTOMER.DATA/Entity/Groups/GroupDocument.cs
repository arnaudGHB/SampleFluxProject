using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity.Groups
{
    public class GroupDocument : BaseEntity
    {
        [Key]
        public string GroupDocumentId { get; set; }
        public string UrlPath { get; set; }
        public string DocumentName { get; set; }
        public string Extension { get; set; }
        public string BaseUrl { get; set; }
        public string DocumentType { get; set; }
        public string? GroupId { get; set; }
        public virtual Group? Group { get; set; }
    }
}
