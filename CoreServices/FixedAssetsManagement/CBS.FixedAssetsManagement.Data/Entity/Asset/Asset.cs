using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class Asset: BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string AssetTypeId { get; set; }
        [Required, StringLength(100)]
        public string AssetName { get; set; }
        [StringLength(50)]
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchaseCost { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentValue { get; set; }
        public string LocationId { get; set; }
        public string DepartmentId { get; set; }
        [StringLength(20)]
        public string Status { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }

        public AssetType AssetType { get; set; }
        public Location Location { get; set; }
        public Department Department { get; set; }
        public ICollection<DepreciationEntry> DepreciationEntries { get; set; }
        public ICollection<MaintenanceLog> MaintenanceLogs { get; set; }
        public ICollection<AssetTransfer> AssetTransfers { get; set; }
        public ICollection<AssetDisposal> AssetDisposals { get; set; }
        public ICollection<AssetRevaluation> AssetRevaluations { get; set; }
    }
}
