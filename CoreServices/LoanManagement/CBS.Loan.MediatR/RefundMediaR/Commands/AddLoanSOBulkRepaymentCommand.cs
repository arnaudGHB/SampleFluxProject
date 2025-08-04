using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.RefundMediaR.Commands
{
    public class AddLoanSOBulkRepaymentCommand : IRequest<ServiceResponse<bool>>
    {

        public string SalaryCode { get; set; }
    }
}
