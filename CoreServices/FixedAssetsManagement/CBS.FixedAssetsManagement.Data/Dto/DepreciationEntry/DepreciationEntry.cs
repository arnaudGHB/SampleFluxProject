using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{

    public class DepreciationEntryDto
    {
        public string Id { get; set; }
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public DateTime DepreciationDate { get; set; }
        public decimal DepreciationAmount { get; set; }
        public decimal BookValueAfter { get; set; }
    }

}
