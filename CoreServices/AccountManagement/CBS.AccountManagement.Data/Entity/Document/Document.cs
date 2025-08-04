
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CBS.AccountManagement.Data
{
    public class Document : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // Navigation property to Accounts
        public virtual ICollection<DocumentType> DocumentTypes { get; set; }
        public ICollection<DocumentReferenceCode> ReferenceCodes { get; set; }
    }
}


