using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class AssetDisposalDto
    {
        public string Id { get; set; }
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public DateTime DisposalDate { get; set; }
        public string DisposalMethod { get; set; }
        public decimal DisposalValue { get; set; }
        public string Reason { get; set; }
    }

    public class AssetDisposalCreateDto
    {
        public string AssetId { get; set; }
        public DateTime DisposalDate { get; set; }
        public string DisposalMethod { get; set; }
        public decimal DisposalValue { get; set; }
        public string Reason { get; set; }
    }
}
