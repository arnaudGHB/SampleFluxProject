using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Command
{
    public class CashMovementDenominationCommand : IRequest<ServiceResponse<bool>>
    {
        public string Reference { get; set; }
        public string BranchId { get; set; }
        public decimal Amount { get; set; }
        public string OperationType { get; set; }
        public CurrencyNotesRequest CurrencyNotesRequest { get; set; }
    }




 
}
