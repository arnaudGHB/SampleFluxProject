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
    public class AddAssetCommand : IRequest<ServiceResponse<AssetDto>>
    {

        public string AssetName { get; set; }
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchaseCost { get; set; }
        public string LocationId { get; set; }
        public string DepartmentId { get; set; }
    }



}
