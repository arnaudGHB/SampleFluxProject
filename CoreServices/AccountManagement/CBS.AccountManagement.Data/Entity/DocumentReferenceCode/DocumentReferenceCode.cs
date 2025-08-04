using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class DocumentReferenceCode : BaseEntity
    {
        
        public string Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string ReferenceCode { get; set; }

        [Required]
        [MaxLength(255)]
        public string Description { get; set; }


        public string DocumentId { get; set; }

        public string DocumentTypeId { get; set; }
  


        public bool IsConditional { get; set; }
        public virtual DocumentType DocumentType { get; set; }
        public virtual Document Document  { get; set; }

        // Navigation property to Accounts
        public virtual ICollection<CorrespondingMapping> CorrespondingMappings { get; set; }
        public ICollection<ConditionalAccountReferenceFinancialReport> ConditionalAccountReferenceFinancialReport { get; set; }
        public ICollection<CorrespondingMappingException> CorrespondingMappingExceptions { get; set; }
        public bool HasException { get; set; }
    }
}
