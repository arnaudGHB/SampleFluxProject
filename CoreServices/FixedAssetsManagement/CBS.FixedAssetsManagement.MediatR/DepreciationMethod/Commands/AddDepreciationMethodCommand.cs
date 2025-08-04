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
  
    // DepreciationMethod Commands
    public class AddDepreciationMethodCommand : IRequest<ServiceResponse<DepreciationMethodDto>>
    {
        public string MethodName { get; set; }
        public string Description { get; set; }
    }




}
