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
    public class UpdateDepreciationEntryCommand : IRequest<ServiceResponse<DepreciationEntryDto>>
    {
        public string  Id { get; set; }
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public DateTime DepreciationDate { get; set; }
        public decimal DepreciationAmount { get; set; }
        public decimal BookValueAfter { get; set; }
    }

 
}
