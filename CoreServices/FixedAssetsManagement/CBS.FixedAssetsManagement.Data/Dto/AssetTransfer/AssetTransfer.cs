using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class AssetTransferDto
    {
        public string Id { get; set; }
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public string FromDepartmentName { get; set; }
        public string ToDepartmentName { get; set; }
        public DateTime TransferDate { get; set; }
        public string Reason { get; set; }
    }

    public class AssetTransferCreateDto
    {
        public string Id { get; set; }
        public string FromDepartmentId { get; set; }
        public string ToDepartmentId { get; set; }
        public DateTime TransferDate { get; set; }
        public string Reason { get; set; }
    }

}
