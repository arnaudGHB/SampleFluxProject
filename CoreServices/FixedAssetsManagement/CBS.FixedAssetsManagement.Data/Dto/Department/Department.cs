using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class DepartmentDto
    {
        [Key]
        public int DepartmentId { get; set; }
        [Required, StringLength(100)]
        public string DepartmentName { get; set; }

        public ICollection<Asset> Assets { get; set; }
        public ICollection<AssetTransfer> FromTransfers { get; set; }
        public ICollection<AssetTransfer> ToTransfers { get; set; }
    }

}
