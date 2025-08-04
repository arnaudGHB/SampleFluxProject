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
    public class AddAccountTypeForSystemCommand : IRequest<ServiceResponse<AccountTypeDto>>
    {
        public string Name { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }
    }
}
