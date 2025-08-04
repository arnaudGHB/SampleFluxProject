using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class AssetDisposal : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string AssetId { get; set; }
        public DateTime DisposalDate { get; set; }
        [StringLength(50)]
        public string DisposalMethod { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DisposalValue { get; set; }
        [StringLength(200)]
        public string Reason { get; set; }

        public Asset Asset { get; set; }
    }
}
