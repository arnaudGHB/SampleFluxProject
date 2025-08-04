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
    public class AddAccountTypeCommand : IRequest<ServiceResponse<AccountTypeDto>>
    {
        public string name { get; set; }
        public string chartOfAccountId { get; set; }
        public string operationAccountTypeId { get; set; } = "xxxxxxxx-xxxxxx-xxxx-xxxxx-xxx";
        public string operationAccountType { get; set; }
    }
}
