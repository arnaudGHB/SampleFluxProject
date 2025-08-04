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


    public class DeleteLocationCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
    }







}
