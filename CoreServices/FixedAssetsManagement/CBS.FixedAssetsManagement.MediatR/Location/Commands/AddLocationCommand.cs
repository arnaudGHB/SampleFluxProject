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

    // Location Commands
    public class AddLocationCommand : IRequest<ServiceResponse<LocationDto>>
    {
        public string BranchId { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
    }

 




}
