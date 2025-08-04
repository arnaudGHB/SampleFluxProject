using CBS.NLoan.Data.Dto.Resources;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.Queries
{
    public class SearchByAnyCriterialQuery : IRequest<ServiceResponse<LoanList>>
    {
        public LoanResource LoanResource { get; set; }

    }
}
