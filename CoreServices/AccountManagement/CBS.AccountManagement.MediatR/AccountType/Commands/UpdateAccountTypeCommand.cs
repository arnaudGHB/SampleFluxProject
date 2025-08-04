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
    public class UpdateAccountTypeCommand : IRequest<ServiceResponse<AccountTypeDto>>
    {
        public string Id { get; set; }
        public string OperationAccountTypeId { get; set; }
        public string Name { get; set; }
        public string OperationAccountType { get; set; }
    }
}
