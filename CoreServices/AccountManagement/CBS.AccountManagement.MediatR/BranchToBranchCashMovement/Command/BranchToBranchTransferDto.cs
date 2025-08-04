using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Command
{
    public class BranchToBranchTransferDto : IRequest<ServiceResponse<bool>>
    {
        public   string Id { get; set; }
        public string ReferenceId { get; set; }
        public decimal Amount { get; set; }
        public CurrencyNotesRequest? CurrencyNotesRequest { get; set; }

    }
}
