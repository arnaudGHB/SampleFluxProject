using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.FixedAssetsManagement.Helper;
using CBS.FixedAssetsManagement.Data;
namespace CBS.FixedAssetsManagement.MediatR.Commands
{
    // Asset Commands 


    public class UpdateAssetCommand : IRequest<ServiceResponse<AssetDto>>
    {

        public string Id { get; set; }
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

   


}
