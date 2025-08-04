using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class AssetType : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        [Required, StringLength(50)]
        public string TypeName { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public string DepreciationMethodId { get; set; }
        public int UsefulLifeYears { get; set; }

        public DepreciationMethod DepreciationMethod { get; set; }
        public ICollection<Asset> Assets { get; set; }
    }
}
