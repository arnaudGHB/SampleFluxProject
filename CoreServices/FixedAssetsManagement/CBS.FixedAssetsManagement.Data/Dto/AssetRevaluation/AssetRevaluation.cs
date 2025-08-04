using System;
 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class AssetRevaluationDto
    {
        public string Id { get; set; }
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public DateTime RevaluationDate { get; set; }
        public decimal OldValue { get; set; }
        public decimal NewValue { get; set; }
        public string Reason { get; set; }
    }

    public class AssetRevaluationCreateDto
    {
        public string Id { get; set; }
        public DateTime RevaluationDate { get; set; }
        public decimal NewValue { get; set; }
        public string Reason { get; set; }
    }
}
