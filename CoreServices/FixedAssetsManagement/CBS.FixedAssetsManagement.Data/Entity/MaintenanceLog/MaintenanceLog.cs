using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{

    public class MaintenanceLog : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string AssetId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
        public string PerformedById { get; set; }

        public Asset Asset { get; set; }
      
    }
}
