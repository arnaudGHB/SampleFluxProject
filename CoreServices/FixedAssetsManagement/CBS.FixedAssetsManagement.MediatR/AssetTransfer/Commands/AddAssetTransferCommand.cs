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

    // AssetTransfer Commands
    public class AddAssetTransferCommand : IRequest<ServiceResponse<AssetTransferDto>>
    {
        public string AssetId { get; set; }
        public string FromDepartmentId { get; set; }
        public string ToDepartmentId { get; set; }
        public DateTime TransferDate { get; set; }
        public string Reason { get; set; }
    }

    




}
