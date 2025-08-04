using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
     public class DepreciationEntry : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string AssetId { get; set; }
        public DateTime DepreciationDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DepreciationAmount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal BookValueAfter { get; set; }

        public Asset Asset { get; set; }
    }
}
