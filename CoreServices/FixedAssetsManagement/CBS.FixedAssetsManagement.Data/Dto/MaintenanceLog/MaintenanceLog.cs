using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{

    public class MaintenanceLogDto
    {
        public string  Id { get; set; }
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string PerformedByUsername { get; set; }
    }

    public class MaintenanceLogCreateDto
    {
        public string AssetId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string PerformedById { get; set; }
    }

}
