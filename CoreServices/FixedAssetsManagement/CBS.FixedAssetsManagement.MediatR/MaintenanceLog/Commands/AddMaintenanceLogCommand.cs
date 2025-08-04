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

    // MaintenanceLog Commands
    public class AddMaintenanceLogCommand : IRequest<ServiceResponse<MaintenanceLogDto>>
    {
        public string AssetId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public string PerformedById { get; set; }
    }




}
