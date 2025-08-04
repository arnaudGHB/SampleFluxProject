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


    // AssetRevaluation Commands
    public class AddAssetRevaluationCommand : IRequest<ServiceResponse<AssetRevaluationDto>>
    {
        public string AssetId { get; set; }
        public DateTime RevaluationDate { get; set; }
        public decimal OldValue { get; set; }
        public decimal NewValue { get; set; }
        public string Reason { get; set; }
    }







}
