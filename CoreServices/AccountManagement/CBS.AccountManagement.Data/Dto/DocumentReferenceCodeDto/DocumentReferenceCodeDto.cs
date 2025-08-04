using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class DocumentReferenceCodeDto
    {
        public string Id { get; set; }
 
        public string ReferenceCode { get; set; }
        public bool HasException { get; set; }
        public string Description { get; set; }
        public string Document { get; set; }

        public string DocumentType { get; set; }

        public string DocumentId { get; set; }

        public string DocumentTypeId { get; set; }
    }
}
