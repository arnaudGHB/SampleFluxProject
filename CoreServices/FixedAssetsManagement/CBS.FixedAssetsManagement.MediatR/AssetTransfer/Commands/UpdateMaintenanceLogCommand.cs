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
    public class UpdateAssetTransferCommand : IRequest<ServiceResponse<AssetTransferDto>>
    {
        public string Id { get; set; }
        public DateTime TransferDate { get; set; }
        public string Reason { get; set; }
    }

  







}
