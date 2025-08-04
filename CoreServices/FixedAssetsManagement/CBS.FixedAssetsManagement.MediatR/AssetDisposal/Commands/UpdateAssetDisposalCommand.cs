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
    public class UpdateAssetDisposalCommand : IRequest<ServiceResponse<AssetDisposalDto>>
    {
        public string Id { get; set; }
        public DateTime DisposalDate { get; set; }
        public string DisposalMethod { get; set; }
        public decimal DisposalValue { get; set; }
        public string Reason { get; set; }
    }


}
