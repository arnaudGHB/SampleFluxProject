using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class UpdateCorrespondingAccountExceptionsCommand : IRequest<ServiceResponse<bool>>
    {
        public string AccountCorrespondingMappingId { get; set; }
        public string ChartOfAccountId { get; set; }
        public string Cartegory { get; set; }
        public string Id { get;   set; }
    }


}
