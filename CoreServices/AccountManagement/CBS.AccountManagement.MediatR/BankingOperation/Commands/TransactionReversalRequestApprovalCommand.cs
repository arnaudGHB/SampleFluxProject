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
    public class TransactionReversalRequestApprovalCommand : IRequest<ServiceResponse<TransactionReversalRequestDataDto>>
    {

        public string Id { get; set; }
        public string ApprovedMessage { get; set; }
  
        public bool IsApproved { get; set; }
    }

    public class TransactionReversalRequestDataDto
    {
        public string Id { get; set; }
        public string ReversalRequest { get; set; }
        public string DataBeforeReversal { get; set; } 
        public string DataAfterReversal { get; set; } 
    }
}
