using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class AssetDto
    {
        public string  Id { get; set; }
        public string AssetName { get; set; }
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchaseCost { get; set; }
        public decimal CurrentValue { get; set; }
        public string Status { get; set; }
        public string AssetTypeName { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
    }
    public class AssetCreateDto
    {
        public string AssetName { get; set; }
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchaseCost { get; set; }
        public string AssetTypeId { get; set; }
        public string LocationId { get; set; }
        public string DepartmentId { get; set; }
    }

    public class AssetUpdateDto
    {
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public string SerialNumber { get; set; }
        public decimal CurrentValue { get; set; }
        public string Status { get; set; }
        public string LocationId { get; set; }
        public string DepartmentId { get; set; }
    }

}
