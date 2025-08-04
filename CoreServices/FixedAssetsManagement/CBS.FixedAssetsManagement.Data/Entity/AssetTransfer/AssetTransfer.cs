using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class AssetTransfer : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string AssetId { get; set; }
        public string FromDepartmentId { get; set; }
        public string ToDepartmentId { get; set; }
        public DateTime TransferDate { get; set; }
        [StringLength(200)]
        public string Reason { get; set; }

        public Asset Asset { get; set; }
        public Department FromDepartment { get; set; }
        public Department ToDepartment { get; set; }
    }

}
