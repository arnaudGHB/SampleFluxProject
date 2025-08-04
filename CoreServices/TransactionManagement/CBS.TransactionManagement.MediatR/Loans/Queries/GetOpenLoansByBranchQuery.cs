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
    public class GetOpenLoansByBranchQuery : IRequest<ServiceResponse<List<LightLoanDto>>>
    {
        public string BranchId { get; set; }
    }
}
