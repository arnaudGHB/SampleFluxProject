using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Command
{
    public class VaultTransferCommand : IRequest<ServiceResponse<bool>>
    {
        public string Reference { get; set; }
        public string FromBranchId { get; set; }
        public string ToBranchId { get; set; }
        public decimal Amount { get; set; }
        public CurrencyNotesRequest CurrencyNotesRequest { get; set; }
    }


}
