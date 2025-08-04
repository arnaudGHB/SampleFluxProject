using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class DepreciationMethod : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        [Required, StringLength(50)]
        public string MethodName { get; set; }
        [StringLength(200)]
        public string Description { get; set; }

        public ICollection<AssetType> AssetTypes { get; set; }
    }
}
