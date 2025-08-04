using CBS.NLoan.Helper.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.CustomerP.Command
{
 
    public class AddLoanAccountCommand : IRequest<ServiceResponse<bool>>
    {
        public string CustomerId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
    }
}
