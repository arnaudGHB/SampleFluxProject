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

    public class UpdateAssetRevaluationCommand : IRequest<ServiceResponse<AssetRevaluationDto>>
    {
        public string Id { get; set; }
        public string AssetId { get; set; }
 

        public DateTime RevaluationDate { get; set; }
        public decimal OldValue { get; set; }
        public decimal NewValue { get; set; }
        public string Reason { get; set; }
    }

  




}
