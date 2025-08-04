using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetTransactionRequestReversalQuery : IRequest<ServiceResponse<TransactionReversalRequestDto>>
    {
        public string Id { get;   set; }
    }
}
