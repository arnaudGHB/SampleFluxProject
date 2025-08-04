using CBS.NLoan.Helper.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.CustomerAccount.Command
{
    public class UpdateLoanAccountBalanceCommand : IRequest<ServiceResponse<bool>>
    {
        public string CustomerId { get; set; }
        public decimal Balance { get; set; }
        public decimal Interest { get; set; }
        public string LoanId { get; set; }
        public string? Token { get; set; }
        public string ExternalReference { get; set; }
    }
}
