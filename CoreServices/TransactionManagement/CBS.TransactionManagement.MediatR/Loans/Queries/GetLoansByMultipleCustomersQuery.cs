using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.Loans.Queries
{
    public class GetLoansByMultipleCustomersQuery : IRequest<ServiceResponse<List<Loan>>>
    {
        public List<string> CustomerIds { get; set; }
        public string QueryParameter { get; set; }
        public string? OperationType { get; internal set; }
    }
}
