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

    // DepreciationEntry Commands
    public class AddDepreciationEntryCommand : IRequest<ServiceResponse<DepreciationEntryDto>>
    {
        public string AssetId { get; set; }
        public DateTime DepreciationDate { get; set; }
        public decimal DepreciationAmount { get; set; }
        public decimal BookValueAfter { get; set; }
    }




}
