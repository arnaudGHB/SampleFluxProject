using CBS.AccountManagement.Helper;
using MediatR;
using Polly;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class QueryLastTransactionForReconciliation : IRequest<ServiceResponse<string>>
    {
        public string LastlyProcessedTransactionId { get; set; }

         
    }
}
