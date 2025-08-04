using CBS.NLoan.Helper.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.CustomerP.Command
{
 
    public class BlockOrUnblockAccountCommand : IRequest<ServiceResponse<bool>>
    {
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string LoanApplicationId { get; set; }
    }
}
