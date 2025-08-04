using CBS.NLoan.Data;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.DocumentP
{
    public class DocumentPack : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<DocumentJoin> DocumentJoins { get; set; }
    }
}
