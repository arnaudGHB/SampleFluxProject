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
  
    public class AddAssetTypeCommand : IRequest<ServiceResponse<AssetTypeDto>>
    {
        public string TypeName { get; set; }
        public string Description { get; set; }
        public string DepreciationMethodId { get; set; }
        public int UsefulLifeYears { get; set; }
    }

   
}
