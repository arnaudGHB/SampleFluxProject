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
    public class UpdateDepreciationMethodCommand : IRequest<ServiceResponse<DepreciationMethodDto>>
    {
        public string Id { get; set; }
        public string MethodName { get; set; }
        public string Description { get; set; }
    }

  



}
