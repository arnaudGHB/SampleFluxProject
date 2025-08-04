using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetTransactionRequestReversalDataQuery : IRequest<ServiceResponse<TransactionReversalRequestDataDto>>
    {
        public string Id { get;   set; }
    }
}
