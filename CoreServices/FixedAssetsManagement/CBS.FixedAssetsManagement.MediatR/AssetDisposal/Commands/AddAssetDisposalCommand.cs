using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Helper;
namespace CBS.FixedAssetsManagement.MediatR.Commands
{

    // AssetDisposal Commands
    public class AddAssetDisposalCommand : IRequest<ServiceResponse<AssetDisposalDto>>
    {
        public string AssetId { get; set; }
        public DateTime DisposalDate { get; set; }
        public string DisposalMethod { get; set; }
        public decimal DisposalValue { get; set; }
        public string Reason { get; set; }
    }

 






}
