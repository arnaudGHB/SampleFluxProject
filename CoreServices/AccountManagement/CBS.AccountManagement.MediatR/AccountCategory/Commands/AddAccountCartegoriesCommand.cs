using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
 
    public class AddAccountCartegoriesCommand : IRequest<ServiceResponse<List<AccountCartegoryDto>>>
    {

        public List<AddAccountCategoryCommand> AccountCartegories { get; set; }
    }
}
